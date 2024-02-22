using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_BossGlobalKnockBack : SkillBase
{
    public Damage damage;

    public float boomTime;
    int boomCount = 4;

    float BOOM_COOLDOWN = 1.1f;
    public float knockbackPower;

    public GameObject aftereffect_globalBoom;
    public Skill_BossGlobalKnockBack_DeathZone deathzone;

    public float deathzoneTime = 1.5f;

    private void Start()
    {
        damage = new();
        damage.damage = 15f;
        damage.type = Damage.Type.Fire;
    }

    void Update()
    {
        if (deathzoneTime > 0)
        {
            deathzoneTime -= Time.deltaTime;
            if (deathzoneTime <= 0) 
            {
                deathzone.isOn = true;
            }
        }

        boomTime -= Time.deltaTime;

        if (boomTime <= 0)
        {
            Collider[] others = Physics.OverlapSphere(transform.position, 10f, 1 << LayerMask.NameToLayer("Player"));

            foreach (Collider other in others)
            {
                ControlComponent control = other.GetComponentInParent<ControlComponent>();
                control.Damaged(damage.damage);
                Vector3 knockbackVector = (other.transform.position - transform.position).normalized;

                KnockBack knockBack = new KnockBack("Æø¹ßÀÇ ¿©ÆÄ", SpecialEffect.Type.Renewable, false, owner, control, knockbackVector, knockbackPower);

                control.AppendSpecialEffect(knockBack);

                alreadyHitObjects.Add(other.gameObject);
            }

            Instantiate(aftereffect_globalBoom, transform.position, transform.rotation);

            boomTime = BOOM_COOLDOWN;
            boomCount--;
        }

        if (boomCount <= 0)
        {
            Destroy(gameObject);
        }
    }

    public override void GetOn()
    {

    }
}
