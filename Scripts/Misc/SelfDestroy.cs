//Created by Arman Awan - ArmanDoesStuff 2018

using UnityEngine;

namespace Armony.Misc
{
    public class SelfDestroy : MonoBehaviour
    {
        [SerializeField]
        private float m_timeToDestroy = 5f;
        private void Start()
        {
            Invoke("DestroySelf", m_timeToDestroy);
        }

        private void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}