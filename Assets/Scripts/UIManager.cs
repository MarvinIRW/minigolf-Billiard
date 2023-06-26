using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private BallManager _ballManager;
    [SerializeField] private Slider _shotStrengthSlider;
    [SerializeField] private Text _shotsText;
    [SerializeField] private Text _statusText;
    [SerializeField] private Text _canShoot;
    [SerializeField] private Text _cameraPositionText;
    [SerializeField] private GameObject _gameOverWinButtons;

    // Setup initial UI text 
    private void Start()
    {
        // Set the initial UI text
        _shotsText.text = $"Shots: 0";
        _shotsText.color = Color.black;
        _statusText.text = "";
        _statusText.color = Color.black;
        _canShoot.text = "No Moving Balls";
        _canShoot.color = Color.green;
        _cameraPositionText.text = "Camera Pos.: Area View";
        _cameraPositionText.color = Color.black;
        
        _gameOverWinButtons.SetActive(false);
    }

    // Update the shot strength slider based on the current shot strength
    private void LateUpdate()
    {
        UpdateShotStrengthSlider();
        UpdateIsMoving();
        UpdateCameraPositionText();
    }

    // Updates the canShoot text based on whether any balls are moving
    private void UpdateIsMoving()
    {
        if (_ballManager.IsAnyMovement())
        {
            _canShoot.text = "Moving Balls";
            _canShoot.color = Color.red;
        }
        else
        {
            _canShoot.text = "No Moving Balls";
            _canShoot.color = Color.green;
        }
    }

    // Updates the shot counter text 
    public void UpdateShotsText(int shots)
    {
        _shotsText.text = $"Shots: {shots}/{_gameManager.MaxShots}";
    }

    // Updates the shot strength slider based on the current shot strength
    private void UpdateShotStrengthSlider()
    {
        float shotStrengthPercentage = (_playerController.ShotStrength - _playerController.MinShotStrength) / (_playerController.MaxShotStrength - _playerController.MinShotStrength);
        _shotStrengthSlider.value = _playerController.ShotStrength;
    }

    // Updates the camera position text based on the current camera state
    public void UpdateCameraPositionText()
    {
        switch (_gameManager.CurrentCameraState)
        {
            case GameManager.CameraState.AreaView:
                _cameraPositionText.text = "Camera Pos.: Area";
                break;
            case GameManager.CameraState.CueBallView:
                _cameraPositionText.text = "Camera Pos.: Cue Ball";
                break;
            case GameManager.CameraState.FreeView:
                _cameraPositionText.text = "Camera Pos.: Free";
                break;
            case GameManager.CameraState.Transition:
                _cameraPositionText.text = "Camera Pos.: Transition";
                break;
            default:
                _cameraPositionText.text = "Camera Pos.:";
                break;
        }
    }

    // Updates the status text when the next level is reached
    public void NextLevel(int shotsTaken)
    {
        _statusText.color = new Color(0.1f, 0f, 0.75f, 1);
        _statusText.text = shotsTaken == 1 ? "Amazing Job, hole-in-one!" : $"Nice Job, you took {shotsTaken} shots!";
    }

    // Updates the status text to Game Over when the game is lost
    public void GameOver()
    {
        _statusText.color = new Color(0.75f, 0.35f, 0.35f, 1);
        _statusText.text = "Game Over";
        _gameOverWinButtons.SetActive(true);
    }

    // Updates the status text to "You won :)" when the game is won
    public void Win()
    {
        _statusText.text = "You won :)";
    }
}
