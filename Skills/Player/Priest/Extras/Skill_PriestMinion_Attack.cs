using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_PriestMinion_Attack : SkillBase
{
    Damage damage;
    float duration = 1f;

    void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0f)
        {
            Destroy(gameObject);
        }
    }

    public override void GetOn()
    {
        //TODO: damage
        damage = new Damage();
        damage.damage = 15f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (alreadyHitObjects.Contains(other.gameObject))
        {
            return;
        }

        if (other.gameObject.layer == 1 << LayerMask.NameToLayer("Enemy"))
        {
            ControlComponent control;
            if (other.TryGetComponent(out control))
            {
                control.Damaged(damage.damage);
            }
        }

        alreadyHitObjects.Add(other.gameObject);
    }
}
