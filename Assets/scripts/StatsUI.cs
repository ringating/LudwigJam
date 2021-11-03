using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsUI : MonoBehaviour
{
    public float yOffset = 36f;

    public Text timerText;
    public Text progressText;

	private void Update()
	{
        timerText.rectTransform.localPosition = new Vector3(
            timerText.rectTransform.localPosition.x,
            SettingsStatic.showProgress ? yOffset : 0,
            timerText.rectTransform.localPosition.z
        );

        timerText.enabled = SettingsStatic.showTimer;
        progressText.enabled = SettingsStatic.showProgress;
	}

	// this was overcomplicating things
	//
	/*void Update()
    {
        for (int i = 0; i < statsText.Length; ++i)
        {
            SetYOffset(i);
        }
    }
    public Text[] statsText; // first element is top, last element is bottom (to match the inspector's array layout)
    private void SetYOffset(int index)
    {
        int numOffsets = (statsText.Length-1) - index;

        Vector3 localPos = statsText[index].rectTransform.localPosition;
        statsText[index].rectTransform.localPosition = new Vector3(localPos.x, numOffsets * yOffset, localPos.z);
    }*/
}
