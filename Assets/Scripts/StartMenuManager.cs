using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuManager : MonoBehaviour
{
    // Function to start the game
    public void GameStart()
    {
        // Load the next scene in the build index
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Function to quit the game
    public void Quit()
    {
        // Quit the application
        Application.Quit();

        // Log that the player has quit the game
        Debug.Log("Player Quit the Game");
    }
}
