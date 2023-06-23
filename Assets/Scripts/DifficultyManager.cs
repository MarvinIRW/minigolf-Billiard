using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DifficultyManager : MonoBehaviour
{
    // Serialized field for the dropdown in the UI
    [SerializeField] private TMP_Dropdown _difficultyDropdown;

    // Serialized field for the reference to the Text component
    [SerializeField] private TMP_Text _difficultyText;

    private void Start()
    {
        // Load the saved difficulty level or use a default value
        // 0 = Easy, 1 = Medium, 2 = Hard, 3 = Insane
        _difficultyDropdown.value = PlayerPrefs.GetInt("difficulty", 1);

        // Update the color of the text based on the selected value
        UpdateTextColor(_difficultyDropdown.value);
    }

    public void SetDifficulty()
    {
        // Save the difficulty level selected
        PlayerPrefs.SetInt("difficulty", _difficultyDropdown.value);

        // Update the color of the text based on the selected value
        UpdateTextColor(_difficultyDropdown.value);
    }

    // Method to update the text color based on the difficulty
    private void UpdateTextColor(int difficulty)
    {
        switch (difficulty)
        {
            case 0: // Easy
                _difficultyText.color = Color.green;
                break;
            case 1: // Medium
                _difficultyText.color = Color.yellow;
                break;
            case 2: // Hard
                _difficultyText.color = Color.red;
                break;
            case 3: // Insane
                _difficultyText.color = Color.magenta;
                break;
            default:
                _difficultyText.color = Color.white;
                break;
        }
    }
}