using UnityEngine;

namespace Armony.Misc.UserInterface
{
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
}
