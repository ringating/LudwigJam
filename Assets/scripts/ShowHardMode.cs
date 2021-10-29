using UnityEngine;
using UnityEngine.UI;

public class ShowHardMode : MonoBehaviour
{
    private Image hardModeImage;

    // Start is called before the first frame update
    void Start()
    {
        hardModeImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        hardModeImage.enabled = StaticValues.hardMode;
        //print(StaticValues.hardMode);
    }
}
