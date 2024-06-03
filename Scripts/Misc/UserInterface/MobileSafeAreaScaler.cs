using UnityEngine;

namespace Armony.Misc.UserInterface
{
    public class MobileSafeAreaScaler : MonoBehaviour
    {
        private void Awake() 
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            Vector2 screenSize = new(Screen.width, Screen.height);

            rectTransform.anchorMin = Screen.safeArea.position / screenSize;
            rectTransform.anchorMax = (Screen.safeArea.position + Screen.safeArea.size) / screenSize;
        }
    }
}
