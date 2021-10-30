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
        volume = 0.1f;
        sensitivity = 1f;
        musicVolume = 0.25f; // must also change in the music source for this to work on game start (this probably isnt the case anymore)
        showProgress = true;
        showTimer = true;
    }
}
