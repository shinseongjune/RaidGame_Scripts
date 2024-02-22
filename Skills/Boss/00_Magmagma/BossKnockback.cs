using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossKnockback : SkillBase
{
    public Damage damage;
    public float boomTime;

    public float knockbackPower;

    public GameObject aftereffect_knockBackBoom;

    private void Awake()
    {
        damage = new Damage();
        damage.damage = 15;
        damage.type = Damage.Type.Fire;
    }

    void Update()
    {
        boomTime -= Time.deltaTime;

        if (boomTime <= 0)
        {
            Collider[] others = Physics.OverlapSphere(transform.position, 1.5f, 1 << LayerMask.NameToLayer("Player"));

            foreach (Collider other in others)
            {
                ControlComponent control = other.GetComponentInParent<ControlComponent>();
                control.Damaged(damage.damage);
                Vector3 knockbackVector = (other.transform.position - transform.position).normalized;

                KnockBack knockBack = new KnockBack("Æø¹ßÀÇ ¿©ÆÄ", SpecialEffect.Type.Renewable, false, owner, control, knockbackVector, knockbackPower);

                control.AppendSpecialEffect(knockBack);

                alreadyHitObjects.Add(other.gameObject);
            }

            Instantiate(aftereffect_knockBackBoom, transform.position, transform.rotation);

            Destroy(gameObject);
        }
    }

    public override void GetOn()
    {

    }
}
