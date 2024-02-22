using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Meteor : SkillBase
{
    public Damage damage;

    public float lifeTime;
    
    public GameObject afterEffect_MagmaGround;

    void Update()
    {
        lifeTime -= Time.deltaTime;

        if (lifeTime <= 0)
        {
            Collider[] targets = Physics.OverlapSphere(transform.position, 4f, 1 << LayerMask.NameToLayer("Enemy"));

            foreach (Collider target in targets)
            {
                target.GetComponentInParent<ControlComponent>().Damaged(damage.damage);
            }

            AfterEffect_Meteor_MagmaGround after = Instantiate(afterEffect_MagmaGround, transform.position, transform.rotation).GetComponent<AfterEffect_Meteor_MagmaGround>();
            after.GetOn();
            after.enabled = true;
            //TODO:after.damage 설정 등 처리

            Destroy(gameObject);
        }
    }

    public override void GetOn()
    {
        //TODO: stat to damage
        damage = new Damage();
        damage.damage = 20 + owner.GetComponent<Stats>()[(int)Stat.Type.Might].Current;
        damage.type = Damage.Type.Fire;
    }
}
