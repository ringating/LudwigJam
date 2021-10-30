using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotShadow : MonoBehaviour
{
    public Transform castFrom;
    public float hoverDistance = 0.005f;
    public float maxShadowDistance = 10f;

    private LayerMask layerMask;
    public WobbleSprite shadow;

    // Start is called before the first frame update
    void Start()
    {
        //shadow = GetComponent<WobbleSprite>();
        layerMask = ~LayerMask.NameToLayer("terrain");
        shadow.enabled = false;
    }

	// Update is called once per frame
	void Update()
	{
        transform.position = new Vector3(castFrom.position.x, transform.position.y, castFrom.position.z);
    }

	void FixedUpdate()
    {
        //RaycastHit hitInfo = new RaycastHit();
        if(Physics.Raycast(castFrom.position, Vector3.down, out RaycastHit hitInfo, maxShadowDistance, layerMask, QueryTriggerInteraction.Ignore))
        {
            shadow.enabled = true;
            Vector3 prevPos = transform.position;
            transform.position = new Vector3(prevPos.x, hitInfo.point.y + hoverDistance, prevPos.y);
        }
		else
        {
            shadow.enabled = false;
        }
    }

	/*private void OnDrawGizmos()
	{
        Gizmos.color = Color.red;
        Gizmos.DrawRay(castFrom.position, Vector3.down);
	}*/
}
