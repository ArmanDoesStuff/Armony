using UnityEngine;
using Random = UnityEngine.Random;
#if ARMONY_NETCODE
using Unity.Netcode;
#endif

namespace Armony.Misc
{
    public class ConstantOscillate : MonoBehaviour
    {
        public Vector3 offset;
        public float timeTaken = 1f;
        private float startTime;
#if ARMONY_NETCODE
        [SerializeField] private bool useServerTime;
#endif

        private Vector3 initialPosition;

        private void OnValidate()
        {
            startTime = Random.Range(0f, 1f);
        }

        private void Start()
        {
            initialPosition = transform.localPosition;
        }

        private void Update()
        {
            float newTime = Time.time;
#if ARMONY_NETCODE
            if (useServerTime && NetworkManager.Singleton)
                newTime = (float)NetworkManager.Singleton.ServerTime.Time;
#endif
            float currentTime = startTime + newTime / timeTaken;
            transform.localPosition = initialPosition + offset * Mathf.Sin(currentTime);
        }
    }
}