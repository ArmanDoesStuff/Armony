using UnityEngine;
#if ARMONY_NETCODE
using Unity.Netcode;
#endif

namespace Armony.Misc
{
    public class ConstantRotate : MonoBehaviour
    {
        [SerializeField] private Vector3 speed;

#if ARMONY_NETCODE
        [SerializeField] private bool useServerTime;
#endif

        private Quaternion startRotation;

        private void Start()
        {
            startRotation = transform.rotation;
        }

        private void Update()
        {
            float newTime = Time.time;
#if ARMONY_NETCODE
            if (useServerTime && NetworkManager.Singleton)
                newTime = (float)NetworkManager.Singleton.ServerTime.Time;
#endif
            transform.eulerAngles = startRotation.eulerAngles + (speed * newTime);
        }
    }
}