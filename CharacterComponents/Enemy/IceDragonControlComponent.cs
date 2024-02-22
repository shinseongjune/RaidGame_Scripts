using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static TempBossControlComponent;

public class IceDragonControlComponent : ControlComponent
{
    public enum State
    {
        Normal,
        Chasing,
        Casting,
        Disappeared,
        Point,
        Infinite
    }

    public enum SkillName
    {
        basic,
        ThreeMissiles,
        PointShooting,
        InfiniteShooting,
    }

    float TARGET_HOLD_DURATION_MIN = 2f;
    float TARGET_HOLD_DURATION_MAX = 8f;
    float targetHoldDuration = 0f;

    public Animator animator;

    public SkillName nowSkillName;
    //string nowSkillNameForAnimatorSetBool;

    public State nowState = State.Normal;
    public SkillSlot nowSkill;
    public GameObject target;
    public float sight = 60f;

    public float waitingTime;

    //특수 기술 사용 가능 여부
    bool canUsePointShooting = true;
    bool canUsePointShooting2 = true;
    bool canUseInfiniteShooting = true;
    bool canUsePointShooting3 = true;

    //스킬 인스펙터 할당
    public Skill basic;
    public Skill iceMissile;
    public Skill pointPattern;
    public Skill infinitePattern;

    //스킬슬롯
    public SkillSlot basicSlot = new();
    public SkillSlot iceMissileSlot = new();
    public SkillSlot pointSkillSlot = new();
    public SkillSlot infiniteSkillSlot = new();

    //스킬 발사 위치
    public Transform skillPoint;
    
    public override void Awake()
    {
        base.Awake();

        animator = GetComponent<Animator>();

        nowSkill = iceMissileSlot;
        nowSkillName = SkillName.ThreeMissiles;

        basicSlot.skill = basic;
        iceMissileSlot.skill = iceMissile;
        pointSkillSlot.skill = pointPattern;
        infiniteSkillSlot.skill = infinitePattern;
    }
    
    public override void Update()
    {
        if (isDead || isEnd)
        {
            return;
        }

        base.Update();
        if (actPreventer > 0)
        {
            CancelSkill();
            return;
        }

        if (basic.coolDown > 0) basic.coolDown -= Time.deltaTime;
        if (iceMissileSlot.cooldown > 0) iceMissileSlot.cooldown -= Time.deltaTime;

        if (canUsePointShooting && stats.HP <= 0.25f * stats[(int)Stat.Type.MaxHP].Current)
        {
            nowSkill = pointSkillSlot;
            nowSkillName = SkillName.PointShooting;
            nowState = State.Point;
        }
        else if (canUsePointShooting2 && stats.HP <= 0.5f * stats[(int)Stat.Type.MaxHP].Current)
        {
            nowSkill = pointSkillSlot;
            nowSkillName = SkillName.PointShooting;
            nowState = State.Point;
        }
        else if (!canUsePointShooting2 && canUseInfiniteShooting && stats.HP <= 0.5f * stats[(int)Stat.Type.MaxHP].Current)
        {
            nowSkill = infiniteSkillSlot;
            nowSkillName = SkillName.InfiniteShooting;
            nowState = State.Infinite;
        }
        else if (canUsePointShooting3 && stats.HP <= 0.75f * stats[(int)Stat.Type.MaxHP].Current)
        {
            nowSkill = pointSkillSlot;
            nowSkillName = SkillName.PointShooting;
            nowState = State.Point;
        }

        Think();
    }
    
    void Think()
    {
        switch (nowState)
        {
            case State.Normal:
            case State.Chasing:
            case State.Casting:
                if (!FindTarget())
                {
                    return;
                }
                break;
        }

        switch (nowState)
        {
            case State.Normal:
                if (isDisappeared)
                {
                    isDisappeared = false;
                    movement.agent.enabled = true;
                    transform.GetChild(0).gameObject.SetActive(true); //TODO: [PunRPC] SetVisibility(bool value) photonView.RPC("SetVisibility", RpcTarget.All, true);
                    stats.SetImmunityOff();
                }

                //기술선택
                SetNowSkill();

                //추격으로 전환
                nowState = State.Chasing;
                break;
            case State.Chasing:
                if (target == null || nowSkill == null)
                {
                    nowState = State.Normal;
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
                    DoSkill();
                    //캐스팅으로 전환
                    nowState = State.Casting;
                }
                break;
            case State.Casting:
                //시간 후 normal
                waitingTime -= Time.deltaTime;
                if (waitingTime <= 0)
                {
                    //animator.SetBool(nowSkillNameForAnimatorSetBool, false);
                    nowState = State.Normal;
                }
                break;
            case State.Disappeared:
                //무적
                if (isDisappeared)
                {
                    //animator.SetBool("isWalking", false);
                    transform.GetChild(0).gameObject.SetActive(false); //TODO: [PunRPC] SetVisibility(bool value) photonView.RPC("SetVisibility", RpcTarget.All, true);
                    movement.agent.enabled = false;
                }

                waitingTime -= Time.deltaTime;
                if (waitingTime <= 0)
                {
                    nowState = State.Normal;
                }
                break;
            case State.Point:
                //animator.SetBool(nowSkillNameForAnimatorSetBool, false);
                //중심이동
                if (canUsePointShooting && Vector3.Distance(transform.position, mapCenter) > 0.5f)
                {
                    //animator.SetBool("isWalking", true);
                    movement.SetStoppingDistance(0.1f);
                    movement.MoveTo(mapCenter);
                }
                else if (canUsePointShooting)
                {
                    //글로벌넉백 사용->disap
                    DoSkill();

                    canUsePointShooting = false;

                    waitingTime = 7.5f;
                    isDisappeared = true;
                    nowState = State.Disappeared;
                    stats.SetImmunityOn();
                }
                else
                {
                    //글로벌넉백 사용->disap
                    DoSkill();

                    if (canUsePointShooting2)
                    {
                        canUsePointShooting2 = false;
                    }
                    else if (canUsePointShooting3)
                    {
                        canUsePointShooting3 = false;
                    }

                    nowState = State.Normal;
                }
                break;
            case State.Infinite:
                //animator.SetBool(nowSkillNameForAnimatorSetBool, false);

                DoSkill();

                canUseInfiniteShooting = false;

                nowState = State.Normal;

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

        Collider[] colliders = Physics.OverlapSphere(transform.position, sight, 1 << LayerMask.NameToLayer("Player"));

        //TODO: aggro 계산 등
        Collider closest = null;
        float minDistance = float.MaxValue;
        foreach (Collider collider in colliders)
        {
            if (collider.GetComponentInParent<ControlComponent>().isDead)
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
            target = closest.transform.root.gameObject;
            SetStoppingDistance();
            float holdDuration = Random.Range(TARGET_HOLD_DURATION_MIN, TARGET_HOLD_DURATION_MAX);
            targetHoldDuration = holdDuration;

            return true;
        }
        return false;
    }

    void SetStoppingDistance()
    {
        float stopDist = GetComponentInChildren<CapsuleCollider>().radius + target.GetComponentInChildren<CapsuleCollider>().radius;

        if (nowSkill != null)
        {
            stopDist += nowSkill.skill.range;
        }

        movement.SetStoppingDistance(stopDist);
    }

    void SetNowSkill()
    {
        if (target == null)
        {
            return;
        }
        if (iceMissileSlot.cooldown <= 0)
        {
            nowSkill = iceMissileSlot;
            nowSkillName = SkillName.ThreeMissiles;
        }
        else
        {
            nowSkill = basicSlot;
            nowSkillName = SkillName.basic;
        }
        
        SetStoppingDistance();
    }

    void DoSkill()
    {
        if (target == null || nowSkill == null || nowSkill.cooldown > 0)
        {
            return;
        }

        Look(target.transform.position);

        //스킬 시전
        waitingTime = nowSkill.skill.preDelay + nowSkill.skill.postDelay;
        nowSkill.cooldown = nowSkill.skill.coolDown;
        Vector3 skillPosition;
        switch (nowSkillName)
        {
            case SkillName.basic:
                //nowSkillNameForAnimatorSetBool = "isBaseAttacking";
                //animator.SetBool(nowSkillNameForAnimatorSetBool, true);
                skillPosition = skillPoint.position;
                StartCoroutine(SpawnPrefab(nowSkill.skill.skillPrefab, skillPosition, transform.rotation, nowSkill.skill.preDelay));
                break;
            case SkillName.ThreeMissiles:
                //nowSkillNameForAnimatorSetBool = "isBaseAttacking";
                //animator.SetBool(nowSkillNameForAnimatorSetBool, true);
                skillPosition = skillPoint.position;
                StartCoroutine(SpawnPrefab(nowSkill.skill.skillPrefab, skillPosition, transform.rotation, nowSkill.skill.preDelay));
                StartCoroutine(SpawnPrefab(nowSkill.skill.skillPrefab, skillPosition, transform.rotation * Quaternion.Euler(0, 45, 0), nowSkill.skill.preDelay));
                StartCoroutine(SpawnPrefab(nowSkill.skill.skillPrefab, skillPosition, transform.rotation * Quaternion.Euler(0, -45, 0), nowSkill.skill.preDelay));
                break;
            case SkillName.PointShooting:
                //nowSkillNameForAnimatorSetBool = "isSpecialAttacking";
                //animator.SetBool(nowSkillNameForAnimatorSetBool, true);

                StartCoroutine(SpawnPrefab(nowSkill.skill.skillPrefab, mapCenter, Quaternion.identity, nowSkill.skill.preDelay));
                break;
            case SkillName.InfiniteShooting:
                StartCoroutine(SpawnPrefab(nowSkill.skill.skillPrefab, mapCenter, Quaternion.identity, nowSkill.skill.preDelay));
                break;
        }
    }

    public IEnumerator SpawnPrefab(GameObject prefab, Vector3 position, Quaternion rotation, float delay)
    {
        yield return new WaitForSeconds(delay);

        //TODO: Instantiate
        SkillBase skill = Instantiate(prefab, position, rotation).GetComponent<SkillBase>();
        skill.owner = gameObject;
        skill.source = nowSkill.skill;

        skill.GetOn();
    }
    
    public void CancelSkill()
    {
        StopAllCoroutines();
    }

    public override void Die()
    {
        //animator.SetBool("isDead", true);
        EndMovement();
        StopAllCoroutines();
        movement.CancelMove();
        isDead = true;
    }

    public override void EndMovement()
    {
        //animator.SetBool("isWalking", false);
    }
}
