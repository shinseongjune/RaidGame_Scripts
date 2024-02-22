using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_PriestBasic : SkillBase
{
    public Damage damage;
    float heal = 8f;

    float flight_time = 1.2f;
    float speed = 15f;
    float radius = 0.4f;

    void Update()
    {
        flight_time -= Time.deltaTime;

        if (flight_time > 0)
        {
            transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
        }
        else
        {
            speed += 10f * Time.deltaTime;
            Vector3 dir = (owner.transform.position + Vector3.up - transform.position).normalized;
            transform.Translate(dir * speed * Time.deltaTime, Space.World);
        }

        if (flight_time < -10f)
        {
            Destroy(gameObject);
        }

        int mask = LayerMask.GetMask("Enemy") | LayerMask.GetMask("Player");
        Collider[] hitObjects = Physics.OverlapSphere(transform.position, radius, mask);

        foreach (Collider other in hitObjects)
        {
            if (flight_time <= 0 && other.transform.root.gameObject == owner)
            {
                Destroy(gameObject);
                return;
            }

            if (alreadyHitObjects.Contains(other.transform.root.gameObject))
            {
                continue;
            }

            bool isChecked = false;
            if (other.transform.root.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                ControlComponent control;
                other.transform.root.TryGetComponent(out control);
                if (control is not null)
                {
                    control.Damaged(damage.damage);
                }
                isChecked = true;
            }
            else if (other.transform.root.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                ControlComponent control;
                other.transform.root.TryGetComponent(out control);
                if (control is not null)
                {
                    control.Healed(heal);
                }
                isChecked = true;
            }

            if (isChecked)
            {
                alreadyHitObjects.Add(other.transform.root.gameObject);
            }
        }
    }

    public override void GetOn()
    {
        //TODO: damage
        damage = new Damage();
        damage.damage = 10f + owner.GetComponent<Stats>()[(int)Stat.Type.Might].Current;
    }
}
