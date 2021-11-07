using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadTheGame : MonoBehaviour
{
    private bool appliedState = false;
    private GameSaveData data;

    public Transform followsFocalPoint;
    public Transform focalPoint;

    public Eraser eraser;

    void Start()
    {
        if (SettingsStatic.SavedGameExists()) // it's up to the main menu scene to delete any saved game data before loading the play scene if a new game is being started
        {
            data = SettingsStatic.GetSavedGame();
            StaticValues.hardMode = data.hardMode;
        }
        else
        {
            data = SettingsStatic.GetStartingGameData();
        }

        StaticValues.timePriorToCurrentSession = data.timePriorToCurrentSession;
    }

	private void Update()
	{
        if (!appliedState) // gotta do this after player's own Start() runs
        {
            appliedState = true;

            GlobalObjects.playerStatic.ApplyState(data);
            followsFocalPoint.position = focalPoint.position; // snap camera instantly to start position
            eraser.currLocationIndex = data.eraserPosIndex;
            eraser.transform.position = eraser.eraserLocations[data.eraserPosIndex].position;
        }
	}
}
