using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CanReceiveMessageFromAnimation
{
    public enum PlayerState
    {
        idle,
        run,
        jump,
        fall,
        plummet,
        charge,
        attack,
        parry,
        dodge
    }

    public AudioSource chargeSoundPlayer;
    public AudioSource generalSoundPlayer;
    public AudioClip dashPunchSound;
    public AudioClip dashPunchHitSound;
    public AudioClip dodgeSound;
    public AudioClip parrySound;
    public AudioClip parryHitSound;
    public AudioClip getHitSound;
    public AudioClip jumpSound;
    public AudioClip landingSound;
    public AudioClip plummetBounceSound;

    public Rigidbody plummeter;

    public float cameraDistance;
    public float cameraDistanceWhenCharging;
    public float cameraSmoothing;
    public float cameraSmoothingWhenCharging;
    public float groundedCheckHeight = 0.02f;
    public float extraGroundedEscapeDistance = 0.02f;
    public float runSpeed = 7f;
    public float dodgeSpeedA = 10f;
    public float dodgeSpeedB = 4f;
    public float jumpSpeed = 15f;
    public float gravity = 30f;
    public float chargeGravity = 10f;
    public float airControlAccel = 30f;
    public float maxHorizontalAirSpeed = 7f;
    public float fallSpeedToPlummet = 60f;
    //public float maxFallSpeed = 100f;
    public float chargeStartupTime;
    public float maxChargeTime = 1f;
    public float maxZWobbleDuringCharge = 0.25f;
    public float zWobblesPerSec = 7.5f;
    public float maxSpeedDuringCharge = 3f;
    public float attackSpeed = 20f;
    public float minAttackDistance = 3f;
    public float maxAttackDistance = 10f;
    public float attackEndJumpSpeed = 5f;
    public float parryActiveTime = 0.25f;
    public float parryInactiveTime = 0.25f;
    public float invulnAfterGettingHit = 1f;
    public float plummetSpeedThresholdToRecover = 1.5f;
    public float plummetTimeUnderSpeedToRecover = 1f;
    public float plummetMercyRuleTime = 15f;
    public float mercyRuleInvulnDuration = 1f;

    public DampFollow cameraDamp;
    public CharacterController controller;
    public SpriteTurnSegmentsAnimation runAnimation;
    public SpriteTurnSegmentsAnimation dodgeAnimation;
    public SpriteTurnSegments idlePose;
    public SpriteTurnSegments jumpPose;
    public SpriteTurnSegments fallPose;
    public SpriteTurnSegments plummetPose;
    public SpriteTurnSegments parryPose;
    public SpriteTurnSegments attackPose;
    public SpriteTurnSegments chargeSprite; // forces over-shoulder camera, so it's literally just 1 sprite
    public SpriteTurnSegments crosshairSprite;

    private Vector3 velocity;
    private Vector3 prevPosition;
    private PlayerState prevState;
    [HideInInspector]
    public PlayerState currState;

    private LayerMask terrainMask;
    private const float runSpeedThreshold = 0.01f; // used to switch between idle and running visuals

    [HideInInspector]
    public float timer = 0;
    public float timer2 = 0;
    private float chargeDistance;
    private Vector3 chargeDirection;
    private bool usedPunchThisAirborne = true;
    private float timeSinceSuccessfulParry = 10f;
    private float timeSinceSuccessfulAttack = 10f;
    private float timeSinceHitByEnemy = 10f;
    private bool wasGrounded = true;
    private bool invuln = false;
    private float timeSinceMercyRuleActivated = 0f;

    void Start()
    {
        terrainMask = ~LayerMask.NameToLayer("terrain");

        prevPosition = transform.position;

        currState = PlayerState.idle;
        prevState = currState;
        idlePose.enabled = true;

        runAnimation.forceHideAndDisable();
        dodgeAnimation.forceHideAndDisable();

        jumpPose.forceHideAndDisable();
        fallPose.forceHideAndDisable();
        plummetPose.forceHideAndDisable();
        parryPose.forceHideAndDisable();
        attackPose.forceHideAndDisable();
        chargeSprite.forceHideAndDisable();
        crosshairSprite.forceHideAndDisable();
    }

    void Update()
    {
        UpdateAccordingToState();
        MaybeFadeOutChargeSound();

        if (!wasGrounded && controller.isGrounded && currState != PlayerState.parry) generalSoundPlayer.PlayOneShot(landingSound, 0.3f);
        wasGrounded = controller.isGrounded;

        prevPosition = transform.position;

        if (currState != PlayerState.plummet)
        {
            controller.Move(velocity * Time.deltaTime);
        }

        timeSinceSuccessfulAttack += Time.unscaledDeltaTime;
        timeSinceSuccessfulParry += Time.unscaledDeltaTime;
        timeSinceHitByEnemy += Time.unscaledDeltaTime;
        timeSinceMercyRuleActivated += Time.unscaledDeltaTime;
    }


	private void ChangeState(PlayerState newState)
    {
		if (newState != currState)
        {
			switch (currState) // cleanup from old state
            {
                case PlayerState.idle:
                    idlePose.forceHideAndDisable();
                    break;

                case PlayerState.run:
                    runAnimation.forceHideAndDisable();
                    break;

                case PlayerState.dodge:
                    dodgeAnimation.StopAndReset(); // in case the animation didnt fully complete (interrupted by getting hit, for example)
                    dodgeAnimation.forceHideAndDisable();
                    break;

                case PlayerState.jump:
                    jumpPose.forceHideAndDisable();
                    break;

                case PlayerState.fall:
                    fallPose.forceHideAndDisable();
                    break;

                case PlayerState.parry:
                    parryPose.forceHideAndDisable();
                    break;

                case PlayerState.charge:
                    chargeSprite.forceHideAndDisable();
                    crosshairSprite.forceHideAndDisable();
                    break;

                case PlayerState.attack:
                    attackPose.forceHideAndDisable();
                    break;

                case PlayerState.plummet:
                    plummetPose.forceHideAndDisable();
                    plummeter.gameObject.SetActive(false);
                    controller.enabled = true;
                    controller.detectCollisions = true;
                    break;

                default:
                    break;
            }

            switch (newState) // setup for new state
            {
                case PlayerState.idle:
                    idlePose.enabled = true;
                    break;

                case PlayerState.run:
                    runAnimation.enabled = true;
                    break;

                case PlayerState.dodge:
                    dodgeAnimation.enabled = true;
                    dodgeAnimation.Play();
                    timer = 0;
                    invuln = true;
                    generalSoundPlayer.PlayOneShot(dodgeSound);
                    break;

                case PlayerState.jump:
                    jumpPose.enabled = true;
                    break;

                case PlayerState.fall:
                    fallPose.enabled = true;
                    break;

                case PlayerState.plummet:
                    plummetPose.enabled = true;
                    timer = 0;
                    timer2 = 0;
                    break;

                case PlayerState.parry:
                    parryPose.enabled = true;
                    timer = 0;
                    generalSoundPlayer.PlayOneShot(parrySound);
                    break;

                case PlayerState.charge:
                    chargeSprite.enabled = true;
                    crosshairSprite.enabled = true;
                    timer = 0;
                    chargeSoundPlayer.volume = 1f;
                    chargeSoundPlayer.Play();
                    break;

                case PlayerState.attack:
                    attackPose.enabled = true;
                    timer = 0;
                    generalSoundPlayer.PlayOneShot(dashPunchSound, 0.5f);
                    usedPunchThisAirborne = true;
                    break;

                default:
                    break;
            }

            prevState = currState;
            currState = newState;
        }
    }

    private void UpdateAccordingToState()
    {
		switch (currState)
        {
            case PlayerState.idle:
                velocity = getDesiredVelocity(runSpeed);
                if (velocity.magnitude >= runSpeedThreshold) ChangeState(PlayerState.run);

                UpdateSharedBetweenIdleAndRun(GetGrounded());
                break;

            case PlayerState.run:
                velocity = getDesiredVelocity(runSpeed);
                if (velocity.magnitude < runSpeedThreshold) ChangeState(PlayerState.idle);

                setPlayerLookDir(velocity);
                UpdateSharedBetweenIdleAndRun(GetGrounded());
                break;

            case PlayerState.jump:
                if (!GetGrounded())
                {
                    velocity = GetDesiredAirVelocity();
                    if (velocity.y <= 0) ChangeState(PlayerState.fall);
                }
                else
                {
                    ChangeState(PlayerState.run);
                }
                setPlayerLookDir(getDesiredVelocity(1f));
                ChargeZoomMaybe();
                ParryMaybe();
                break;

            case PlayerState.fall:
                if (!GetGrounded())
                {
                    velocity = GetDesiredAirVelocity();
                    if (velocity.y > 0)
                    {
                        ChangeState(PlayerState.jump);
                    }
                    else if (velocity.y < -fallSpeedToPlummet)
                    {
                        ChangeState(PlayerState.plummet);
                    }
                }
                else
                {
                    ChangeState(PlayerState.run);
                }
                setPlayerLookDir(getDesiredVelocity(1f));
                ChargeZoomMaybe();
                ParryMaybe();
                break;

            case PlayerState.charge:
                AttackChargePhase(); // ParryMaybe is in here
                setChargeCameraSettings();
                break;

            case PlayerState.attack:
                AttackDashPhase();
                setDefaultCameraSettings();
                setPlayerLookDir(velocity);
                break;

            case PlayerState.parry:
                ParryUpdate();
                setDefaultCameraSettings();
                break;

            case PlayerState.dodge:
                velocity = getVelocityForward(dodgeAnimation.time < dodgeAnimation.durations[0] ? dodgeSpeedA : dodgeSpeedB);
                break;

            case PlayerState.plummet:
                PlummetUpdate();
                break;
        }
    }

    private void UpdateSharedBetweenIdleAndRun(bool grounded)
    {
        ChargeZoomMaybe();

        if (grounded)
        {
            bool changedState = dodgeMaybe();
            changedState = changedState || jumpMaybe();
            changedState = changedState || ParryMaybe();

            if (!changedState)
            {
                // add gravity to appease the CharacterController overlord
                velocity += Vector3.down * gravity * Time.deltaTime;
            }
        }
        else
        {
            ChangeState(PlayerState.fall);
        }
    }

    private Vector3 GetDesiredAirVelocity()
    {
        // constant gravity accel
        float y = velocity.y - (gravity*Time.deltaTime);

        // gradual movement up to a horizontal max
        Vector3 horizontalAccelVec3 = getDesiredVelocity(airControlAccel * Time.deltaTime);
        Vector2 horiAccel = new Vector2(velocity.x + horizontalAccelVec3.x, velocity.z + horizontalAccelVec3.z);
        if (horiAccel.magnitude > maxHorizontalAirSpeed) horiAccel = horiAccel.normalized * maxHorizontalAirSpeed;

        return new Vector3(horiAccel.x, y, horiAccel.y);
    }

    private Vector3 getDesiredVelocity(float speed)
    {
        AnalogInput currInput = GlobalObjects.pauseMenuStatic.paused ? AnalogInput.GetZeroInputs() : AnalogInput.GetCurrentInputs();
        Vector2 currLeft = currInput.LeftAnalogAdjusted;
        Vector3 velNotAdjustedForCamera = new Vector3(currLeft.x, 0, currLeft.y) * speed;

        Vector2 cameraLookDir = Tools.DegreesToVec2(GlobalObjects.cameraScriptStatic.yaw);
        Quaternion cameraRotationHorizontal = Quaternion.LookRotation(new Vector3(cameraLookDir.x, 0, cameraLookDir.y));

        return cameraRotationHorizontal * velNotAdjustedForCamera;
    }

    private Vector3 getVelocityForward(float speed)
    {
        return transform.TransformDirection(Vector3.back * speed);
    }

	public override void MessageFromAnimation(string message)
    {
        switch (message)
        {
            case "dodge invuln end":
                invuln = false;
                break;

            case "dodge end":
                ChangeState(PlayerState.jump);
                break;

            default:
                Debug.LogWarning("received unrecognized animation message: \"" + message + "\"");
                break;
        }
    }

    private bool dodgeMaybe()
    {
		if (Input.GetButtonDown("Dodge") && !GlobalObjects.pauseMenuStatic.paused)
        {
            ChangeState(PlayerState.dodge);
            return true;
        }
        return false;
    }

    private bool jumpMaybe()
    {
        if (Input.GetButtonDown("Jump") && !GlobalObjects.pauseMenuStatic.paused)
        {
            velocity = new Vector3(velocity.x, jumpSpeed, velocity.z);
            ChangeState(PlayerState.jump);
            generalSoundPlayer.PlayOneShot(jumpSound, 1f);
            return true;
        }
        return false;
    }

    /*private void rotateToFaceHorizontalVelocity()
    {
        Vector3 velHorizontalOnly = new Vector3(velocity.x, 0, velocity.z);

        if (velHorizontalOnly.magnitude > 0.001f)
        {
            Quaternion newRotation = Quaternion.LookRotation(-velHorizontalOnly);
            transform.rotation = newRotation;
        }
    }*/

    private void setPlayerLookDir(Vector3 lookDir)
    {
        Vector3 velHorizontalOnly = new Vector3(lookDir.x, 0, lookDir.z);

        if (velHorizontalOnly.magnitude > 0.001f)
        {
            Quaternion newRotation = Quaternion.LookRotation(-velHorizontalOnly);
            transform.rotation = newRotation;
        }
    }

    private bool ChargeZoomMaybe()
    {
        if (Input.GetButton("Attack") && !GlobalObjects.pauseMenuStatic.paused && !usedPunchThisAirborne)
        {
            setChargeCameraSettings();

            timer += Time.deltaTime;

            if (timer >= chargeStartupTime)
            {
                timer = 0;
                ChangeState(PlayerState.charge);
            }

            return true;
        }
        else
        {
            setDefaultCameraSettings();

            timer = 0;
        }

        return false;
    }

    private void setChargeCameraSettings()
    {
        GlobalObjects.cameraScriptStatic.maxDistance = Tools.Damp(GlobalObjects.cameraScriptStatic.maxDistance, cameraDistanceWhenCharging, cameraSmoothingWhenCharging, Time.deltaTime);
        cameraDamp.smoothingRate = cameraSmoothingWhenCharging;
    }

    private void setDefaultCameraSettings()
    {
        GlobalObjects.cameraScriptStatic.maxDistance = Tools.Damp(GlobalObjects.cameraScriptStatic.maxDistance, cameraDistance, cameraSmoothing, Time.deltaTime);
        cameraDamp.smoothingRate = cameraSmoothing;
    }

    private void AttackChargePhase()
    {
        bool charging = Input.GetButton("Attack") && !GlobalObjects.pauseMenuStatic.paused;

        if (charging)
        {
            timer += Time.deltaTime;
            chargeSprite.transform.localPosition = new Vector3(chargeSprite.transform.localPosition.x, chargeSprite.transform.localPosition.y,
                Mathf.Sin(Time.time * 2 * Mathf.PI * zWobblesPerSec) * Tools.Map(timer, 0, maxChargeTime, 0, maxZWobbleDuringCharge, true)
            );

			if (velocity.magnitude > maxSpeedDuringCharge)
            {
                velocity = velocity.normalized * maxSpeedDuringCharge;
            }

            ParryMaybe();
        }

        if (!charging || timer >= maxChargeTime)
        {
            chargeDistance = Tools.Map(timer, 0, maxChargeTime, minAttackDistance, maxAttackDistance, true);
            chargeDirection = GlobalObjects.cameraScriptStatic.rotatingFocalPointParent.forward;
            timer = 0;
            ChangeState(PlayerState.attack);
        }
    }

    private void AttackDashPhase()
    {
        // in here, timer will store the DISTANCE travelled so far
        timer += attackSpeed * Time.deltaTime;
        if (timer < chargeDistance)
        {
            velocity = chargeDirection * attackSpeed;
        }
        else
        {
            Vector2 velHor = new Vector2(velocity.x, velocity.z);
            if (velHor.magnitude > maxHorizontalAirSpeed) velHor = velHor.normalized * maxHorizontalAirSpeed;
            velocity = new Vector3(velHor.x, attackEndJumpSpeed, velHor.y);
            ChangeState(PlayerState.jump);
        }
    }

    private bool ParryMaybe()
    {
        if (Input.GetButtonDown("Parry") && !GlobalObjects.pauseMenuStatic.paused)
        {
            ChangeState(PlayerState.parry);
            return true;
        }

        return false;
    }

    private void ParryUpdate()
    {
        velocity += Vector3.down * gravity * Time.deltaTime;

        if (GetGrounded())
        {
            velocity = new Vector3(0, velocity.y, 0);
        }

        if (timer < parryActiveTime)
        {
            // parrying, invinvible
            invuln = true;
        }
        else if (timer < parryActiveTime + parryInactiveTime)
        {
            // endlag of parry, vulnerable
            invuln = false;
        }
        else
        {
            // done parrying
            timer = 0;
            invuln = false;
            ChangeState(PlayerState.jump);
            return;
        }

        timer += Time.deltaTime;
    }

    public float groundedTimeAfterParryHit = 0.5f;
    public float groundedTimeAfterAttackHit = 0.5f;
    private bool GetGrounded()
    {
        bool grounded = controller.isGrounded || timeSinceSuccessfulAttack <= groundedTimeAfterAttackHit || timeSinceSuccessfulParry <= groundedTimeAfterParryHit;
        if (grounded) usedPunchThisAirborne = false;
        return grounded;
    }

    public void OnEnemyAttackHit(Hazard enemy)
    {
        bool beenHit = timeSinceMercyRuleActivated > mercyRuleInvulnDuration; // true unless mercy rule activated

        if (currState == PlayerState.attack) // attack trades, lmao
        {
            enemy.BeenHit();
        }

        if (currState == PlayerState.parry && timer <= parryActiveTime)
        {
            // successful parry!
            GlobalObjects.timeControllerStatic.TempChangeTimescale(0.05f, 0f, 0.3f, 0f);
            generalSoundPlayer.PlayOneShot(parryHitSound);
            beenHit = false;
            enemy.Parried();
        }

        if (currState == PlayerState.dodge && invuln)
        {
            beenHit = false;
        }

        if (beenHit && timeSinceHitByEnemy > invulnAfterGettingHit)
        {
            KnockedOut(enemy.transform.position, 10f, 20f);
            timeSinceHitByEnemy = 0f;
            generalSoundPlayer.PlayOneShot(getHitSound);
        }
    }

    public void KnockedOut(Vector3 positionToFlyAwayFrom, float horizontalLaunchSpeed, float verticalLaunchSpeed)
    {
        Vector3 rawLaunchVector = (transform.position - positionToFlyAwayFrom);
        Vector2 hor = new Vector2(rawLaunchVector.x, rawLaunchVector.z).normalized * horizontalLaunchSpeed;
        Vector3 launchVector = new Vector3(hor.x, verticalLaunchSpeed, hor.y);

        // disable character controller
        controller.detectCollisions = false;
        controller.enabled = false;
        
        // enable rigidbody and launch it
        plummeter.gameObject.SetActive(true);
        plummeter.transform.position = transform.position;
        plummeter.AddForce(launchVector, ForceMode.VelocityChange);

        // change to plummet state so the player actually follows the rigidbody
        ChangeState(PlayerState.plummet);
    }

    private void MaybeFadeOutChargeSound()
    {
		if (currState != PlayerState.charge)
        {
            chargeSoundPlayer.volume = Mathf.Max(0, chargeSoundPlayer.volume - (Time.unscaledTime * 10f));
        }
    }

    public void PlayPlummetBounceSound()
    {
        generalSoundPlayer.PlayOneShot(plummetBounceSound);
    }

    private void PlummetUpdate() 
    {
        transform.position = plummeter.transform.position;
        //print(plummeter.velocity.magnitude);

        if (plummeter.velocity.magnitude < plummetSpeedThresholdToRecover)
        {
            timer += Time.deltaTime;
            if (timer > plummetTimeUnderSpeedToRecover)
            {
                timer = 0;
                ChangeState(PlayerState.idle);
            }
        }
        else
        {
            timer = 0;
        }

        if (timer2 > plummetMercyRuleTime)
        {
            timeSinceMercyRuleActivated = 0;
            ChangeState(PlayerState.idle);
        }

        timer2 += Time.deltaTime;
    }

    public void HitAnEnemy(Hazard enemy)
    {
        //generalSoundPlayer.PlayOneShot(dashPunchHitSound, 0.5f);
        GlobalObjects.timeControllerStatic.TempChangeTimescale(0.05f, 0f, 0.3f, 0f);
    }
}
