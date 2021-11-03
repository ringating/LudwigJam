using UnityEngine;
using UnityEngine.SceneManagement;

public class QuickRestart : MonoBehaviour
{
    void Update()
    {
        bool rCtrl = Input.GetKeyDown(KeyCode.LeftControl) && Input.GetKey(KeyCode.R);
        bool ctrlR = Input.GetKeyDown(KeyCode.R) && Input.GetKey(KeyCode.LeftControl);
        
        if (rCtrl || ctrlR)
        {
            SceneManager.LoadScene(1);
        }
    }
}
