using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    [SerializeField] private float minSpeedToStop = 0.05f;

    private List<Rigidbody> ballRigidbodies;
    // Initialize ballRigidbodies list with Rigidbody components of all children objects
    void Start()
    {
        ballRigidbodies = new List<Rigidbody>();
        foreach (Transform child in transform)
        {
            Rigidbody rb = child.GetComponent<Rigidbody>();
            if (rb != null)
            {
                ballRigidbodies.Add(rb);
            }
        }
    }
    // Call StopBallIfSlow() for each ball on every frame
    void Update()
    {
        foreach (Rigidbody rb in ballRigidbodies)
        {
            StopBallIfSlow(rb);
        }
    }
    // If a ball's speed is lower than minSpeedToStop, set its velocity and angular velocity to zero, effectively stopping it
    private void StopBallIfSlow(Rigidbody rb)
    {
        if (rb.velocity.magnitude <= minSpeedToStop)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
    // checks if there is movement
    public bool IsAnyMovement()
    {
        foreach (Rigidbody rb in ballRigidbodies)
        {
            if (rb.velocity.magnitude > minSpeedToStop)
            {
                return true;
            }
        }
        return false;
    }
}
