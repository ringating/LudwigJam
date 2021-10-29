using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Victory : MonoBehaviour
{
	public Image blackness;
	public Text youWin;
	public Text time;
	public AudioSource voidSound;
	private AudioSource musicSource;
	public AudioSource eraserSoundSource;
	public GameObject mainMenuButton;
	

	[HideInInspector]
	public bool victory = false;
	private bool oneTimeStuffDone = false;
	private float startTime;
	private float finalTime;
	private float fadeSpeed = 0.2f;
	private string timeString;

	private float endMusicVolume;

	private void Start()
	{
		startTime = Time.unscaledTime;

		musicSource = FindObjectsOfType<MusicObject>()[0].musicSource;
	}

	private void Update()
	{
		if (victory)
		{
			// multi frame stuff



			// fade out music
			// fade out eraser
			FadeOutMusicAndEraser(Time.unscaledDeltaTime);

			// fade in void sound loop
			FadeInVoidSound(Time.unscaledDeltaTime);


			// fade in black screen
			// enable time + text once black screen reaches full opacity
			FadeInCanvasElements(Time.unscaledDeltaTime);


			// bring up the main menu button, on an extra delay compared to the win text
			if (Time.unscaledTime > finalTime + (1/fadeSpeed) + 2f)
			{
				mainMenuButton.SetActive(true);
				Cursor.lockState = CursorLockMode.None;
			}
		}
	}

	public void StartVictorySequence()
    {
        victory = true;

		if (!oneTimeStuffDone)
		{
			oneTimeStuffDone = true;

			finalTime = Time.unscaledTime;

			timeString = GetTimeString(finalTime, true);

			// enable black screen
			blackness.gameObject.SetActive(true);

			// start void sound (0 volume) + eraser sound (default volume)
			voidSound.volume = 0;
			voidSound.Play();
			eraserSoundSource.Play();

			// set time text
			time.text = timeString;

			// store volume
			endMusicVolume = musicSource.volume;
		}
    }

	public string GetTimeString(float currTime, bool includeMs)
	{
		float totalSeconds = currTime - startTime;

		int hours = (int) (totalSeconds / (60f * 60f));
		//print("total seconds: " + totalSeconds + ", hours: " + hours);
		int minutes = (int) ( (totalSeconds - (hours * 60f * 60f)) / 60f );
		int seconds = (int) ( totalSeconds - ((hours * 60f * 60f) + (minutes * 60)) );
		int ms = (int) (1000f * ( totalSeconds - ( (hours * 60f * 60f) + (minutes * 60) + seconds )));

		//return hours + ":" + minutes + ":" + seconds + "." + ms;

		return FormatHours(hours) + FormatMinutes(hours, minutes) + FormatSeconds(hours, minutes, seconds) + (includeMs ? FormatMilliseconds(ms) : "");
	}

	private void FadeInCanvasElements(float dt)
	{
		blackness.color = new Color(1, 1, 1,Mathf.Min( 1f, blackness.color.a + (fadeSpeed * dt) ));
		
		if (blackness.color.a >= 1f)
		{
			youWin.gameObject.SetActive(true);
			time.gameObject.SetActive(true);
		}
	}

	private void FadeOutMusicAndEraser(float dt)
	{
		musicSource.volume = Mathf.Max(0f, musicSource.volume - (fadeSpeed * dt));
		eraserSoundSource.volume = Mathf.Max(0f, eraserSoundSource.volume - (fadeSpeed * dt));
	}

	private void FadeInVoidSound(float dt)
	{
		voidSound.volume = Mathf.Min(1f, voidSound.volume + (fadeSpeed * dt));
	}

	public void GoToMainMenuScene()
	{
		musicSource.volume = endMusicVolume; // reset the music volume
		musicSource.Play(); // restart the music
		SceneManager.LoadScene(0);
	}

	private string FormatHours(int hours)
	{
		if (hours > 0)
		{
			return hours + ":";
		}
		else
		{
			return "";
		}
	}

	private string FormatMinutes(int hours, int minutes)
	{
		if (minutes > 0)
		{
			if (hours > 0 && minutes < 10)
			{
				return "0" + minutes + ":";
			}
			else
			{
				return minutes + ":";
			}
		}
		else
		{
			if (hours > 0)
			{
				return "00:";
			}
			else
			{
				return "";
			}
		}
	}

	private string FormatSeconds(int hours, int minutes, int seconds)
	{
		if (seconds > 0)
		{
			if ((hours > 0 || minutes > 0) && seconds < 10)
			{
				return "0" + seconds + "";
			}
			else
			{
				return seconds + "";
			}
		}
		else
		{
			if (hours > 0 || minutes > 0)
			{
				return "00";
			}
			else
			{
				return "0";
			}
		}
	}

	private string FormatMilliseconds(float ms)
	{
		if (ms < 10)
		{
			return ".00" + ms;
		}

		if (ms < 100)
		{
			return ".0" + ms;
		}

		return "." + ms;
	}
}
