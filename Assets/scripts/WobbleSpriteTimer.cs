using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WobbleSpriteTimer : MonoBehaviour
{
    public float framerate = 4f;
    public float secondsPerFrame{ get { return 1 / framerate; } }

    private float timer = 0;

    public List<WobbleSprite> wobbleSpritesToUpdate;

    private int wobbleUpdatesPerFrame;
    private int currIndex;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= secondsPerFrame)
        {
            timer -= secondsPerFrame;

            /*foreach (WobbleSprite w in wobbleSpritesToUpdate)
            {
                w.changeSprite();
            }*/

            // print("peformed " + currIndex + " wobble updates");

            currIndex = 0;
        }

        RecalculateWobbleUpdatesPerFrame();
        PerformThisFramesUpdates();
    }

    private void RecalculateWobbleUpdatesPerFrame()
    {
        wobbleUpdatesPerFrame = Mathf.CeilToInt((wobbleSpritesToUpdate.Count * Time.deltaTime) / secondsPerFrame);
    }

    private void PerformThisFramesUpdates()
    {
        int stopBefore = Mathf.Min(wobbleSpritesToUpdate.Count, currIndex + wobbleUpdatesPerFrame);

        for (; currIndex < stopBefore; ++currIndex)
        {
            wobbleSpritesToUpdate[currIndex].changeSprite();
        }
    }
}
