using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityGLTF.Interactivity.Frontend
{
    public abstract class SocketUI : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
    {
        [Header("Internal References")]
        [SerializeField] protected TextMeshProUGUI _title;
        [SerializeField] protected Image _ring;

        [field: SerializeField] public SocketSide socketSide { get; private set; }
        [field: SerializeField] public SocketType socketType { get; private set; }

        public NodeUI nodeUI { get; private set; }
        public Transform socketPoint => _ring.transform;

        public void OnBeginDrag(PointerEventData eventData)
        {
            nodeUI.graphUI.flowsManager.ShowTempFlow(socketPoint, eventData.pointerCurrentRaycast.worldPosition, socketSide);
        }

        public void OnDrag(PointerEventData eventData)
        {
            nodeUI.graphUI.flowsManager.MoveTempFlowTargetPoint(eventData.pointerCurrentRaycast.worldPosition);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            nodeUI.graphUI.flowsManager.HideTempFlow();

            var endObj = eventData.pointerCurrentRaycast.gameObject;

            if (!endObj.TryGetComponent(out SocketUIPoint endPoint))
                return;

            var endSocket = endPoint.socket;

            // Don't connect inputs to inputs or outputs to outputs.
            if (endSocket.socketSide == socketSide)
                return;

            var thisSideIsInput = this.socketSide == SocketSide.Input;
            var input = thisSideIsInput ? this : endSocket;
            var output = thisSideIsInput ? endSocket : this;

            ConnectSockets(input, output);
        }

        public void SetData(NodeUI nodeUI)
        {
            this.nodeUI = nodeUI;
        }

        protected abstract void ConnectSockets(SocketUI input, SocketUI output);
    }
}