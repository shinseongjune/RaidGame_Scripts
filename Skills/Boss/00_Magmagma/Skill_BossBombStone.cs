using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Skill_BossBombStone : SkillBase
{
    public float lifeTime;
    float maxLifeTime;

    public Damage damage;
    public Stats stats;

    public GameObject afterEffect_BoomStoneBurst;

    public float knockbackPower;

    public SliderValueSetter hpBar;
    public Image boomCountImage;
    public float boomCountImageMaxX;

    public TempBossControlComponent bossControl;

    void Update()
    {
        if (stats.isDead)
        {
            bossControl.waitingTime = 0.3f;
            Destroy(gameObject);
            return;
        }

        lifeTime = Mathf.Max(0, lifeTime - Time.deltaTime);

        float restLifeTimeRate = lifeTime / maxLifeTime;
        float imageX = boomCountImageMaxX * restLifeTimeRate;
        Vector2 pos = boomCountImage.rectTransform.anchoredPosition;
        boomCountImage.rectTransform.anchoredPosition = new Vector2(imageX, pos.y);

        if (lifeTime <= 0)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 30f, 1 << LayerMask.NameToLayer("Player"));

            foreach (Collider collider in colliders)
            {
                ControlComponent control = collider.GetComponentInParent<ControlComponent>();
                control.Damaged(damage.damage);
                Vector3 knockbackVector = (control.transform.position - transform.position).normalized;

                KnockBack knockBack = new KnockBack("폭발의 여파", SpecialEffect.Type.Renewable, false, owner, control, knockbackVector, knockbackPower);

                control.AppendSpecialEffect(knockBack);
            }

            Instantiate(afterEffect_BoomStoneBurst, transform.position, transform.rotation);

            bossControl.waitingTime = 0.3f;
            Destroy(gameObject);
            return;
        }
    }

    public override void GetOn()
    {
        stats.InitializeStats();
        stats.enabled = true;

        hpBar.targetStats = stats;
        hpBar.targetType = Stat.Type.MaxHP;
        maxLifeTime = lifeTime;
        boomCountImageMaxX = boomCountImage.rectTransform.anchoredPosition.x;

        bossControl = owner.GetComponent<TempBossControlComponent>();

        //TODO: 임시 스탯 설정. 제대로 된 시스템 구축 후 수정 필요.
        damage = new();
        damage.damage = 210f;
        damage.type = Damage.Type.Fire;
        stats[(int)Stat.Type.MaxHP].SetBaseValue(300);
    }
}
