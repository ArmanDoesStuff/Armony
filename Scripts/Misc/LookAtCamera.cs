using System;
using UnityEngine;

namespace Armony.Misc
{
    public class LookAtCamera : MonoBehaviour
    {
        private Transform mainCameraTransform;
        private Transform MainCameraTransform => mainCameraTransform ??= Camera.main.transform;

        private void FixedUpdate()
        {
            transform.LookAt(MainCameraTransform);
        }
    }
}