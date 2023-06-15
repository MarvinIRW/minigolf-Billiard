using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private BallManager ballManager;
    [SerializeField] private Slider shotStrengthSlider;
    [SerializeField] private Text shotsText;
    [SerializeField] private Text statusText;
    [SerializeField] private Text canShoot;
    [SerializeField] private Text cameraPositionText;
    // Setup initial UI text 
    void Start()
    {
        shotsText.text = $"Shots: 0";
        shotsText.color = Color.black;
        statusText.text = "";
        statusText.color = Color.black;
        canShoot.text = "No Moving Balls";
        canShoot.color = Color.green;
        cameraPositionText.text = "Camera Pos.: Area View";
        cameraPositionText.color = Color.black;
    }
    // Update the shot strength slider based on the current shot strength
    private void LateUpdate()
    {
        UpdateShotStrengthSlider();
        UpdateIsMoving();
        UpdateCameraPositionText();
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
    // update game camera text
    public void UpdateCameraPositionText()
    {
        switch (gameManager._CameraState)
        {
            case GameManager.CameraState.AreaView:
                cameraPositionText.text = "Camera Pos.: Area";
                break;
            case GameManager.CameraState.CueBallView:
                cameraPositionText.text = "Camera Pos.: Cue Ball";
                break;
            case GameManager.CameraState.FreeView:
                cameraPositionText.text = "Camera Pos.: Free";
                break;
            case GameManager.CameraState.Transition:
                cameraPositionText.text = "Camera Pos.: Transition";
                break;
            default:
                cameraPositionText.text = "Camera Pos.:";
                break;
        }
    }
    // next level is reached
    public void NextLevel(int shotsTaken)
    {
        statusText.color = new Color(0.1f, 0f, 0.75f, 1);
        if (shotsTaken == 1)
        {
            statusText.text = "Amazing Job, you took only one shot!";
        }
        else
        {
            statusText.text = $"Nice Job, you took {shotsTaken} shots!";
        }
    }

    // Updates the status text to Game Over when the game is lost
    public void GameOver()
    {
        statusText.color = new Color(0.75f, 0.35f, 0.35f, 1);
        statusText.text = "Game Over";
    }
    // Updates the status text to You won :) when the game is won
    public void Win()
    {
        statusText.text = "You won :)";
    }
}
