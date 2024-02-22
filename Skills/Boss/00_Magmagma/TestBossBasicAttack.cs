using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBossBasicAttack : SkillBase
{
    public Damage damage;
    public float lifeTime;

    private void Start()
    {
        damage = new Damage();
        damage.damage = 5f;
        damage.type = Damage.Type.Physical;
    }

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
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Stats stats = other.GetComponentInParent<Stats>();
            if (stats != null)
            {
                other.GetComponentInParent<ControlComponent>().Damaged(damage.damage);
            }
        }
    }

    public override void GetOn()
    {

    }
}
