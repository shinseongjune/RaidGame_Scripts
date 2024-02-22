using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public CharacterBaseData data;

    List<Stat> stats = new List<Stat>();

    float REGEN_TICK_TIME = 0.2f;
    float hpRegenCurrent = 0.2f;
    float mpRegenCurrent = 0.2f;

    public bool canRegen = false;

    public bool isDead = false;

    bool isImmune = false;

    public bool IsImmune
    {
        get { return isImmune; }
    }

    [SerializeField]
    float hp;
    [SerializeField]
    float mp;

    public float HP
    {
        get { return hp; }
        private set { hp = value; }
    }

    public float MP
    {
        get { return mp; }
        private set { mp = value; }
    }

    public Stat this[int index]
    {
        get
        {
            return stats[index];
        }
    }

    public void InitializeStats()
    {
        if (data is null)
        {
            throw new NullReferenceException("No Character Data!! Please Check Character Summoning Code.");
        }

        stats.Add(new Stat(data.MaxHP));
        hp = data.MaxHP;
        stats.Add(new Stat(data.MaxMP));
        mp = data.MaxMP;
        stats.Add(new Stat(data.Might));
        stats.Add(new Stat(data.Armor));
        stats.Add(new Stat(data.FireResist));
        stats.Add(new Stat(data.ColdResist));
        stats.Add(new Stat(data.LightningResist));
        stats.Add(new Stat(data.CritChance));
        stats.Add(new Stat(data.CritDamage));
    }

    public void SetImmunityOn()
    {
        isImmune = true;
    }

    public void SetImmunityOff()
    {
        isImmune = false;
    }

    //TODO: control.damaged로 고쳐야함
    public void Damaged(float damage)
    {
        if (IsImmune)
        {
            return;
        }

        float reducedDamage = damage - stats[(int)Stat.Type.Armor].Current;
        if (reducedDamage <= 0)
        {
            return;
        }

        hp = Mathf.Max(hp - reducedDamage, 0);

        if (hp <= 0)
        {
            Die(); //TODO: photonView.RPC(Die, RpcTarget.All)
        }
    }

    void Die()
    {
        isDead = true;
        GetComponent<ControlComponent>().Die();
    }

    public void Healed(float heal)
    {
        hp = Mathf.Min(hp + heal, stats[(int)Stat.Type.MaxHP].Current);
    }

    public bool UseMana(float cost, Skill.CostStat costStat)
    {
        if ((costStat == Skill.CostStat.MP && mp < cost) || (costStat == Skill.CostStat.HP && hp < cost))
        {
            return false;
        }
        else
        {
            switch (costStat)
            {
                case Skill.CostStat.MP:
                    mp -= cost;
                    return true;
                case Skill.CostStat.HP:
                    hp -= cost;
                    return true;
            }
        }

        //something wrong
        return false;
    }

    public void HealMana(float heal)
    {
        mp = Mathf.Min(mp + heal, stats[(int)Stat.Type.MaxMP].Current);
    }

    private void Update()
    {
        if (canRegen)
        {
            if (hp != stats[(int)Stat.Type.MaxHP].Current)
            {
                hpRegenCurrent -= Time.deltaTime;

                if (hpRegenCurrent <= 0)
                {
                    hp = Mathf.Min(hp + 0.5f, stats[(int)Stat.Type.MaxHP].Current);

                    hpRegenCurrent = REGEN_TICK_TIME;
                }
            }

            if (mp != stats[(int)Stat.Type.MaxMP].Current)
            {
                mpRegenCurrent -= Time.deltaTime;

                if (mpRegenCurrent <= 0)
                {
                    mp = Mathf.Min(mp + 0.5f, stats[(int)Stat.Type.MaxMP].Current);

                    mpRegenCurrent = REGEN_TICK_TIME;
                }
            }
        }
    }
}
