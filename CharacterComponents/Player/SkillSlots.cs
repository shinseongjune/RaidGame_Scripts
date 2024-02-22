using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SkillSlots : MonoBehaviour
{
    public SkillSlot basicAttack = new SkillSlot();

    public Dictionary<string, SkillSlot> slots = new Dictionary<string, SkillSlot>();

    Stats stats;

    public Transform firePoint;

    public SkillSlot q
    {
        get { return slots["q"]; }
    }
    public SkillSlot w
    {
        get { return slots["w"]; }
    }
    public SkillSlot e
    {
        get { return slots["e"]; }
    }
    public SkillSlot r
    {
        get { return slots["r"]; }
    }

    /*
    public SkillSlot a;
    public SkillSlot s;
    public SkillSlot d;
    public SkillSlot f;
    
    public SkillSlot z;
    public SkillSlot x;
    public SkillSlot c;
    public SkillSlot v;
    */

    private void Awake()
    {
        stats = GetComponent<Stats>();

        PlayerData data = LoginDataManager.Instance.currentPlayer;
        List<Skill> basic = null;
        switch (data.recentCharacter)
        {
            case 0:
                basic = SkillDatabase.Instance.warriorBasic;
                break;
            case 1:
                basic = SkillDatabase.Instance.priestBasic;
                break;
            case 2:
                //basic = SkillDatabase.Instance.archerBasic;
                break;
        }
        if (basic is null)
        {
            throw new NullReferenceException("Basic Attack Dictionary is Null! Check Recent Character of PlayerData: currentPlayer of LoginDataManager");
        }
        basicAttack.skill = basic[data.dic_charSettings[data.recentCharacter].basic];

        List<Skill> skill = null;
        switch (data.recentCharacter)
        {
            case 0:
                skill = SkillDatabase.Instance.warriorSkill;
                break;
            case 1:
                skill = SkillDatabase.Instance.priestSkill;
                break;
            case 2:
                //skill = SkillDatabase.Instance.archerSkill;
                break;
        }
        if (skill is null)
        {
            throw new NullReferenceException("Skill Dictionary is Null! Check Recent Character of PlayerData: currentPlayer of LoginDataManager");
        }
        basicAttack.skill = basic[data.dic_charSettings[data.recentCharacter].basic];

        SkillSlot qSlot = new();
        int qIndex = data.dic_charSettings[data.recentCharacter].q;
        if (qIndex != -1)
        {
            qSlot.skill = skill[qIndex];
        }
        else
        {
            qSlot.skill = null;
        }
        slots.Add("q", qSlot);

        SkillSlot wSlot = new();
        int wIndex = data.dic_charSettings[data.recentCharacter].w;
        if (wIndex != -1)
        {
            wSlot.skill = skill[wIndex];
        }
        else
        {
            wSlot.skill = null;
        }
        slots.Add("w", wSlot);

        SkillSlot eSlot = new();
        int eIndex = data.dic_charSettings[data.recentCharacter].e;
        if (eIndex != -1)
        {
            eSlot.skill = skill[eIndex];
        }
        else
        {
            eSlot.skill = null;
        }
        slots.Add("e", eSlot);
    }

    void Update()
    {
        basicAttack.cooldown = Mathf.Max(basicAttack.cooldown - Time.deltaTime, 0);

        foreach (SkillSlot slot in slots.Values)
        {
            slot.cooldown = Mathf.Max(slot.cooldown - Time.deltaTime, 0);
        }
    }

    public void DoBasicAttack(Vector3 point)
    {
        Skill skill = basicAttack.skill;

        if (basicAttack.cooldown > 0)
        {
            return;
        }

        if (stats.UseMana(skill.cost, skill.costStat))
        {
            basicAttack.cooldown = skill.coolDown;
            Vector3 skillPosition;
            switch (skill.type)
            {
                case Skill.Type.PROJECTILE:
                    skillPosition = firePoint.position == null ? transform.position + Vector3.up : firePoint.position;

                    StartCoroutine(SpawnPrefab(skill, skill.skillPrefab, skillPosition, skill.preDelay, point));
                    //TODO: predelay, postdelay를 캐릭터컨트롤러에 전달하기.
                    break;
                case Skill.Type.PLACE:
                    break;
                case Skill.Type.TARGET:
                    break;
                case Skill.Type.INSTANT:
                    skillPosition = firePoint.position == null ? transform.position + Vector3.up : firePoint.position;

                    StartCoroutine(SpawnPrefab(skill, skill.skillPrefab, skillPosition, skill.preDelay, point));
                    break;
            }
        }
    }

    public bool DoSkill(string input, Vector3 point)
    {
        SkillSlot slot = slots[input];
        if (slot == null || slot.skill == null)
        {
            return false;
        }
        Skill skill = slot.skill;

        if (slot.cooldown > 0)
        {
            return false;
        }

        if (stats.UseMana(skill.cost, skill.costStat))
        {
            slot.cooldown = skill.coolDown;
            Vector3 skillPosition;
            switch (skill.type)
            {
                case Skill.Type.PROJECTILE:
                    skillPosition = firePoint.position == null ? transform.position + Vector3.up : firePoint.position;

                    StartCoroutine(SpawnPrefab(skill, skill.skillPrefab, skillPosition, skill.preDelay, point));
                    //TODO: predelay, postdelay를 캐릭터컨트롤러에 전달하기.
                    break;
                case Skill.Type.PLACE:

                    StartCoroutine(SpawnPrefab(skill, skill.skillPrefab, point, skill.preDelay, point));
                    break;
                case Skill.Type.TARGET:
                    break;
                case Skill.Type.INSTANT:
                    skillPosition = firePoint.position == null ? transform.position + Vector3.up : firePoint.position;

                    StartCoroutine(SpawnPrefab(skill, skill.skillPrefab, skillPosition, skill.preDelay, point));
                    break;
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    //TODO: 마우스 위치따라 시전 방향 받기.
    public IEnumerator SpawnPrefab(Skill skillObject, GameObject prefab, Vector3 position, float delay, Vector3 point)
    {
        yield return new WaitForSeconds(delay);

        SkillBase skill = Instantiate(prefab, position, transform.rotation).GetComponent<SkillBase>();
        NavMeshHit navMeshHit;
        if (NavMesh.SamplePosition(point, out navMeshHit, 100f, NavMesh.AllAreas))
        {
            skill.clickedPoint = navMeshHit.position;
        }
        skill.owner = gameObject;
        skill.source = skillObject;
        skill.GetOn();
    }

    /// <summary>
    /// use this method when character is cancelled skill casting
    /// </summary>
    public void CancelSkill()
    {
        StopAllCoroutines();
    }
}
