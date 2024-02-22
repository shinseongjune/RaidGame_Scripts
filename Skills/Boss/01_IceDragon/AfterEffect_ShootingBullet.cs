using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterEffect_ShootingBullet : SkillBase
{
    public Vector3 dir;
    float speed = 4f;
    float duration = 7f;
    Damage damage;

    private void Update()
    {
        duration -= Time.deltaTime;

        transform.Translate(dir * speed * Time.deltaTime, Space.World);

        if (duration <= 0)
        {
            Destroy(gameObject);
        }
    }

    public override void GetOn()
    {
        damage = new Damage();
        damage.damage = 40f;
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject go_other = other.transform.root.gameObject;

        if (go_other.layer == LayerMask.NameToLayer("Player"))
        {
            var control = go_other.GetComponent<ControlComponent>();

            if (control != null)
            {
                control.Damaged(damage.damage);
                Destroy(gameObject);
            }
        }
    }
}
