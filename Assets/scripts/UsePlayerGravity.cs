using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsePlayerGravity : MonoBehaviour
{
    public Rigidbody rb;

    void FixedUpdate()
    {
        rb.AddForce(GlobalObjects.playerStatic.gravity * Vector3.down, ForceMode.Acceleration);
    }

	private void OnCollisionEnter(Collision collision)
	{
        GlobalObjects.playerStatic.PlayPlummetBounceSound();
	}
}
