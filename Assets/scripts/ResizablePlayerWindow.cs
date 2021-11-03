using UnityEngine;
using UnityEditor;

public class ResizablePlayerWindow : MonoBehaviour
{
    void Start()
    {
        // this causes a compiler error that prevents the game from even building, but it's not detected by VS 
        // PlayerSettings.resizableWindow = true;
    }
}
