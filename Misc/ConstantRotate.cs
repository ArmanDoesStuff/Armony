using UnityEngine;

namespace Armony.Misc
{
    public class ConstantRotate : MonoBehaviour
    {
        [SerializeField]
        private Vector3 m_speed;
        private Vector3 Speed => m_speed;
        

        private void Update()
        {
            transform.eulerAngles += Speed * Time.deltaTime;
        }
    }
}
