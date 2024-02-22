using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombStoneControlComponent : ControlComponent
{
    public override void Die()
    {
        GetComponent<Skill_BossBombStone>().bossControl.waitingTime = 0.3f;
        Destroy(gameObject);
    }

    public override void EndMovement()
    {

    }
}
