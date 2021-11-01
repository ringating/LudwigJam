using UnityEngine;

public class ParryPlatformTiers : MonoBehaviour
{
    public float firstVolume =  0.8f;
    public float secondVolume = 0.9f;
    public float thirdVolume =  1f;

    public SpriteTurnSegments plat3; // large
    public SpriteTurnSegments plat2; // medium
    public SpriteTurnSegments plat1; // small

    public AudioClip platBreakFirst;
    public AudioClip platBreakSecond;
    public AudioClip platBreakThird;

    public AudioSource noDoppler;

    private SpriteTurnSegments currPlat;
    private PlayerController player;
    private LayerMask layerMask;

    void Start()
    {
        player = GlobalObjects.playerStatic;

        plat3.forceHideAndDisable();
        plat2.forceHideAndDisable();
        plat1.forceHideAndDisable();

        layerMask = LayerMask.GetMask("terrain");
    }

    void Update()
    {
        MaybeChangePlatformSprites();
    }

    private void MaybeChangePlatformSprites()
    {
        bool somePlatVisible = player.GetGrounded() && !player.controller.isGrounded;
        float remainingParryPlatTime = player.parryPlatformRemaining;

        if (!somePlatVisible)
        {
            if (plat3.visible) plat3.forceHideAndDisable();
            if (plat2.visible) plat2.forceHideAndDisable();
            if (plat1.visible)
            {
                plat1.forceHideAndDisable();
                if (remainingParryPlatTime == 0f && player.currState != PlayerController.PlayerState.jump)
                {
                    noDoppler.PlayOneShot(platBreakThird, thirdVolume);
                }
            }
        }
        else
        {
            if (remainingParryPlatTime > 2f / 3f)
            {
                plat3.enabled = true;
                if (plat2.visible) plat2.forceHideAndDisable();
                if (plat1.visible) plat1.forceHideAndDisable();
            }
            else if (remainingParryPlatTime > 1f / 3f)
            {

                if (plat3.visible)
                {
                    plat3.forceHideAndDisable();
                    noDoppler.PlayOneShot(platBreakFirst, firstVolume);
                }
                plat2.enabled = true;
                if (plat1.visible) plat1.forceHideAndDisable();
            }
            else
            {
                if (plat3.visible) plat3.forceHideAndDisable();
                if (plat2.visible) 
                {
                    plat2.forceHideAndDisable();
                    noDoppler.PlayOneShot(platBreakSecond, secondVolume);
                }
                plat1.enabled = true;
            }
        }
    }

    // screw all this, fixed via jank stuff in PlayerControler
    /*private bool DodgingAndGrounded()
    {
        bool dodging = player.currState == PlayerController.PlayerState.dodge;

        float yOffsetFromPlayerCenter = player.controller.height / 2 + 0.05f;
        Vector3 castFrom = player.transform.position - ;

        bool wackGrounded = Physics.Raycast(castFrom.position, Vector3.down, out RaycastHit hitInfo, maxShadowDistance, layerMask, QueryTriggerInteraction.Ignore);
    }*/
}
