using UnityEngine;
using UnityEngine.Serialization;

namespace Armony.Misc
{
    public class ConstantOscillate : MonoBehaviour
    {
        [SerializeField]
        private Vector3 offset;
        
        [SerializeField]
        private float timeTaken = 1f;

        private Vector3 initialPosition;
        private float timer;

        private void Start()
        {
            initialPosition = transform.localPosition;
            timer += Random.Range(0f, 1f);
        }

        private void Update()
        {
            timer += Time.deltaTime / timeTaken;
            transform.localPosition = initialPosition + offset * Mathf.Sin(timer);
        }
    }
}