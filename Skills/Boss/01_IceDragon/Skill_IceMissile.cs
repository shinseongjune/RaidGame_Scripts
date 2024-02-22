using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Skill_IceMissile : SkillBase
{
    Damage damage;
    float speed = 2f;
    float accel = 30f;

    float duration = 3f;

    private void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0)
        {
            Destroy(gameObject);
        }

        transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);

        speed += accel * Time.deltaTime;

        if (speed > 10)
        {
            accel = 200;
        }
    }

    public override void GetOn()
    {
        damage = new Damage();
        damage.damage = 40f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            var control = other.transform.root.GetComponent<ControlComponent>();

            if (control != null)
            {
                control.Damaged(damage.damage);

                Destroy(gameObject);
            }
        }
    }
}
