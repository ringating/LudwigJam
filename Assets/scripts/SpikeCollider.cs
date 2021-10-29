using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeCollider : MonoBehaviour
{
    public float verticalLaunchSpeed = 10f;
    public float horizontalLaunchSpeed = 1f;
	public float cooldown = 1f;

	public AudioClip stabSound;

	private PlayerController player;
	private float cooldownTimer;

	private void Start()
	{
		player = GlobalObjects.playerStatic;
		cooldownTimer = cooldown + 1;
	}

	private void Update()
	{
		cooldownTimer += Time.deltaTime;
	}

	public void Stabby()
	{
		if (cooldownTimer > cooldown)
		{
			cooldownTimer = 0;

			Vector2 temp = Random.insideUnitCircle.normalized;
			Vector3 posToFlyAwayFrom = player.transform.position + new Vector3(temp.x, -1, temp.y);

			player.startAirRecoveryTimer(float.MaxValue);

			player.KnockedOut(posToFlyAwayFrom, horizontalLaunchSpeed, verticalLaunchSpeed);

			PlayStabSound();
		}
	}

	public void PlayStabSound()
	{
		player.generalSoundPlayer.PlayOneShot(stabSound);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.gameObject.tag == "PlayerRigidBody")
		{
			PlayStabSound();
		}
	}
}
