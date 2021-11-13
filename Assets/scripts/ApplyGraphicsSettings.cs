using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ApplyGraphicsSettings : MonoBehaviour
{
    public Dropdown resolutionDropdown;
    public Dropdown windowTypeDropdown;
    public Toggle vsyncToggle;

    private Resolution[] resolutions;

	public void ApplyGraphics()
    {
        Resolution chosenRes = resolutions[resolutionDropdown.value];
        FullScreenMode chosenMode = GetChosenFullScreenMode();
        Screen.SetResolution(chosenRes.width, chosenRes.height, chosenMode, chosenRes.refreshRate);
        QualitySettings.vSyncCount = vsyncToggle.isOn ? 1 : 0;
    }

    public void UpdateResolutionOptions()
    {
        resolutionDropdown.ClearOptions();

        resolutions = Screen.resolutions;
        List <string> s = new List<string>();

        for (int i = 0; i < resolutions.Length; ++i)
        {
            s.Add( resolutions[i].ToString() );
        }

        resolutionDropdown.AddOptions(s);
    }

    private FullScreenMode GetChosenFullScreenMode()
    {
        FullScreenMode ret;

        switch (windowTypeDropdown.value)
        {
            case 0:
                ret = FullScreenMode.FullScreenWindow;
                break;

            case 1:
                ret = FullScreenMode.ExclusiveFullScreen;
                break;

            case 2:
                ret = FullScreenMode.Windowed;
                break;

            default:
                ret = FullScreenMode.Windowed;
                break;
        }

        return ret;
    }

    public void SetDropdownsToMatchCurrentSettings()
    {
        switch (Screen.fullScreenMode)
        {
            case FullScreenMode.FullScreenWindow:
                windowTypeDropdown.value = 0;
                break;

            case FullScreenMode.ExclusiveFullScreen:
                windowTypeDropdown.value = 1;
                break;

            case FullScreenMode.Windowed:
                windowTypeDropdown.value = 2;
                break;
        }

        resolutionDropdown.value = GetClosestResolutionIndex();

        vsyncToggle.isOn = QualitySettings.vSyncCount > 0;
    }

    private int GetClosestResolutionIndex()
    {
        print(Screen.currentResolution.ToString());
        for (int i = 0; i < resolutions.Length; ++i)
        {
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height &&
                resolutions[i].refreshRate == Screen.currentResolution.refreshRate)
            {
                return i;
            }
        }

        return Mathf.Max(0, resolutions.Length - 1);
    }
}
