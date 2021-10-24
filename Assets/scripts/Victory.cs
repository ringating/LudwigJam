using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Victory : MonoBehaviour
{
    private bool victory = false;

	private void Update()
	{
		if (victory)
		{
			//TODO
		}
	}

	public void StartVictorySequence()
    {
        victory = true;
    }
}
