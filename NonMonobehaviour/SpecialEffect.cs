using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpecialEffect : IEquatable<SpecialEffect>
{
    public enum Type
    {
        Sequencial,
        Renewable,
        SharedDuration,
        IndividualDuration,
        Hidden,
    }

    public string Name
    {
        get;
        private set;
    }

    public Type EffectType
    {
        get;
        private set;
    }

    public bool IsHidden
    {
        get;
        private set;
    }

    public GameObject Source
    {
        get;
        private set;
    }

    public ControlComponent Target
    {
        get;
        private set;
    }

    public float EndTime
    {
        get;
        private set;
    }

    public float Duration
    {
        get;
        set;
    }

    bool isEnd = false;

    public bool IsEnd
    {
        get
        {
            return isEnd;
        }
        private set
        {
            isEnd = value;
        }
    }

    int stack = 1;

    public int Stack
    {
        get
        {
            return stack;
        }
        set
        {
            stack = value;
        }
    }

    public SpecialEffect(string name, Type type, bool isHidden, GameObject source, ControlComponent target, float endTime)
    {
        Name = name;
        EffectType = type;
        IsHidden = isHidden;
        Source = source;
        Target = target;
        EndTime = endTime;
        Duration = endTime;
    }

    /// <param name="deltaTime">일반적으로 Time.deltaTime을 사용할 것.</param>
    public void UpdateTime(float deltaTime)
    {
        Duration -= deltaTime;
        if (Duration <= 0)
        {
            IsEnd = true;
        }
    }

    public abstract void OnEnter();

    public abstract void OnUpdate();

    public abstract void OnExit();

    public bool Equals(SpecialEffect other)
    {
        return Name == other.Name;
    }
}

public class Stun : SpecialEffect
{
    public Stun(string name, Type type, bool isHidden, GameObject source, ControlComponent target, float endTime) : base(name, type, isHidden, source, target, endTime) { }

    public override void OnEnter()
    {
        Target.actPreventer++;
    }

    public override void OnExit()
    {
        Target.actPreventer--;
    }

    public override void OnUpdate()
    {
        Duration -= Time.deltaTime;

        if (Duration <= 0)
        {
            Target.AddToRemoveSpecialEffectList(this);
        }
    }
}

public class Snare : SpecialEffect
{
    public Snare(string name, Type type, bool isHidden, GameObject source, ControlComponent target, float endTime) : base(name, type, isHidden, source, target, endTime) { }

    public override void OnEnter()
    {
        Target.movePreventer++;
    }

    public override void OnExit()
    {
        Target.movePreventer--;
    }

    public override void OnUpdate()
    {
        Duration -= Time.deltaTime;

        if (Duration <= 0)
        {
            Target.AddToRemoveSpecialEffectList(this);
        }
    }
}

public class KnockBack : SpecialEffect
{
    public static float KnockBack_Time = 0.1f;
    public Vector3 direction;

    public KnockBack(string name, Type type, bool isHidden, GameObject source, ControlComponent target, Vector3 dir, float knockbackPower) : base(name, type, isHidden, source, target, KnockBack_Time)
    {
        if (dir == Vector3.zero)
        {
            direction = (target.transform.forward * -1) * knockbackPower;
        }
        else
        {
            direction = dir * knockbackPower;
        }
    }

    public override void OnEnter()
    {
        if (!Target.isEnd && !Target.isDead && Target.movement != null)
        {
            Target.actPreventer++;
            Target.movement.GetKnockBack(direction);
        }
    }

    public override void OnExit()
    {
        if (!Target.isEnd && !Target.isDead && Target.movement != null)
        {
            Target.actPreventer--;
            Target.movement.EnableMovement();
        }
    }

    public override void OnUpdate()
    {
        Duration -= Time.deltaTime;
        if (Duration <= 0)
        {
            Target.AddToRemoveSpecialEffectList(this);
        }
    }
}