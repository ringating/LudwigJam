using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Hazard : MonoBehaviour
{
    public abstract void BeenHit();

    public abstract void Parried();
}
