using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AfterEffect_Meteor_MagmaGround : SkillBase
{
    public Damage damage;

    public float lifeTime;

    [HideInInspector]
    public float TICK_TIME = 0.2f;

    public Dictionary<GameObject, float> alreadyHitObjectsCooldowns = new Dictionary<GameObject, float>();

    void Update()
    {
        foreach (var obj in alreadyHitObjectsCooldowns.Keys.ToList())
        {
            alreadyHitObjectsCooldowns[obj] -= Time.deltaTime;

            if (alreadyHitObjectsCooldowns[obj] <= 0)
            {
                alreadyHitObjectsCooldowns.Remove(obj);
            }
        }

        Collider[] targets = Physics.OverlapSphere(transform.position, 4f, 1 << LayerMask.NameToLayer("Enemy"));

        foreach (Collider target in targets)
        {
            if (!alreadyHitObjectsCooldowns.ContainsKey(target.gameObject))
            {
                target.GetComponentInParent<ControlComponent>().Damaged(damage.damage);
                alreadyHitObjectsCooldowns.Add(target.gameObject, TICK_TIME);
            }
        }

        lifeTime -= Time.deltaTime;

        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    public override void GetOn()
    {
        //TODO: stat따라 메테오에서 설정
        damage = new Damage();
        damage.damage = 5;
        damage.type = Damage.Type.Fire;
    }
}
