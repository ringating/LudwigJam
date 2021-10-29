using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
	public Slider slider;
    private float prevValue;
	private float prevVolume;

	private void Start()
	{
		//slider = GetComponent<Slider>();

		slider.value = SettingsStatic.volume;			// start by inheriting value from the static settings class
		prevValue = SettingsStatic.volume;				//
		prevVolume = SettingsStatic.volume;				//
		AudioListener.volume = SettingsStatic.volume;	//
	}

	private void Update()
	{
		if (slider.value != prevValue)
		{
			SetVolume();
		}

		if (AudioListener.volume != prevVolume)
		{
			SetSlider();
		}
	}

	private void SetVolume()
	{
		SettingsStatic.volume = slider.value;
		AudioListener.volume = slider.value;
		prevValue = slider.value;
		prevVolume = prevValue;
	}

	private void SetSlider()
	{
		slider.value = AudioListener.volume;
		prevValue = slider.value;
		prevVolume = prevValue;
	}

	public float GetSliderValue()
	{
		return slider.value;
	}

	private void OnEnable()
	{
		AudioListener.volume = SettingsStatic.volume;
		slider.value = SettingsStatic.volume;
	}
}
