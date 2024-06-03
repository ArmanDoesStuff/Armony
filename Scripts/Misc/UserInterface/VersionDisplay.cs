//ArmanDoesStuff 2017

using TMPro;
using UnityEngine;

namespace Armony.Misc.UserInterface
{
    [RequireComponent(typeof(TMP_Text))]
    public class VersionDisplay : MonoBehaviour
    {
        [SerializeField]
        private bool m_showGameName = true;

        private void Start()
        {
            GetComponent<TMP_Text>().text = (m_showGameName ? Application.productName : "Version") + " - " + Application.version;
        }
    }
}