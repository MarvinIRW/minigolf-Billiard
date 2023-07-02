using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using UnityEngine.Rendering.PostProcessing;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CueBallController _cueBallController;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private BallManager _ballManager;
    [SerializeField] private GameObject _eightBall;
    [SerializeField] private AimModeButton _aimModeButton;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private int _aimingLineLength = 20;
    [SerializeField] private float _rotationSpeedKeyAiming = 50f;
    [SerializeField] private float _maxShotStrength = 20f;
    [SerializeField] private float _minShotStrength = 0.5f;


    private float _shotStrengthMultiplier;
    private float _shotStrengthDirection;
    private float _mouseDownTime;
    private float _shotStrength;
    private int _shotsTaken;
    private bool _useKeyAiming;
    private Vector3 _aimDirectionKey;
    private float _accumulatedKeyPressTime;
    private bool _isShooting;


    // Properties for other scripts to access variables
    public float ShotStrength { get { return _shotStrength; } }
    public float MinShotStrength { get { return _minShotStrength; } }
    public float MaxShotStrength { get { return _maxShotStrength; } }
    public int ShotsTaken { get { return _shotsTaken; } }
    public CueBallController CueBallController { get { return _cueBallController; } }
    public Camera MainCamera { get { return _mainCamera; } }
    public bool IsAimingLineEnabled { get; set; } = true;
    public bool UseKeyAiming { get { return _useKeyAiming; } set { _useKeyAiming = value; } }
    public bool IsShooting { get { return _isShooting; } set { _isShooting = value; } }

    private void Start()
    {
        // Set initial values
        _shotStrengthMultiplier = 0.5f;
        _shotStrengthDirection = 1f;
        _shotStrength = 0;
        _shotsTaken = 0;
        _useKeyAiming = false;
        _accumulatedKeyPressTime = 0f;
    }
    private void Update()
    {
        // If game is paused, do nothing
        if (PauseMenu.Paused) return;

        // if game is over, do nothing
        if (_gameManager.GameOver) return;

        // Check if player is shooting
        if (_isShooting)
        {
            return;
        }

        // Toggle between aiming control methods
        if (Input.GetKeyDown(KeyCode.R))
        {
            ToggleAimingMode();
        }

        // If Player uses KEYS to aim:
        if (_useKeyAiming)
        {
            HandleKeyAiming();
            return; // Don't further go down and use mouse aiming
        }

        // Only proceed if you actually aim with mouse 
        HandleMouseAiming();
    }

    // Handle aiming and shooting with keyboard
    private void HandleKeyAiming()
    {
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E))
        {
            // Add the time passed since the last frame to accumulatedKeyPressTime.
            _accumulatedKeyPressTime += Time.deltaTime;

            // Calculate the increased rotation speed.
            float increasedRotationSpeed = _rotationSpeedKeyAiming * _accumulatedKeyPressTime * 2;

            // Determine the rotation angle.
            float rotationAngle = increasedRotationSpeed * Time.deltaTime;

            // Determine the rotation direction based on which key is pressed.
            int rotationDirection = Input.GetKey(KeyCode.Q) ? -1 : 1;

            // Rotate the aiming direction.
            _aimDirectionKey = Quaternion.Euler(0, rotationAngle * rotationDirection, 0) * _aimDirectionKey;
        }
        else
        {
            // If neither Q nor E is pressed, reset accumulatedKeyPressTime.
            _accumulatedKeyPressTime = 0f;
        }
        // Calculate a point in the world along the aimDirection from the cue ball
        Vector3 aimPoint = _cueBallController.transform.position + _aimDirectionKey * 1;
        UpdateAimingLine(aimPoint);
        HandleShooting(_useKeyAiming, _aimDirectionKey); // Handle shooting with keyboard aiming
    }

    // Handle aiming and shooting with mouse
    private void HandleMouseAiming()
    {
        // Aim the shot using the mouse position
        // Only proceed if not over UI
        if (IsPointerOverUIButton())
        {
            return;
        }

        Plane aimPlane = new Plane(Vector3.up, _cueBallController.transform.position);
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        // The ray does not intersects the plane
        if (!aimPlane.Raycast(ray, out float distance))
        {
            UpdateAimingLine(null);
            return;
        }

        Vector3 hitPoint = ray.GetPoint(distance);
        Vector3 aimDirectionMouse = (hitPoint - _cueBallController.transform.position).normalized;
        aimDirectionMouse.y = 0;  // Make sure Y direction is always 0

        UpdateAimingLine(hitPoint);

        // Handle shooting with mouse aiming
        HandleShooting(_useKeyAiming, aimDirectionMouse);
    }

    // Handle shooting with space or mouse, depending on the useKeyAiming state
    private void HandleShooting(bool useSpaceAiming, Vector3 aimDirection)
    {
        // Determine the relevant input methods based on the aiming mode
        bool inputDown = useSpaceAiming ? Input.GetKeyDown(KeyCode.Space) : Input.GetMouseButtonDown(0);
        bool inputHeld = useSpaceAiming ? Input.GetKey(KeyCode.Space) : Input.GetMouseButton(0);
        bool inputUp = useSpaceAiming ? Input.GetKeyUp(KeyCode.Space) : Input.GetMouseButtonUp(0);

        // If input is initially pressed, save the balls positions and record the time
        if (inputDown)
        {
            _ballManager.StoreInitialState();
            _mouseDownTime = Time.time;
            _shotStrength = 0f;
        }

        // While input is held, calculate the shot strength based on time
        if (inputHeld)
        {
            float pressDuration = (Time.time - _mouseDownTime);
            float shotStrengthChange = pressDuration * _shotStrengthMultiplier * _shotStrengthDirection;
            _shotStrength = Mathf.Clamp(_shotStrength + shotStrengthChange, _minShotStrength, _maxShotStrength);

            // Flip the direction if we hit the minimum or maximum shot strength
            if (_shotStrength >= _maxShotStrength && _shotStrengthDirection > 0 || _shotStrength <= _minShotStrength && _shotStrengthDirection < 0)
            {
                _shotStrengthDirection *= -1f;
                _mouseDownTime = Time.time;
            }
        }

        // If input is released, shoot the cue ball and increment shots taken counter
        if (inputUp)
        {
            _cueBallController.Shoot(aimDirection * _shotStrength);
            _shotsTaken++;
            UpdateAimingLine(null);
            _isShooting = true;
            StartCoroutine(WaitForBallsToMove());
        }
    }

    // Balls dont regester movement instantly so we need to wait a bit
    private IEnumerator WaitForBallsToMove()
    {
        yield return new WaitForSeconds(0.1f);
        while (_ballManager.IsAnyMovement())
        {
            yield return null; // Wait for next frame
        }

        _isShooting = false;
    }

    public void ToggleAimingMode()
    {
        // Initialize aiming direction
        _aimDirectionKey = (_eightBall.transform.position - _cueBallController.transform.position).normalized;
        _aimDirectionKey.y = 0;  // Make sure Y direction is always 0
        _useKeyAiming = !_useKeyAiming;
        _aimModeButton.UpdateButton();
    }

    // Draws the aiming line if aiming
    private void UpdateAimingLine(Vector3? worldPoint)
    {
        if (worldPoint == null || !IsAimingLineEnabled)
        {
            // If no point to aim at or if the aiming line is disabled, disable the line renderer
            _lineRenderer.enabled = false;
        }
        else
        {
            Vector3 startPosition = _cueBallController.transform.position;
            // Calculate direction from the start position (cue ball) to the aim point
            Vector3 direction = ((Vector3)worldPoint - startPosition).normalized;
            direction.y = 0; // Make sure Y direction is always 0
            Vector3 reflectedDirection = direction;
            Vector3 currentPosition = startPosition;
            Vector3 endPosition = currentPosition;

            List<Vector3> positions = new List<Vector3>();
            positions.Add(startPosition);

            float totalRaycastLength = _aimingLineLength; // Max length of all raycasts combined

            for (int i = 0; i < 10; i++) // Limit to 10 reflections for performance reasons
            {
                // Cast ray from current position in the reflected direction, up to the remaining length of totalRaycastLength
                Ray ray = new Ray(currentPosition, reflectedDirection);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, totalRaycastLength))
                {
                    // Check if the raycast hit an object tagged as "GameEnvironment" or "Hole"
                    if (hit.collider.CompareTag("GameEnvironment") || hit.collider.CompareTag("Hole"))
                    {
                        endPosition = hit.point;
                        // If the hit object is not a hole, reflect the direction
                        if (!hit.collider.CompareTag("Hole"))
                        {
                            reflectedDirection = Vector3.Reflect(reflectedDirection, hit.normal);
                            reflectedDirection.y = 0; // Ensure the reflected direction remains horizontal
                        }
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
            _lineRenderer.positionCount = positions.Count;
            _lineRenderer.SetPositions(positions.ToArray());
            _lineRenderer.enabled = true;
        }
    }

    // Checks if mouse if over UI elements that could be clicked
    private bool IsPointerOverUIButton()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        return results.Exists(result => result.gameObject.CompareTag("UIBlocking"));
    }
}
