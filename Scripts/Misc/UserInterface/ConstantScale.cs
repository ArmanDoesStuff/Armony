using UnityEngine;

namespace Armony.Misc.UserInterface
{
    public class ConstantScale : MonoBehaviour
    {
        [SerializeField]
        private Vector3 offset;
        
        [SerializeField]
        private float timeTaken = 1f;

        private Vector3 initialScale;
        private float timer;

        private void Start()
        {
            initialScale = transform.localScale;
        }

        private void Update()
        {
            timer += Time.deltaTime / timeTaken;
            transform.localScale = initialScale + offset * Mathf.Sin(timer);
        }
    }
}