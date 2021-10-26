using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlummetBehaviors : MonoBehaviour
{
    public PhysicMaterial noBounce;
    public float horizontalSpeedThresholdToStop;
    public float verticalSpeedThresholdToStop;

    private PhysicMaterial bouncy;
    private Rigidbody rb;
    private CapsuleCollider col;
    private PlayerController player;

    private bool splat = false;

	private void Start()
	{
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        bouncy = col.material;
        player = GlobalObjects.playerStatic;
	}

	private void Update()
	{
        Vector2 hor = new Vector2(rb.velocity.x, rb.velocity.z);
        splat = Mathf.Abs(rb.velocity.y) < verticalSpeedThresholdToStop && hor.magnitude < horizontalSpeedThresholdToStop;
        //print(Mathf.Abs(rb.velocity.y));
	}

	private void OnCollisionEnter(Collision collision)
	{
        if (splat)
        {
            rb.velocity = Vector3.up;
        }
	}
}
