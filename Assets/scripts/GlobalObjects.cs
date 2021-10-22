using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalObjects : MonoBehaviour
{
    public CameraScript cameraScript;
    public static CameraScript cameraScriptStatic;

    public WobbleSpriteTimer wobbleSpriteTimer;
    public static WobbleSpriteTimer wobbleSpriteTimerStatic;

    public PauseMenu pauseMenu;
    public static PauseMenu pauseMenuStatic;


    // Awake is called before Start
    void Awake()
    {
        cameraScriptStatic = cameraScript;
        wobbleSpriteTimerStatic = wobbleSpriteTimer;
        pauseMenuStatic = pauseMenu;
    }
}
