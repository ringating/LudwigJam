using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHelperSprites : MonoBehaviour
{
    public SpriteTurnSegments fist;
    public SpriteTurnSegments line;
    public SpriteTurnSegments crosshair;
    public float fistIconDuration = 1.5f;
    public bool disableFist;
    public bool disableOldPlat;
    
    private PlayerController player;
    private float attackIconTimer;
    private bool lineWasOn = false;
    private bool fistWasOn = false;
    private bool crosshairWasOn = false;
    private bool playerHasNotBeenGroundedInThisDuration = true;


    // Start is called before the first frame update
    void Start()
    {
        player = GlobalObjects.playerStatic;

        line.forceHideAndDisable();
        fist.forceHideAndDisable();
        crosshair.forceHideAndDisable();

        attackIconTimer = fistIconDuration + 1f;
    }

    // Update is called once per frame
    void Update()
    {
        // line

        //bool hasBeenRealGroundedSinceSuccessfulParry;
        bool lineOn = player.GetGrounded() && !player.controller.isGrounded && !disableOldPlat;

        if (lineWasOn && !lineOn)
        {
            line.forceHideAndDisable();
        }

        if (!lineWasOn && lineOn)
        {
            line.enabled = true;
        }

        lineWasOn = lineOn;



        //fist

        if (attackIconTimer < fistIconDuration && player.controller.isGrounded)
        {
            playerHasNotBeenGroundedInThisDuration = false;
        }

        bool fistOn = !disableFist && attackIconTimer < fistIconDuration && !player.usedPunchThisAirborne && !player.controller.isGrounded && playerHasNotBeenGroundedInThisDuration;

        if (fistWasOn && !fistOn)
        {
            fist.forceHideAndDisable();
        }

        if (!fistWasOn && fistOn)
        {
            fist.enabled = true;
        }

        if (fistOn)
        {
            fist.SetTint(new Color(1, 1, 1, Mathf.Lerp(0, 1, 1 - attackIconTimer / fistIconDuration)));
        }

        fistWasOn = fistOn;
        attackIconTimer += Time.deltaTime;



        // crosshair

        bool crosshairOn = 
            !player.usedPunchThisAirborne && 
            !player.controller.isGrounded && 
            player.currState != PlayerController.PlayerState.charge && 
            player.currState != PlayerController.PlayerState.plummet && 
            player.currState != PlayerController.PlayerState.dodge;

        if (crosshairWasOn && !crosshairOn)
        {
            crosshair.forceHideAndDisable();
        }

        if (!crosshairWasOn && crosshairOn)
        {
            crosshair.enabled = true;
        }

        crosshairWasOn = crosshairOn;
    }

    public void ShowFistIcon()
    {
        attackIconTimer = 0;
        playerHasNotBeenGroundedInThisDuration = !player.controller.isGrounded;
    }
}
