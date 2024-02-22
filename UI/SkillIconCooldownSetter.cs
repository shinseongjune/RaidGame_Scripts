using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillIconCooldownSetter : MonoBehaviour
{
    public SkillSlot slot;

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
        if (slot.skill is null)
        {
            Destroy(this);
        }

        if (slot.cooldown > 0)
        {
            if (!isOn)
            {
                SetOnOff();
            }

            float cool = slot.cooldown;

            if (cool >= 1)
            {
                text.text = cool.ToString("0");
            }
            else
            {
                text.text = cool.ToString("0.0");
            }

            fill.fillAmount = cool / slot.skill.coolDown;
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
