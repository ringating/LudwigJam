using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiveForeverObject : MonoBehaviour
{
	private void Awake()
	{
        LiveForeverObject[] curObjectScripts = FindObjectsOfType<LiveForeverObject>();
        if (curObjectScripts.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
}
