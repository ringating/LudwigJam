using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteTurnSegmentsAnimation : MonoBehaviour
{
    public bool looping;
    public bool playing;

    public SpriteTurnSegments[] toAnimate; // these must all
    public float[] durations;              // be the same length
    public string[] segmentEndMessages;     // (though this one can have empty entries)

    public CanReceiveMessageFromAnimation[] messageReceivers;

    private float[] endTimes;
    private float timer = 0;
    private int currIndex = 0;

    public float time { get { return timer; } private set {} } // empty private set to make VS shut up

    private bool queuedStopAndReset = false;
    private string queuedMessage = "";

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

            if(timer > endTimes[currIndex]) // if it's time to switch sprites
			{
                queuedMessage = segmentEndMessages[currIndex];  // queue the associated message, if there is one

                if(currIndex == endTimes.Length - 1) // if it was the last sprite
                {
                    switchToThisIndex(0); // return to the first sprite
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
				else // it wasn't the last sprite
                {
                    switchToThisIndex(currIndex + 1);
                }
			}
        }

        if (queuedMessage != "") // do message sending at the end so nothing weird happens from other scripts interjecting
        {
            SendAnimMessage(queuedMessage);
            queuedMessage = "";
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

    private bool _visible = true;
    public bool visible
    {
        set
        {
            if (value && !_visible)
            {
                toAnimate[currIndex].enabled = true;
                _visible = true;
            }
            else if (!value && _visible)
            {
                toAnimate[currIndex].enabled = false;
                _visible = false;
            }
        }

        get { return _visible; }
    }

    private void OnEnable()
    {
        visible = true;
    }

    private void OnDisable()
    {
        visible = false;
    }

    public void forceHideAndDisable()
    {
        for (int i = 0; i < toAnimate.Length; ++i)
        {
            toAnimate[i].forceHideAndDisable();
        }
        enabled = false;
    }

    private void SendAnimMessage(string message)
    {
		foreach (CanReceiveMessageFromAnimation c in messageReceivers)
        {
            c.MessageFromAnimation(message);
        }
    }

    public void StopAndReset()
    {
        switchToThisIndex(0);
        timer = 0f;
        playing = false;
    }

    public void Play()
    {
        playing = true;
        toAnimate[currIndex].enabled = true;
    }
}
