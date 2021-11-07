using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject deleteConfirmScreen;
    public GameObject newGameConfirmScreen;
    public GameObject newGameConfirmScreenHardModeIndicator;

    public GameObject hardModeContinueImage; // image on the continue button if it's a hard mode save
    public GameObject continueButton;
    //public GameObject deleteButton;

    public LoadPlayScene loadPlayScene;

	private void Start()
	{
        if (SettingsStatic.SavedGameExists())
        {
            ShowContinueButtonStuff();
        }
	}

	public void ConfirmNewGame()
    {
        ConfirmDelete();

        // the correct hard mode setting should already be set via onf of the two Maybe functions in LoadPlayScene
        if (StaticValues.hardMode)
        {
            loadPlayScene.LoadSceneInHardMode();
        }
        else
        {
            loadPlayScene.LoadScene();
        }
    }

    public void DenyNewGame()
    {
        newGameConfirmScreen.SetActive(false);
        newGameConfirmScreenHardModeIndicator.SetActive(false);
        StaticValues.hardMode = false; // reset this so it doesnt show up in the settings menu
    }

    public void ConfirmDelete()
    {
        SettingsStatic.DeleteSavedGame();
        HideContinueButtonStuff();
        deleteConfirmScreen.SetActive(false);
    }

    public void DenyDelete()
    {
        deleteConfirmScreen.SetActive(false);
    }

    public void ShowDeleteConfirm()
    {
        deleteConfirmScreen.SetActive(true);
    }

    private void ShowContinueButtonStuff()
    {
        continueButton.SetActive(true);
        //deleteButton.SetActive(true); // childed to continue button, so don't need to individually enable/disable

        GameSaveData data = SettingsStatic.GetSavedGame();
        
        if (data.hardMode)
        {
            hardModeContinueImage.SetActive(true);
        }
    }

    private void HideContinueButtonStuff()
    {
        continueButton.SetActive(false);
        hardModeContinueImage.SetActive(false);
        //deleteButton.SetActive(false);
    }
}
