using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MercyRuleDisplay : MonoBehaviour
{
    public Image mercy1;
    public Image mercy2;
    public Image mercy3;

    private PlayerController player;

    void Start()
    {
        mercy1.enabled = false;
        mercy2.enabled = false;
        mercy3.enabled = false;
        player = GlobalObjects.playerStatic;
    }

    void Update()
    {
        if (player.currState != PlayerController.PlayerState.plummet)
        {
            SetMercyText(0);
            return;
        }

        if (player.timer2 > player.plummetMercyRuleTime - 3f)
        {
            if (player.timer2 > player.plummetMercyRuleTime - 2f)
            {
                if (player.timer2 > player.plummetMercyRuleTime - 1f)
                {
                    SetMercyText(1);
                    return;
                }

                SetMercyText(2);
                return;
            }

            SetMercyText(3);
            return;
        }
    }

    private void SetMercyText(int num)
    {
        switch (num)
        {
            case 1:
                mercy1.enabled = true;
                mercy2.enabled = false;
                mercy3.enabled = false;
                break;
            
            case 2:
                mercy1.enabled = false;
                mercy2.enabled = true;
                mercy3.enabled = false;
                break;

            case 3:
                mercy1.enabled = false;
                mercy2.enabled = false;
                mercy3.enabled = true;
                break;

            case 0:
                mercy1.enabled = false;
                mercy2.enabled = false;
                mercy3.enabled = false;
                break;
        }
    }
}
