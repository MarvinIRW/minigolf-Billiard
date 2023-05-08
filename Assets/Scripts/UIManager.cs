using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text shotsText;
    [SerializeField] private Text statusText;


    void Start()
    {
        shotsText.text = $"Shots: 0";
        statusText.text = "";
    }

    public void UpdateShotsText(int shots)
    {
        shotsText.text = $"Shots: {shots}";
    }

    // DEFINE WIN & GAME OVER TEXT
    public void gameOver()
    {
        statusText.text = "Game Over";
    }

    public void win()
    {
        statusText.text = "You won :)";
    }
}
