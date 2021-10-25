using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobInAxis : MonoBehaviour
{
    public bool x;
    public bool y;
    public bool z;

    public float timeOffsetX;
    public float periodX;
    public float amplitudeX;

    public float timeOffsetY;
    public float periodY;
    public float amplitudeY;

    public float timeOffsetZ;
    public float periodZ;
    public float amplitudeZ;

    private Vector3 origin;

    void Start()
    {
        origin = transform.position;
    }

    void Update()
    {
        if (x && periodX != 0)
        {
            transform.position = new Vector3(origin.x + Sinful(timeOffsetX, periodX, amplitudeX), transform.position.y, transform.position.z);
        }

        if (y && periodY != 0)
        {
            transform.position = new Vector3(transform.position.x, origin.y + Sinful(timeOffsetY, periodY, amplitudeY), transform.position.z);
        }

        if (z && periodZ != 0)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, origin.z + Sinful(timeOffsetZ, periodZ, amplitudeZ));
        }
    }

    private static float Sinful(float timeOffset, float period, float amplitude)
    {
        float timeScalar = 2 * Mathf.PI * (1/period);
        return amplitude *  Mathf.Sin((Time.time + timeOffset) * timeScalar);
    }
}
