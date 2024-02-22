using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_InfiniteShooting : SkillBase
{
    public GameObject prefab_InfiniteBullet;

    public List<Transform> firePoints;

    float FIRE_COOLDOWN = 0.8f;
    float restCooldown = 0f;

    private void Update()
    {
        /*
        if (owner.GetComponent<ControlComponent>().isDead)
        {
            Destroy(gameObject);
        }
        */
        restCooldown -= Time.deltaTime;
        if (restCooldown > 0)
        {
            return;
        }

        int from = Random.Range(0, firePoints.Count);
        int to = Random.Range(0, firePoints.Count);

        while (from == to)
        {
            to = Random.Range(0, firePoints.Count);
        }

        Fire(from, to);
        restCooldown = FIRE_COOLDOWN;
    }

    void Fire(int from, int to)
    {
        Vector3 fromPos = firePoints[from].position;
        Vector3 toPos = firePoints[to].position;
        Vector3 dir = (toPos - fromPos).normalized;

        AfterEffect_InfiniteBullet bullet = Instantiate(prefab_InfiniteBullet, fromPos, Quaternion.LookRotation(dir)).GetComponent<AfterEffect_InfiniteBullet>();
        bullet.from = fromPos;
        bullet.to = toPos;

        bullet.GetOn();
    }

    public override void GetOn()
    {
        
    }
}
