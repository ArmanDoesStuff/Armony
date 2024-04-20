using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMoveUI : MonoBehaviour
{
    [SerializeField]
    private float m_rotateSpeed;
    private float RotateSpeed => m_rotateSpeed;
    
    private void Update()
    {
        transform.Rotate(Vector3.back, RotateSpeed * Time.deltaTime);
    }
}
