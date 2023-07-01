using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighScoreUpdate : MonoBehaviour
{

    private string _levelName;
    // flag to determine if the level is selectable (beaten once before)
    private bool _selectable = false;
    
    // Start is called before the first frame update
    void Start()
    {
        _levelName = gameObject.name;
        Debug.Log("Level name: " + _levelName);
        UpdateHighScores(_levelName);
        DisableButton();
    }

    void UpdateHighScores(string levelName)
    {
        // Create unique keys for time and shots high scores per level
        string timeKey = levelName + "_TimeHighScore";
        string shotsKey = levelName + "_ShotsHighScore";

        // Get the current high scores, default to infinity for time and shots
        float currentTimeHighScore = PlayerPrefs.GetFloat(timeKey, float.MaxValue);
        int currentShotsHighScore = PlayerPrefs.GetInt(shotsKey, int.MaxValue);

        // Get TMP text objects for the high score displays
        TextMeshProUGUI timeHighScoreDisplay = transform.Find("HighScoreText/TimeHighScoreDisplay").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI shotsHighScoreDisplay = transform.Find("HighScoreText/ShotsHighScoreDisplay").GetComponent<TextMeshProUGUI>();
        
        Debug.Log("Time high score: " + currentTimeHighScore);
        Debug.Log("Shots high score: " + currentShotsHighScore);

        // Update the high score displays
        if (currentTimeHighScore != float.MaxValue)
        {
            timeHighScoreDisplay.text = "Time:  " + currentTimeHighScore +"s";
            // if the time high score is not infinity, the level is selectable
            _selectable = true;
        }
        else
        {
            timeHighScoreDisplay.text = "Time:  None";
        }

        if (currentShotsHighScore != int.MaxValue)
        {
            shotsHighScoreDisplay.text = "Shots: " + currentShotsHighScore;
            // if the shots high score is not infinity, the level is selectable
            _selectable = true;
        }
        else
        {
            shotsHighScoreDisplay.text = "Shots: None";
        }
    }
    // function to disable the level select button if the level is not selectable
    private void DisableButton()
    {
        if (!_selectable)
        {
            gameObject.GetComponent<UnityEngine.UI.Button>().interactable = false;
            //change color of button to grey
            gameObject.GetComponent<UnityEngine.UI.Image>().color = new Color32(128, 128, 128, 255);
        }
    }
}
