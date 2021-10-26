using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountUnscaledTime : MonoBehaviour
{
    public Victory victoryScript;
    private float startTime;

    void Start()
    {
        startTime = Time.unscaledTime;
    }

    void Update()
    {
        if (victoryScript.victory)
        {

        }
    }
}
