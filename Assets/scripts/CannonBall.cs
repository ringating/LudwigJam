using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : Hazard
{
    public float parryDeactivateTime = 1.5f;
    public float attackRadius = 1.5f;
    
    //[HideInInspector]
    public Vector3 velocity;
    //[HideInInspector]
    public float lifeTime;
    public SpriteTurnSegments visuals;

    private float lifeTimer = 0;
    private float timeSinceParried;
    private bool wasDeactivated = false;

	private void Start()
	{
        timeSinceParried = parryDeactivateTime + 1;
    }

	// Update is called once per frame
	void Update()
    {
        // lifetime stuff

        transform.position = transform.position + (velocity * Time.deltaTime);

        if (lifeTimer > lifeTime)
        {
            Destroy(gameObject);
        }

        lifeTimer += Time.deltaTime;



        //parry stuff

        bool deactivated = timeSinceParried < parryDeactivateTime;

        if (!deactivated)
        {
            TryToHitPlayer();
        }

        if (wasDeactivated && !deactivated)
        {
            visuals.SetTint(Color.white);
        }

        if (!wasDeactivated && deactivated)
        {
            visuals.SetTint(new Color(0.7f, 0.7f, 0.7f, 1f));
        }

        wasDeactivated = deactivated;
        timeSinceParried += Time.deltaTime;
    }

	public override void BeenHit()
	{
		// lmao nothing.
	}

	public override void Parried()
	{
        timeSinceParried = 0;
	}

    private void TryToHitPlayer()
    {
        if (Vector3.Distance(GlobalObjects.playerStatic.transform.position, transform.position) < attackRadius)
        {
            GlobalObjects.playerStatic.OnEnemyAttackHit(this);
        }
    }

	private void OnDrawGizmos()
	{
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
	}
}
