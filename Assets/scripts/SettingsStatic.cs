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
}
