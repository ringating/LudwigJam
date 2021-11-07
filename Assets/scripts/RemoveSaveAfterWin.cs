using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveSaveAfterWin : MonoBehaviour
{
    void Awake()
    {
        if (StaticValues.nukeTheSave)
        {
            StaticValues.nukeTheSave = false;
            SettingsStatic.DeleteSavedGame();
        }
    }
}
