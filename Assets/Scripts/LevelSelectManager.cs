using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour
{
    // Function to load a level
    public void LoadLevel(string levelName)
    {
        // Load the specified level
        SceneManager.LoadScene(levelName);
    }
}
