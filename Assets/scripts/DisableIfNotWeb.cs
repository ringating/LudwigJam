using UnityEngine;

public class DisableIfNotWeb : MonoBehaviour
{
    void Start()
    {
        if (Application.platform != RuntimePlatform.WebGLPlayer)
        {
            gameObject.SetActive(false);
        }
    }
}
