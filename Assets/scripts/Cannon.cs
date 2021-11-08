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

    public bool addShadowToCannonBalls = false;

    private int shotInterval;

    private Stack<CannonBall> readyToReuse = new Stack<CannonBall>();

    public bool mute;

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
        if (!mute) audioSource.PlayOneShot(shotSound);

        CannonBall ball = GetNewCannonBall();
        ball.lifeTime = projectileLife;
        ball.velocity = (fireFrom.position - bodyTransform.position).normalized * projectileSpeed;

        if (addShadowToCannonBalls)
        {
            ball.InstantiateShadow(); // only does anything if the ball doesnt already have a shadow
        }
    }

    public void CannonBallTimeout(CannonBall ball)
    {
        readyToReuse.Push(ball);
        ball.gameObject.SetActive(false);
    }

    private CannonBall GetNewCannonBall()
    {
        CannonBall ret;

        if (readyToReuse.Count > 0)
        {
            ret = readyToReuse.Pop();
            ret.ResetState();
            GameObject obj = ret.gameObject;
            obj.transform.position = fireFrom.position;
            obj.SetActive(true);
            //print("reusing cannonball");
        }
        else
        {
            ret = Instantiate(cannonBallToInstantiate, fireFrom.position, Quaternion.identity).GetComponent<CannonBall>();
            ret.cannon = this;
            //print("instantiating new cannonball");
        }

        return ret;
    }
}
