using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace GLTF.Interactivity.Frontend
{
    public class ScrollToZoom : MonoBehaviour, IScrollHandler
    {
        [SerializeField] private RectTransform _transformToScale;
        [SerializeField] private float _scrollScale = 0.1f;

        private float _scale = 1f;

        public void OnScroll(PointerEventData data)
        {
            var t = _transformToScale as RectTransform;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(t, data.position, data.pressEventCamera, out var pos))
            {
                SetPivot(t, pos);
            }

            var scrollValue = data.scrollDelta.y;

            if (scrollValue > 0)
                _scale *= 1f + _scrollScale;
            else if (scrollValue < 0)
                _scale *= 1f - _scrollScale;

            _transformToScale.localScale = _scale * Vector3.one;
        }

        private void SetPivot(RectTransform rectTransform, Vector2 localPoint)
        {
            // Calculate the pivot by normalizing the values
            var targetRect = rectTransform.rect;
            var pivotX = (float)((localPoint.x - (double)targetRect.x) / (targetRect.xMax - (double)targetRect.x));
            var pivotY = (float)((localPoint.y - (double)targetRect.y) / (targetRect.yMax - (double)targetRect.y));
            var newPivot = new Vector2(pivotX, pivotY);

            // Delta pivot = (current - new) * scale
            var deltaPivot = (rectTransform.pivot - newPivot) * _scale;
            // The delta position to add after pivot change is the inversion of the delta pivot change * size of the rect * current scale of the rect
            var rectSize = targetRect.size;
            var deltaPosition = new Vector3(deltaPivot.x * rectSize.x, deltaPivot.y * rectSize.y) * -1f;

            // Set the pivot
            rectTransform.pivot = newPivot;
            rectTransform.localPosition += deltaPosition;
        }
    }
}