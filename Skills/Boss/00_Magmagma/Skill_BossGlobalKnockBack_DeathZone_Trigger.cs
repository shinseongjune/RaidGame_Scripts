using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_BossGlobalKnockBack_DeathZone_Trigger : MonoBehaviour
{
    public Skill_BossGlobalKnockBack_DeathZone parentScript;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            parentScript.Triggered(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            parentScript.TriggerExited(other.gameObject);
        }
    }
}
