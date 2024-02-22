using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Boss01Basic : SkillBase
{
    Damage damage;

    float duration = 1f;

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
        damage = new Damage();
        damage.damage = 25f;
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject go_other = other.transform.root.gameObject;

        if (alreadyHitObjects.Contains(go_other))
        {
            return;
        }

        if (go_other.layer == LayerMask.NameToLayer("Player"))
        {
            var control = go_other.GetComponent<ControlComponent>();

            if (control != null)
            {
                control.Damaged(damage.damage);
            }
        }
        alreadyHitObjects.Add(gameObject);
    }
}
