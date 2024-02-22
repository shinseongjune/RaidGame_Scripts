using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TempBossControlComponent : ControlComponent
{
    public enum State
    {
        Normal,
        Chasing,
        Casting,
        Disappeared,
        Global,
        BombStone,
    }

    public enum SkillName
    {
        basic,
        knockback,
        multiknockback,
        globalknockback,
        bombstone,
        hellfire
    }

    float TARGET_HOLD_DURATION_MIN = 2f;
    float TARGET_HOLD_DURATION_MAX = 8f;
    float targetHoldDuration = 0f;

    public Animator animator;

    SkillName nowSkillName;
    string nowSkillNameForAnimatorSetBool;

    public State nowState = State.Normal;
    public SkillSlot nowSkill;
    public GameObject target;
    public float sight = 60f;

    public float waitingTime;

    //특수 기술 사용 가능 여부
    bool canUseGlobalKnockBack = true;
    bool canUseBombStone = true;
    bool canUseBombStone2 = true;

    //스킬 인스펙터 할당
    public Skill basic;
    public Skill knockback;
    public Skill globalknockback;
    public Skill bombstone;
    public Skill hellfire;

    //스킬슬롯
    public SkillSlot basicAttackSlot = new();
    public SkillSlot KnockBackSkillSlot = new();
    public SkillSlot MultiKnockBackSkillSlot = new();

    public SkillSlot globalKnockBackSkillSlot = new();
    public SkillSlot bombStoneSkillSlot = new();
    public SkillSlot hellfireBallSkillSlot = new();

    //스킬 발사 위치
    public Transform skillPoint;
    public Transform hellfirePoint;

    public override void Awake()
    {
        base.Awake();

        animator = GetComponent<Animator>();

        nowSkill = basicAttackSlot;
        nowSkillName = SkillName.basic;

        basicAttackSlot.skill = basic;
        KnockBackSkillSlot.skill = knockback;
        MultiKnockBackSkillSlot.skill = knockback;
        globalKnockBackSkillSlot.skill = globalknockback;
        bombStoneSkillSlot.skill = bombstone;
        hellfireBallSkillSlot.skill = hellfire;
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

        if (basicAttackSlot.cooldown > 0) basicAttackSlot.cooldown -= Time.deltaTime;
        if (KnockBackSkillSlot.cooldown > 0) KnockBackSkillSlot.cooldown -= Time.deltaTime;
        if (MultiKnockBackSkillSlot.cooldown > 0) MultiKnockBackSkillSlot.cooldown -= Time.deltaTime;

        if (hellfireBallSkillSlot.cooldown > 0) hellfireBallSkillSlot.cooldown -= Time.deltaTime;

        if (canUseBombStone2 && stats.HP <= 0.25f * stats[(int)Stat.Type.MaxHP].Current)
        {
            nowSkill = bombStoneSkillSlot;
            nowSkillName = SkillName.bombstone;
            nowState = State.BombStone;
        }
        else if (canUseBombStone && stats.HP <= 0.5f * stats[(int)Stat.Type.MaxHP].Current)
        {
            nowSkill = bombStoneSkillSlot;
            nowSkillName = SkillName.bombstone;
            nowState = State.BombStone;
        }
        else if (canUseGlobalKnockBack && stats.HP <= 0.75f * stats[(int)Stat.Type.MaxHP].Current)
        {
            nowSkill = globalKnockBackSkillSlot;
            nowSkillName = SkillName.globalknockback;
            nowState = State.Global; ;
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
                        animator.SetBool("isWalking", true);
                        movement.MoveTo(target.transform.position);
                    }
                }
                //범위 도달 시 기술 사용
                if (isReached)
                {
                    animator.SetBool("isWalking", false);
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
                    animator.SetBool(nowSkillNameForAnimatorSetBool, false);
                    nowState = State.Normal;
                }
                break;
            case State.Disappeared:
                //무적
                if (isDisappeared)
                {
                    animator.SetBool("isWalking", false);
                    transform.GetChild(0).gameObject.SetActive(false); //TODO: [PunRPC] SetVisibility(bool value) photonView.RPC("SetVisibility", RpcTarget.All, true);
                    movement.agent.enabled = false;
                }

                waitingTime -= Time.deltaTime;
                if (waitingTime <= 0)
                {
                    nowState = State.Normal;
                }
                break;
            case State.Global:
                animator.SetBool(nowSkillNameForAnimatorSetBool, false);
                //중심이동
                if (Vector3.Distance(transform.position, mapCenter) > 0.5f)
                {
                    animator.SetBool("isWalking", true);
                    movement.SetStoppingDistance(0.1f);
                    movement.MoveTo(mapCenter);
                }
                else
                {
                    //글로벌넉백 사용->disap
                    DoSkill();

                    canUseGlobalKnockBack = false;

                    waitingTime = 7.5f;
                    isDisappeared = true;
                    nowState = State.Disappeared;
                    stats.SetImmunityOn();
                }
                break;
            case State.BombStone:
                animator.SetBool(nowSkillNameForAnimatorSetBool, false);
                //중심이동
                if (Vector3.Distance(transform.position, mapCenter) > 0.5f)
                {
                    animator.SetBool("isWalking", true);
                    movement.SetStoppingDistance(0.1f);
                    movement.MoveTo(mapCenter);
                }
                else
                {
                    //bombstone 사용->disap, canusebomb = false, 이미 false일 경우 canusebomb2 = false
                    DoSkill();

                    if (canUseBombStone)
                    {
                        canUseBombStone = false;
                    }
                    else
                    {
                        canUseBombStone2 = false;
                    }

                    waitingTime = 15f;
                    isDisappeared = true;
                    nowState = State.Disappeared;
                    stats.SetImmunityOn();
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

        Collider[] colliders = Physics.OverlapSphere(transform.position, sight, 1 << LayerMask.NameToLayer("Player"));

        //TODO: aggro 계산 등
        Collider closest = null;
        float minDistance = float.MaxValue;
        foreach(Collider collider in colliders)
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

        if (stats.HP <= 0.25f * stats[(int)Stat.Type.MaxHP].Current && hellfireBallSkillSlot.cooldown <= 0)
        {
            nowSkill = hellfireBallSkillSlot;
            nowSkillName = SkillName.hellfire;
        }
        else if (MultiKnockBackSkillSlot.cooldown <= 0)
        {
            nowSkill = MultiKnockBackSkillSlot;
            nowSkillName = SkillName.multiknockback;
        }
        else if (KnockBackSkillSlot.cooldown <= 0)
        {
            nowSkill = KnockBackSkillSlot;
            nowSkillName = SkillName.knockback;
        }
        else
        {
            nowSkill = basicAttackSlot;
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
                nowSkillNameForAnimatorSetBool = "isBaseAttacking";
                animator.SetBool(nowSkillNameForAnimatorSetBool, true);
                skillPosition = skillPoint.position;
                StartCoroutine(SpawnPrefab(nowSkill.skill.skillPrefab, skillPosition, nowSkill.skill.preDelay));
                break;
            case SkillName.knockback:
                nowSkillNameForAnimatorSetBool = "isSpecialAttacking";
                animator.SetBool(nowSkillNameForAnimatorSetBool, true);
                skillPosition = target.transform.position;
                StartCoroutine(SpawnPrefab(nowSkill.skill.skillPrefab, skillPosition, nowSkill.skill.preDelay));
                break;
            case SkillName.multiknockback:
                nowSkillNameForAnimatorSetBool = "isSpecialAttacking";
                animator.SetBool(nowSkillNameForAnimatorSetBool, true);
                StartCoroutine(SpawnPrefab(nowSkill.skill.skillPrefab, nowSkill.skill.preDelay));
                StartCoroutine(SpawnPrefab(nowSkill.skill.skillPrefab, nowSkill.skill.preDelay + waitingTime));
                StartCoroutine(SpawnPrefab(nowSkill.skill.skillPrefab, nowSkill.skill.preDelay + waitingTime * 2));
                waitingTime *= 3;
                break;
            case SkillName.globalknockback:
                skillPosition = Vector3.zero;
                StartCoroutine(SpawnPrefab(nowSkill.skill.skillPrefab, skillPosition, nowSkill.skill.preDelay));
                break;
            case SkillName.bombstone:
                skillPosition = Vector3.zero;
                StartCoroutine(SpawnPrefab(nowSkill.skill.skillPrefab, skillPosition, nowSkill.skill.preDelay));
                break;
            case SkillName.hellfire:
                nowSkillNameForAnimatorSetBool = "isHellfireAttacking";
                animator.SetBool(nowSkillNameForAnimatorSetBool, true);
                skillPosition = hellfirePoint.position;
                StartCoroutine(SpawnPrefab(nowSkill.skill.skillPrefab, skillPosition, nowSkill.skill.preDelay));
                break;
        }
    }

    public IEnumerator SpawnPrefab(GameObject prefab, Vector3 position, float delay)
    {
        yield return new WaitForSeconds(delay);

        //TODO: Instantiate
        SkillBase skill = Instantiate(prefab, position, transform.rotation).GetComponent<SkillBase>();
        skill.owner = gameObject;
        skill.source = nowSkill.skill;
        if (target != null) skill.target = target;
        skill.GetOn();
    }

    public IEnumerator SpawnPrefab(GameObject prefab, float delay)
    {
        yield return new WaitForSeconds(delay);

        //TODO: Instantiate
        SkillBase skill = Instantiate(prefab, target.transform.position, transform.rotation).GetComponent<SkillBase>();
        skill.owner = gameObject;
        skill.source = nowSkill.skill;
        if (target != null) skill.target = target;
        skill.GetOn();
    }

    public void CancelSkill()
    {
        StopAllCoroutines();
    }

    public override void Die()
    {
        animator.SetBool("isDead", true);
        EndMovement();
        StopAllCoroutines();
        movement.CancelMove();
        isDead = true;
    }

    public override void EndMovement()
    {
        animator.SetBool("isWalking", false);
    }
}
