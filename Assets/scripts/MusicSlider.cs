using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicSlider : MonoBehaviour
{
	public Slider slider;
	private float prevValue;
	private float prevVolume;
	private AudioSource musicPlayer;

	private void Start()
	{
		//slider = GetComponent<Slider>();
		slider.value = SettingsStatic.musicVolume; // start by inheriting value from the static settings class

		if (!musicPlayer)
		{
			musicPlayer = FindObjectsOfType<MusicObject>()[0].musicSource;
		}

		SetVolume();
	}

	private void Update()
	{
		if (slider.value != prevValue)
		{
			SetVolume();
		}

		if (musicPlayer.volume != prevVolume)
		{
			SetSlider();
		}
	}

	private void SetVolume()
	{
		SettingsStatic.musicVolume = slider.value;
		musicPlayer.volume = slider.value;
		prevValue = slider.value;
		prevVolume = prevValue;
	}

	private void SetSlider()
	{
		slider.value = musicPlayer.volume;
		prevValue = slider.value;
		prevVolume = prevValue;
	}

	public float GetSliderValue()
	{
		return slider.value;
	}

	private void OnEnable()
	{
		if (!musicPlayer)
		{
			musicPlayer = FindObjectsOfType<MusicObject>()[0].musicSource;
		}
		SetSlider();
	}
}
