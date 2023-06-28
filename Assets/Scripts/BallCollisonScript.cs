using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCollisonScript : MonoBehaviour
{
    private BallManager _ballManager;

    private void Start()
    {
        _ballManager = FindObjectOfType<BallManager>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Terrain"))
        {
            // Start the reset coroutine
            _ballManager.StartResetCoroutine();
        }
    }
}
