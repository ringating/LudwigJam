using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalObjects : MonoBehaviour
{
    public CameraScript cameraScript;
    public static CameraScript cameraScriptStatic;

    public WobbleSpriteTimer wobbleSpriteTimer;
    public static WobbleSpriteTimer wobbleSpriteTimerStatic;

    // Awake is called before Start
    void Awake()
    {
        cameraScriptStatic = cameraScript;
        wobbleSpriteTimerStatic = wobbleSpriteTimer;
    }
}
