using UnityEngine;

public class SetAudioListenerVolume : MonoBehaviour
{
    void Start()
    {
        AudioListener.volume = SettingsStatic.volume;
    }
}
