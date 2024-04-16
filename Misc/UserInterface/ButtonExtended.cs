//ArmanDoesStuff 2017

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Armony.Misc.UserInterface
{
    public class ButtonExtended : Selectable, IPointerClickHandler, ISubmitHandler //TODO: Make abstract and move interaction sounds child - https://armandoesstuff.atlassian.net/browse/ARM-3
    {

        [SerializeField]
        private UnityEvent m_submitted;
        public UnityEvent Submitted => m_submitted;

        public event EventHandler<bool> SelectedEvent;
        public event EventHandler<bool> HighlightedEvent;

        private bool Selected { get; set; }

        public void OnPointerClick(PointerEventData eventData)
        {
            Submitted?.Invoke();
        }

        public void OnSubmit(BaseEventData eventData)
        {
            Submitted?.Invoke();
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
            SelectedEvent?.Invoke(this,selected);
        }

        protected virtual void SetHighlighted(bool highlighted)
        {
            if(Selected) return;
            HighlightedEvent?.Invoke(this, highlighted);
        }
    }
}
