using UnityEngine;
using UnityEngine.UI;

public class SetTimerTextString : MonoBehaviour
{
    private Text timerText;
    public Victory victoryScript;

    // Start is called before the first frame update
    void Start()
    {
        timerText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        timerText.text = victoryScript.GetTimeString(Time.unscaledTime, false);
    }
}
