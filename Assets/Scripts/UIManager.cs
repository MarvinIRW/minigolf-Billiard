using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Slider shotStrengthSlider;
    [SerializeField] private Text shotsText;
    [SerializeField] private Text statusText;


    void Start()
    {
        shotsText.text = $"Shots: 0";
        statusText.text = "";
    }

    private void LateUpdate()
    {
        UpdateShotStrengthSlider();

    }

    public void UpdateShotsText(int shots)
    {
        shotsText.text = $"Shots: {shots}";
    }

    private void UpdateShotStrengthSlider()
    {
        float shotStrengthPercentage = (playerController.ShotStrength - playerController.MinShotStrength) / (playerController.MaxShotStrength - playerController.MinShotStrength);
        shotStrengthSlider.value = playerController.ShotStrength;
    }

    // DEFINE WIN & GAME OVER TEXT
    public void GameOver()
    {
        statusText.text = "Game Over";
    }

    public void Win()
    {
        statusText.text = "You won :)";
    }
}
