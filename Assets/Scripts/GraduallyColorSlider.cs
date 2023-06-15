using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraduallyColorSlider : MonoBehaviour
{
    // Serialized fields for the slider and the slider fill image
    [SerializeField] private Slider _slider;
    [SerializeField] private Image _sliderFill;

    private void Start()
    {
        // Adds a listener to the slider and invokes a method when the value changes
        _slider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
    }

    // Invoked when the value of the slider changes
    public void ValueChangeCheck()
    {
        // Change color of the slider fill from green to red based on the current slider value
        _sliderFill.color = Color.Lerp(Color.green, Color.red, _slider.value / 10);
    }
}
