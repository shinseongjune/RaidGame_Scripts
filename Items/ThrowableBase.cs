using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ThrowableBase : ItemBase
{
    public Vector3 startPosition;
    public Vector3 endPosition;

    public static float FLIGHT_DURATION = 0.4f;
    public float elapsedTime = 0;

    public virtual void Update()
    {
        if (elapsedTime < FLIGHT_DURATION)
        {
            elapsedTime += Time.deltaTime;

            Flight();
        }
        else
        {
            WhenArrived();
        }
    }

    protected abstract void Flight();

    protected abstract void WhenArrived();

    public abstract override void GetOn();
}
