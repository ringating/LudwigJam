using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SettingsStatic
{
    public static float volume { get; set; }
    public static float sensitivity { get; set; }
    public static float musicVolume { get; set; }
    public static bool showProgress { get; set; }
    public static bool showTimer { get; set; }

    static SettingsStatic()
    {
        volume = 0.25f;
        sensitivity = 1f;
        musicVolume = 0.25f;
        showProgress = true;
        showTimer = true;
    }

    public static void SaveSettings()
    {
        PlayerPrefs.SetFloat("sensitivity", SettingsStatic.sensitivity);
        PlayerPrefs.SetFloat("masterVolume", SettingsStatic.volume);
        PlayerPrefs.SetFloat("musicVolume", SettingsStatic.musicVolume);
        PlayerPrefs.SetInt("showProgressBool", SettingsStatic.showProgress ? 1 : 0);
        PlayerPrefs.SetInt("showTimerBool", SettingsStatic.showTimer ? 1 : 0);
    }

    public static void LoadSettings()
    {
        // loads settings from PlayerPrefs, if they exist

        SettingsStatic.sensitivity = PlayerPrefs.GetFloat("sensitivity", SettingsStatic.sensitivity);
        SettingsStatic.volume = PlayerPrefs.GetFloat("masterVolume", SettingsStatic.volume);
        SettingsStatic.musicVolume = PlayerPrefs.GetFloat("musicVolume", SettingsStatic.musicVolume);
        SettingsStatic.showProgress = 1 == PlayerPrefs.GetInt("showProgressBool", SettingsStatic.showProgress ? 1 : 0);
        SettingsStatic.showTimer = 1 == PlayerPrefs.GetInt("showTimerBool", SettingsStatic.showTimer ? 1 : 0);
    }

    public static void SaveGame(GameSaveData saveData)
    {
        PlayerPrefs.SetFloat("playerX", saveData.playerX);
        PlayerPrefs.SetFloat("playerY", saveData.playerY);
        PlayerPrefs.SetFloat("playerZ", saveData.playerZ);
        PlayerPrefs.SetFloat("timePriorToCurrentSession", saveData.timePriorToCurrentSession);
        PlayerPrefs.SetString("progress", saveData.progress);
        PlayerPrefs.SetInt("hardMode", saveData.hardMode ? 1 : 0);

        PlayerPrefs.SetFloat("velocityX", saveData.velocityX);
        PlayerPrefs.SetFloat("velocityY", saveData.velocityY);
        PlayerPrefs.SetFloat("velocityZ", saveData.velocityZ);
        PlayerPrefs.SetInt("playerState", (int)saveData.playerState);

        PlayerPrefs.SetInt("eraserPosIndex", saveData.eraserPosIndex);
    }

    public static GameSaveData GetSavedGame()
    {
        if (!SavedGameExists())
        {
            Debug.LogError("attempting to get saved game data when there's none saved!");
        }

        GameSaveData ret = new GameSaveData();

        ret.playerX = PlayerPrefs.GetFloat("playerX", 5);
        ret.playerY = PlayerPrefs.GetFloat("playerY", -5); // to hopefully make it obvious if you mess something up here
        ret.playerZ = PlayerPrefs.GetFloat("playerZ", 5);
        ret.timePriorToCurrentSession = PlayerPrefs.GetFloat("timePriorToCurrentSession", 0);
        ret.progress = PlayerPrefs.GetString("progress", "???");
        ret.hardMode = 1 == PlayerPrefs.GetInt("hardMode", 0);
        
        ret.velocityX = PlayerPrefs.GetFloat("velocityX", 0);
        ret.velocityY = PlayerPrefs.GetFloat("velocityY", 0);
        ret.velocityZ = PlayerPrefs.GetFloat("velocityZ", 0);
        ret.playerState = (PlayerController.PlayerState)PlayerPrefs.GetInt("playerState", (int)PlayerController.PlayerState.fall);

        ret.eraserPosIndex = PlayerPrefs.GetInt("eraserPosIndex", 0);

        return ret;
    }

    public static bool SavedGameExists()
    {
        return PlayerPrefs.HasKey("playerX"); // sufficient unless you mess up :)
    }

    public static void DeleteSavedGame()
    {
        PlayerPrefs.DeleteKey("playerX");
        PlayerPrefs.DeleteKey("playerY");
        PlayerPrefs.DeleteKey("playerZ");
        PlayerPrefs.DeleteKey("timePriorToCurrentSession");
        PlayerPrefs.DeleteKey("progress");
        PlayerPrefs.DeleteKey("hardMode");
        
        PlayerPrefs.DeleteKey("velocityX");
        PlayerPrefs.DeleteKey("velocityY");
        PlayerPrefs.DeleteKey("velocityZ");
        PlayerPrefs.DeleteKey("playerState");

        PlayerPrefs.DeleteKey("eraserPosIndex");
    }

    public static GameSaveData GetStartingGameData() // used to spawn the player when there's no saved game, probably
    {
        GameSaveData ret = new GameSaveData();

        ret.playerX = 5f;
        ret.playerY = 1.22f;
        ret.playerZ = 5f;
        ret.timePriorToCurrentSession = 0f;
        ret.progress = "???"; // this shouldnt be used
        ret.hardMode = StaticValues.hardMode;

        ret.velocityX = 0;
        ret.velocityY = 0;
        ret.velocityZ = 0;
        ret.playerState = PlayerController.PlayerState.fall;

        ret.eraserPosIndex = 0;

        return ret;
    }
}

public struct GameSaveData
{
    public float playerX;
    public float playerY;
    public float playerZ;
    public float timePriorToCurrentSession;
    public string progress;
    public bool hardMode;

    public float velocityX;
    public float velocityY;
    public float velocityZ;
    public PlayerController.PlayerState playerState;

    public int eraserPosIndex;
}
