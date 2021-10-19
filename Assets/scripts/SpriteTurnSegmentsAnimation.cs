using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteTurnSegmentsAnimation : MonoBehaviour
{
    public bool looping;
    public bool playing;

    public SpriteTurnSegments[] toAnimate;
    public float[] durations;

    private float[] endTimes;
    private float timer = 0;
    private int currIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
		if (toAnimate.Length > durations.Length) { Debug.LogError("not every SpriteTurnSegment has an associated duration!"); }
		if (toAnimate.Length == 0) { Debug.LogError("missing SpriteTurnSegments to animate!"); }

        endTimes = new float[durations.Length];
        float currEndTime = 0;
        for(int i = 0; i < endTimes.Length; ++i) 
        {
            currEndTime += durations[i];
            endTimes[i] = currEndTime;
        }

        toAnimate[0].enabled = true;
        for (int i = 1; i < endTimes.Length; ++i)
        {
            toAnimate[i].enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
		if (playing)
        {
            timer += Time.deltaTime;

            if(timer > endTimes[currIndex])
			{
                if(currIndex == endTimes.Length - 1)
                {
                    switchToThisIndex(0);
					if (looping)
                    {
                        timer -= endTimes[endTimes.Length - 1];
                    }
					else
                    {
                        playing = false;
                        timer = 0;
                        toAnimate[currIndex].enabled = false;
                    }
                }
				else
                {
                    switchToThisIndex(currIndex + 1);
                }
			}
        }
    }

    private void switchToThisIndex(int newIndex)
    {
		if (currIndex != newIndex)
        {
            toAnimate[currIndex].enabled = false;
            toAnimate[newIndex].enabled = true;
            currIndex = newIndex;
        }
    }
}
