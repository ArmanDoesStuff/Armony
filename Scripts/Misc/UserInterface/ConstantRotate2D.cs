using UnityEngine;

namespace Armony.Misc.UserInterface
{
    public class ConstantRotate2D : MonoBehaviour
    {
        [SerializeField]
        private float m_rotateSpeed;
        private float RotateSpeed => m_rotateSpeed;
    
        private void Update()
        {
            transform.Rotate(Vector3.back, RotateSpeed * Time.deltaTime);
        }
    }
}
