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
    }

    public static GameSaveData GetSavedGame()
    {
        GameSaveData ret = new GameSaveData();

        ret.playerX = PlayerPrefs.GetFloat("playerX", 5);
        ret.playerY = PlayerPrefs.GetFloat("playerY", -5); // to hopefully make it obvious if you mess something up here
        ret.playerZ = PlayerPrefs.GetFloat("playerZ", 5);
        ret.timePriorToCurrentSession = PlayerPrefs.GetFloat("timePriorToCurrentSession", 0);
        ret.progress = PlayerPrefs.GetString("progress", "???");
        ret.hardMode = 1 == PlayerPrefs.GetInt("hardMode", 0);

        return ret;
    }

    public static bool SavedGameExists()
    {
        return PlayerPrefs.HasKey("playerX"); // sufficient unless you mess up :)
    }

    public static GameSaveData GetDefaultGameData()
    {
        GameSaveData ret = new GameSaveData();

        ret.playerX = 5f;
        ret.playerY = 1.22f;
        ret.playerZ = 5f;
        ret.timePriorToCurrentSession = 0f;
        ret.progress = "???";
        ret.hardMode = false;

        ret.velocityX = 0;
        ret.velocityY = 0;
        ret.velocityZ = 0;
        ret.plumetting = false;

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
    public bool plumetting;
}
