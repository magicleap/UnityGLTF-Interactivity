using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GLTF.Interactivity.Frontend
{
    public class DraggableUI : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] private RectTransform _draggingPlane;
        [SerializeField] private PointerEventData.InputButton _mouseButton;
        private Vector3 _lastMousePos;

        public event Action onDragBegin;
        public event Action onDragEnd;

        public void OnBeginDrag(PointerEventData data)
        {
            if (data.button != _mouseButton)
                return;

            // TODO: Add undo/redo for begin and enddrag
            if (!RectTransformUtility.ScreenPointToWorldPointInRectangle(_draggingPlane, data.position, data.pressEventCamera, out var globalMousePos))
                return;

            _lastMousePos = globalMousePos;

            onDragBegin?.Invoke();
        }

        public void OnDrag(PointerEventData data)
        {
            if (data.button != _mouseButton)
                return;

            if (!RectTransformUtility.ScreenPointToWorldPointInRectangle(_draggingPlane, data.position, data.pressEventCamera, out var globalMousePos))
                return;

            _draggingPlane.position += globalMousePos - _lastMousePos;
            _draggingPlane.rotation = _draggingPlane.rotation;
            _lastMousePos = globalMousePos;
        }

        public void OnEndDrag(PointerEventData data)
        {
            if (data.button != _mouseButton)
                return;

            onDragEnd?.Invoke();
        }
    }
}