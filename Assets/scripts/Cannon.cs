using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    public GameObject cannonBallToInstantiate;
    public Transform fireFrom;
    public Transform bodyTransform;
    public AudioSource audioSource;
    public AudioClip shotSound;

    public float projectileSpeed;
    public float projectileLife;
    public float timeBetweenShots;
    public float shotTimingOffset;

    private int shotInterval;

    // Start is called before the first frame update
    void Start()
    {
        shotInterval = GetShotInterval();
    }

	private void OnEnable()
	{
        shotInterval = GetShotInterval(); // prevents the cannon from firing instantly every time its enabled
    }

	// Update is called once per frame
	void Update()
    {
        int currInterval = GetShotInterval();
        if (shotInterval != currInterval)
        {
            FireCannon();
            shotInterval = currInterval;
        }
    }

    private int GetShotInterval() 
    {
        return Mathf.RoundToInt((Time.time + shotTimingOffset) / timeBetweenShots);
    }

    private void FireCannon()
    {
        audioSource.PlayOneShot(shotSound);

        GameObject ballObj = Instantiate(cannonBallToInstantiate, fireFrom.position, Quaternion.identity);
        CannonBall ball = ballObj.GetComponent<CannonBall>();
        ball.lifeTime = projectileLife;
        ball.velocity = (fireFrom.position - bodyTransform.position).normalized * projectileSpeed;
    }
}
