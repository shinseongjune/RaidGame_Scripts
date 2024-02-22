using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_PointShooting : SkillBase
{
    public GameObject prefab_shootingBullet;

    public List<Transform> firePoints;

    float duration = 10f;

    float FIRE_COOLDOWN = 0.3f;
    float restCooldown = 0f;

    private void Update()
    {
        if (owner.GetComponent<ControlComponent>().isDead)
        {
            Destroy(gameObject);
        }

        duration -= Time.deltaTime;
        if (duration <= 0)
        {
            Destroy(gameObject);
        }

        restCooldown -= Time.deltaTime;

        if (restCooldown > 0)
        {
            return;
        }

        int idx = Random.Range(0, firePoints.Count);

        Fire(idx);
        restCooldown = FIRE_COOLDOWN;
    }

    void Fire(int idx)
    {
        Vector3 pos = firePoints[idx].position;
        Quaternion dir = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
        AfterEffect_ShootingBullet bullet = Instantiate(prefab_shootingBullet, pos, dir).GetComponent<AfterEffect_ShootingBullet>();
        bullet.dir = dir * Vector3.forward;
        bullet.GetOn();
    }

    public override void GetOn()
    {
        Stats stats = owner.GetComponent<Stats>();

        if (stats.HP <= 0.25f * stats[(int)Stat.Type.MaxHP].Current)
        {
            duration = 30f;
            FIRE_COOLDOWN = 0.08f;
        }
        else if (stats.HP <= 0.5f * stats[(int)Stat.Type.MaxHP].Current)
        {
            duration = 10f;
            FIRE_COOLDOWN = 0.15f;
        }
        else
        {
            duration = 8f;
            FIRE_COOLDOWN = 0.3f;
        }
    }
}
