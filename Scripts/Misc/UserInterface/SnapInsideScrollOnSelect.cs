using UnityEngine;
using UnityEngine.UI;
using Armony.Utilities.Libraries;

namespace Armony.Misc.UserInterface
{
    [RequireComponent(typeof(ButtonExtended))]
    public class SnapInsideScrollOnSelect : MonoBehaviour
    {
        private void Awake()
        {
            ScrollRect s = GetComponentInParent<ScrollRect>();
            if (s == null)
            {
                Debug.LogError("No ScrollRect found above " + gameObject.name);
                return;
            }

            RectTransform rect = GetComponent<RectTransform>();
            GetComponent<ButtonExtended>().SelectedEvent += selected =>
            {
                if (selected)
                {
                    rect.MoveContentToReveal();
                }
            };
        }
    }
}