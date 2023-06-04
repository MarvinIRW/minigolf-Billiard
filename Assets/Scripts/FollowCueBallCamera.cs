using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCueBallCamera : MonoBehaviour
{
    [SerializeField] private GameObject cueBall;
    [SerializeField] private float distanceToCueBall = 10f;
    public float DistanceToCueBall { get { return distanceToCueBall; } }
    [SerializeField] private float cameraSpeed = 5f;
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float minDistance = 5f;
    [SerializeField] private float maxDistance = 15f;
    [SerializeField] private float minVerticalAngle = -60f; // Limit the vertical angle
    [SerializeField] private float maxVerticalAngle = 60f; // Limit the vertical angle

    private Vector3 currentVelocity;
    private float verticalAngle; // Keep track of the vertical rotation


    void Start()
    {
        // Set the initial vertical angle based on the camera's rotation in the scene
        verticalAngle = transform.eulerAngles.x;

        // If the angle is larger than 180, it needs to be mapped into [-180, 180] range
        if (verticalAngle > 180f)
        {
            verticalAngle -= 360f;
        }
    }
    void Update()
    {
        // calculate the new position and rotation
        Vector3 newPosition = cueBall.transform.position - transform.forward * distanceToCueBall;
        Quaternion newRotation = Quaternion.LookRotation(cueBall.transform.position - transform.position);

        if (Input.GetMouseButton(1)) // right mouse button
        {
            float x = cameraSpeed * Input.GetAxis("Mouse X");
            float y = -cameraSpeed * Input.GetAxis("Mouse Y"); // invert the y-axis input

            // Rotate horizontally
            transform.RotateAround(cueBall.transform.position, Vector3.up, x);

            // Compute the new vertical angle
            verticalAngle = Mathf.Clamp(verticalAngle + y, minVerticalAngle, maxVerticalAngle);

            // Rotate vertically (around the right axis, because we're looking horizontally)
            transform.RotateAround(cueBall.transform.position, transform.right, verticalAngle - transform.eulerAngles.x);
        }

        // zoom with scroll wheel
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0f)
        {
            distanceToCueBall = Mathf.Clamp(distanceToCueBall - scrollInput * zoomSpeed, minDistance, maxDistance);
        }
        //newPosition = AdjustPositionForTerrain(newPosition);

        // smoothly move and rotate the camera
        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref currentVelocity, cameraSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, cameraSpeed * Time.deltaTime);
    }

    /*private Vector3 AdjustPositionForTerrain(Vector3 position)
    {
        // Assuming your terrain's y position is at 0
        float terrainHeightAtPosition = Terrain.activeTerrain.SampleHeight(position);

        if (position.y < terrainHeightAtPosition)
        {
            position.y = terrainHeightAtPosition;
        }

        return position;
    }*/

}
