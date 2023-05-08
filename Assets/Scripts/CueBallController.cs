using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CueBallController : MonoBehaviour
{
    [SerializeField] private float shotStrength = 5f;
    private Rigidbody cueBallRigidbody;
    [SerializeField] private AudioClip hitSound;
    private AudioSource audioSource;

    private void Start()
    {
        cueBallRigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

    }

    public void Shoot(Vector3 direction)
    {
        Vector3 force = direction.normalized * shotStrength;
        cueBallRigidbody.AddForce(force, ForceMode.Impulse);
        audioSource.PlayOneShot(hitSound);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("EightBall"))
        {
            // Play hit sound
            audioSource.PlayOneShot(hitSound);
        }
    }
}

