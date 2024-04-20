//AWAN SOFTWORKS LTD 2023

using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = System.Object;

namespace Armony.Utilities.Libraries
{
    public static class LibUserInterface
    {
        public static void SetListener(this Button button, UnityAction action)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(action);
        }

        public static bool Contains(this RectTransform self, RectTransform rect)
        {
            Vector3[] selfCorners = new Vector3[4];
            self.GetWorldCorners(selfCorners);

            Vector3[] objectCorners = new Vector3[4];
            rect.GetWorldCorners(objectCorners);

            return selfCorners[0].x <= objectCorners[0].x //bottom left corner
                   && selfCorners[0].y <= objectCorners[0].y
                   && selfCorners[2].x >= objectCorners[2].x //top right corner
                   && selfCorners[2].y >= objectCorners[2].y;
        }

        public static void AnchorToCorner(this RectTransform t)
        {
            RectTransform tP = t.parent as RectTransform;

            Vector2 newAnchorsMin = new(t.anchorMin.x + t.offsetMin.x / tP.rect.width, t.anchorMin.y + t.offsetMin.y / tP.rect.height);
            Vector2 newAnchorsMax = new(t.anchorMax.x + t.offsetMax.x / tP.rect.width, t.anchorMax.y + t.offsetMax.y / tP.rect.height);
            t.anchorMin = newAnchorsMin;
            t.anchorMax = newAnchorsMax;

            t.offsetMin = t.offsetMax = new Vector2(0, 0);
        }

        public static void AnchorToCanvas(this RectTransform t)
        {
            t.anchorMin = new Vector2(0, 0);
            t.anchorMax = new Vector2(1, 1);
        }

        public static void CornerToAnchor(this RectTransform t)
        {
            t.offsetMin = t.offsetMax = new Vector2(0, 0);
        }
        
        public static void Maximise(this RectTransform t)
        {
            t.AnchorToCanvas();
            t.AnchorToCorner();
        }

        public static void MoveContentToReveal(this RectTransform snap) =>
            MoveContentToReveal(snap, snap.GetComponentInParent<ScrollRect>());

        public static void MoveContentToReveal(this RectTransform snap, ScrollRect scroll)
        {
            RectTransform scrollRect = (RectTransform)scroll.transform;
            RectTransform contentRect = scroll.content;

            if (!scrollRect.Contains(snap))
                contentRect.anchoredPosition = (Vector2)scrollRect.InverseTransformPoint(contentRect.position) - new Vector2(0, scrollRect.InverseTransformPoint(snap.position).y);
        }

        public static Selectable LinkNavigationVertical(Selectable[] fileBtns)
        {
            int len = fileBtns.Length;
            for (int i = 0; i < len; i++)
            {
                Navigation n = fileBtns[i].navigation;
                n.selectOnUp = fileBtns[(i - 1).LoopInt(len)];
                n.selectOnDown = fileBtns[(i + 1).LoopInt(len)];
                fileBtns[i].navigation = n;
            }

            return fileBtns[0];
        }

        public static async Task Fade(this CanvasGroup cGroup, bool endAlpha, bool force = false, float timeTaken = 0.3f, float delay = 0f)
        {
            if (force)
                cGroup.alpha = (!endAlpha).ToInt();
            await Fade(cGroup, endAlpha.ToInt(), timeTaken, delay, endAlpha);
        }

        public static async Task Fade(this CanvasGroup cGroup, float endAlpha, float timeTaken = 0.3f, float delay = 0f, bool activeAfter = true)
        {
            await Task.Delay((int)(delay * 1000));
            if (!cGroup.gameObject.activeSelf)
                cGroup.gameObject.SetActive(true);
            if (Math.Abs(cGroup.alpha - endAlpha) < 0.01f && cGroup.gameObject.activeSelf == activeAfter) //Check if nothing will be changed
                return;

            float startAlpha = cGroup.alpha;
            float timer = 0;
            timer = 0;
            cGroup.interactable = cGroup.blocksRaycasts = false;
            while (timer <= 1)
            {
                cGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, timer);
                timer += Time.deltaTime / timeTaken;
                await Task.Yield();
            }

            cGroup.alpha = endAlpha;

            cGroup.interactable = cGroup.blocksRaycasts = activeAfter;
            cGroup.gameObject.SetActive(activeAfter);
        }
    }
}