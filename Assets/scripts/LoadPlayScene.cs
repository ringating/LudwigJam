using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadPlayScene : MonoBehaviour
{
    public void LoadScene()
    {
        StaticValues.hardMode = false;
        SceneManager.LoadScene(1);
    }

    public void LoadSceneInHardMode()
    {
        StaticValues.hardMode = true;
        SceneManager.LoadScene(1);
    }
}
