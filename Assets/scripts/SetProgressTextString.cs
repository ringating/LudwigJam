using UnityEngine;
using UnityEngine.UI;

public class SetProgressTextString : MonoBehaviour
{
    private Text progressText;
    public Eraser eraserScript;

    // Start is called before the first frame update
    void Start()
    {
        progressText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        progressText.text = eraserScript.GetPercentCompletionString();
    }
}
