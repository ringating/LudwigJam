using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eraser : MonoBehaviour
{
    public Transform[] eraserLocations;
    public float distanceToTriggerRunAway;
    public float moveSpeed;

    private int currLocationIndex = 0;
    private float moveTime;

    private float timer;
    
    // Start is called before the first frame update
    void Start()
    {
        transform.position = eraserLocations[0].position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y + (Time.deltaTime * 180f), transform.localEulerAngles.z);
        
        if (Vector3.Distance(GlobalObjects.playerStatic.transform.position, transform.position) > distanceToTriggerRunAway)
        {

        }
    }
}
