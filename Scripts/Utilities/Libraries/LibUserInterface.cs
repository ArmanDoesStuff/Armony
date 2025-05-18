//Copyright AWAN SOFTWORKS LTD 2025

using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Armony.Utilities.Libraries
{
    public static class LibUserInterface
    {
        public static void SetListener(this Button _button, UnityAction _action)
        {
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(_action);
        }

        public static void Passthrough(this CanvasGroup _canvasGroup, bool _pass)
        {
            _canvasGroup.interactable = _canvasGroup.blocksRaycasts = !_pass;
        }

        public static bool Contains(this RectTransform _self, RectTransform _rect)
        {
            Vector3[] selfCorners = new Vector3[4];
            _self.GetWorldCorners(selfCorners);

            Vector3[] objectCorners = new Vector3[4];
            _rect.GetWorldCorners(objectCorners);

            return selfCorners[0].x <= objectCorners[0].x //bottom left corner
                   && selfCorners[0].y <= objectCorners[0].y
                   && selfCorners[2].x >= objectCorners[2].x //top right corner
                   && selfCorners[2].y >= objectCorners[2].y;
        }

        public static void AnchorToCorner(this RectTransform _t)
        {
            RectTransform tP = _t.parent as RectTransform;

            Vector2 newAnchorsMin = new(_t.anchorMin.x + _t.offsetMin.x / tP.rect.width, _t.anchorMin.y + _t.offsetMin.y / tP.rect.height);
            Vector2 newAnchorsMax = new(_t.anchorMax.x + _t.offsetMax.x / tP.rect.width, _t.anchorMax.y + _t.offsetMax.y / tP.rect.height);
            _t.anchorMin = newAnchorsMin;
            _t.anchorMax = newAnchorsMax;

            _t.offsetMin = _t.offsetMax = new Vector2(0, 0);
        }

        public static void AnchorToCanvas(this RectTransform _t)
        {
            _t.anchorMin = new Vector2(0, 0);
            _t.anchorMax = new Vector2(1, 1);
        }

        public static void CornerToAnchor(this RectTransform _t)
        {
            _t.offsetMin = _t.offsetMax = new Vector2(0, 0);
        }

        public static void Maximise(this RectTransform _t)
        {
            _t.AnchorToCanvas();
            _t.AnchorToCorner();
        }

        /// <summary>
        /// Moves the content of a ScrollRect to ensure the selected item is seen.
        /// Used OnSelect for buttons in a ScrollRect
        /// </summary>
        /// <param name="_snap"></param>
        public static void MoveContentToReveal(this RectTransform _snap)
        {
            if (_snap.TryGetComponentInParent(out ScrollRect scrollRect))
                MoveContentToReveal(_snap, scrollRect);
        }

        private static void MoveContentToReveal(this RectTransform _snap, ScrollRect _scroll)
        {
            RectTransform scrollRect = (RectTransform)_scroll.transform;
            RectTransform contentRect = _scroll.content;

            if (!scrollRect.Contains(_snap))
                contentRect.anchoredPosition = (Vector2)scrollRect.InverseTransformPoint(contentRect.position) - new Vector2(0, scrollRect.InverseTransformPoint(_snap.position).y);
        }

        public static Selectable LinkNavigationVertical(Selectable[] _fileButtons)
        {
            for (int i = 0; i < _fileButtons.Length; i++)
            {
                Navigation n = _fileButtons[i].navigation;
                n.selectOnUp = _fileButtons.Looped(i - 1);
                n.selectOnDown =_fileButtons.Looped(i + 1);
                _fileButtons[i].navigation = n;
            }

            return _fileButtons[0];
        }

        public static void FadeInstant(this CanvasGroup _cGroup, bool _setActive)
        {
            _cGroup.interactable = _cGroup.blocksRaycasts = _setActive;
            _cGroup.alpha = _setActive.ToInt();
            _cGroup.gameObject.SetActive(_setActive);
        }
        public static async Task Fade(this CanvasGroup _cGroup, bool _setActive, float _timeTaken = 0.3f, float _delay = 0f)
        {
            _cGroup.interactable = _cGroup.blocksRaycasts = _setActive;
            if (Math.Abs(_cGroup.alpha - _setActive.ToInt()) < 0.1f)
            {
                _cGroup.alpha = _setActive.ToInt();
                _cGroup.gameObject.SetActive(_setActive);
                return;
            }
            await Fade(_cGroup, _setActive.ToInt(), _timeTaken, _delay, _setActive);
        }

        public static async Task Fade(this CanvasGroup _cGroup, float _endAlpha, float _timeTaken = 0.3f, float _delay = 0f, bool _activeAfter = true)
        {
            await Task.Delay((int)(_delay * 1000));
            if (!_cGroup.gameObject.activeSelf)
                _cGroup.gameObject.SetActive(true);
            if (Math.Abs(_cGroup.alpha - _endAlpha) < 0.01f && _cGroup.gameObject.activeSelf == _activeAfter) //Check if nothing will be changed
                return;

            float startAlpha = _cGroup.alpha;
            float timer = 0;
            _cGroup.interactable = false;
            while (timer <= 1)
            {
                _cGroup.alpha = Mathf.Lerp(startAlpha, _endAlpha, timer);
                timer += Time.deltaTime / _timeTaken;
                await Task.Yield();
            }

            _cGroup.alpha = _endAlpha;
            _cGroup.interactable = _cGroup.blocksRaycasts = _activeAfter;
            _cGroup.gameObject.SetActive(_activeAfter);
        }

        /// <summary>
        /// Adds a space before capital letters for use with Pascal case names
        /// </summary>
        /// <param name="_text"></param>
        /// <returns></returns>
        public static string PascalSpace(string _text)
        {
            if (string.IsNullOrEmpty(_text))
                return _text;

            System.Text.StringBuilder stringBuilder = new(_text.Length * 2);
            stringBuilder.Append(_text[0]);

            for (int i = 1; i < _text.Length; i++)
            {
                if (char.IsUpper(_text[i]) &&
                    (char.IsLower(_text[i - 1]) || char.IsDigit(_text[i - 1]) || char.IsPunctuation(_text[i - 1]) || !char.IsUpper(_text[i - 1])))
                    stringBuilder.Append(' ');
                stringBuilder.Append(_text[i]);
            }

            return stringBuilder.ToString();
        }
    }
}