using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CueBallController cueBallController;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameManager gameManager;
    private float maxShotStrength = 20f;
    private float minShotStrength = 1f;
    private float shotStrengthMultiplier = 0.5f;
    private float shotStrengthDirection = 1f;
    private float mouseDownTime;
    private float shotStrength = 0;
    private int shotsTaken = 0;

    // Properties for other scripts to access variables
    public float ShotStrength { get { return shotStrength; } }
    public float MinShotStrength { get { return minShotStrength; } }
    public float MaxShotStrength { get { return maxShotStrength; } }
    public int ShotsTaken { get { return shotsTaken; } }
    public CueBallController CueBallController { get { return cueBallController; } }
    public Camera MainCamera { get { return mainCamera; } }

    private void Update()
    {
        // Only proceed if not in free view
        if (gameManager.CameraView != 2)
        {
            // Aim the shot using the mouse position
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 aimDirection = (hit.point - cueBallController.transform.position).normalized;

                // Check if the cue ball is stationary otherwise no shot possible
                if (cueBallController.IsStationary())
                {
                    // On mouse down, start measuring the shot strength
                    if (Input.GetMouseButtonDown(0))
                    {
                        mouseDownTime = Time.time;
                        shotStrength = 0f;
                    }
                    // While mouse button is held down, calculate the shot strength based on time
                    if (Input.GetMouseButton(0))
                    {
                        float pressDuration = (Time.time - mouseDownTime);
                        //float shotStrengthNormalized = Mathf.Clamp01(pressDuration * shotStrengthMultiplier);
                        //shotStrength = minShotStrength + Mathf.Pow(shotStrengthNormalized, 2) * (maxShotStrength - minShotStrength);

                        float shotStrengthChange = pressDuration * shotStrengthMultiplier * shotStrengthDirection;
                        shotStrength = Mathf.Clamp(shotStrength + shotStrengthChange, minShotStrength, maxShotStrength);

                        // Flip the direction if we hit the minimum or maximum shot strength
                        if (shotStrength >= maxShotStrength && shotStrengthDirection > 0 || shotStrength <= minShotStrength && shotStrengthDirection < 0)
                        {
                            shotStrengthDirection *= -1f;
                            mouseDownTime = Time.time;
                        }
                    }
                    // On mouse up, shoot the cue ball and increment shots taken counter
                    if (Input.GetMouseButtonUp(0))
                    {
                        cueBallController.Shoot(aimDirection * shotStrength);
                        shotsTaken++;
                    }
                }
            }
        }
    }
        
}
