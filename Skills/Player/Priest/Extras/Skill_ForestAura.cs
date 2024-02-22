using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_ForestAura : SkillBase
{
    public Damage damage;

    public GameObject followee;

    float TICK_TIME = 0.5f;
    float RANGE = 1.8f;

    float duration = 10f;

    Dictionary<GameObject, float> hitObjects = new Dictionary<GameObject, float>();
    Dictionary<GameObject, float> tempHitObjects = new Dictionary<GameObject, float>();

    public GameObject prefab_hit;

    void Update()
    {
        if (followee.GetComponent<ControlComponent>().isDead)
        {
            Destroy(gameObject);
        }

        transform.position = followee.transform.position;

        foreach (var obj in hitObjects)
        {
            float restTime = obj.Value - Time.deltaTime;
            if (restTime > 0)
            {
                tempHitObjects.Add(obj.Key, restTime);
            }
        }

        var swap = tempHitObjects;
        tempHitObjects = hitObjects;
        hitObjects = swap;

        tempHitObjects.Clear();

        int mask = LayerMask.GetMask("Enemy");

        Collider[] cols = Physics.OverlapSphere(transform.position, RANGE, mask);

        foreach (Collider col in cols)
        {
            if (hitObjects.ContainsKey(col.transform.root.gameObject))
            {
                continue;
            }

            ControlComponent control = col.transform.root.GetComponent<ControlComponent>();
            if (control != null && !control.isDead && !control.isDisappeared)
            {
                Instantiate(prefab_hit, col.ClosestPoint(transform.position), transform.rotation);
                control.Damaged(damage.damage);
                hitObjects.Add(control.gameObject, TICK_TIME);
            }
        }

        duration -= Time.deltaTime;
        if (duration <= 0f)
        {
            Destroy(gameObject);
        }
    }

    public override void GetOn()
    {
        damage = new Damage();
        damage.damage = 8f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, RANGE);
    }
}
