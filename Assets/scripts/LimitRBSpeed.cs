using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitRBSpeed : MonoBehaviour
{
    public Rigidbody rb;
	public float maxSpeed;

	private void FixedUpdate()
	{
		if (rb.velocity.magnitude > maxSpeed)
		{
			rb.velocity = rb.velocity.normalized * maxSpeed;
		}
	}
}
