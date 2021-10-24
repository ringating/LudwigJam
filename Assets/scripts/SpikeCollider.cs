using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeCollider : MonoBehaviour
{
    public float verticalLaunchSpeed = 10f;
    public float horizontalLaunchSpeed = 1f;
    public Transform[] pointsToSphereCheckFrom;
	public Transform centerSphere;
    public float sphereCheckRadius = 1f;
	public float optimizationRadius = 3f;
	public float cooldown = 1f;

	private PlayerController player;
	private float cooldownTimer;

	private void Start()
	{
		player = GlobalObjects.playerStatic;
		cooldownTimer = cooldown + 1;
	}

	private void Update()
	{
		if (cooldownTimer > cooldown)
		{
			if (Vector3.Distance(player.transform.position, centerSphere.transform.position) < optimizationRadius)
			{
				for (int i = 0; i < pointsToSphereCheckFrom.Length; ++i)
				{
					if (Vector3.Distance(player.transform.position, pointsToSphereCheckFrom[i].position) < sphereCheckRadius)
					{
						Vector2 temp = Random.insideUnitCircle.normalized;
						Vector3 posToFlyAwayFrom = player.transform.position + new Vector3(temp.x, -1, temp.y);

						player.KnockedOut(posToFlyAwayFrom, horizontalLaunchSpeed, verticalLaunchSpeed);

						cooldownTimer = 0;

						return;
					}
				}
			}
		}

		cooldownTimer += Time.deltaTime;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		foreach (Transform t in pointsToSphereCheckFrom)
		{
			Gizmos.DrawWireSphere(t.position, sphereCheckRadius);
		}
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(centerSphere.transform.position, optimizationRadius);
	}

	/*private void OnCollisionEnter(Collision collision)
	{
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        print("collision");

		if (player)
        {
            Vector2 temp = Random.insideUnitCircle;
            Vector3 posToFlyAwayFrom = player.transform.position + new Vector3(temp.x, -1, temp.y);

            player.KnockedOut(posToFlyAwayFrom, horizontalLaunchSpeed, verticalLaunchSpeed);
        }
	}*/
}
