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
            Vector3 targetPosition = PlayerCamera.transform.position;
            targetPosition.y = transform.position.y;
            transform.rotation = Quaternion.LookRotation(transform.position - targetPosition);
        }
    }
}