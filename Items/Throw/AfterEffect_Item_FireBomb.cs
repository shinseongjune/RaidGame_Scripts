using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterEffect_Item_FireBomb : MonoBehaviour
{
    //TODO: damage 제대로 설정할 것.
    float damage = 30f;

    float duration = 5f;

    void Update()
    {
        if (duration <= 0)
        {
            Destroy(gameObject);
        }

        duration -= Time.deltaTime;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.root.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            other.transform.root.GetComponent<ControlComponent>().Damaged(damage * Time.deltaTime);
        }
    }
}
