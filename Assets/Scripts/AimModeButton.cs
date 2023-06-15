using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AimModeButton : MonoBehaviour
{
    // Serialized fields for the player controller, button, and button text
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Button _button;
    [SerializeField] private Text _buttonText;

    // Private variables for the key and mouse colors
    private Color _keyColor = new Color(0, 0.5f, 1, 0.6f);  // Blue with 60% opacity
    private Color _mouseColor = new Color(1, 0.92f, 0.16f, 0.6f);  // Yellow with 60% opacity

    // Start is called before the first frame update
    private void Start()
    {
        _buttonText.text = "Mode: Mouse";  // Setting the button text
        UpdateButton();
    }

    // This method updates the button's color and text based on the current aiming mode
    public void UpdateButton()
    {
        // Check if key aiming is enabled and change the color and text accordingly
        if (_playerController.UseKeyAiming)
        {
            _button.image.color = _keyColor;
            _buttonText.text = "Mode: Key";
        }
        else
        {
            _button.image.color = _mouseColor;
            _buttonText.text = "Mode: Mouse";
        }
    }

    // This method is called when the button is clicked
    public void OnClick()
    {
        // Toggle the aiming mode
        _playerController.ToggleAimingMode();

        // Deselect the button
        EventSystem.current.SetSelectedGameObject(null);
    }
}
