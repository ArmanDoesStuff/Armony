using UnityEngine;

namespace Armony.Misc
{
    public class ConstantOscillate : MonoBehaviour
    {
        [SerializeField]
        private Vector3 m_offset;
        private Vector3 Offset => m_offset;
        
        [SerializeField]
        private float m_timeTaken = 1f;
        private float TimeTaken => m_timeTaken;
        
        private Vector3 InitialPosition { get; set; }
        private float Timer { get; set; }

        private void Start()
        {
            InitialPosition = transform.localPosition;
            Timer += Random.Range(0f, 1f); //Start randomly along path
        }

        private void Update()
        {
            Timer += Time.deltaTime / TimeTaken;
            transform.localPosition = InitialPosition + Offset * Mathf.Sin(Timer);
        }
    }
}