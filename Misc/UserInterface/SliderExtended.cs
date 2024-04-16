using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Armony.Misc.UserInterface
{
    public class SliderExtended : Slider
    {
        [SerializeField]
        private UnityEvent m_selectE;
        private UnityEvent SelectEvent => m_selectE;
        [SerializeField]
        private UnityEvent m_deselectEvent;
        private UnityEvent DeselectEvent => m_deselectEvent;
        
        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            SelectEvent.Invoke();
        }
        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            DeselectEvent .Invoke();
        }
    }
}
