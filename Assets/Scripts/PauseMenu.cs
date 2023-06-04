using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private static bool Paused = false;
    private bool optionsMenuOpen = false; // New variable to track if the options menu is open
    [SerializeField] GameObject PausedMenuCanvas;
    [SerializeField] GameObject OptionsMenuCanvas; // Add a reference to your options menu canvas

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (optionsMenuOpen) // If options menu is open, close it
            {
                CloseOptionsMenu();
            }
            else if (Paused)
            {
                Play();
            }
            else
            {
                Stop();
            }
        }
    }

    // New function to open the options menu
    public void OpenOptionsMenu()
    {
        optionsMenuOpen = true;
        OptionsMenuCanvas.SetActive(true);
        PausedMenuCanvas.SetActive(false);
    }

    // New function to close the options menu
    public void CloseOptionsMenu()
    {
        optionsMenuOpen = false;
        OptionsMenuCanvas.SetActive(false);
        PausedMenuCanvas.SetActive(true);

    }

    private void Stop()
    {
        PausedMenuCanvas.SetActive(true);
        Time.timeScale = 0f;
        Paused = true;
    }

    public void Play()
    {
        PausedMenuCanvas.SetActive(false);
        Time.timeScale = 1f;
        Paused = false;
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
