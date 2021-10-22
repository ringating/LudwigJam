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
    public float maxFallSpeed = 100f;
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

    private float timer = 0;
    private float chargeDistance;
    private Vector3 chargeDirection;

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

        prevPosition = transform.position;

        controller.Move(velocity * Time.deltaTime);
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
                    break;

                case PlayerState.parry:
                    parryPose.enabled = true;
                    timer = 0;
                    break;

                case PlayerState.charge:
                    chargeSprite.enabled = true;
                    crosshairSprite.enabled = true;
                    timer = 0;
                    break;

                case PlayerState.attack:
                    attackPose.enabled = true;
                    timer = 0;
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

                UpdateSharedBetweenIdleAndRun(controller.isGrounded);
                break;

            case PlayerState.run:
                velocity = getDesiredVelocity(runSpeed);
                if (velocity.magnitude < runSpeedThreshold) ChangeState(PlayerState.idle);

                setPlayerLookDir(velocity);
                UpdateSharedBetweenIdleAndRun(controller.isGrounded);
                break;

            case PlayerState.jump:
                if (!controller.isGrounded)
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
                if (!controller.isGrounded)
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
        if (Input.GetButton("Attack") && !GlobalObjects.pauseMenuStatic.paused)
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

        if (controller.isGrounded)
        {
            velocity = new Vector3(0, velocity.y, 0);
        }

        if (timer < parryActiveTime)
        {
            // parrying, invinvible
        }
        else if (timer < parryActiveTime + parryInactiveTime)
        {
            // endlag of parry, vulnerable
        }
        else
        {
            // done parrying
            timer = 0;
            ChangeState(PlayerState.jump);
            return;
        }

        timer += Time.deltaTime;
    }

    public void OnEnemyAttackHit() { }
}
