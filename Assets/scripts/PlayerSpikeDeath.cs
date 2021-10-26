using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpikeDeath : MonoBehaviour
{
    private CharacterController controller;
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
        if (hit.gameObject.tag == "spike")
        {
            // uh oh
            SpikeCollider spikeScript = hit.collider.GetComponent<SpikeCollider>();
            spikeScript.Stabby();
        }
	}
}
