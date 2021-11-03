using UnityEngine;

public class SetValuesMainMenu : MonoBehaviour
{
    void Start()
    {
        StaticValues.hardMode = false;

        // force what i assume is native res
        Resolution[] resolutions =  Screen.resolutions;
        Resolution theOne = resolutions[resolutions.Length - 1];
        Screen.SetResolution(theOne.width, theOne.height, true);
    }
}
