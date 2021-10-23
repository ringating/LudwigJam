using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SettingsStatic
{
    public static float volume { get; set; }
    public static float sensitivity { get; set; }
    public static float musicVolume { get; set; }

    static SettingsStatic()
    {
        volume = 0.1f;
        sensitivity = 1f;
        musicVolume = 1f;
    }
}
