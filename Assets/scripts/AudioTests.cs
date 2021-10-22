using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTests : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clipA;
    public AudioClip clipB;
    public bool playAOneShot;
    public bool playBOneShot;
    public bool playA;
    public bool playB;
    public bool stop;

    // Start is called before the first frame update
    void Start()
    {
        playAOneShot = false;
        playBOneShot = false;
        playA = false;
        playB = false;
        stop = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (playAOneShot)
        {
            playAOneShot = false;
            audioSource.PlayOneShot(clipA);
        }

        if (playBOneShot)
        {
            playBOneShot = false;
            audioSource.PlayOneShot(clipB);
        }

        if (playA)
        {
            playA = false;
            audioSource.clip = clipA;
            audioSource.Play();
        }

        if (playB)
        {
            playB = false;
            audioSource.clip = clipB;
            audioSource.Play();
        }

        if(stop)
		{
            stop = false;
            audioSource.Stop();
		}
    }
}
