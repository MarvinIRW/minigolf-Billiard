using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    [SerializeField] private float minSpeedToStop = 0.05f;

    private List<Rigidbody> ballRigidbodies;

    // saved location of balls
    private Dictionary<GameObject, (Vector3 position, Quaternion rotation)> initialState = new Dictionary<GameObject, (Vector3 position, Quaternion rotation)>();
    // for out of bounds checks
    [SerializeField] GameObject levelBounds;
    
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
        // Cast a ray downwards from the ball
        RaycastHit hit;
        if (Physics.Raycast(rb.position, -Vector3.up, out hit, 1.5f)) // Adjust the distance as needed
        {
            // Check if the ray hit the ground
            if (hit.collider.CompareTag("Ground"))
            {
                // Now we know that the ball is grounded and can safely stop it if it's slow
                if (rb.velocity.magnitude <= minSpeedToStop)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }
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
    // bounds checks
    public void StoreInitialState()
    {
        initialState.Clear();
        foreach (Transform child in transform)
        {
            Debug.Log("saving ball pos");
            initialState[child.gameObject] = (child.position, child.rotation);
        }
    }
    public void ResetToInitialState()
    {
        foreach (var pair in initialState)
        {
            Debug.Log("resetting ball pos");

            pair.Key.transform.position = pair.Value.position;
            pair.Key.transform.rotation = pair.Value.rotation;
            // Also reset velocity and angular velocity
            var rigidbody = pair.Key.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                Debug.Log("resetting ball speed");

                rigidbody.velocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
            }
        }
    }
    // checks if any ball is out of bounds
    public bool IsAnyBallOutOfBounds()
    {
        // Make sure to have a collider component on the 'levelBounds' game object
        var levelBoundsCollider = levelBounds.GetComponent<Collider>();
        if (levelBoundsCollider == null)
        {
            Debug.LogError("levelBounds game object doesn't have a Collider component!");
            return false;
        }
        foreach (Rigidbody rb in ballRigidbodies)
        {
            if (!levelBoundsCollider.bounds.Contains(rb.position))
            {
                Debug.Log("ball out of bounds");

                return true;
            }
        }
        return false;
    }


    public void CheckBallsOutBounds()
    {
        if (IsAnyBallOutOfBounds())
        {
            ResetToInitialState();
        }
    }

}
