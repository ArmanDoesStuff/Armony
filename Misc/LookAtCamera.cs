using System;
using UnityEngine;

namespace Armony.Misc
{
    public class LookAtCamera : MonoBehaviour
    {
        private Transform PlayerCamera { get; set; }

        private void Awake()
        {
            if (Camera.main != null) PlayerCamera = Camera.main.transform;
        }

        private void Update()
        {
            if (PlayerCamera == null) return;
            Vector3 lookDirection = transform.position - PlayerCamera.position ;
            lookDirection.y = 0;
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }
    }
}