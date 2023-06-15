using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AimLineButtonColor : MonoBehaviour
{
    // Serialized fields for the player controller, button, and button text
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Button _button;
    [SerializeField] private Text _buttonText;

    // Private variables for the active and inactive colors
    private Color _activeColor = new Color(0, 1, 0, 0.7f);  // Green with 70% opacity
    private Color _inactiveColor = new Color(1, 0, 0, 0.7f);  // Red with 70% opacity

    // Start is called before the first frame update
    private void Start()
    {
        _buttonText.text = "Aiming Line";  // Setting the button text
        UpdateButton();
    }

    // This method updates the button's color and text based on whether the aiming line is enabled
    private void UpdateButton()
    {
        // Check if aiming line is enabled and change the color accordingly
        if (_playerController.IsAimingLineEnabled)
        {
            _button.image.color = _activeColor;
        }
        else
        {
            _button.image.color = _inactiveColor;
        }
    }

    // This method is called when the button is clicked
    public void OnClick()
    {
        // Toggle the state of aiming line
        _playerController.IsAimingLineEnabled = !_playerController.IsAimingLineEnabled;
        UpdateButton();  // Update the button's color and text

        // Deselect the button
        EventSystem.current.SetSelectedGameObject(null);
    }
}
