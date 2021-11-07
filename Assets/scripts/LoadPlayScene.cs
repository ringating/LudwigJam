using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadPlayScene : MonoBehaviour
{
    public GameObject newGameConfirmScreen;
    public GameObject newGameConfirmScreenHardModeIndicator;

    public void LoadScene()
    {
        StaticValues.hardMode = false;
        SceneManager.LoadScene(1);
    }

    public void LoadSceneInHardMode()
    {
        StaticValues.hardMode = true;
        SceneManager.LoadScene(1);
    }

    public void MaybeLoadScene()
    {
        StaticValues.hardMode = false; // gotta set this now

        if (SettingsStatic.SavedGameExists())
        {
            ShowNewGameConfirmScreen(false);
        }
        else
        {
            LoadScene();
        }
    }

    public void MaybeLoadSceneInHardMode()
    {
        StaticValues.hardMode = true; // gotta set this now

        if (SettingsStatic.SavedGameExists())
        {
            ShowNewGameConfirmScreen(true);
        }
        else
        {
            LoadSceneInHardMode();
        }
    }

    private void ShowNewGameConfirmScreen(bool hardMode)
    {
        newGameConfirmScreen.SetActive(true);
        if (hardMode)
        {
            newGameConfirmScreenHardModeIndicator.SetActive(true);
        }
    }

    /*private void HideNewGameConfirmScreen()
    {
        newGameConfirmScreen.SetActive(false);
        newGameConfirmScreenHardModeIndicator.SetActive(false);
    }*/
}
