using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BallRotator : MonoBehaviour
{
    public float rotationSpeed = 1f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Get the velocity of the ball
        Vector3 velocity = rb.velocity;

        // Calculate the rotation speed based on the velocity
        float speed = velocity.magnitude * rotationSpeed;

        // Calculate the rotation axis
        Vector3 rotationAxis = Vector3.Cross(velocity.normalized, Vector3.up);

        // Apply the rotation
        transform.Rotate(rotationAxis, speed * Time.deltaTime, Space.World);
    }
}
