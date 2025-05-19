using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Armony.Misc.UserInterface
{
    public class SliderExtended : Slider
    {
        public UnityEvent selectEvent;
        public UnityEvent deselectEvent;

        public override void OnSelect(BaseEventData _eventData)
        {
            base.OnSelect(_eventData);
            selectEvent.Invoke();
        }

        public override void OnDeselect(BaseEventData _eventData)
        {
            base.OnDeselect(_eventData);
            deselectEvent.Invoke();
        }
    }
}