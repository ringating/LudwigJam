using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebReduceFarClip : MonoBehaviour
{
    public Camera cam;

    void Start()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            cam.farClipPlane = 60f;
        }
    }
}
