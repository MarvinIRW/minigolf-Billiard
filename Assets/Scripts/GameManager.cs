using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Variables for camera switch
    private CueBallController cueBallController;
    private Camera mainCamera;
    [SerializeField] private GameObject eightBall;
    [SerializeField] private Vector3 cueBallCameraOffset = new Vector3(0, 0, 0);
    private Vector3 fieldViewCameraPosition;
    private Quaternion fieldViewCameraRotation;
    private Vector3 freeViewCameraPosition;
    private Quaternion freeViewCameraRotation;
    [SerializeField] private float cameraTransitionSpeed = 5f;
    private FreeCameraController freeCameraController;
    public enum CameraState
    {
        AreaView,
        CueBallView,
        FreeView,
        Transition
    }
    public CameraState cameraState = CameraState.AreaView;
    // Variables for game management
    [SerializeField] private UIManager _UIManager;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private int maxShots = 10;
    private bool gameOver = false; // Flag to indicate if the game is over
    // for out of bounds checks
    [SerializeField] BallManager ballManager;
    public void Start()
    {
        cueBallController = playerController.CueBallController;
        mainCamera = playerController.MainCamera;
        fieldViewCameraPosition = mainCamera.transform.position;
        fieldViewCameraRotation = mainCamera.transform.rotation;

        // Ensure FreeCameraController is not active at the start
        freeCameraController = mainCamera.GetComponent<FreeCameraController>();
        freeCameraController.enabled = false;
        freeViewCameraPosition = mainCamera.transform.position;
        freeViewCameraRotation = mainCamera.transform.rotation;
    }
    private void Update()
    {
        if (gameOver) return; // If game is over, do not proceed further

        // Update UI with the number of shots taken
        _UIManager.UpdateShotsText(playerController.ShotsTaken);

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
        ballManager.CheckBallsOutBounds();
    }
    private void OnTriggerEnter(Collider other)
    {
        // If EightBall falls into the hole, the game is over
        if (other.gameObject.CompareTag("EightBall"))
        {
            EightBallInHole();
        }
    }
    private void EightBallInHole()
    {
        Debug.Log("Eight-ball in the hole!");
        gameOver = true;
        CheckGameStatus();
    }
    private void CheckGameStatus()
    {
        // If the game is over and player has taken less or equal shots than maxShots, player wins
        if (gameOver && playerController.ShotsTaken <= maxShots)
        {
            Debug.Log("You won!");
            _UIManager.Win(); // Display win text
        }
        // If player has taken more than maxShots, player loses
        else if (playerController.ShotsTaken >= maxShots)
        {
            Debug.Log("You lost!");
            gameOver = true;
            _UIManager.GameOver(); // Display game over text
        }
    }




    // camera stuff
    private void ChangeCameraState()
    {
        switch (cameraState)
        {
            case CameraState.AreaView:
                cameraState = CameraState.CueBallView;
                freeCameraController.enabled = false;
                break;
            case CameraState.CueBallView:
                StartCoroutine(TransitionToFreeView());
                break;
            case CameraState.FreeView:
                // When switching away from free view, store the current position of the free camera
                freeViewCameraPosition = mainCamera.transform.position;
                freeViewCameraRotation = mainCamera.transform.rotation;

                cameraState = CameraState.AreaView;
                freeCameraController.enabled = false;
                break;
        }
    }
    private IEnumerator TransitionToFreeView()
    {
        cameraState = CameraState.Transition; // transition phase
        freeCameraController.enabled = false;

        Vector3 targetPosition = freeViewCameraPosition;
        Quaternion targetRotation = freeViewCameraRotation;


        while ((mainCamera.transform.position - targetPosition).sqrMagnitude > 0.01f ||
                Quaternion.Angle(mainCamera.transform.rotation, targetRotation) > 0.1f)
        {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, cameraTransitionSpeed * Time.deltaTime);
            mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, targetRotation, cameraTransitionSpeed * Time.deltaTime);
            Debug.Log((mainCamera.transform.position - targetPosition).sqrMagnitude);
            Debug.Log((Quaternion.Angle(mainCamera.transform.rotation, targetRotation)));
            yield return null;
        }
        cameraState = CameraState.FreeView; // free view
        freeCameraController.enabled = true;
    }
    private void UpdateCameraState()
    {
        Vector3 targetPosition = Vector3.zero;
        Quaternion targetRotation = Quaternion.identity;

        switch (cameraState)
        {
            case CameraState.AreaView:
                targetPosition = fieldViewCameraPosition;
                targetRotation = fieldViewCameraRotation;
                break;
            case CameraState.CueBallView:
                Vector3 cueBallToEightBallDirection = (eightBall.transform.position - cueBallController.transform.position).normalized;
                targetPosition = cueBallController.transform.position - cueBallToEightBallDirection * cueBallCameraOffset.magnitude;
                targetRotation = Quaternion.LookRotation(cueBallToEightBallDirection, Vector3.up);
                break;
            case CameraState.FreeView:
                return; // In free view, we do nothing and let the FreeCameraController handle everything.
            case CameraState.Transition:
                return;
        }

        // Interpolate from the current position/rotation
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, cameraTransitionSpeed * Time.deltaTime);
        mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, targetRotation, cameraTransitionSpeed * Time.deltaTime);
    }
}
