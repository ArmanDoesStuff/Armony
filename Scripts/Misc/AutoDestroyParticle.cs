using UnityEngine;

namespace Armony.Misc
{
    public class AutoDestroyParticle : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem mainParticleSystem;

        private void Update()
        {
            if (mainParticleSystem == null || !mainParticleSystem.IsAlive())
            {
                Destroy(gameObject);
            }
        }
    }
}