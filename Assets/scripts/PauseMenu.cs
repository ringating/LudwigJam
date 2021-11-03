using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject toToggle;
    public GameObject controlsScreen;
    public GameObject exitGameScreen;
    public GameObject exitGameButton;
    public InputField sensitivityInput;
    public Slider volumeSlider;
    public AudioSource audioSource;

    private float defaultSens = 1f;

    public bool paused { get { return toToggle.activeSelf; } }

	private void Start()
	{
        sensitivityInput.text = "" + SettingsStatic.sensitivity;
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

    public void ShowExitGameScreen()
    {
        exitGameScreen.SetActive(true);
    }

    public void HideExitGameScreen()
    {
        exitGameScreen.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
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

        if (!unpausing && Application.platform != RuntimePlatform.WebGLPlayer)
        {
            exitGameButton.SetActive(true);
        }
    }

    public void PlayUISound()
    {
        audioSource.Play();
    }

    private const float cooldownUISound = 0.2f;
    private float cooldownTimer = 0;
    public void PlayUISoundOnCooldown()
    {
        if(cooldownTimer <= 0)
        {
            audioSource.Play();
            cooldownTimer = cooldownUISound;
        }
		else 
        {
            cooldownTimer -= Time.deltaTime;
        }
    }
}
