using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighScoreUpdate : MonoBehaviour
{

    private string _levelName;
    
    // Start is called before the first frame update
    void Start()
    {
        _levelName = gameObject.name;
        Debug.Log("Level name: " + _levelName);
        UpdateHighScores(_levelName);
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
        }
        else
        {
            timeHighScoreDisplay.text = "Time:  None";
        }

        if (currentShotsHighScore != int.MaxValue)
        {
            shotsHighScoreDisplay.text = "Shots: " + currentShotsHighScore;
        }
        else
        {
            shotsHighScoreDisplay.text = "Shots: None";
        }
    }
}
