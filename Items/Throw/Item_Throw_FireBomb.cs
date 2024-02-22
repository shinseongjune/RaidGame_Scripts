using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Item_Throw_FireBomb : ThrowableBase
{
    float maxHeight;
    float rotationSpeed = 5.0f;

    public GameObject boomPrefab;

    private void Start()
    {
        maxHeight = transform.position.y + 0.7f;
    }

    public override void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

        base.Update();
    }

    public override void GetOn()
    {

    }

    protected override void Flight()
    {
        float elapsedRate = elapsedTime / FLIGHT_DURATION;
        Vector3 linearPosition = Vector3.Lerp(startPosition, endPosition, elapsedRate);

        float yOffset = 4 * maxHeight * elapsedRate * (1 - elapsedRate);

        transform.position = new Vector3(linearPosition.x, linearPosition.y + yOffset, linearPosition.z);
    }
    protected override void WhenArrived()
    {
        Instantiate(boomPrefab, transform.position, transform.rotation);

        Destroy(gameObject);
    }
}
