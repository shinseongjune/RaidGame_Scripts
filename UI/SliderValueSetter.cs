using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderValueSetter : MonoBehaviour
{
    Slider slider;
    public Stats targetStats;
    public Stat.Type targetType;

    void Start()
    {
        slider = GetComponent<Slider>();
    }

    void Update()
    {
        switch (targetType)
        {
            case Stat.Type.MaxHP:
                slider.value = targetStats.HP / targetStats[(int)Stat.Type.MaxHP].Current;
                break;
            case Stat.Type.MaxMP:
                slider.value = targetStats.MP / targetStats[(int)Stat.Type.MaxMP].Current;
                break;
            case Stat.Type.Might:
            case Stat.Type.Armor:
            case Stat.Type.FireRes:
            case Stat.Type.ColdRes:
            case Stat.Type.LightningRes:
            case Stat.Type.MovementSpeed:
            case Stat.Type.CritChance:
            case Stat.Type.CritDamage:
            case Stat.Type.__COUNT:
                throw new System.Exception("Invalid Stat Type! Please Check Slider Value Setting Code.");
        }
    }
}
