using UnityEngine;
using UnityEngine.UI;

public class AimButtonColor : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Button button;
    [SerializeField] private Text buttonText;

    private Color activeColor = new Color(0, 1, 0, 0.7f);  // Green with 70% opacity
    private Color inactiveColor = new Color(1, 0, 0, 0.7f);  // Red with 70% opacity

    private void Start()
    {
        buttonText.text = "Aiming Line";  // Setting the button text
        UpdateButton();
    }

    // This method updates the button's color and text based on whether the aiming line is enable
    private void UpdateButton()
    {
        

        // Check if aiming line is enabled and change the color accordingly
        if (playerController.IsAimingLineEnabled)
        {
            button.image.color = activeColor;
        }
        else
        {
            button.image.color = inactiveColor;
        }
    }

    // This method is called when the button is clicked
    public void OnClick()
    {
        playerController.IsAimingLineEnabled = !playerController.IsAimingLineEnabled;  // Toggle the state of aiming line
        UpdateButton();  // Update the button's color and text
    }
}
