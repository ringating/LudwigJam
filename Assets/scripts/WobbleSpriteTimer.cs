using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WobbleSpriteTimer : MonoBehaviour
{
    public float framerate = 12f;
    public float secondsPerFrame{ get { return 1 / framerate; } }

    private float timer = 0;

    public List<WobbleSprite> wobbleSpritesToUpdate;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= secondsPerFrame)
        {
            timer -= secondsPerFrame;
			foreach (WobbleSprite w in wobbleSpritesToUpdate)
            {
                w.changeSprite();
            }
        }
    }

}
