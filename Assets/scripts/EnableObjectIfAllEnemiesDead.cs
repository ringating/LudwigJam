using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableObjectIfAllEnemiesDead : MonoBehaviour
{
    public GameObject objectToEnable;

    public PaperEnemy[] enemies;

    public AudioSource secretSoundSource;

    void Start()
    {
        objectToEnable.SetActive(false);
    }

    void Update()
    {
        if (!objectToEnable.activeSelf && allEnemiesDead())
        {
            objectToEnable.SetActive(true);
            secretSoundSource.Play();
        }
    }

    public bool allEnemiesDead()
    {
        for (int i = 0; i < enemies.Length; ++i)
        {
            if (enemies[i].currState != PaperEnemy.EnemyState.dead)
            {
                return false;
            }
        }

        return true;
    }
}
