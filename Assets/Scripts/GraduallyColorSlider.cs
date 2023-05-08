using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraduallyColorSlider : MonoBehaviour
{
	public Slider Slider;
	public Image sliderFill;

	public void Start()
	{
		//Adds a listener to the main slider and invokes a method when the value changes.
		Slider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
	}

	// Invoked when the value of the slider changes.
	public void ValueChangeCheck()
	{
		sliderFill.color = Color.Lerp(Color.green, Color.red, Slider.value/10);
	}
}