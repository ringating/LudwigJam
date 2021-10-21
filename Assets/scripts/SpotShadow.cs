using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotShadow : MonoBehaviour
{
    public Transform castFrom;
    public Rigidbody shadowRB;
    public float hoverDistance = 0.005f;
    public float maxShadowDistance = 10f;

    private LayerMask layerMask;
    private WobbleSprite shadow;

    // Start is called before the first frame update
    void Start()
    {
        shadow = GetComponent<WobbleSprite>();
        layerMask = ~LayerMask.NameToLayer("terrain");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //RaycastHit hitInfo = new RaycastHit();
        if(Physics.Raycast(castFrom.position, Vector3.down, out RaycastHit hitInfo, maxShadowDistance, layerMask, QueryTriggerInteraction.Ignore))
        {
            shadow.enabled = true;
            shadowRB.MovePosition(hitInfo.point + new Vector3(0f, hoverDistance, 0f));
            //transform.position = hitInfo.point + new Vector3(0f, hoverDistance, 0f);
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
