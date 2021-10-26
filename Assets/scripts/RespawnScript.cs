using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnScript : MonoBehaviour
{
    public float yThreshold;

    private PlayerController player;

	private void Start()
	{
        player = GlobalObjects.playerStatic;
	}

	// Update is called once per frame
	void Update()
    {
        if (player.transform.position.y < yThreshold)
        {
            player.transform.position = transform.position;
            player.velocity = Vector3.zero;

            player.plummeter.position = transform.position;
            player.plummeter.velocity = Vector3.zero;
        }
    }
}
