using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoNotDestroy : MonoBehaviour
{
    private void Awake()
    {
        // Find all game objects tagged as "BackgroundMusic"
        GameObject[] musicObjects = GameObject.FindGameObjectsWithTag("BackgroundMusic");

        // If there is more than one music object, destroy the current game object
        if (musicObjects.Length > 1)
        {
            Destroy(this.gameObject);
        }

        // Prevent the game object from being destroyed when loading a new scene
        DontDestroyOnLoad(this.gameObject);
    }
}
