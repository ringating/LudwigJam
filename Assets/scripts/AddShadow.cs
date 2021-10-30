using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddShadow : MonoBehaviour
{
    public GameObject shadowPrefab;
    public float hoverDistance = 0.03f;
    public float maxShadowDistance = 2f;

    private GameObject shadowInstance;
    private SpotShadow shadowScript;
    
    void Start()
    {
        shadowInstance = Instantiate(shadowPrefab);

        shadowScript = shadowInstance.GetComponent<SpotShadow>();

        shadowScript.castFrom = transform;
        shadowScript.hoverDistance = hoverDistance;
        shadowScript.maxShadowDistance = maxShadowDistance;
    }

	private void OnDestroy()
	{
        Destroy(shadowInstance);
	}
}
