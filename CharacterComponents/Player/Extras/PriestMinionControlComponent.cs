using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriestMinionControlComponent : ControlComponent
{
    public enum State
    {
        Idle,
        Chasing,
        Attacking,
    }

    float TARGET_HOLD_DURATION_MIN = 2f;
    float TARGET_HOLD_DURATION_MAX = 8f;
    float targetHoldDuration = 0f;

    public Animator animator;

    public State nowState = State.Idle;
    public SkillSlot attack;
    public Skill attackObject;
    public GameObject target;
    public float sight = 60f;

    public float waitingTime;

    public Transform attackPoint;

    public GameObject go_hpCanvas;

    float destroyTime = 5f;

    public override void Awake()
    {
        base.Awake();

        TryGetComponent(out animator);
        TryGetComponent(out stats);

        attack = new SkillSlot();
        attack.skill = attackObject;

        stats.InitializeStats();
    }

    public override void Update()
    {
        if (isDead || isEnd)
        {
            stats.SetImmunityOn();
            destroyTime -= Time.deltaTime;
            if (destroyTime <= 0)
            {
                Destroy(gameObject);
            }
            return;
        }

        base.Update();

        if (actPreventer > 0)
        {
            CancelSkill();
            return;
        }
        if (attack.cooldown > 0) attack.cooldown -= Time.deltaTime;

        Think();
    }

    void Think()
    {
        switch (nowState)
        {
            case State.Idle:
            case State.Chasing:
            case State.Attacking:
                if (!FindTarget())
                {
                    return;
                }
                break;
        }

        switch (nowState)
        {
            case State.Idle:
                if (target is not null)
                {
                    nowState = State.Chasing;
                }
                break;
            case State.Chasing:
                if (target is null)
                {
                    nowState = State.Idle;
                    break;
                }
                //move preventer > 0일 경우 이동스킵
                bool isReached = Vector3.Distance(target.transform.position, transform.position) <= movement.agent.stoppingDistance;
                if (movePreventer == 0)
                {
                    if (!isReached)
                    {
                        //animator.SetBool("isWalking", true);
                        movement.MoveTo(target.transform.position);
                    }
                }
                //범위 도달 시 기술 사용
                if (isReached)
                {
                    //animator.SetBool("isWalking", false);
                    //기술 사용
                    if (DoAttack())
                    {
                        nowState = State.Attacking;
                    }
                    //캐스팅으로 전환
                }
                break;
            case State.Attacking:
                //시간 후 normal
                waitingTime -= Time.deltaTime;
                if (waitingTime <= 0)
                {
                    //animator.SetBool(nowSkillBoolName, false);
                    nowState = State.Idle;
                }
                break;
        }
    }

    bool FindTarget()
    {
        targetHoldDuration -= Time.deltaTime;

        if (target != null)
        {
            ControlComponent targetControl = target.GetComponentInParent<ControlComponent>();
            if (targetControl is not null)
            {
                if (targetControl.isDead || targetControl.isDisappeared)
                {
                    target = null;
                }
            }
        }

        if (target != null && targetHoldDuration > 0)
        {
            return true;
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, sight, 1 << LayerMask.NameToLayer("Enemy"));

        Collider closest = null;
        float minDistance = float.MaxValue;
        foreach (Collider collider in colliders)
        {
            if (collider.GetComponentInParent<ControlComponent>().isDead || collider.GetComponentInParent<ControlComponent>().isDisappeared)
            {
                continue;
            }

            float dis = Vector3.Distance(transform.position, collider.transform.position);

            if (dis < minDistance)
            {
                minDistance = dis;
                closest = collider;
            }
        }

        if (closest is not null)
        {
            target = closest.gameObject;
            SetStoppingDistance();
            float holdDuration = Random.Range(TARGET_HOLD_DURATION_MIN, TARGET_HOLD_DURATION_MAX);
            targetHoldDuration = holdDuration;

            return true;
        }
        return false;
    }

    void SetStoppingDistance()
    {
        float stopDist = GetComponent<CapsuleCollider>().radius + target.GetComponentInChildren<CapsuleCollider>().radius;
        if (attack.skill != null)
        {
            stopDist += attack.skill.range;
        }
        movement.SetStoppingDistance(stopDist);
    }

    bool DoAttack()
    {
        if (target is null || attack is null || attack.cooldown > 0)
        {
            return false;
        }

        Look(target.transform.position);

        //공격 시전
        waitingTime = attack.skill.preDelay + attack.skill.postDelay;
        attack.cooldown = attack.skill.coolDown;

        //animator.SetBool(nowSkillBoolName, true);
        StartCoroutine(SpawnPrefab(attack.skill.skillPrefab, attackPoint.position, attack.skill.preDelay));
        return true;
    }

    public IEnumerator SpawnPrefab(GameObject prefab, Vector3 position, float delay)
    {
        yield return new WaitForSeconds(delay);

        //TODO: Instantiate
        SkillBase skill = Instantiate(prefab, position, transform.rotation).GetComponent<SkillBase>();
        skill.owner = gameObject;
        skill.source = attack.skill;
        if (target != null) skill.target = target;
        skill.GetOn();
    }

    public void CancelSkill()
    {
        StopAllCoroutines();
    }

    public override void Die()
    {
        go_hpCanvas.SetActive(false);
        Destroy(movement.agent);
        Destroy(movement);
        //EndMovement();
        StopAllCoroutines();
        stats.SetImmunityOn();
        //animator.SetTrigger("DyingTrigger");

        transform.GetChild(0).gameObject.SetActive(false);
        isDead = true;
    }

    public override void EndMovement()
    {
        //animator.SetBool("isWalking", false);
        //animator.SetBool("isDashing", false);
        //animator.SetBool("isKnockBacked", false);
    }
}
