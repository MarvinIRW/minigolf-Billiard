using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CueBallController : MonoBehaviour
{
    private Rigidbody cueBallRigidbody;
    [SerializeField] private AudioClip hitSound;
    private AudioSource audioSource;
    // Initialize Rigidbody and AudioSource
    private void Start()
    {
        cueBallRigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }
    // Apply force to the cue ball and play hit sound
    public void Shoot(Vector3 force)
    {
        cueBallRigidbody.AddForce(force, ForceMode.Impulse);
        audioSource.PlayOneShot(hitSound);
    }
    // Play hit sound when the cue ball hits the eight ball
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("EightBall"))
        {
            audioSource.PlayOneShot(hitSound);
        }
    }
    // Check if the cue ball is stationary
    public bool IsStationary()
    {
        return cueBallRigidbody.velocity.magnitude < 0.1f;
    }
}
