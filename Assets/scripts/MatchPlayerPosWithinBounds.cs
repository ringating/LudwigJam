using UnityEngine;

public class MatchPlayerPosWithinBounds : MonoBehaviour
{
    public Vector3 upperBounds; // these are relative to the initial position of the transform
    public Vector3 lowerBounds;

    private Vector3 homePos;
    private PlayerController player;

    void Start()
    {
        homePos = transform.position;
        player = GlobalObjects.playerStatic;
    }

    void Update()
    {
        Vector3 upper = GetUpperCorner();
        Vector3 lower = GetLowerCorner();

        transform.position = new Vector3(
            Mathf.Clamp(player.transform.position.x, lower.x, upper.x),
            Mathf.Clamp(player.transform.position.y, lower.y, upper.y),
            Mathf.Clamp(player.transform.position.z, lower.z, upper.z)
        );

        print(transform.position);
    }

    private Vector3 GetLowerCorner()
    {
        return new Vector3(
            homePos.x + lowerBounds.x, 
            homePos.y + lowerBounds.y, 
            homePos.z + lowerBounds.z
        );
    }

    private Vector3 GetUpperCorner()
    {
        return new Vector3(
            homePos.x + upperBounds.x, 
            homePos.y + upperBounds.y, 
            homePos.z + upperBounds.z
        );
    }

	private void OnDrawGizmos()
	{
        if (!Application.isPlaying)
        {
            homePos = transform.position;
        }
        
        Vector3 upper = GetUpperCorner();
        Vector3 lower = GetLowerCorner();

        //print(upper);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(lower, upper);
	}
}
