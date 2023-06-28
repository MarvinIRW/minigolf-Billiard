using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    // Serialized fields for minimum speed to stop and level bounds
    [SerializeField] private float _minSpeedToStop = 0.05f;
    [SerializeField] private GameObject _levelBounds;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private GameManager _gameManager;

    // Private variables for ball rigidbodies and initial state
    private List<Rigidbody> _ballRigidbodies;
    private Dictionary<GameObject, (Vector3 position, Quaternion rotation)> _initialState = new Dictionary<GameObject, (Vector3 position, Quaternion rotation)>();
    private Coroutine _resetCoroutine = null;
    // A dictionary to store each ball's idle time
    private Dictionary<Rigidbody, float> _ballIdleTimes = new Dictionary<Rigidbody, float>();


    // Initialize ballRigidbodies list with Rigidbody components of all children objects
    private void Start()
    {
        _ballRigidbodies = new List<Rigidbody>();
        foreach (Transform child in transform)
        {
            Rigidbody rb = child.GetComponent<Rigidbody>();
            if (rb != null)
            {
                _ballRigidbodies.Add(rb);
                _ballIdleTimes[rb] = 0;
            }
        }
    }

    // Call StopBallIfSlow() for each ball on every frame
    private void Update()
    {
        foreach (Rigidbody rb in _ballRigidbodies)
        {
            StopBallIfSlow(rb);
        }
    }



    // If a ball's speed is lower than minSpeedToStop, set its velocity and angular velocity to zero, effectively stopping it
    private void StopBallIfSlow(Rigidbody rb)
    {
        RaycastHit hit;
        if (Physics.Raycast(rb.position, -Vector3.up, out hit, 1.5f))
        {
            if (hit.collider.CompareTag("Ground") && rb.velocity.magnitude <= _minSpeedToStop)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }

    // Checks if there is any moving ball
    
    public bool IsAnyMovement()
    {
        foreach (Rigidbody rb in _ballRigidbodies)
        {
            // If ball's speed is higher than minSpeedToStop, consider it as moving
            if (rb.velocity.magnitude > _minSpeedToStop)
            {
                // Reset this ball's idle time
                _ballIdleTimes[rb] = 0;
                return true;
            }

            // Otherwise, check if the ball is over an obstacle
            if (IsBallOverObstacle(rb))
            {
                // If it's over an obstacle, increase its idle time
                _ballIdleTimes[rb] += Time.deltaTime;

                // If it hasn't moved for less than 1 second, still consider it as moving to prevent shooting
                // in the unlicly case that a ball gets stuck on an obstacle, it will be considered as not moving if it hasn't moved for more than 1 second
                if (_ballIdleTimes[rb] < 1f)
                {
                    return true;
                }
            }
        }
        return false;
    }

    // Check if a ball is over an obstacle
    private bool IsBallOverObstacle(Rigidbody ball)
    {
        RaycastHit hit;
        if (Physics.Raycast(ball.position, -Vector3.up, out hit, 1.5f))
        {
            if (hit.collider.CompareTag("Obstacle"))
            {
                return true;
            }
        }
        return false;
    }

    // Stores the initial state of the balls
    public void StoreInitialState()
    {
        _initialState.Clear();
        foreach (Transform child in transform)
        {
            _initialState[child.gameObject] = (child.position, child.rotation);
        }
    }

    // Resets the balls to their initial state
    public void ResetToInitialState()
    {
        foreach (var pair in _initialState)
        {
            pair.Key.transform.position = pair.Value.position;
            pair.Key.transform.rotation = pair.Value.rotation;
            var rigidbody = pair.Key.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.velocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
            }
        }
    }

    public void StartResetCoroutine()
    {
        // If a reset coroutine is already running, do nothing
        if (_resetCoroutine != null || _gameManager.GameOver)
        {
            return;
        }

        // Otherwise, start a new reset coroutine
        _resetCoroutine = StartCoroutine(ResetAfterDelay(2f));
    }
    // This coroutine waits for the specified delay before resetting all balls
    private IEnumerator ResetAfterDelay(float delay)
    {
        _uiManager.UpdateResettingBallText(true);
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Reset all balls to their initial state
        ResetToInitialState();

        // Hide the reset text at the end of the coroutine
        _uiManager.UpdateResettingBallText(false);

        // Clear the reset coroutine
        _resetCoroutine = null;
    }
    
    //Checks if any ball is out of bounds
    //public bool IsAnyBallOutOfBounds()
    //{
    //    var levelBoundsCollider = _levelBounds.GetComponent<Collider>();
    //    if (levelBoundsCollider == null)
    //    {
    //        Debug.LogError("levelBounds game object doesn't have a Collider component!");
    //        return false;
    //    }
    //    foreach (Rigidbody rb in _ballRigidbodies)
    //    {
    //        if (!levelBoundsCollider.bounds.Contains(rb.position))
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    //// Checks if balls are out of bounds and resets them if necessary
    //public void CheckBallsOutBounds()
    //{
    //    if (IsAnyBallOutOfBounds())
    //    {
    //        ResetToInitialState();
    //    }
    //}
}
