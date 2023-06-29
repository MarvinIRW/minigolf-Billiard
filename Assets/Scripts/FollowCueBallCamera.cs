using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCueBallCamera : MonoBehaviour
{
    // Serialized fields for cue ball, distance to cue ball, camera speed, zoom speed, min and max distances, and min and max vertical angles
    [SerializeField] private GameObject _cueBall;
    [SerializeField] private float _distanceToCueBall = 10f;
    [SerializeField] private float _cameraSpeed = 5f;
    [SerializeField] private float _zoomSpeed = 10f;
    [SerializeField] private float _minDistance = 3f;
    [SerializeField] private float _maxDistance = 20f;
    [SerializeField] private float _minVerticalAngle = -60f;
    [SerializeField] private float _maxVerticalAngle = 60f;

    // Private variables for current velocity and vertical angle
    private Vector3 _currentVelocity;
    private float _verticalAngle;

    // Property to get distance to cue ball
    public float DistanceToCueBall { get { return _distanceToCueBall; } }

    private void Start()
    {
        // Set the initial vertical angle based on the camera's rotation in the scene
        _verticalAngle = transform.eulerAngles.x;

        // If the angle is larger than 180, it needs to be mapped into [-180, 180] range
        if (_verticalAngle > 180f)
        {
            _verticalAngle -= 360f;
        }
    }

    private void Update()
    {
        // Calculate the new position and rotation
        Vector3 newPosition = _cueBall.transform.position - transform.forward * _distanceToCueBall;
        Quaternion newRotation = Quaternion.LookRotation(_cueBall.transform.position - transform.position);

        // Rotate camera on right mouse button click
        if (Input.GetMouseButton(1))
        {
            float x = _cameraSpeed * Input.GetAxis("Mouse X");
            float y = -_cameraSpeed * Input.GetAxis("Mouse Y");

            // Rotate horizontally
            transform.RotateAround(_cueBall.transform.position, Vector3.up, x);

            // Compute the new vertical angle
            _verticalAngle = Mathf.Clamp(_verticalAngle + y, _minVerticalAngle, _maxVerticalAngle);

            // Rotate vertically
            transform.RotateAround(_cueBall.transform.position, transform.right, _verticalAngle - transform.eulerAngles.x);
        }

        // Zoom with scroll wheel
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0f)
        {
            _distanceToCueBall = Mathf.Clamp(_distanceToCueBall - scrollInput * _zoomSpeed, _minDistance, _maxDistance);
        }

        // Smoothly move and rotate the camera
        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref _currentVelocity, _cameraSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, _cameraSpeed * Time.deltaTime);
    }
    
}
