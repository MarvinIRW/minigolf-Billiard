using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    // Static variable to track if the game is paused
    public static bool Paused = false;

    // Private variable to track if the options menu is open
    private bool _optionsMenuOpen = false;

    // Serialized fields for the pause and options menu canvases
    [SerializeField] private GameObject _pausedMenuCanvas;
    [SerializeField] private GameObject _optionsMenuCanvas;

    // Called before the first frame update
    void Start()
    {
        // Set the game to not paused and the time scale to normal
        Paused = false;
        Time.timeScale = 1f;
    }

    // Called once per frame
    void Update()
    {
        // Check if the escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // If the options menu is open, close it
            if (_optionsMenuOpen)
            {
                CloseOptionsMenu();
            }
            // If the game is paused, unpause it
            else if (Paused)
            {
                PlayEsc();
            }
            // Otherwise, pause the game
            else
            {
                Stop();
            }
        }
    }

    // Function to open the options menu
    public void OpenOptionsMenu()
    {
        _optionsMenuOpen = true;
        _optionsMenuCanvas.SetActive(true);
        _pausedMenuCanvas.SetActive(false);
    }

    // Function to close the options menu
    public void CloseOptionsMenu()
    {
        _optionsMenuOpen = false;
        _optionsMenuCanvas.SetActive(false);
        _pausedMenuCanvas.SetActive(true);
    }

    // Function to pause the game
    private void Stop()
    {
        _pausedMenuCanvas.SetActive(true);
        Time.timeScale = 0f;
        Paused = true;
    }

    // Function to be called by the play button
    public void PlayButton()
    {
        StartCoroutine(PlayMouse());
    }

    // Function to unpause the game when the escape key is pressed
    public void PlayEsc()
    {
        _pausedMenuCanvas.SetActive(false);
        Time.timeScale = 1f;
        Paused = false;
    }

    // Coroutine to unpause the game when the mouse is used
    private IEnumerator PlayMouse()
    {
        _pausedMenuCanvas.SetActive(false);
        Time.timeScale = 1f;
        yield return new WaitForSeconds(0.1f);  // Waits for 0.1 seconds (bug fix - shooting when exiting pause menu with mouse)
        Paused = false;
    }

    // Function to return to the main menu
    public void MainMenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
