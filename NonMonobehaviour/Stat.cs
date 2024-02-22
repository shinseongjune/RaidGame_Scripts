using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatMod : IComparable<StatMod>
{
    public enum Type
    {
        BaseAdd,
        BaseMul,
        TotalMulA,
        TotalMulB,
        TotalMulC,
    }

    public Stat.Type TargetStat
    {
        get;
    }

    public Type ModType
    {
        get;
    }

    public float Value
    {
        get;
    }

    public string Name
    {
        get;
    }

    public int stack = 1;
    public readonly int MAX_STACKS;

    /// <param name="value">곱연산의 경우 기본 배율 1을 제외한 추가 배율만 작성할 것.
    /// ex) 50% 추가 피해의 경우 0.5</param>
    /// <param name="name">이름 양식은 영문으로 '소스이름_대상스탯_수치_필요시추가정보'로 지정할 것.</param>
    /// <param name="maxStacks">기본값 1</param>
    public StatMod(Stat.Type targetStat, Type modType, float value, string name, int maxStacks = 1)
    {
        if (maxStacks < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(MAX_STACKS), "값은 1 이상이어야 합니다.");
        }

        TargetStat = targetStat;
        ModType = modType;
        Value = value;
        Name = name;
        MAX_STACKS = maxStacks;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        return Name == ((StatMod)obj).Name;
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }

    public int CompareTo(StatMod other)
    {
        return ModType.CompareTo(other.ModType);
    }
}

[Serializable]
public class Stat
{
    public enum Type
    {
        MaxHP,
        MaxMP,
        Might,
        Armor,
        FireRes,
        ColdRes,
        LightningRes,
        MovementSpeed,
        CritChance,
        CritDamage,
        __COUNT
    }

    public float BaseValue
    {
        get;
        private set;
    }

    float currentValue;

    public float Current
    {
        get
        {
            if (IsDirty)
            {
                CalculateCurrentValue();
            }
            return currentValue;
        }
        private set
        {
            currentValue = value;
        }
    }

    public bool IsDirty
    {
        get;
        private set;
    }

    List<StatMod> mods = new List<StatMod>();

    public Stat(float baseValue)
    {
        BaseValue = baseValue;
        Current = baseValue;
        IsDirty = true;
    }

    public void AppendMod(StatMod mod)
    {
        StatMod found = mods.Find((x) => x.Equals(mod));

        if (found != null)
        {
            found.stack++;
        }
        else
        {
            mods.Add(mod);
            mods.Sort();
        }

        IsDirty = true;
    }

    public void RemoveMod(StatMod mod)
    {
        StatMod found = mods.Find((x) => x.Equals(mod));

        if (found != null)
        {
            found.stack--;

            if (found.stack <= 0)
            {
                mods.Remove(found);
            }

            IsDirty = true;
        }
    }

    void CalculateCurrentValue()
    {
        float origin = BaseValue;

        List<StatMod> totalMulA = new List<StatMod>();
        List<StatMod> totalMulB = new List<StatMod>();
        List<StatMod> totalMulC = new List<StatMod>();

        foreach (StatMod mod in mods)
        {
            switch (mod.ModType)
            {
                case StatMod.Type.BaseAdd:
                    origin += mod.Value * mod.stack;
                    break;
                case StatMod.Type.BaseMul:
                    origin *= 1 + mod.Value * mod.stack;
                    break;
                case StatMod.Type.TotalMulA:
                    totalMulA.Add(mod);
                    break;
                case StatMod.Type.TotalMulB:
                    totalMulB.Add(mod);
                    break;
                case StatMod.Type.TotalMulC:
                    totalMulC.Add(mod);
                    break;
            }
        }

        origin *= 1 + CalculateTotalMulMods(totalMulA);
        origin *= 1 + CalculateTotalMulMods(totalMulB);
        origin *= 1 + CalculateTotalMulMods(totalMulC);

        currentValue = origin;

        IsDirty = false;
    }

    float CalculateTotalMulMods(List<StatMod> mods)
    {
        float total = 0;
        foreach (StatMod mod in mods)
        {
            total += mod.Value * mod.stack;
        }
        return total;
    }

    //TODO: 임시 베이스밸류 세팅 함수. 제대로 스탯 로드&초기화 시스템 구축 후 지울것.
    public void SetBaseValue(float value)
    {
        BaseValue = value;
        IsDirty = true;
    }
}