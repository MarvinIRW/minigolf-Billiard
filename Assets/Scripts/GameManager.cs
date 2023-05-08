using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] private UIManager _UIManager;
    [SerializeField] private int maxShots = 10;

    private int shotsTaken = 0;
    private bool gameOver = false;

    private void Update()
    {
        if (gameOver) return;

        if (Input.GetMouseButtonDown(0))
        {
            shotsTaken++;
            _UIManager.UpdateShotsText(shotsTaken);
            CheckGameStatus();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("EightBall"))
        {
            EightBallInHole();
        }
    }

    private void EightBallInHole()
    {
        Debug.Log("Eight-ball in the hole!");
        gameOver = true;
        CheckGameStatus();
    }

    private void CheckGameStatus()
    {
        if (gameOver && shotsTaken <= maxShots)
        {
            Debug.Log("You won!");
            _UIManager.Win();
            // Progress to the next level or show a victory screen
        }
        else if (shotsTaken >= maxShots)
        {
            Debug.Log("You lost!");
            gameOver = true;
            _UIManager.GameOver();
            // Show a game over screen or restart the level
        }
    }

    public int getShotsTaken()
    {
        return shotsTaken;
    }
}