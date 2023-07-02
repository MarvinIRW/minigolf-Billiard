using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        //Debug.Log("Level name: " + _levelName);
        UpdateHighScores(_levelName);
        DisableButton();
    }

    private void UpdateHighScores(string levelName)
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
        
        //Debug.Log("Time high score: " + currentTimeHighScore);
        //Debug.Log("Shots high score: " + currentShotsHighScore);

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
    // function to disable the level select button if the level is not selectable
    private void DisableButton()
    {
        // check if the level is selectable by checking if levelReached is equal to or greater
        int levelIndex = int.Parse(_levelName.Substring(_levelName.Length - 2));
        if (PlayerPrefs.GetInt("levelReached") >= levelIndex)
        {
            _selectable = true;
        }
        else
        {
            _selectable = false;
        }
        
        //Debug.Log("Level " + gameObject.transform.GetSiblingIndex() + " is selectable: " + _selectable);
        if (!_selectable)
        {
            gameObject.GetComponent<UnityEngine.UI.Button>().interactable = false;
            //change color of button to grey
            gameObject.GetComponent<UnityEngine.UI.Image>().color = new Color32(128, 128, 128, 255);
        }else
        {
            gameObject.GetComponent<UnityEngine.UI.Button>().interactable = true;
            //change color of button to brown
            gameObject.GetComponent<UnityEngine.UI.Image>().color = new Color32(107, 82, 46, 255);
        }
    }
}
