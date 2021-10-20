using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DampFollow : MonoBehaviour
{
    public Transform target;
    public float smoothingRate; // dictates the proportion of distance remaining after one second
    public bool startAtTarget;

    void Start()
    {
		if (startAtTarget) 
        {
            transform.position = target.position;
        }
    }

    void Update()
    {
        transform.position = Tools.DampVec3(transform.position, target.position, smoothingRate, Time.deltaTime);
    }
}
