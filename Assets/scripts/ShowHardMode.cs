using UnityEngine;
using UnityEngine.UI;

public class ShowHardMode : MonoBehaviour
{
    public Image hardModeImage;

    void Update()
    {
        hardModeImage.enabled = StaticValues.hardMode;
        //print(StaticValues.hardMode);
    }
}
