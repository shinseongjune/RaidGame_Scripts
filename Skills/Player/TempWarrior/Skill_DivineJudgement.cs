using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_DivineJudgement : SkillBase
{
    public Damage damage;

    float duration = 1f;

    float radius = 5.3f;

    void Update()
    {
        Collider[] hitObjects = Physics.OverlapSphere(transform.position, radius, 1 << LayerMask.NameToLayer("Enemy"));

        foreach (Collider collider in hitObjects)
        {
            if (alreadyHitObjects.Contains(collider.gameObject))
            {
                continue;
            }

            ControlComponent control;
            collider.TryGetComponent(out control);
            if (control is not null)
            {
                control.Damaged(damage.damage);
            }

            alreadyHitObjects.Add(collider.gameObject);
        }

        duration -= Time.deltaTime;
        if (duration <= 0)
        {
            Destroy(gameObject);
        }
    }

    public override void GetOn()
    {
        //TODO: damage Ã³¸®
        damage = new Damage();
        damage.damage = 65f + owner.GetComponent<Stats>()[(int)Stat.Type.Might].Current;
    }
}
