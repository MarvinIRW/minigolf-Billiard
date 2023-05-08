using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CueBallController cueBallController;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float maxShotStrength = 10f;
    [SerializeField] private float minShotStrength = 1f;

    private float shotStrength;

    private void Update()
    {
        // Aim the shot using the mouse position
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 aimDirection = (hit.point - cueBallController.transform.position).normalized;

            // Adjust shot strength using scroll wheel
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            shotStrength = Mathf.Clamp(shotStrength + scrollInput, minShotStrength, maxShotStrength);

            // Shoot the cue ball on left mouse button click
            if (Input.GetMouseButtonDown(0))
            {
                cueBallController.Shoot(aimDirection * shotStrength);
            }
        }
    }
}
