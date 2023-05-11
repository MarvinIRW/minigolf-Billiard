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

    [SerializeField] private float cameraTransitionSpeed = 2f;
    private int cameraView = 0; // Flag for toggling camera view, 0 = Area view, 1 = cue ball viwe,  2 = free view, 3 = transition
    private FreeCameraController freeCameraController;



    // Variables for game management
    [SerializeField] private UIManager _UIManager;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private int maxShots = 10;
    private bool gameOver = false; // Flag to indicate if the game is over

    // getters
    public int CameraView { get { return cameraView; } }


    public void Start()
    {
        cueBallController = playerController.CueBallController;
        mainCamera = playerController.MainCamera;
        fieldViewCameraPosition = mainCamera.transform.position;
        fieldViewCameraRotation = mainCamera.transform.rotation;

        // Ensure FreeCameraController is not active at the start
        freeCameraController = mainCamera.GetComponent<FreeCameraController>();
        freeCameraController.enabled = false;
    }

    private void Update()
    {
        if (gameOver) return; // If game is over, do not proceed further

        // Update UI with the number of shots taken
        _UIManager.UpdateShotsText(playerController.ShotsTaken);

        // Check the game status
        CheckGameStatus();

        // Camera update
        if (Input.GetKeyDown(KeyCode.C)) // If C key is pressed, toggle camera view
        {
            ToggleCameraView();
        }

        // Update camera's position based on the current view (field or cue ball)
        UpdateCameraPosition();
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

    private void ToggleCameraView()
    {
        if (cameraView == 0)
        {
            cameraView = 1;
            freeCameraController.enabled = false;
        }
        else if (cameraView == 1)
        {
            // When switching to free view, first go to the transition phase
            cameraView = 3;
            freeCameraController.enabled = false;
        }
        else if (cameraView == 2)
        {
            // When switching away from free view, store the current position of the free camera
            freeViewCameraPosition = mainCamera.transform.position;
            freeViewCameraRotation = mainCamera.transform.rotation;

            cameraView = 0;
            freeCameraController.enabled = false;
        }
    }

    private void UpdateCameraPosition()
    {
        Vector3 targetPosition = Vector3.zero;
        Quaternion targetRotation = Quaternion.identity;

        if (cameraView == 0) // area view
        {
            targetPosition = fieldViewCameraPosition;
            targetRotation = fieldViewCameraRotation;
        }
        else if (cameraView == 1) // cueball view
        {
            Vector3 cueBallToEightBallDirection = (eightBall.transform.position - cueBallController.transform.position).normalized;
            targetPosition = cueBallController.transform.position - cueBallToEightBallDirection * cueBallCameraOffset.magnitude;
            targetRotation = Quaternion.LookRotation(cueBallToEightBallDirection, Vector3.up);
        }
        else if (cameraView == 2) // free view
        {
            return; // In free view, we do nothing and let the FreeCameraController handle everything.
        }
        else if (cameraView == 3) // transition phase
        {
            // In the transition phase, interpolate towards the target position and then exit.
            targetPosition = freeViewCameraPosition;
            targetRotation = freeViewCameraRotation;

            // If we are in the transition phase and have reached the free camera's position, switch to free view
            if ((mainCamera.transform.position - freeViewCameraPosition).sqrMagnitude < 1f && // Increased from 0.01f
    Quaternion.Angle(mainCamera.transform.rotation, freeViewCameraRotation) < 1f) // Increased from 0.1f
            {
                cameraView = 2;
                freeCameraController.enabled = true;
            }

            Debug.Log("transition phase");

        }

        // Interpolate from the current position/rotation
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, cameraTransitionSpeed * Time.deltaTime);
        mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, targetRotation, cameraTransitionSpeed * Time.deltaTime);
    }

}
