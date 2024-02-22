using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bilboard : MonoBehaviour
{
    Transform camTrans;
    RectTransform rect;

    private void Awake()
    {
        camTrans = Camera.main.transform;
        rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        rect.LookAt(camTrans.position);
    }
}
