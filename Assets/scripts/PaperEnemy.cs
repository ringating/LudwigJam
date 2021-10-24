using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperEnemy : Hazard
{
    public AudioSource audioPlayer;

    public AudioClip alertSound;
    public AudioClip deathSound;
    public AudioClip respawnSound;
    public AudioClip attackAnticipationSound;
    public AudioClip attackSound;

    public SpriteTurnSegments enemySprites;
    public SpriteTurnSegments deadSprite1;
    public SpriteTurnSegments deadSprite2;

    public CharacterController controller;

    public float maxDistanceFromHome = 30f;
    public float minReturnHomeTime = 4f;
    public Color parriedTint;
    public float generalSpeed = 15f;
    public float attackSpeed = 30f;
    public float attackDistance = 15f;
    public float attackPullBackDistance = 1f;
    public float attackPullBackTime = 0.25f;
    public float attackPullBackSmoothingRate = 0.01f;
    public float attackSphereRadius = 2f;
    public float hazardousRadius = 1f;
    public float playerAttackSphereRadius = 2f;
    public float minInRangeTimeBeforeAttack = 0f; // how long the enemy will sit still before attacking
    public float maxInRangeTimeBeforeAttack = 5f; //
    public float aggroTargetHeight;       // plane tries to fly here when aggro, 
    public float aggroTargetStopDistance; // and will stop flying if it gets at least this close,
    public float attackTimerStartDistance; // and will start flying again if the player gets this far away
    public float aggroRandomOffsetMaxDistance = 2;
    public float parryStunTime = 3f;
    public float stuckStunTime = 1f;
    public float respawnTime = 5f;
    public float respawnDriftSpeed = 5f;
    public bool reversePatrolDirection;
    public float patrolRadius;
    public float aggroRange;
    public float patrolSpeedScalar = 1f;


    // MUST BE TRUE: aggroRange > attackTimerStartDistance > (aggroTargetStopDistance + aggroRandomOffsetMaxDistance)

    private enum EnemyState
    {
        patrolling,     // flying along a fixed path, waiting for player to get in aggro range
        aggro,          // chasing player
        attackStartup,  // pulling back before the attack
        attacking,      // diving, attack hitbox active
        stuck,          // if the enemy hits terrain during the attack, they get stuck there for a time
        stunned,        // the state when it's been parried by the player. frozen, vulnerable
        dead,            // puff of shredded paper, floats in place and interacts with nothing. after a time, re-enters patrolling state
        forceReturnHome
    }

    private Vector3 homePos;
    private EnemyState currState = EnemyState.patrolling;
    private float attackWaitTimer = 0;
    private float attackWaitTime;
    private Vector3 randomWaitPosOffset;
    private float timer = 0;
    private Vector3 velocity;
    private PlayerController player;
    private bool aggrodAndMustMove = true;
    private Vector3 attackDir;
    private Vector3 pullBackTargetPos;
    private Vector3 currPullBackPos;
    private float returnHomeTimer = 0;

	private void Start()
	{
        player = GlobalObjects.playerStatic;
        homePos = transform.position;
        ChangeState(EnemyState.patrolling);

        if (aggroRange < attackTimerStartDistance || attackTimerStartDistance < aggroTargetStopDistance)
        {
            Debug.LogError("THIS MUST BE TRUE: aggroRange > attackTimerStartDistance > (aggroTargetStopDistance + aggroRandomOffsetMaxDistance)");
        }

        deadSprite1.forceHideAndDisable();
        deadSprite2.forceHideAndDisable();

        controller.detectCollisions = false;
	}

	private void Update()
	{
        float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

        CheckIfDead(distanceToPlayer);

        UpdateBasedOnState(distanceToPlayer);

        if(velocity.magnitude > 0.00000000001 && currState != EnemyState.attackStartup)
        {
            setLookDir(velocity);
        }

        if (currState != EnemyState.attackStartup)
        {
            controller.Move(velocity * Time.deltaTime);
        }
        else
        {
            SetControllerPos(currPullBackPos);
        }

        if (currState != EnemyState.forceReturnHome && Vector3.Distance(transform.position, homePos) > maxDistanceFromHome)
        {
            ChangeState(EnemyState.forceReturnHome);
        }
	}

    private void SetControllerPos(Vector3 targetPos)
    {
        controller.Move(targetPos - transform.position);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, attackSphereRadius);
        if (currState == EnemyState.attackStartup || currState == EnemyState.attacking)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(pullBackTargetPos, 0.05f);
        }
        if(currState == EnemyState.attacking) 
        {
            Gizmos.DrawLine(transform.position, transform.position + velocity);
            print(velocity);
        }
    }

    private void ChangeState(EnemyState newState)
    {
        switch (currState) // clean up for the old state
        {
            case EnemyState.patrolling:
                break;

            case EnemyState.aggro:
                break;

            case EnemyState.attackStartup:
                break;

            case EnemyState.attacking:
                break;

            case EnemyState.stuck:
                enemySprites.SetTint(Color.white);
                break;

            case EnemyState.stunned:
                enemySprites.SetTint(Color.white);
                break;

            case EnemyState.dead:
                deadSprite1.forceHideAndDisable();
                deadSprite2.forceHideAndDisable();
                enemySprites.enabled = true;
                break;

            case EnemyState.forceReturnHome:
                returnHomeTimer = 0;
                break;
        }

        switch (newState) // setup for the new state
        {
            case EnemyState.patrolling:
                break;

            case EnemyState.aggro:
                audioPlayer.PlayOneShot(alertSound);
                attackWaitTimer = 0;
                attackWaitTime = GetNewAttackWaitTime();
                randomWaitPosOffset = GetNewWaitPosOffset();
                aggrodAndMustMove = true;
                break;

            case EnemyState.attackStartup:
                audioPlayer.PlayOneShot(attackAnticipationSound);
                SetPullBackTargetPos();
                timer = 0;
                break;

            case EnemyState.attacking:
                audioPlayer.PlayOneShot(attackSound);
                timer = 0;
                break;

            case EnemyState.stuck:
                enemySprites.SetTint(parriedTint);
                timer = 0;
                break;

            case EnemyState.stunned:
                enemySprites.SetTint(parriedTint);
                timer = 0;
                break;

            case EnemyState.dead:
                timer = 0;
                enemySprites.forceHideAndDisable();
                deadSprite1.enabled = true;
                deadSprite2.enabled = true;
                audioPlayer.PlayOneShot(deathSound);
                break;

            case EnemyState.forceReturnHome:
                returnHomeTimer = 0;
                break;
        }

        currState = newState;
    }

    private void UpdateBasedOnState(float distanceToPlayer)
    {
        switch (currState)
        {
            case EnemyState.patrolling:
                PatrolUpdate();
                CheckIfHazardHit(distanceToPlayer);
                break;

            case EnemyState.aggro:
                AggroUpdate();
                CheckIfHazardHit(distanceToPlayer);
                break;

            case EnemyState.attackStartup:
                CheckIfHazardHit(distanceToPlayer);
                AttackStartupUpdate();
                break;

            case EnemyState.attacking:
                TryAttack();
                AttackUpdate();
                break;

            case EnemyState.stuck:
                velocity = Vector3.zero;
                if (timer > stuckStunTime)
                    ChangeState(EnemyState.patrolling);
                else
                    timer += Time.deltaTime;
                break;

            case EnemyState.stunned:
                velocity = Vector3.zero;
                if (timer > parryStunTime)
                    ChangeState(EnemyState.patrolling);
                else
                    timer += Time.deltaTime;
                break;

            case EnemyState.dead:
                DeadUpdate();
                break;

            case EnemyState.forceReturnHome:
                if (returnHomeTimer < minReturnHomeTime)
                {
                    velocity = Tools.MinAbsMagnitude((homePos - transform.position).normalized * generalSpeed, homePos - transform.position);
                    returnHomeTimer += Time.deltaTime;
                    if (velocity.magnitude == 0)
                    {
                        ChangeState(EnemyState.patrolling);
                    }
                }
                else
                {
                    ChangeState(EnemyState.patrolling);
                }
                break;
        }
    }

    private void DeadUpdate()
    {
        velocity = (homePos - transform.position).normalized * respawnDriftSpeed;

        if (timer < respawnTime)
        {
            timer += Time.deltaTime;
        }
        else
        {
            ChangeState(EnemyState.patrolling);
        }
    }

	private void PatrolUpdate() 
    {
        Vector3 diff = GetPatrolPositionForTime(Time.time) - transform.position;
        velocity = Vector3.ClampMagnitude(diff, generalSpeed * 100);

        if (Vector3.Distance(transform.position, player.transform.position) < aggroRange)
        {
            ChangeState(EnemyState.aggro);
        }
    }

    private Vector3 GetPatrolPositionForTime(float time)
    {
        time *= reversePatrolDirection ? -1 : 1;
        float circumfrence = 2 * Mathf.PI * patrolRadius;
        float scalar = 2 * Mathf.PI * (generalSpeed / circumfrence);
        float x = Mathf.Cos(time * scalar * patrolSpeedScalar) * patrolRadius;
        float z = Mathf.Sin(time * scalar * patrolSpeedScalar) * patrolRadius;
        return new Vector3(homePos.x + x, homePos.y, homePos.z + z);
    }

    private void AggroUpdate()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer > aggroRange + 0.5f) // a little extra distance to prevent vibrating in/out of aggro
        {
            ChangeState(EnemyState.patrolling);
            return;
        }

        bool inRangeForTimer = distanceToPlayer < attackTimerStartDistance;
        bool inRangeToNotMoveIfHaveReachedTargetPos = inRangeForTimer;
        Vector3 posAbovePlayer = player.transform.position + (Vector3.up * aggroTargetHeight);
        Vector3 targetPos = posAbovePlayer + randomWaitPosOffset + ((transform.position - posAbovePlayer).normalized * aggroTargetStopDistance);

        Vector3 diff = targetPos - transform.position;

        if (diff.magnitude < 0.1)
        {
            aggrodAndMustMove = false;
        }

        velocity = Vector3.ClampMagnitude(diff, generalSpeed);

        if (inRangeForTimer)
        {
            if (attackWaitTimer > attackWaitTime)
            {
                attackWaitTimer = 0;
                ChangeState(EnemyState.attackStartup);
                return;
            }

            if (!aggrodAndMustMove) 
            { 
                velocity = Vector3.zero; 
            }

            attackWaitTimer += Time.deltaTime;
        }
        else
        {
            aggrodAndMustMove = true;
        }
    }

    private void AttackStartupUpdate()
    {
        attackDir = (player.transform.position - transform.position).normalized;
        setLookDir(attackDir);

        currPullBackPos = Tools.DampVec3(currPullBackPos, pullBackTargetPos, attackPullBackSmoothingRate, Time.deltaTime);

        if (timer > attackPullBackTime)
        {
            ChangeState(EnemyState.attacking);
        }
        timer += Time.deltaTime;
    }

    public void SetPullBackTargetPos()
    {
        attackDir = player.transform.position - transform.position;
        pullBackTargetPos = transform.position - (attackDir.normalized * attackPullBackDistance);
        currPullBackPos = transform.position;
    }

    public void AttackUpdate()
    {
        if (controller.isGrounded)
        {
            ChangeState(EnemyState.stuck);
            return;
        }

        velocity = attackDir * attackSpeed;

        if (timer > attackDistance)
        {
            ChangeState(EnemyState.aggro);
        }
        else
        {
            timer += velocity.magnitude * Time.deltaTime;
        }
    }

    public override void Parried()
    {
        //print("parry successful!");
        ChangeState(EnemyState.stunned);
    }

    public override void BeenHit()
    {
        ChangeState(EnemyState.dead);
    }

	private float GetNewAttackWaitTime()
    {
        return Random.Range(minInRangeTimeBeforeAttack, maxInRangeTimeBeforeAttack);
    }

	private Vector3 GetNewWaitPosOffset()
    {
        Vector3 ret = Random.onUnitSphere * aggroRandomOffsetMaxDistance;
        return new Vector3(ret.x, ret.y/2, ret.z);
    }

    private void TryAttack()
    {
        if (Vector3.Distance(GlobalObjects.playerStatic.transform.position, transform.position) < attackSphereRadius)
        {
            GlobalObjects.playerStatic.OnEnemyAttackHit(this);
        }
    }

    private void setLookDir(Vector3 lookDir)
    {
        Vector3 velHorizontalOnly = new Vector3(lookDir.x, 0, lookDir.z);

        if (velHorizontalOnly.magnitude > 0.001f)
        {
            Quaternion newRotation = Quaternion.LookRotation(-velHorizontalOnly);
            transform.rotation = newRotation;
        }
    }

    private void CheckIfDead(float distanceToPlayer)
    {
        if
        (
            player.currState == PlayerController.PlayerState.attack &&
            currState != EnemyState.dead &&
            currState != EnemyState.attacking && // this case is actually handled in the player script, so ignore it here for consistent behavior
            distanceToPlayer < playerAttackSphereRadius
        )
        {
            BeenHit();
            player.HitAnEnemy(this);
        }
    }

    private void CheckIfHazardHit(float distanceToPlayer) // hitting the player while not doing an attack ourselves
    {
        if
        (
            player.currState != PlayerController.PlayerState.attack &&
            (currState == EnemyState.patrolling || currState == EnemyState.aggro || currState == EnemyState.attackStartup) &&
            distanceToPlayer < hazardousRadius
        )
        {
            player.OnEnemyAttackHit(this);
        }
    }
}
