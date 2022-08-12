using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModesWidget : MonoBehaviour
{
    [SerializeField]
    Image[] images;

    [SerializeField]
    Color locked_colour;
    [SerializeField]
    Color dormant_colour;
    [SerializeField]
    Color active_colour;

    public void Refresh(int i, CombatMode mode)
    {
        images[i].enabled = true;
        images[i].sprite = mode.skill_rune.icon;

        if(mode.unlocked)
        {
            images[i].color = Color.Lerp(dormant_colour, active_colour, mode.readiness);
        }
        else{ images[i].color = locked_colour; }
    }
}
