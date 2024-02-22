using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBasicAttack : SkillBase
{
    public Damage damage;
    public float lifeTime;

    public GameObject hitEffect;

    void Update()
    {
        lifeTime -= Time.deltaTime;

        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") && !alreadyHitObjects.Contains(other.gameObject))
        {
            alreadyHitObjects.Add(other.gameObject);
            other.GetComponentInParent<ControlComponent>().Damaged(damage.damage);

            Vector3 closest = other.ClosestPoint(transform.position);
            Instantiate(hitEffect, closest, Quaternion.identity);
        }
    }

    public override void GetOn()
    {
        damage = new Damage();
        damage.damage = 5 + owner.GetComponent<Stats>()[(int)Stat.Type.Might].Current;
        damage.type = Damage.Type.Physical;
    }
}
