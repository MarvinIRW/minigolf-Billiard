using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

// This class manages the game state and camera transitions.
public class GameManager : MonoBehaviour
{
    // Variables for camera switch
    private CueBallController _cueBallController;
    private Camera _mainCamera;
    [SerializeField] private GameObject _eightBall;
    [SerializeField] private Vector3 _cueBallCameraOffset = new Vector3(0, 0, 0);
    private Vector3 _fieldViewCameraPosition;
    private Quaternion _fieldViewCameraRotation;
    private Vector3 _freeViewCameraPosition;
    private Quaternion _freeViewCameraRotation;
    [SerializeField] private float _cameraTransitionSpeed = 5f;
    private FreeCameraController _freeCameraController;
    private FollowCueBallCamera _followCueBallCamera;

    // Enum for camera states
    public enum CameraState
    {
        AreaView,
        CueBallView,
        FreeView,
        Transition
    }
    private CameraState _cameraState = CameraState.AreaView;
    public CameraState CurrentCameraState { get { return _cameraState; } }

    // Variables for game management
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private int _maxShots = 10;
    private bool _gameOver = false; // Flag to indicate if the game is over
    [SerializeField] private string _nextLevelSceneName; //name of the next level to be loaded

    // For out of bounds checks
    [SerializeField] private BallManager _ballManager;

    //getter for maxShots
    public int MaxShots { get { return _maxShots; } }

    public void Start()
    {
        _cueBallController = _playerController.CueBallController;
        _mainCamera = _playerController.MainCamera;
        _fieldViewCameraPosition = _mainCamera.transform.position;
        _fieldViewCameraRotation = _mainCamera.transform.rotation;

        // Ensure FreeCameraController is not active at the start
        _freeCameraController = _mainCamera.GetComponent<FreeCameraController>();
        _freeCameraController.enabled = false;
        _freeViewCameraPosition = _mainCamera.transform.position;
        _freeViewCameraRotation = _mainCamera.transform.rotation;

        _followCueBallCamera = _mainCamera.GetComponent<FollowCueBallCamera>();
        _followCueBallCamera.enabled = true;
    }

    private void Update()
    {
        if (_gameOver) return; // If game is over, do not proceed further

        // Update UI with the number of shots taken
        _uiManager.UpdateShotsText(_playerController.ShotsTaken);

        // Check the game status
        CheckGameStatus();

        // Camera update
        if (Input.GetKeyDown(KeyCode.C)) // If C key is pressed, change camera state
        {
            ChangeCameraState();
        }

        // Update camera's position based on the current state
        UpdateCameraState();

        // Check if any ball is out of bounds and resets it 
        _ballManager.CheckBallsOutBounds();
    }

    private void OnTriggerEnter(Collider other)
    {
        // If EightBall falls into the hole, the game is over
        if (other.gameObject.CompareTag("EightBall"))
        {
            StartCoroutine(Sinking(other.gameObject));
            EightBallInHole();
        }
    }

    private void EightBallInHole()
    {
        Debug.Log("Eight-ball in the hole!");
        _gameOver = true;
        CheckGameStatus();
    }

    private IEnumerator Sinking(GameObject eightball)
    {
        // Disable the ball's physics while it sinks
        var rigidBody = _eightBall.GetComponent<Rigidbody>();
        rigidBody.isKinematic = true;

        // Make the ball sink for a set amount of time
        float sinkDuration = 4f; // The time it takes for the ball to sink
        float startTime = Time.time;

        Vector3 initialPosition = _eightBall.transform.position;
        Vector3 holePosition = transform.position; // If GameManager script is attached to the hole object
        Vector3 targetPosition = new Vector3(holePosition.x, holePosition.y - 0.5f, holePosition.z); // Adjust y value based on how deep the hole is

        while (Time.time < startTime + sinkDuration)
        {
            float t = (Time.time - startTime) / sinkDuration;
            _eightBall.transform.position = Vector3.Lerp(initialPosition, targetPosition, t);

            yield return null; // Wait for next frame
        }
    }

    private void CheckGameStatus()
    {
        // If the game is over and player has taken less or equal shots than maxShots, player wins
        if (_gameOver && _playerController.ShotsTaken <= _maxShots)
        {
            Debug.Log("You won!");
            //_UIManager.Win(); // Display win text

            //maybe if condition to play only this level for later
            _uiManager.NextLevel(_playerController.ShotsTaken);
            Invoke("LoadNextLevel", 4);  // Load next level in 4 sec
        }
        // If player has taken more than maxShots, player loses
        else if (_playerController.ShotsTaken >= _maxShots)
        {
            Debug.Log("You lost!");
            _gameOver = true;
            _uiManager.GameOver(); // Display game over text
        }
    }

    // Method to load next level
    private void LoadNextLevel()
    {
        // Load the next scene using SceneManager
        SceneManager.LoadScene(_nextLevelSceneName);
    }

    // Camera related methods
    private void ChangeCameraState()
    {
        switch (_cameraState)
        {
            case CameraState.AreaView:
                _fieldViewCameraPosition = _mainCamera.transform.position;
                _fieldViewCameraRotation = _mainCamera.transform.rotation;
                _cameraState = CameraState.CueBallView;
                _followCueBallCamera.enabled = false;
                _freeCameraController.enabled = false;
                break;
            case CameraState.CueBallView:
                _cameraState = CameraState.Transition;
                StartCoroutine(TransitionToCameraState(CameraState.FreeView, _freeViewCameraPosition, _freeViewCameraRotation));
                break;
            case CameraState.FreeView:
                // When switching away from free view, store the current position of the free camera
                _freeViewCameraPosition = _mainCamera.transform.position;
                _freeViewCameraRotation = _mainCamera.transform.rotation;

                _cameraState = CameraState.Transition;
                StartCoroutine(TransitionToCameraState(CameraState.AreaView, _fieldViewCameraPosition, _fieldViewCameraRotation));
                break;
        }
    }

    private IEnumerator TransitionToCameraState(CameraState nextState, Vector3 targetPosition, Quaternion targetRotation)
    {
        _freeCameraController.enabled = false;
        _followCueBallCamera.enabled = false;

        while ((_mainCamera.transform.position - targetPosition).sqrMagnitude > 0.01f ||
                Quaternion.Angle(_mainCamera.transform.rotation, targetRotation) > 0.1f)
            
        {
            _mainCamera.transform.position = Vector3.Lerp(_mainCamera.transform.position, targetPosition, _cameraTransitionSpeed * Time.deltaTime);
            _mainCamera.transform.rotation = Quaternion.Lerp(_mainCamera.transform.rotation, targetRotation, _cameraTransitionSpeed * Time.deltaTime);
            yield return null;
        }

        _cameraState = nextState; // change to the next state after transition

        switch (nextState)
        {
            case CameraState.AreaView:
                _followCueBallCamera.enabled = true;
                break;
            case CameraState.CueBallView:
                break;
            case CameraState.FreeView:
                _freeCameraController.enabled = true;
                break;
        }
    }

    private void UpdateCameraState()
    {
        Vector3 targetPosition = Vector3.zero;
        Quaternion targetRotation = Quaternion.identity;

        switch (_cameraState)
        {
            case CameraState.AreaView:
                return;
            case CameraState.CueBallView:
                Vector3 cueBallToEightBallDirection = (_eightBall.transform.position - _cueBallController.transform.position).normalized;
                targetPosition = _cueBallController.transform.position - cueBallToEightBallDirection * _cueBallCameraOffset.magnitude;

                // Adjust y-coordinate of targetPosition
                targetPosition.y += 0.1f;  // hopefully fixing a bug where the player cant shoot the ball
                targetRotation = Quaternion.LookRotation(cueBallToEightBallDirection, Vector3.up);
                break;

            case CameraState.FreeView:
                return; // In free view, we do nothing and let the FreeCameraController handle everything.
            case CameraState.Transition:
                return;
        }

        // Interpolate from the current position/rotation
        _mainCamera.transform.position = Vector3.Lerp(_mainCamera.transform.position, targetPosition, _cameraTransitionSpeed * Time.deltaTime);
        _mainCamera.transform.rotation = Quaternion.Lerp(_mainCamera.transform.rotation, targetRotation, _cameraTransitionSpeed * Time.deltaTime);
    }
}
