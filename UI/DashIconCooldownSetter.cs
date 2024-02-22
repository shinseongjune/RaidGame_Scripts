using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DashIconCooldownSetter : MonoBehaviour
{
    public Movement move;

    public Image back;
    public Image fill;
    public TextMeshProUGUI text;

    bool isOn = true;

    private void Start()
    {
        SetOnOff();
    }

    void Update()
    {
        if (move.restDashCooldown > 0)
        {
            if (!isOn)
            {
                SetOnOff();
            }

            float cool = move.restDashCooldown;

            if (cool >= 1)
            {
                text.text = cool.ToString("0");
            }
            else
            {
                text.text = cool.ToString("0.0");
            }

            fill.fillAmount = cool / move.DASH_COOLDOWN;
        }
        else
        {
            if (isOn)
            {
                SetOnOff();
            }
        }
    }

    void SetOnOff()
    {
        if (isOn)
        {
            text.enabled = false;
            fill.enabled = false;
            back.enabled = false;

            isOn = false;
        }
        else
        {
            back.enabled = true;
            fill.enabled = true;
            text.enabled = true;

            isOn = true;
        }
    }
}
