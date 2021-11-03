using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject toToggle;
    public GameObject controlsScreen;
    public GameObject exitGameScreen;
    public GameObject exitGameButton;
    public InputField sensitivityInput;
    public Slider volumeSlider;
    public AudioSource audioSource;

    public GameObject mainMenuButton;
    public GameObject mainMenuScreen;

    private float defaultSens = 1f;

    public bool paused { get { return toToggle.activeSelf; } }

	private void Start()
	{
        sensitivityInput.text = "" + SettingsStatic.sensitivity;
        MaybeEnableGameplayOnlyUI();
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

    public void ShowMainMenuScreen()
    {
        mainMenuScreen.SetActive(true);
    }

    public void HideMainMenuScreen()
    {
        mainMenuScreen.SetActive(false);
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

    private void MaybeEnableGameplayOnlyUI()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1) // gameplay scene
        {
            mainMenuButton.SetActive(true);
        }
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
