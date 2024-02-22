using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterEffect_InfiniteBullet : SkillBase
{
    public Vector3 from;
    public Vector3 to;

    public Vector3 dir;

    float speed = 40f;

    Damage damage;

    public GameObject prefab_hitEffect;

    float duration = 15f;
    void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0)
        {
            Instantiate(prefab_hitEffect, transform.position, transform.rotation);

            Destroy(gameObject);
        }

        dir = (to - transform.position).normalized;

        transform.Translate(dir * speed * Time.deltaTime, Space.World);

        if (Vector3.Distance(transform.position, to) < 0.5f)
        {
            Instantiate(prefab_hitEffect, transform.position, transform.rotation);

            Destroy(gameObject);
        }
    }

    public override void GetOn()
    {
        damage = new Damage();
        damage.damage = 100f;
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject go_Other = other.transform.root.gameObject;
        if (go_Other.layer == LayerMask.NameToLayer("Player"))
        {
            var control = go_Other.GetComponent<ControlComponent>();

            if (control != null)
            {
                control.Damaged(damage.damage);

                Instantiate(prefab_hitEffect, transform.position, transform.rotation);

                Destroy(gameObject);
            }
        }
    }
}
