using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Skill_ManaField : SkillBase
{
    //Restores a certain percentage of lost mana per tick.
    float manaRegenRateOfLostMana = 0.1f;

    public float lifeTime;

    [HideInInspector]
    public float TICK_TIME = 0.2f;

    public Dictionary<GameObject, float> alreadyHitObjectsCooldowns = new Dictionary<GameObject, float>();

    void Update()
    {
        foreach (var obj in alreadyHitObjectsCooldowns.Keys.ToList())
        {
            alreadyHitObjectsCooldowns[obj] -= Time.deltaTime;

            if (alreadyHitObjectsCooldowns[obj] <= 0)
            {
                alreadyHitObjectsCooldowns.Remove(obj);
            }
        }

        Collider[] targets = Physics.OverlapSphere(transform.position, 4f, 1 << LayerMask.NameToLayer("Player"));

        foreach (Collider target in targets)
        {
            if (!alreadyHitObjectsCooldowns.ContainsKey(target.gameObject))
            {
                Stats stats = target.GetComponent<Stats>();
                if (stats is not null)
                {
                    float curMp = stats.MP;
                    float maxMp = stats[(int)Stat.Type.MaxMP].Current;
                    float lost = maxMp - curMp;
                    if (lost <= 0)
                    {
                        continue;
                    }
                    ControlComponent control = target.GetComponent<ControlComponent>();
                    control.HealMana(lost * manaRegenRateOfLostMana);
                    alreadyHitObjectsCooldowns.Add(target.gameObject, TICK_TIME);
                }
            }
        }

        lifeTime -= Time.deltaTime;

        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    public override void GetOn()
    {
        //TODO: 마나 리젠율 + 스탯 * 계수. 인스펙터 노출을 막아야하므로 세터 필요함.
    }
}
