using UnityEngine;
using UnityEngine.UI;

public class SetContinueProgressText : MonoBehaviour
{
    public Text timerText;
    public Text progressText;

    void Start()
    {
        GameSaveData data = SettingsStatic.GetSavedGame();
        timerText.text = Victory.GetTimeStringFromTotalSeconds(data.timePriorToCurrentSession, false);
        progressText.text = data.progress;
    }
}
