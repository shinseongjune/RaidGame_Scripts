using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_BossHellfireBall : SkillBase
{
    public float startTime;
    public float lifeTime;

    public bool isStarted = false;

    public float speed;

    public GameObject aftereffect_hellfireBoomEffect;

    public Damage damage;

    bool isOn = false;

    private void Start()
    {
        damage = new Damage();
        damage.damage = 350f;
        damage.type = Damage.Type.Fire;
    }

    void Update()
    {
        if (!isOn)
        {
            return;
        }

        if (!isStarted)
        {
            startTime -= Time.deltaTime;

            if (startTime <= 0)
            {
                isStarted = true;
            }
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, target.transform.position + Vector3.up, speed * Time.deltaTime);

            lifeTime -= Time.deltaTime;

            if (lifeTime <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public override void GetOn()
    {
        isOn = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            other.GetComponentInParent<ControlComponent>().Damaged(damage.damage);

            Instantiate(aftereffect_hellfireBoomEffect, target.transform.position, target.transform.rotation);

            Destroy(gameObject);
        }
    }
}
