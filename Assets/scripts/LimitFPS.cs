using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitFPS : MonoBehaviour
{
    public int framesPerSecond = 60;
    public bool updateFramerate = false;

    void Start()
    {
        if (Application.isEditor)
        {
            Application.targetFrameRate = framesPerSecond;
            updateFramerate = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.isEditor && updateFramerate)
        {
            Application.targetFrameRate = framesPerSecond;
            updateFramerate = false;
        }
    }
}
