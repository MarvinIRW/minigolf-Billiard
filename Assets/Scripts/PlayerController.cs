using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CueBallController cueBallController;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private BallManager ballManager;
    private float maxShotStrength = 20f;
    private float minShotStrength = 1f;
    private float shotStrengthMultiplier = 0.5f;
    private float shotStrengthDirection = 1f;
    private float mouseDownTime;
    private float shotStrength = 0;
    private int shotsTaken = 0;
    // for key aiming
    private bool useKeyAiming = false;
    [SerializeField] private float rotationSpeedKeyAiming = 50f;
    [SerializeField] private GameObject eightBall;
    private Vector3 aimDirectionKey;
    private float accumulatedKeyPressTime = 0f;
    [SerializeField] AimModeButton aimModeButton;

    //aiming line
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private int aimingLineLength = 20;

    // Properties for other scripts to access variables
    public float ShotStrength { get { return shotStrength; } }
    public float MinShotStrength { get { return minShotStrength; } }
    public float MaxShotStrength { get { return maxShotStrength; } }
    public int ShotsTaken { get { return shotsTaken; } }
    public CueBallController CueBallController { get { return cueBallController; } }
    public Camera MainCamera { get { return mainCamera; } }
    public bool IsAimingLineEnabled { get; set; } = true;
    public bool UseKeyAiming { get { return useKeyAiming; } set { useKeyAiming = value; } }


    private void Update()
    {
        //prevent nested if statements
        // Check if the cue ball is stationary otherwise no shot
        if (ballManager.IsAnyMovement())
        {
            // if balls moving dont show aiming line
            UpdateAimingLine(null);
            return;
        }
        // toggle between aiming control methods
        if (Input.GetKeyDown(KeyCode.R))
        {
            ToggleAimingMode();
        }
        // if Player uses KEYS to aim:
        if (useKeyAiming)
        {
            if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E))
            {
                // Add the time passed since the last frame to accumulatedKeyPressTime.
                accumulatedKeyPressTime += Time.deltaTime;

                // Calculate the increased rotation speed.
                float increasedRotationSpeed = rotationSpeedKeyAiming * accumulatedKeyPressTime * 2;

                // Determine the rotation angle.
                float rotationAngle = increasedRotationSpeed * Time.deltaTime;

                // Determine the rotation direction based on which key is pressed.
                int rotationDirection = Input.GetKey(KeyCode.Q) ? 1 : -1;

                // Rotate the aiming direction.
                aimDirectionKey = Quaternion.Euler(0, rotationAngle * rotationDirection, 0) * aimDirectionKey;
                Debug.Log("rotating key aiming");

            }
            else
            {
                // If neither Q nor E is pressed, reset accumulatedKeyPressTime.
                accumulatedKeyPressTime = 0f;
            }

            // Calculate a point in the world along the aimDirection from the cue ball
            Vector3 aimPoint = cueBallController.transform.position + aimDirectionKey * 1;
            UpdateAimingLine(aimPoint);
            Debug.Log("key aiming pre shot");


            HandleShooting(useKeyAiming, aimDirectionKey); // Handle shooting with keyboard aiming
            return; // don't further go down and use mouse aiming
        }

        // only proceed if you actually aim with mouse 
        // Aim the shot using the mouse position
        // Only proceed if not over UI
        if (IsPointerOverUIButton())
        {
            Debug.Log("over UI");
            return;
        }

        Plane aimPlane = new Plane(Vector3.up, cueBallController.transform.position);
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        // The ray does not intersects the plane
        if (!aimPlane.Raycast(ray, out float distance))
        {
            UpdateAimingLine(null);
            return;
        }

        Vector3 hitPoint = ray.GetPoint(distance);
        Vector3 aimDirectionMouse = (hitPoint - cueBallController.transform.position).normalized;
        aimDirectionMouse.y = 0;  // Make sure Y direction is always 0

        UpdateAimingLine(hitPoint);

        // Handle shooting with mouse aiming
        HandleShooting(useKeyAiming, aimDirectionMouse);
    }

    // can shoot with space or mouse, depending on the useKeyAiming state
    private void HandleShooting(bool useSpaceAiming, Vector3 aimDirection)
    {
        // Determine the relevant input methods based on the aiming mode
        bool inputDown = useSpaceAiming ? Input.GetKeyDown(KeyCode.Space) : Input.GetMouseButtonDown(0);
        bool inputHeld = useSpaceAiming ? Input.GetKey(KeyCode.Space) : Input.GetMouseButton(0);
        bool inputUp = useSpaceAiming ? Input.GetKeyUp(KeyCode.Space) : Input.GetMouseButtonUp(0);

        // If input is initially pressed, save the balls positions and record the time
        if (inputDown)
        {
            ballManager.StoreInitialState();
            mouseDownTime = Time.time;
            shotStrength = 0f;
            Debug.Log("input down");

        }

        // While input is held, calculate the shot strength based on time
        if (inputHeld)
        {
            float pressDuration = (Time.time - mouseDownTime);
            float shotStrengthChange = pressDuration * shotStrengthMultiplier * shotStrengthDirection;
            shotStrength = Mathf.Clamp(shotStrength + shotStrengthChange, minShotStrength, maxShotStrength);

            // Flip the direction if we hit the minimum or maximum shot strength
            if (shotStrength >= maxShotStrength && shotStrengthDirection > 0 || shotStrength <= minShotStrength && shotStrengthDirection < 0)
            {
                shotStrengthDirection *= -1f;
                mouseDownTime = Time.time;
            }
            Debug.Log("shot strength up");

        }

        // If input is released, shoot the cue ball and increment shots taken counter
        if (inputUp)
        {
            Debug.Log("shooting");

            cueBallController.Shoot(aimDirection * shotStrength);
            shotsTaken++;
        }
    }

    public void ToggleAimingMode()
    {
        // Initialize aiming direction
        aimDirectionKey = (eightBall.transform.position - cueBallController.transform.position).normalized;
        aimDirectionKey.y = 0;  // Make sure Y direction is always 0
        useKeyAiming = !useKeyAiming;
        aimModeButton.UpdateButton();
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
            direction.y = 0; // Make sure Y direction is always 0
            Vector3 reflectedDirection = direction;
            Vector3 currentPosition = startPosition;
            Vector3 endPosition = currentPosition;

            List<Vector3> positions = new List<Vector3>();
            positions.Add(startPosition);

            float totalRaycastLength = aimingLineLength; // Max length of all raycasts combined

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
                        reflectedDirection.y = 0; // Ensure the reflected direction remains horizontal
                        totalRaycastLength -= hit.distance; // Subtract the distance traveled from the total
                        positions.Add(endPosition);
                    }
                    else
                    {
                        // Handle case of hitting non-"GameEnvironment" objects
                        endPosition = hit.point;
                        positions.Add(endPosition);
                        break;
                    }
                }
                else
                {
                    // If the raycast did not hit, extend line by the remaining length
                    endPosition = currentPosition + reflectedDirection * totalRaycastLength;
                    positions.Add(endPosition);
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
