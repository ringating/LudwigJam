using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eraser : MonoBehaviour
{
    public Victory victoryScript;
    public Transform[] eraserLocations;
    public Transform failSafeLocation;
    public float failSafeRadius;
    public float distanceToTriggerRunAway;
    public float distanceToTriggerFinalGoal;
    public float moveSpeed;

    private PlayerController player;

    private int currLocationIndex;
    private float moveTime;

    private float timer;
    
    // Start is called before the first frame update
    void Start()
    {
        currLocationIndex = 0;
        transform.position = eraserLocations[currLocationIndex].position;
        player = GlobalObjects.playerStatic;
    }

    // Update is called once per frame
    void Update()
    {
        // speen
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y + (Time.deltaTime * 180f), transform.localEulerAngles.z);



        // everything else

        Vector3 toTarget = eraserLocations[currLocationIndex].position - transform.position;
        Vector3 toPlayer = player.transform.position - transform.position;
        Vector3 playerToFailSafe = failSafeLocation.position - player.transform.position;

        MoveTowardsTargetPos(toTarget);

        if (currLocationIndex == eraserLocations.Length - 1)
        {
            if (toTarget.magnitude < 0.1f && toPlayer.magnitude < distanceToTriggerFinalGoal)
            {
                victoryScript.StartVictorySequence();
            }
        }
        else if(toTarget.magnitude < 0.1f && toPlayer.magnitude < distanceToTriggerRunAway)
        {
            currLocationIndex++;
        }

        if (playerToFailSafe.magnitude < failSafeRadius)
        {
            // teleport eraser to end position

            currLocationIndex = eraserLocations.Length - 1;
            transform.position = eraserLocations[currLocationIndex].position;
        }
    }

    private void MoveTowardsTargetPos(Vector3 toTarget)
    {
        transform.position += Tools.MinAbsMagnitude(toTarget.normalized * moveSpeed * Time.deltaTime, toTarget);
    }

	private void OnDrawGizmos()
	{
        Gizmos.color = Color.green;
        for (int i  = 0; i < eraserLocations.Length-1; ++i)
        {
            Gizmos.DrawWireSphere(eraserLocations[i].position, distanceToTriggerRunAway);
        }

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(eraserLocations[eraserLocations.Length - 1].position, distanceToTriggerFinalGoal);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(failSafeLocation.position, failSafeRadius);
    }
}
