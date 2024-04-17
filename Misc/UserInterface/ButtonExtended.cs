//ArmanDoesStuff 2017

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Armony.Misc.UserInterface
{
    public class ButtonExtended : Button //TODO: Make abstract and move interaction sounds child - https://armandoesstuff.atlassian.net/browse/ARM-3
    {

        [SerializeField]
        private UnityEvent m_submittedEvent;
        public UnityEvent SubmittedEvent => m_submittedEvent;

        public delegate void ButtonEvent(bool activate);
        
        public event ButtonEvent SelectedEvent;
        public event ButtonEvent HighlightedEvent;

        protected bool Selected { get; private set; }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            Submitted();
        }

        public override void OnSubmit(BaseEventData eventData)
        {
            base.OnSubmit(eventData);
            Submitted();
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            SetSelected(true);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            SetSelected(false);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            SetHighlighted(true);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            SetHighlighted(false);
        }

        protected virtual void SetSelected(bool selected)
        {
            Selected = selected;
            SelectedEvent?.Invoke(selected);
        }

        protected virtual void SetHighlighted(bool highlighted)
        {
            if(Selected) return;
            HighlightedEvent?.Invoke(highlighted);
        }
        
        protected virtual void Submitted()
        {
            SubmittedEvent?.Invoke();
        }
    }
}
