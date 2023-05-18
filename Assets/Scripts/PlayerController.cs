using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

    //aiming line
    [SerializeField] private LineRenderer lineRenderer;

    // Properties for other scripts to access variables
    public float ShotStrength { get { return shotStrength; } }
    public float MinShotStrength { get { return minShotStrength; } }
    public float MaxShotStrength { get { return maxShotStrength; } }
    public int ShotsTaken { get { return shotsTaken; } }
    public CueBallController CueBallController { get { return cueBallController; } }
    public Camera MainCamera { get { return mainCamera; } }
    public bool IsAimingLineEnabled { get; set; } = true;


    private void Update()
    {
        //prevent nested if statements
        
        // Check if the cue ball is stationary otherwise no shot
        if (!cueBallController.IsStationary())
        {
            // if balls moving dont show aiming line
            UpdateAimingLine(null);
            return;
        }
        // only proceed if you actually aim
        // Aim the shot using the mouse position
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit))
        {
            // if not aiming on abject no display line
            UpdateAimingLine(null);
            return;
        }
        UpdateAimingLine(hit.point);
        /*// Only proceed if not in free view
        if (gameManager.cameraState == GameManager.CameraState.FreeView)
        {
            Debug.Log("freecam or over UI");
            return;
        }*/
        // Only proceed if not over UI
        if (IsPointerOverUIButton())
        {
            Debug.Log("over UI");
            return;
        }
        // Aim the shot using the mouse position
        Vector3 aimDirection = (hit.point - cueBallController.transform.position).normalized;
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

    //daws the aiming line if aming //fuck this thing..
    private void UpdateAimingLine(Vector3? worldPoint)
    {
        if (worldPoint == null || !IsAimingLineEnabled)
        {
            // If no point to aim at or if the aiming line is disabled, disable the line renderer
            lineRenderer.enabled = false;
        }
        else
        {
            Vector3 startPosition = cueBallController.transform.position;
            // Calculate direction from the start position (cue ball) to the aim point
            Vector3 direction = ((Vector3)worldPoint - startPosition).normalized;
            Vector3 reflectedDirection = direction;
            Vector3 currentPosition = startPosition;
            Vector3 endPosition = currentPosition;

            List<Vector3> positions = new List<Vector3>();
            positions.Add(startPosition);

            float totalRaycastLength = 20; // Max length of all raycasts combined

            for (int i = 0; i < 10; i++) // Limit to 10 reflections for performance reasons
            {
                // Cast ray from current position in the reflected direction, up to the remaining length of totalRaycastLength
                Ray ray = new Ray(currentPosition, reflectedDirection);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, totalRaycastLength))
                {
                    // Check if the raycast hit an object tagged as "GameEnvironment"
                    if (hit.collider.CompareTag("GameEnvironment"))
                    {
                        endPosition = hit.point;
                        reflectedDirection = Vector3.Reflect(reflectedDirection, hit.normal);
                        totalRaycastLength -= hit.distance; // Subtract the distance traveled from the total

                        // Cast a ray downwards to check for the ground and its tag
                        RaycastHit groundHit;
                        if (Physics.Raycast(endPosition, Vector3.down, out groundHit) && groundHit.collider.CompareTag("Ground"))
                        {
                            float groundHeight = groundHit.point.y;
                            // Only allow line position if it's within a certain range from the ground
                            if (Mathf.Abs(endPosition.y - groundHeight) < 2f)
                            {
                                positions.Add(endPosition);
                            }
                        }
                    }
                    // This will handle the case of the ray hitting the eightball, hole or ground directly
                    else if (hit.collider.CompareTag("Ground") || hit.collider.CompareTag("EightBall") || hit.collider.CompareTag("Hole"))
                    {
                        endPosition = hit.point;
                        positions.Add(endPosition);
                        break;
                    }
                }
                else
                {
                    // If the raycast did not hit, extend line by the remaining length
                    endPosition = currentPosition + reflectedDirection * totalRaycastLength;
                    totalRaycastLength = 0; // Set the remaining length to 0
                }

                currentPosition = endPosition;

                // If all the length has been used, stop looping
                if (totalRaycastLength <= 0)
                    break;
            }

            lineRenderer.positionCount = positions.Count;
            lineRenderer.SetPositions(positions.ToArray());
            lineRenderer.enabled = true;
        }
    }


    // checking if mouse if over stuff that could be clicked
    private bool IsPointerOverUIButton()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        return results.Exists(result => result.gameObject.CompareTag("UIBlocking"));
    }


}
