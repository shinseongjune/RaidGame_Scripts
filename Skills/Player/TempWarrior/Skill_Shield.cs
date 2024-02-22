using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Shield : SkillBase
{
    public float lifeTime;

    SpecialEffect effect;

    void Update()
    {
        if (owner is null)
        {
            Destroy(gameObject);

            return;
        }

        ControlComponent control;
        if (!owner.TryGetComponent(out control))
        {
            Destroy(gameObject);

            return;
        }

        if (control.isDead)
        {
            Destroy(gameObject);

            return;
        }

        transform.position = owner.transform.position;

        lifeTime -= Time.deltaTime;

        if (lifeTime <= 0 )
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        RemoveMod();
    }

    void ApplyMod()
    {
        CharacterControlComponent control = owner.GetComponent<CharacterControlComponent>();
        if (control != null)
        {
            control.AppendSpecialEffect(effect);
        }
    }

    void RemoveMod()
    {
        CharacterControlComponent control = owner.GetComponent<CharacterControlComponent>();
        if (control != null)
        {
            control.AddToRemoveSpecialEffectList(effect);
        }
    }

    public override void GetOn()
    {
        effect = new ShieldEffect("½Çµå", SpecialEffect.Type.Renewable, false, owner, owner.GetComponent<CharacterControlComponent>(), lifeTime);
        ApplyMod();
    }
}

public class ShieldEffect : SpecialEffect
{
    StatMod mod = new StatMod(Stat.Type.Armor, StatMod.Type.BaseAdd, 50, "Skill_Shield_Armor_50");

    public ShieldEffect(string name, Type type, bool isHidden, GameObject source, ControlComponent target, float endTime) : base(name, type, isHidden, source, target, endTime)
    {
    }

    public override void OnEnter()
    {
        Stats stats = Target.GetComponentInParent<Stats>();
        stats[(int)Stat.Type.Armor].AppendMod(mod);
    }

    public override void OnExit()
    {
        Stats stats = Target.GetComponentInParent<Stats>();
        stats[(int)Stat.Type.Armor].RemoveMod(mod);
    }

    public override void OnUpdate()
    {

    }
}