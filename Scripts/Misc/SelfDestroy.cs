//Created by Arman Awan - ArmanDoesStuff 2018

using System.Threading.Tasks;
using UnityEngine;

namespace Armony.Misc
{
    public class SelfDestroy : MonoBehaviour
    {
        [SerializeField]
        private int timeToDestroy = 5000;
        private async void Start()
        {
            await Task.Delay(timeToDestroy);
            Destroy(gameObject);

        }
    }
}