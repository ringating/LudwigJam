using UnityEngine;

public class SaveTheGame : MonoBehaviour
{
    private PlayerController player;

    void Start()
    {
        player = GlobalObjects.playerStatic;
    }

    void Update()
    {
        // upon entering plummet state, save
        bool plummetStarted = player.prevState != PlayerController.PlayerState.plummet && player.currState == PlayerController.PlayerState.plummet;

        // constantly save while real grounded + in idle/run state
        bool actionableGround = (player.currState == PlayerController.PlayerState.idle || player.currState == PlayerController.PlayerState.run) && player.controller.isGrounded;

        // constantly save while not grounded + in jump/fall state
        bool actionableAir = (player.currState == PlayerController.PlayerState.jump || player.currState == PlayerController.PlayerState.fall) && !player.GetGrounded();

        if (plummetStarted || actionableGround || actionableAir)
        {
            //print("would save now, if it were enabled");
            SettingsStatic.SaveGame(player.GetCurrSaveDataState());
        }

        QuickRestart.MaybeQuickRestart(); // must come after the saves in an update, or else the save data persists into the new scene
    }
}
