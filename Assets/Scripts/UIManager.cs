using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private BallManager ballManager;
    [SerializeField] private Slider shotStrengthSlider;
    [SerializeField] private Text shotsText;
    [SerializeField] private Text statusText;
    [SerializeField] private Text canShoot;

    // Setup initial UI text 
    void Start()
    {
        shotsText.text = $"Shots: 0";
        statusText.text = "";
        canShoot.text = "No Moving Balls";
        canShoot.color = Color.green;
    }

    // Update the shot strength slider based on the current shot strength
    private void LateUpdate()
    {
        UpdateShotStrengthSlider();
        UpdateIsMoving();
    }

    //Updates the canShoot text
    private void UpdateIsMoving()
    {
        if (ballManager.IsAnyMovement())
        {
            canShoot.text = "Moving Balls";
            canShoot.color = Color.red;
        }
        else
        {
            canShoot.text = "No Moving Balls";
            canShoot.color = Color.green;
        }
    }

    // Updates the shot counter text 
    public void UpdateShotsText(int shots)
    {
        shotsText.text = $"Shots: {shots}";
    }

    // Updates the shot strength slider based on the current shot strength
    private void UpdateShotStrengthSlider()
    {
        float shotStrengthPercentage = (playerController.ShotStrength - playerController.MinShotStrength) / (playerController.MaxShotStrength - playerController.MinShotStrength);
        shotStrengthSlider.value = playerController.ShotStrength;
    }



    // Updates the status text to Game Over when the game is lost
    public void GameOver()
    {
        statusText.text = "Game Over";
    }

    // Updates the status text to You won :) when the game is won
    public void Win()
    {
        statusText.text = "You won :)";
    }
}
