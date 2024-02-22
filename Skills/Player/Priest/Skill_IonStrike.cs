using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Skill_IonStrike : SkillBase
{
    Damage damage;

    NavMeshAgent agent;

    ControlComponent targetControl;

    float range = 4f;
    float duration = 3.5f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        
        FindTarget();
    }

    void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0)
        {
            Destroy(gameObject);
            return;
        }

        FindTarget();
        if (targetControl != null)
        {
            agent.SetDestination(targetControl.transform.position);
        }
        else
        {
            agent.SetDestination(transform.position);
        }

        int mask = LayerMask.GetMask("Enemy");
        var cols = Physics.OverlapSphere(transform.position, range, mask);

        foreach (var col in cols)
        {
            ControlComponent nowControl = col.transform.root.GetComponent<ControlComponent>();
            if (nowControl != null && !nowControl.isDead && !nowControl.isDisappeared)
            {
                nowControl.Damaged(damage.damage * Time.deltaTime);
            }
        }
    }

    void FindTarget()
    {
        int mask = LayerMask.GetMask("Enemy");
        var cols = Physics.OverlapSphere(transform.position, 20f, mask);

        if (cols.Length <= 0)
        {
            return;
        }

        float distance = float.MaxValue;
        ControlComponent closest = null;
        foreach (var col in cols)
        {
            float nowDistance = Vector3.Distance(col.transform.position, transform.position);
            if (nowDistance < distance)
            {
                ControlComponent temp = col.transform.root.GetComponent<ControlComponent>();

                if (temp != null && !temp.isDead && !temp.isDisappeared)
                {
                    distance = nowDistance;
                    closest = temp;
                }
            }
        }

        targetControl = closest;
    }

    public override void GetOn()
    {
        damage = new Damage();
        damage.damage = 15f + owner.GetComponent<Stats>()[(int)Stat.Type.Might].Current;
    }
}
