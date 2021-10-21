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

    public float groundedCheckHeight = 0.02f;
    public float extraGroundedEscapeDistance = 0.02f;
    public float runSpeed = 7f;
    public float dodgeSpeedA = 10f;
    public float dodgeSpeedB = 4f;
    public float jumpSpeed = 15f;
    public float gravity = 30f;
    public float airControlAccel = 30f;
    public float maxHorizontalAirSpeed = 7f;
    public float fallSpeedToPlummet = 60f;
    public float maxFallSpeed = 100f;

    public CharacterController controller;
    public SpriteTurnSegmentsAnimation runAnimation;
    public SpriteTurnSegmentsAnimation dodgeAnimation;
    public SpriteTurnSegments idlePose;
    public SpriteTurnSegments jumpPose;
    public SpriteTurnSegments fallPose;
    public SpriteTurnSegments plummetPose;
    public SpriteTurnSegments parryPose;
    public SpriteTurnSegments attackPose;
    public WobbleSprite chargeSprite; // forces over-shoulder camera, so it's literally just 1 sprite
    public WobbleSprite crosshairSprite;

    private Vector3 velocity;
    private Vector3 prevPosition;
    private PlayerState prevState;
    [HideInInspector]
    public PlayerState currState;

    private LayerMask terrainMask;
    private const float runSpeedThreshold = 0.01f; // used to switch between idle and running visuals

    void Start()
    {
        terrainMask = ~LayerMask.NameToLayer("terrain");

        prevPosition = transform.position;

        currState = PlayerState.idle;
        idlePose.enabled = true;

        runAnimation.forceHideAndDisable();
        dodgeAnimation.forceHideAndDisable();

        jumpPose.forceHideAndDisable();
        fallPose.forceHideAndDisable();
        //plummetPose.forceHideAndDisable();
        //parryPose.forceHideAndDisable();
        //attackPose.forceHideAndDisable();
        //chargeSprite.forceHideAndDisable();
        //crosshairSprite.forceHideAndDisable();
    }

    void Update()
    {
        Vector3 velHorizontalOnly = new Vector3(velocity.x, 0, velocity.z);
        //print(velHorizontalOnly.magnitude);

        if (velHorizontalOnly.magnitude > 0.001f)
        {
            //print(rb.rotation.eulerAngles);
            Quaternion newRotation = Quaternion.LookRotation(-velHorizontalOnly);
            transform.rotation = newRotation;
        }

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

                case PlayerState.plummet:
                    plummetPose.forceHideAndDisable();
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
                    break;

                case PlayerState.jump:
                    jumpPose.enabled = true;
                    break;

                case PlayerState.fall:
                    fallPose.enabled = true;
                    break;

                case PlayerState.plummet:
                    plummetPose.enabled = true;
                    break;

                default:
                    break;
            }

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
                break;

            case PlayerState.charge:
                break;

            case PlayerState.attack:
                break;

            case PlayerState.parry:
                break;

            case PlayerState.dodge:
                velocity = getVelocityForward(dodgeAnimation.time < dodgeAnimation.durations[0] ? dodgeSpeedA : dodgeSpeedB);
                break;
        }
    }

    private void UpdateSharedBetweenIdleAndRun(bool grounded)
    {
        if (grounded)
        {
            bool changedState = dodgeMaybe();
            changedState = changedState || jumpMaybe();

            if (!changedState)
            {
                // add gravity to appease the CharacterController overlord
                velocity += Vector3.down * gravity;
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
        AnalogInput currInput = AnalogInput.GetCurrentInputs();
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
                ChangeState(PlayerState.idle);
                break;

            default:
                Debug.LogWarning("received unrecognized animation message: \"" + message + "\"");
                break;
        }
    }

    private bool dodgeMaybe()
    {
		if (Input.GetButtonDown("Dodge"))
        {
            ChangeState(PlayerState.dodge);
            return true;
        }
        return false;
    }

    private bool jumpMaybe()
    {
        if (Input.GetButtonDown("Jump"))
        {
            velocity = new Vector3(velocity.x, jumpSpeed, velocity.z);
            ChangeState(PlayerState.jump);
            return true;
        }
        return false;
    }
}
