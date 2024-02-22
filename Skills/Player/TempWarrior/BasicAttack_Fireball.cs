using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttack_Fireball : SkillBase
{
    public float speed;
    public Damage damage;

    public float lifeTime;

    Rigidbody rb;

    [SerializeField]
    GameObject afterEffect_Explosion;
    void Update()
    {
        lifeTime -= Time.deltaTime;

        Vector3 movePosition = transform.position + transform.forward * speed * Time.deltaTime;
        rb.MovePosition(movePosition);

        if (lifeTime <= 0)
        {
            Boom();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") && !alreadyHitObjects.Contains(other.gameObject))
        {
            alreadyHitObjects.Add(other.gameObject);
            other.GetComponentInParent<ControlComponent>().Damaged(damage.damage);
            Boom();
        }
    }

    void Boom()
    {
        AfterEffect_BasicAttack_Fireball boom = Instantiate(afterEffect_Explosion, transform.position, transform.rotation).GetComponent<AfterEffect_BasicAttack_Fireball>();
        boom.SetDataAndTriggerOn(damage, alreadyHitObjects, owner);
        Destroy(gameObject);
    }

    public override void GetOn()
    {
        rb = GetComponent<Rigidbody>();

        damage = new Damage();
        damage.damage = 20 + owner.GetComponent<Stats>()[(int)Stat.Type.Might].Current;
    }

}
