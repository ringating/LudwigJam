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
    public AudioClip getUpSound;
    //public AudioClip splatSound; // might not be necessary, can just use existing plummet bounce sound

    public Rigidbody plummeter;

    public Color vulnerableTint;

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
    public float fallSpeedToPlummet = 60f; // hard mode only now
    public float fallSpeedToSplat = 60f; // if greater than the above value, then only matters in easy mode
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
    public ShowHelperSprites helperSprites;
    public StunStars stunStars;

    [HideInInspector]
    public Vector3 velocity;
    [HideInInspector]
    public Vector3 prevVelocity;
    private Vector3 prevPosition;
    [HideInInspector]
    public PlayerState prevState;
    [HideInInspector]
    public PlayerState currState;

    private LayerMask terrainMask;
    private const float runSpeedThreshold = 0.01f; // used to switch between idle and running visuals

    [HideInInspector]
    public float timer = 0;
    [HideInInspector]
    public float timer2 = 0;
    private float chargeDistance;
    private Vector3 chargeDirection;
    [HideInInspector]
    public bool usedPunchThisAirborne = true;
    private float timeSinceSuccessfulParry = 10f;
    private float timeSinceSuccessfulAttack = 10f;
    private float timeSinceHitByEnemy = 10f;
    private bool wasGrounded = true;
    private bool invuln = false;
    private float timeSinceMercyRuleActivated = 0f;
    private bool parrySuccessful = false;
    private Vector3 prevDashVelocity;
    private float timeSinceDashEnded;
    private const float parryDashLeniencyTime = 0.3f;

    public float plummetStartY { private set; get; }


    public void startAirRecoveryTimer(float timerDuration)
    {
        if (StaticValues.hardMode) timerDuration = float.MaxValue;

        airRecoveryTimer = timerDuration;
        airRecoveryTime = timerDuration;
    }
    private float airRecoveryTimer = 0f;
    private float airRecoveryTime = 3f;
    public float recoveryRemaining
    {
        get
        {
            return Mathf.Max(0f, airRecoveryTimer / airRecoveryTime);
        }
    }
    public float airRecoveryTimeInEasyMode = 3f;
    public float airRecoveryHeight = 10f; // air recovery can only occur if the player is lower than this height from where they were launched

    public float parryPlatformRemaining 
    {
        get
        {
            float ret = (groundedTimeAfterParryHit - timeSinceSuccessfulParry) / groundedTimeAfterParryHit;
            return Mathf.Clamp01(ret);
        } 
    }

    public float stillGetUpRemaining
    {
        get
        {
            return Mathf.Max( 0, (plummetTimeUnderSpeedToRecover-timer)/plummetTimeUnderSpeedToRecover );
        }
    }

    void Start()
    {
        /*if (!StaticValues.hardMode)
        {
            fallSpeedToPlummet = float.MaxValue;
        }*/

        timeSinceDashEnded = parryDashLeniencyTime + 1f;

        timeSinceMercyRuleActivated = mercyRuleInvulnDuration + 1f;

        terrainMask = ~LayerMask.NameToLayer("terrain");

        prevPosition = transform.position;

        currState = PlayerState.idle;
        prevState = currState;
        idlePose.enabled = true;

        dodgeAnimation.toAnimate[1].SetTint(new Color(0.7f, 0.7f, 0.7f, 1));

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
        prevVelocity = velocity;
        UpdateAccordingToState();
        MaybeFadeOutChargeSound();

        if (!wasGrounded && controller.isGrounded && currState != PlayerState.parry) generalSoundPlayer.PlayOneShot(landingSound, 0.3f);
        wasGrounded = controller.isGrounded;

        prevPosition = transform.position;

        if (currState != PlayerState.plummet)
        {
            controller.Move(velocity * Time.deltaTime);
        }

        timeSinceSuccessfulAttack += Time.deltaTime;    // these used to use unscaledDeltaTime for  some reason
        timeSinceSuccessfulParry += Time.deltaTime;     //
        timeSinceHitByEnemy += Time.deltaTime;          //
        timeSinceMercyRuleActivated += Time.deltaTime;  // (just noting in case changing them to deltaTime breaks something)
        timeSinceDashEnded += Time.deltaTime;
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
                    if(fallPose.visible)
                        fallPose.forceHideAndDisable();
                    if (plummetPose.visible)
                        plummetPose.forceHideAndDisable();
                    break;

                case PlayerState.parry:
                    parryPose.SetTint(Color.white);
                    parryPose.forceHideAndDisable();
                    timer2 = 0;
                    break;

                case PlayerState.charge:
                    chargeSprite.forceHideAndDisable();
                    crosshairSprite.forceHideAndDisable();
                    break;

                case PlayerState.attack:
                    attackPose.forceHideAndDisable();
                    usedPunchThisAirborne = true;
                    timeSinceSuccessfulParry = groundedTimeAfterParryHit + 1;
                    break;

                case PlayerState.plummet:
                    plummetPose.forceHideAndDisable();
                    plummeter.gameObject.SetActive(false);
                    controller.enabled = true;
                    controller.detectCollisions = true;
                    //usedPunchThisAirborne = false; // give them their dash back, only really useful in easy mode or after mercy rule
                    GiveParryBenefits();             // actually maybe this instead
                    //generalSoundPlayer.PlayOneShot(getUpSound, 0.8f); // only do this if recovering due to standstill
                    velocity = Vector3.zero; // v v v
                    prevVelocity = velocity; // these are to fix a bug w/ splat triggering after air recovery from a hit that happened while falling fast enought to splat
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
                    //airRecoveryTimer = 0; // this breaks things, don't do this
                    plummetPose.enabled = true;
                    timer = 0;
                    if (prevState != PlayerState.plummet) timer2 = 0; // don't reset the mercy timer if prev state was also plummet, lol
                    plummetStartY = transform.position.y;
                    EnablePlummeterAndDisableController(true);
                    break;

                case PlayerState.parry:
                    parrySuccessful = false;
                    parryPose.enabled = true;
                    timer = 0;
                    timer2 = 0;
                    generalSoundPlayer.PlayOneShot(parrySound);
                    velocity += Vector3.down * gravity * Time.deltaTime; // fix 1 frame of attack icon flicker
                    break;

                case PlayerState.charge:
                    chargeSprite.enabled = true;
                    crosshairSprite.enabled = true;
                    timer = 0;
                    timer2 = 0;
                    chargeSoundPlayer.volume = 0.7f; // change chargeSoundPlayer volume here
                    chargeSoundPlayer.Play();
                    break;

                case PlayerState.attack:
                    attackPose.enabled = true;
                    timer = 0;
                    generalSoundPlayer.PlayOneShot(dashPunchSound, 1f);
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
                bool leniencyParryHappened = false;
                if (!controller.isGrounded)
                {
                    velocity = GetDesiredAirVelocity();
                    if (velocity.y <= 0)
                    {
                        ChangeState(PlayerState.fall);
                    }
                    else
                    {
                        // still rising
                        if (timeSinceDashEnded < parryDashLeniencyTime)
                        {
                            leniencyParryHappened = ParryMaybe();
                            if (leniencyParryHappened) 
                            {
                                //print("lenient parry dash!");
                                velocity = prevDashVelocity;
                                timeSinceDashEnded = parryDashLeniencyTime + 1f;
                            }
                        }
                    }
                }
                else
                {
                    ChangeState(PlayerState.run);
                }
                setPlayerLookDir(getDesiredVelocity(1f));
                ChargeZoomMaybe();
                if (!leniencyParryHappened) ParryMaybe();
                break;

            case PlayerState.fall:
                if (!GetGrounded())
                {
                    velocity = GetDesiredAirVelocity();
                    if (velocity.y > 0)
                    {
                        ChangeState(PlayerState.jump);
                    }
                    else if (StaticValues.hardMode && velocity.y < -fallSpeedToPlummet)
                    {
                        startAirRecoveryTimer(1337); // value doesnt matter, gets overwritten in hard mode but this only gets called in hard mode
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
                if (parrySuccessful)
                {
                    if(!jumpMaybe()) 
                        ChargeZoomMaybe();
                }
                break;

            case PlayerState.dodge:
                velocity = getVelocityForward(dodgeAnimation.time < dodgeAnimation.durations[0] ? dodgeSpeedA : dodgeSpeedB);
                if (controller.isGrounded)
                {
                    velocity += Vector3.down;
                    // little bit of fake gravity to make dodge actually count as grounded, and therefore not show the platform
                    // but for some reason this only sometimes works...
                    // nevermind, fixed via some jankery in UpdateSharedBetweenIdleAndRun
                }
                break;

            case PlayerState.plummet:
                PlummetUpdate();
                setDefaultCameraSettings();
                break;
        }
    }

    private void UpdateSharedBetweenIdleAndRun(bool grounded)
    {
        ChargeZoomMaybe();

        if (grounded)
        {
            dodgeMaybe(); // excluding this from the changedState bool helps fix the air platform from showing up during dodge
            bool changedState = jumpMaybe();
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
            if (parrySuccessful)
            {
                //print("ok");
                timeSinceSuccessfulParry = groundedTimeAfterParryHit + 1;
            }

            if (timeSinceSuccessfulAttack < groundedTimeAfterAttackHit) // hopefully this just works
            {
                timeSinceSuccessfulAttack = groundedTimeAfterAttackHit + 1;
            }

            /*if (timeSinceSuccessfulParry < groundedTimeAfterParryHit) // lmao this too
            {
                timeSinceSuccessfulParry = groundedTimeAfterParryHit + 1;
            }*/

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

            timer2 += Time.deltaTime;

            if (timer2 >= chargeStartupTime)
            {
                timer2 = 0;
                ChangeState(PlayerState.charge);
            }

            return true;
        }
        else
        {
            setDefaultCameraSettings();

            timer2 = 0;
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
        if (!ParryMaybe())
        {
            // in here, timer will store the DISTANCE travelled so far
            timer += attackSpeed * Time.deltaTime;
            if (timer < chargeDistance)
            {
                velocity = chargeDirection * attackSpeed;
                prevDashVelocity = velocity;
                timeSinceDashEnded = 0;
            }
            else
            {
                Vector2 velHor = new Vector2(velocity.x, velocity.z);
                if (velHor.magnitude > maxHorizontalAirSpeed) velHor = velHor.normalized * maxHorizontalAirSpeed;
                velocity = new Vector3(velHor.x, attackEndJumpSpeed, velHor.y);
                ChangeState(PlayerState.jump);
            }
        }
    }

    private bool ParryMaybe()
    {
        if (Input.GetButtonDown("Parry") && !GlobalObjects.pauseMenuStatic.paused)
        {
            /*if (timeSinceSuccessfulParry < groundedTimeAfterParryHit) // see "lmao this too"
            {
                timeSinceSuccessfulParry = groundedTimeAfterParryHit + 1;
            }*/ // drop the player if they parry while floating (doesnt really seem necessary tbh)

            //parrySuccessful = false;

            ChangeState(PlayerState.parry);
            return true;
        }

        return false;
    }

    private void ParryUpdate()
    {
        velocity += Vector3.down * gravity * Time.deltaTime;

        if (parrySuccessful)
        {
            // can't fall during parry state if your parry succeeds
            velocity = new Vector3(velocity.x, Mathf.Max(velocity.y, 0), velocity.z);
        }

        if (GetGrounded())
        {
            velocity = Vector3.zero;

            //velocity = new Vector3(0, velocity.y, 0);
            //print("goodbye horizontal velocity!");
        }
        
        if (controller.isGrounded) 
        {
            velocity += Vector3.down * gravity * Time.deltaTime;
        }

        /*if (GetGrounded())
        {
            if (controller.isGrounded)
            {
                velocity += Vector3.down * gravity * Time.deltaTime;
            }
            else
            {
                velocity = Vector3.zero;
            }
        }*/

        if (timer < parryActiveTime)
        {
            // parrying, invinvible
            invuln = true;
        }
        else if (timer < parryActiveTime + parryInactiveTime)
        {
            // endlag of parry, vulnerable
            invuln = false;
            parryPose.SetTint(vulnerableTint);

            // nerf to parry-cancelled attacks: restricted to maxHorizontalAirSpeed once in parry recovery (even the vertical component)
            velocity = new Vector3(velocity.x, Mathf.Min(velocity.y, maxHorizontalAirSpeed), velocity.z);
            Vector2 horizontalVel = new Vector2(velocity.x, velocity.z);
            if (horizontalVel.magnitude > maxHorizontalAirSpeed)
            {
                horizontalVel = horizontalVel.normalized * maxHorizontalAirSpeed;
                velocity = new Vector3(horizontalVel.x, velocity.y, horizontalVel.y);
            }
            
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
    public bool GetGrounded()
    {
        bool grounded = controller.isGrounded || timeSinceSuccessfulAttack <= groundedTimeAfterAttackHit || timeSinceSuccessfulParry <= groundedTimeAfterParryHit;

        if (controller.isGrounded || timeSinceSuccessfulParry <= groundedTimeAfterParryHit)
            usedPunchThisAirborne = false;

        return grounded;
    }

    public void OnEnemyAttackHit(Hazard enemy)
    {
        bool beenHit = timeSinceMercyRuleActivated > mercyRuleInvulnDuration; // true unless mercy rule activated

        if (currState == PlayerState.attack) 
        {
            enemy.BeenHit(); // attack trades, lmao
            if ( !(enemy.gameObject.tag == "cannonball") && !StaticValues.hardMode)
            {
                beenHit = false; // ...unless not playing hard mode
            }
            GlobalObjects.timeControllerStatic.TempChangeTimescale(0.05f, 0f, 0.3f, 0f); // hitstop for trades
        }

        if (inParryActiveWindow)
        {
            // successful parry!
            timeSinceSuccessfulParry = 0;
            GlobalObjects.timeControllerStatic.TempChangeTimescale(0.05f, 0f, 0.3f, 0f);
            generalSoundPlayer.PlayOneShot(parryHitSound);
            beenHit = false;
            enemy.Parried();
            helperSprites.ShowFistIcon();
            parrySuccessful = true;
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

            startAirRecoveryTimer(airRecoveryTimeInEasyMode);
        }
    }
    public bool inParryActiveWindow 
    { 
        get 
        { 
            return (currState == PlayerState.parry && timer <= parryActiveTime); 
        } 
    }

    public void KnockedOut(Vector3 positionToFlyAwayFrom, float horizontalLaunchSpeed, float verticalLaunchSpeed)
    {
        if (timeSinceMercyRuleActivated > mercyRuleInvulnDuration) // if mercy rule not active
        {
            Vector3 rawLaunchVector = (transform.position - positionToFlyAwayFrom);
            Vector2 hor = new Vector2(rawLaunchVector.x, rawLaunchVector.z).normalized * horizontalLaunchSpeed;
            Vector3 launchVector = new Vector3(hor.x, verticalLaunchSpeed, hor.y);

            // enable rigidbody and launch it
            EnablePlummeterAndDisableController(false);
            plummeter.AddForce(launchVector, ForceMode.VelocityChange);

            // change to plummet state so the player actually follows the rigidbody
            ChangeState(PlayerState.plummet);
        }
    }

    private void EnablePlummeterAndDisableController(bool inheretControllerVelocity)
    {
        controller.detectCollisions = false;
        controller.enabled = false;

        plummeter.transform.position = transform.position;
        plummeter.gameObject.SetActive(true);

        if (inheretControllerVelocity)
        {
            plummeter.AddForce(velocity, ForceMode.VelocityChange);
        }
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
                //generalSoundPlayer.PlayOneShot(getUpSound, 0.7f); // regular getup sound
                stunStars.PlayUnstun3Sound(); // replaceing the old getup sound with the unstun sound
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
            generalSoundPlayer.PlayOneShot(getUpSound, 0.7f); // this sound is fine for now, something fancier could replace it though
        }

        timer2 += Time.deltaTime;

        AirRecoveryUpdate();
    }

    private void AirRecoveryUpdate()
    {
        if (transform.position.y <= plummetStartY + airRecoveryHeight)
        {
            airRecoveryTimer -= Time.deltaTime;
        }

        if (airRecoveryTimer <= 0)
        {
            ChangeState(PlayerState.fall);
            stunStars.PlayUnstun3Sound();
            return;
        }
    }

    public void HitAnEnemy(Hazard enemy)
    {
        //generalSoundPlayer.PlayOneShot(dashPunchHitSound, 0.5f);
        GlobalObjects.timeControllerStatic.TempChangeTimescale(0.05f, 0f, 0.3f, 0f);
        if (groundedTimeAfterAttackHit > 0)
        {
            timeSinceSuccessfulAttack = 0;
        }
    }

    public void GiveParryBenefits()
    {
        timeSinceSuccessfulParry = 0;
        parrySuccessful = true;
    }

    public void Splat()
    {
        startAirRecoveryTimer(airRecoveryTimeInEasyMode);
        
        velocity = Vector3.zero;
        plummeter.velocity = Vector3.zero;

        EnablePlummeterAndDisableController(false);

        ChangeState(PlayerState.plummet);
    }

    public void SwitchFallToPlummetVisual() //switches to the plummet visual without actually going into the plummet state
    {
        if (fallPose.visible)
            fallPose.forceHideAndDisable();
        plummetPose.enabled = true;
    }
}
