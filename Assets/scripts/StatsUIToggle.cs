
using UnityEngine;
using UnityEngine.UI;

public class StatsUIToggle : MonoBehaviour
{
    public Toggle timerToggle;
    public Toggle progressToggle;

	private void Start()
	{
        timerToggle.isOn = SettingsStatic.showTimer;
        progressToggle.isOn = SettingsStatic.showProgress;
    }

	public void UpdateShowTimerSetting()
    {
        SettingsStatic.showTimer = timerToggle.isOn;
    }

    public void UpdateShowProgressSetting()
    {
        SettingsStatic.showProgress = progressToggle.isOn;
    }
}
