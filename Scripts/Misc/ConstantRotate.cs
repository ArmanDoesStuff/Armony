using UnityEngine;
using UnityEngine.Serialization;

namespace Armony.Misc
{
    public class ConstantRotate : MonoBehaviour
    {
        [SerializeField]
        private Vector3 speed;
        

        private void Update()
        {
            transform.Rotate(speed * Time.deltaTime);
        }
    }
}
