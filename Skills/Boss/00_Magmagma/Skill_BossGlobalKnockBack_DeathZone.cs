using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Skill_BossGlobalKnockBack_DeathZone : MonoBehaviour
{
    public Damage damage;

    public Dictionary<GameObject, int> targets = new Dictionary<GameObject, int>();

    public bool isOn = false;

    void Start()
    {
        damage = new();
        damage.damage = 45f;
        damage.type = Damage.Type.Fire;
    }

    private void Update()
    {
        if (isOn)
        {
            foreach (GameObject target in targets.Keys.ToList())
            {
                if (target != null)
                {
                    target.GetComponentInParent<ControlComponent>().Damaged(damage.damage * Time.deltaTime);
                }
            }
        }
    }

    public void Triggered(GameObject obj)
    {
        if (!targets.ContainsKey(obj))
        {
            targets.Add(obj, 1);
        }
        else
        {
            targets[obj] += 1;
        }
    }

    public void TriggerExited(GameObject obj)
    {
        if (targets.ContainsKey(obj))
        {
            targets[obj] -= 1;

            if (targets[obj] <= 0)
            {
                targets.Remove(obj);
            }
        }
    }
}
