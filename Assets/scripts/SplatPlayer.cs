using UnityEngine;

public class SplatPlayer : MonoBehaviour
{
    private PlayerController player;

    //public AudioSource windSource;
    public AudioClip splatSound;
    //public float maxWindVolume = 1f;

    void Start()
    {
        player = GlobalObjects.playerStatic;
    }
    void Update()
    {
        bool splattableState = player.currState == PlayerController.PlayerState.idle || player.currState == PlayerController.PlayerState.run;
        bool wasFalling = player.prevState == PlayerController.PlayerState.fall;
        bool fastEnough = player.prevVelocity.y <= -player.fallSpeedToSplat;

        //print(player.prevVelocity.y + " < " + -player.fallSpeedToSplat + " ?");
        
        if (splattableState && wasFalling && fastEnough)
        {
            player.Splat();
            player.generalSoundPlayer.PlayOneShot(splatSound, 0.8f);
        }

        /*if (fastEnough && player.currState == PlayerController.PlayerState.fall)
        {
            if (!windSource.isPlaying)
            {
                windSource.Play();
            }

            //player.SwitchFallToPlummetVisual(); // this sorta muddies what's communicated by the plummet pose
        }
        else
        {
            windSource.Stop();
        }*/

        /*bool windPlaying = fastEnough && player.currState == PlayerController.PlayerState.fall;
        windSource.volume = Mathf.Clamp01( windSource.volume + ((windPlaying ? 10f : -2f) * Time.deltaTime) ) * maxWindVolume;*/

        // i don't like how the wind feels, and i think ultimately this mechanic doesnt need the feedback (it's just there to sell the impact of extreme falls, so it already only happens when the player messes up)
    }
}
