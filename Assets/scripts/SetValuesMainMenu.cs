using UnityEngine;

public class SetValuesMainMenu : MonoBehaviour
{
    void Start()
    {
        StaticValues.hardMode = false;

        // force what i assume is native res (used to fix a weird stored res from a build w/ resizable window, commenting out for now)
        //Resolution[] resolutions = Screen.resolutions;
        //Resolution theOne = resolutions[resolutions.Length - 1];
        //Screen.SetResolution(theOne.width, theOne.height, true);
    }
}
