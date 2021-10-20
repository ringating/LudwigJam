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
        charge,
        attack,
        parry,
        dodge
    }

    public float runSpeed = 7f;
    public float dodgeSpeedA = 10f;
    public float dodgeSpeedB = 4f;

    public Rigidbody rb;
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

    private Vector3 prevPosition;
    private PlayerState prevState;
    [HideInInspector]
    public PlayerState currState;

    private const float runSpeedThreshold = 0.01f; // used to switch between idle and running visuals

    void Start()
    {
        prevPosition = transform.position;

        currState = PlayerState.idle;
        idlePose.enabled = true;

        runAnimation.forceHideAndDisable();
        dodgeAnimation.forceHideAndDisable();

        //jumpPose.forceHideAndDisable();
        fallPose.forceHideAndDisable();
        //plummetPose.forceHideAndDisable();
        //parryPose.forceHideAndDisable();
        //attackPose.forceHideAndDisable();
        //chargeSprite.forceHideAndDisable();
        //crosshairSprite.forceHideAndDisable();
    }

    void Update()
    {
        Vector3 velHorizontalOnly = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        //print(velHorizontalOnly.magnitude);

        if (velHorizontalOnly.magnitude > 0.001f)
        {
            //print(rb.rotation.eulerAngles);
            Quaternion newRotation = Quaternion.LookRotation(-velHorizontalOnly);
            rb.MoveRotation(newRotation);
        }

        UpdateAccordingToState();

        prevPosition = transform.position;
    }

	private void FixedUpdate()
	{
        FixedUpdateAccordingToState();
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
                break;

            case PlayerState.run:
                break;

            case PlayerState.jump:
                break;

            case PlayerState.charge:
                break;

            case PlayerState.attack:
                break;

            case PlayerState.parry:
                break;

            case PlayerState.dodge:
                break;
        }
    }

    private void FixedUpdateAccordingToState()
    {
        switch (currState)
        {
            case PlayerState.idle:
                rb.velocity = getDesiredVelocity(runSpeed);
                if (rb.velocity.magnitude >= runSpeedThreshold) ChangeState(PlayerState.run);
                dodgeMaybe();
                break;

            case PlayerState.run:
                rb.velocity = getDesiredVelocity(runSpeed);
                if (rb.velocity.magnitude < runSpeedThreshold) ChangeState(PlayerState.idle);
                dodgeMaybe();
                break;

            case PlayerState.jump:
                break;

            case PlayerState.charge:
                break;

            case PlayerState.attack:
                break;

            case PlayerState.parry:
                break;

            case PlayerState.dodge:
                rb.velocity = getVelocityForward(dodgeAnimation.time < dodgeAnimation.durations[0] ? dodgeSpeedA : dodgeSpeedB);
                break;
        }
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

    private void dodgeMaybe()
    {
		if (Input.GetButton("Dodge"))
        {
            ChangeState(PlayerState.dodge);
        }
    }
}
