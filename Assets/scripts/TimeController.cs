using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    private float easeInTime = 0;
    private float easeOutTime = 0;
    private float fullyChangedDuration = 0;

    private float timer = 1f;
    private float startTimeScale;
    private float targetTimeScale;

	private void Start()
	{
        startTimeScale = Time.timeScale;
	}

	// Update is called once per frame
	void Update()
    {
        if (timer < easeInTime + fullyChangedDuration + easeOutTime)
        {
            if (timer < easeInTime + fullyChangedDuration)
            {
                if (timer < easeInTime)
                {
                    Time.timeScale = Mathf.Lerp(startTimeScale, targetTimeScale, timer / easeInTime);
                }
                else
                {
                    Time.timeScale = targetTimeScale;
                }
            }
            else
            {
                Time.timeScale = Mathf.Lerp(targetTimeScale, 1f, (timer - (easeInTime + fullyChangedDuration)) / easeOutTime);
            }
        }
        else
        {
            Time.timeScale = 1f;
        }

        timer += Time.unscaledDeltaTime;
    }

    public void TempChangeTimescale(float targetTimeScale, float easeInTime, float fullyChangedDuration, float easeOutTime)
    {
        startTimeScale = Time.timeScale;
        this.targetTimeScale = targetTimeScale;
        this.easeInTime = easeInTime;
        this.fullyChangedDuration = fullyChangedDuration;
        this.easeOutTime = easeOutTime;

        timer = 0;
    }
}
