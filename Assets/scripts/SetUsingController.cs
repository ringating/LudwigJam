using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetUsingController : MonoBehaviour
{
    public bool useController;

    // Start is called before the first frame update
    void Start()
    {
        StaticValues.useController = useController;
    }

    // Update is called once per frame
    void Update()
    {
        StaticValues.useController = useController;
    }
}
