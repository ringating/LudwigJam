using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchPlayerAxis : MonoBehaviour
{
	// one time use script lets go

	private PlayerController player;
	private float minY;

	public float minZ;
	public float maxY;

	private void Start()
	{
		player = GlobalObjects.playerStatic;
		minY = transform.position.y;
	}

	void Update()
    {
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(player.transform.position.y, minY, maxY), Mathf.Max(minZ, player.transform.position.z));
    }
}
