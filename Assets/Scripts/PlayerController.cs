using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CueBallController cueBallController;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float maxShotStrength = 10f;
    [SerializeField] private float minShotStrength = 1f;
    [SerializeField] private float shotStrengthMultiplier = 3f;
    private float mouseDownTime;
    private float shotStrength = 0;

    public float ShotStrength { get { return shotStrength; } }
    public float MinShotStrength { get { return minShotStrength; } }
    public float MaxShotStrength { get { return maxShotStrength; } }


    private void Update()
    {
        

        // Aim the shot using the mouse position
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 aimDirection = (hit.point - cueBallController.transform.position).normalized;

            //get first downpress
            if (Input.GetMouseButtonDown(0))
            {
                mouseDownTime = Time.time;
            }
            // while its pressed (needed for the ui Slider)
            if (Input.GetMouseButton(0))
            {
                float pressDuration = (Time.time - mouseDownTime) + 1;
                shotStrength = Mathf.Clamp(pressDuration * shotStrengthMultiplier, minShotStrength, maxShotStrength);         
            }
            // when released
            if (Input.GetMouseButtonUp(0))
            {
                cueBallController.Shoot(aimDirection * shotStrength);
                shotStrength = 0f;
            }

        }
        
    }
}
