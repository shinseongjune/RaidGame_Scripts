using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScene_CameraRotation : MonoBehaviour
{
    public float rotationSpeed = 10f;

    private void Start()
    {
        Camera.main.transform.LookAt(transform.position);
    }

    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}
