using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeIndicator : MonoBehaviour
{
    public SpriteTurnSegments spikeSprite;

	private PlayerController player;

	private void Start()
	{
		spikeSprite.forceHideAndDisable();
		player = GlobalObjects.playerStatic;
	}

	private void Update()
	{
		if (player.spikePlummeting)
		{
			spikeSprite.enabled = true;
		}
		else if(spikeSprite.visible)
		{
			spikeSprite.forceHideAndDisable();
		}
	}
}
