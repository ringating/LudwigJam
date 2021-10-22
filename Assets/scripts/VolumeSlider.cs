using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
	private Slider slider;
    private float prevValue;
	private float prevVolume;

	private void Start()
	{
		slider = GetComponent<Slider>();
		SetVolume();
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
}
