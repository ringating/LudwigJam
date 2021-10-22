using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperEnemy : MonoBehaviour
{
    public AudioSource audioPlayer;

    public AudioClip alertNoise;
    public AudioClip deathNoise;
    public AudioClip respawnNoise;
    public AudioClip attackStartupNoise;
    public AudioClip attackNoise;

    public SpriteTurnSegments alertSprite;
    public SpriteTurnSegments enemySprites;

    //public virtual void Parried();
    //public abstract void BeenHit();
}
