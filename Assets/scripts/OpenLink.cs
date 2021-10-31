using UnityEngine;

public class OpenLink : MonoBehaviour
{
    public string link;

    public void TryOpenLink()
    {
        Application.OpenURL(link);
    }
}
