using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CueBallController cueBallController;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float maxShotStrength = 10f;
    [SerializeField] private float minShotStrength = 1f;
    [SerializeField] private float shotStrengthMultiplier = 3f;
    private float mouseDownTime;
    private float shotStrength = 0;


    //needed for camera switch
    [SerializeField] private GameObject eightBall;
    [SerializeField] private Vector3 cueBallCameraOffset = new Vector3(0, 5, -5);
    [SerializeField] private Vector3 fieldViewCameraPosition;
    [SerializeField] private Quaternion fieldViewCameraRotation;
    [SerializeField] private float cameraTransitionSpeed = 2f;
    private bool isFieldView = true;

    public float ShotStrength { get { return shotStrength; } }
    public float MinShotStrength { get { return minShotStrength; } }
    public float MaxShotStrength { get { return maxShotStrength; } }

    public void Start()
    {
        fieldViewCameraPosition = mainCamera.transform.position;
        fieldViewCameraRotation = mainCamera.transform.rotation;
    }
    private void Update()
    {
        // Aim the shot using the mouse position
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 aimDirection = (hit.point - cueBallController.transform.position).normalized;

            //get first downpress
            if (Input.GetMouseButtonDown(0))
            {
                mouseDownTime = Time.time;
            }
            // while its pressed (needed for the ui Slider)
            if (Input.GetMouseButton(0))
            {
                float pressDuration = (Time.time - mouseDownTime) + 1;
                shotStrength = Mathf.Clamp(pressDuration * shotStrengthMultiplier, minShotStrength, maxShotStrength);         
            }
            // when released
            if (Input.GetMouseButtonUp(0))
            {
                cueBallController.Shoot(aimDirection * shotStrength);
                shotStrength = 0f;
            }

        }

        // camera update
        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleCameraView();
        }

        UpdateCameraPosition();

    }
    //camera functions
    private void ToggleCameraView()
    {
        isFieldView = !isFieldView;
    }
    private void UpdateCameraPosition()
    {
        Vector3 targetPosition;
        Quaternion targetRotation;

        if (isFieldView)
        {
            targetPosition = fieldViewCameraPosition;
            targetRotation = fieldViewCameraRotation;
        }
        else
        {
            Vector3 cueBallToEightBallDirection = (eightBall.transform.position - cueBallController.transform.position).normalized;
            targetPosition = cueBallController.transform.position - cueBallToEightBallDirection * cueBallCameraOffset.magnitude;
            targetRotation = Quaternion.LookRotation(cueBallToEightBallDirection, Vector3.up);
        }

        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, cameraTransitionSpeed * Time.deltaTime);
        mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, targetRotation, cameraTransitionSpeed * Time.deltaTime);
    }



}
