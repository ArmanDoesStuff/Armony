//ArmanDoesStuff 2017

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Armony.Misc.UserInterface
{
    public class ButtonExtended : Button //TODO: Make abstract and move interaction sounds child - https://armandoesstuff.atlassian.net/browse/ARM-3
    {

        public UnityEvent submittedEvent;
        public Action<bool> selectedEvent;
        public Action<bool> highlightedEvent;
        public Action<bool> pressedEvent;

        protected bool Selected { get; private set; }

        public override void OnPointerClick(PointerEventData _eventData)
        {
            base.OnPointerClick(_eventData);
            Submitted();
        }

        public override void OnSubmit(BaseEventData _eventData)
        {
            base.OnSubmit(_eventData);
            Submitted();
        }

        public override void OnSelect(BaseEventData _eventData)
        {
            base.OnSelect(_eventData);
            SetSelected(true);
        }

        public override void OnDeselect(BaseEventData _eventData)
        {
            base.OnDeselect(_eventData);
            SetSelected(false);
        }

        public override void OnPointerEnter(PointerEventData _eventData)
        {
            base.OnPointerEnter(_eventData);
            SetHighlighted(true);
        }

        public override void OnPointerExit(PointerEventData _eventData)
        {
            base.OnPointerExit(_eventData);
            SetHighlighted(false);
        }

        public override void OnPointerDown(PointerEventData _eventData)
        {
            base.OnPointerDown(_eventData);
            SetPressed(true);
        }

        public override void OnPointerUp(PointerEventData _eventData)
        {
            base.OnPointerUp(_eventData);
            SetPressed(false);
        }

        protected virtual void SetSelected(bool _selected)  
        {
            Selected = _selected;
            selectedEvent?.Invoke(_selected);
        }

        protected virtual void SetHighlighted(bool _highlighted)
        {
            if(Selected) return;
            highlightedEvent?.Invoke(_highlighted);
        }
        
        protected virtual void SetPressed(bool _pressed)
        {
            if(Selected) return;
            pressedEvent?.Invoke(_pressed);
        }
        protected virtual void Submitted()
        {
            submittedEvent?.Invoke();
        }
    }
}
