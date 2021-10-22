using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject toToggle;
    public GameObject controlsScreen;
    public InputField sensitivityInput;
    public Slider volumeSlider;

    private float defaultSens = 1f;

    public bool paused { get { return toToggle.activeSelf; } }

	private void Start()
	{
        AudioListener.volume = volumeSlider.value;
	}

	void Update()
    {
		if (Input.GetButtonDown("Pause"))
        {
            TogglePauseMenu();
        }
    }

    public void ShowControlsScreen()
    {
        controlsScreen.SetActive(true);
    }

    public void HideControlsScreen()
    {
        controlsScreen.SetActive(false);
    }

    public void SensChanged()
    {
        float newSens;
		try 
        {
            newSens = float.Parse(sensitivityInput.text);
        }
        catch
        {
            newSens = defaultSens;
        }
        print("new sens: " + newSens);

        SettingsStatic.sensitivity = newSens;
        sensitivityInput.text = "" + newSens;
        sensitivityInput.ForceLabelUpdate();
    }

    public void TogglePauseMenu()
    {
        bool unpausing = toToggle.activeSelf;
        HideControlsScreen();
        toToggle.SetActive(!toToggle.activeSelf);

        if (unpausing && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 1)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
