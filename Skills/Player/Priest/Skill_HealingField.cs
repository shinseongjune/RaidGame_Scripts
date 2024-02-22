using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_HealingField : SkillBase
{
    float heal = 16f;
    float manaHeal = 10f;

    float duration = 5f;

    void Update()
    {
        duration -= Time.deltaTime;

        if (duration <= 0)
        {
            Destroy(gameObject);
        }
    }

    public override void GetOn()
    {
        heal = 10 + owner.GetComponent<Stats>()[(int)Stat.Type.Might].Current;
        manaHeal = 5 + owner.GetComponent<Stats>()[(int)Stat.Type.Might].Current;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            ControlComponent control = other.transform.root.GetComponent<ControlComponent>();

            if (control != null && !control.isDead)
            {
                control.Healed(heal * Time.deltaTime);
                control.HealMana(manaHeal * Time.deltaTime);
            }
        }
    }
}
