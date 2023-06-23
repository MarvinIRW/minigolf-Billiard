using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class StartMenuManager : MonoBehaviour
{
    // Reference to the volume slider for prefs reset
    [SerializeField] private Slider _volumeSlider;  
    // Reference to the difficulty dropdown for prefs reset
    [SerializeField] private TMP_Dropdown _difficultyDropdown;  

    // Function to start the game
    public void GameStart()
    {
        // Load the next scene in the build index
        SceneManager.LoadScene("SampleScene");
    }

    // Function to quit the game
    public void Quit()
    {
        // Quit the application
        Application.Quit();

        // Log that the player has quit the game
        Debug.Log("Player Quit the Game");
    }
    // Function for level select
    public void LevelSelect()
    {
        // Load the LevelSelectScene
        SceneManager.LoadScene("LevelSelectScene");
    }

    // Function for resettign the player prefs
    public void ResetPlayerPrefs()
    {
        // Delete all player prefs
        // backgroundVolume ; default 0.5
        // difficulty ; default 1
        PlayerPrefs.DeleteAll();

        // Log that the player prefs have been reset
        Debug.Log("Player Prefs Reset");

        // Reset all UI components to their default values
        ResetUI();
    }
    // Function to reset all UI components to their default values
    public void ResetUI()
    {
        // Reset the volume slider to its default value
        _volumeSlider.value = 0.5f;

        // Reset the difficulty dropdown to its default value
        _difficultyDropdown.value = 1;
    }
}
