using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunStars : MonoBehaviour
{
    private PlayerController player;

    public SpriteTurnSegments star1;
    public SpriteTurnSegments star2;
    public SpriteTurnSegments star3;

    public float cyclePeriod;
    public float pathRadius;

    private bool vis1;
    private bool vis2;
    private bool vis3;

    void Start()
    {
        player = GlobalObjects.playerStatic;

        star1.forceHideAndDisable();
        star2.forceHideAndDisable();
        star3.forceHideAndDisable();
    }

    void Update()
    {
        SetVisibleBasedOnHitstunEscapeTimer();

        SetLocalPosForPeriodOffset(ref star1, 0f);
        SetLocalPosForPeriodOffset(ref star2, 1f/3f);
        SetLocalPosForPeriodOffset(ref star3, 2f/3f);
    }

    private void SetLocalPosForPeriodOffset(ref SpriteTurnSegments star, float periodOffset)
    {
        float twopi = Mathf.PI * 2;

        float rotPerSec = 1f/cyclePeriod;

        float s = Mathf.Sin( twopi * rotPerSec * (Time.unscaledTime + (cyclePeriod * periodOffset)) );
        float c = Mathf.Cos( twopi * rotPerSec * (Time.unscaledTime + (cyclePeriod * periodOffset)) );

        star.transform.localPosition = new Vector3(c * pathRadius, star.transform.localPosition.y, s * pathRadius);
    }

    private void SetVisibleBasedOnHitstunEscapeTimer()
    {
        float remainingStun = player.recoveryRemaining;

        if (player.currState == PlayerController.PlayerState.plummet)
        {
            if (remainingStun > 2f / 3f)
            {
                star1.enabled = true;
                star2.enabled = true;
                star3.enabled = true;
            }
            else if (remainingStun > 1f / 3f)
            {
                if (star1.visible) star1.forceHideAndDisable();
                star2.enabled = true;
                star3.enabled = true;
            }
            else
            {
                if (star1.visible) star1.forceHideAndDisable();
                if (star2.visible) star2.forceHideAndDisable();
                star3.enabled = true;
            }
        }
        else
        {
            if (star1.visible) star1.forceHideAndDisable();
            if (star2.visible) star2.forceHideAndDisable();
            if (star3.visible) star3.forceHideAndDisable();
        }
    }
}
