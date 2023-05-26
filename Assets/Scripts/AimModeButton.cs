using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AimModeButton : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Button button;
    [SerializeField] private Text buttonText;

    private Color KeyColor = new Color(0, 0.5f, 1, 0.6f);  // Green with 70% opacity
    private Color MouseColor = new Color(1, 0.92f, 0.16f, 0.6f);  // Red with 70% opacity

    private void Start()
    {
        buttonText.text = "Mode: Mouse";  // Setting the button text
        UpdateButton();
    }
    // This method updates the button's color and text based on whether the aiming line is enable
    public void UpdateButton()
    {
        // Check if aiming line is enabled and change the color accordingly
        if (playerController.UseKeyAiming)

        {
            button.image.color = KeyColor;
            buttonText.text = "Mode: Key";
        }
        else
        {
            button.image.color = MouseColor;
            buttonText.text = "Mode: Mouse";

        }
    }
    // This method is called when the button is clicked
    public void OnClick()
    {
        playerController.UseKeyAiming = !playerController.UseKeyAiming;  // Toggle the state of aiming line
        UpdateButton();  // Update the button's color and text

        // Deselect the button....
        EventSystem.current.SetSelectedGameObject(null);
    }
}
