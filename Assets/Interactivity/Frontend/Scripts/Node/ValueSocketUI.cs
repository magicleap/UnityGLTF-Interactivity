using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UnityGLTF.Interactivity.Frontend
{
    public class ValueSocketUI : SocketUI
    {
        [SerializeField] private TMP_InputField _input;
        [SerializeField] private Image _dot;
        [SerializeField] private Image _line;

        public NodeValue valueData { get; private set; }
        public Value value { get; private set; }

        public void SetValue(NodeValue valueData)
        { 
            this.valueData = valueData;
            _title.text = valueData.id;

            if (socketSide == SocketSide.Input)
            {
                if (!nodeUI.node.TryGetValueById(valueData.id, out Value value))
                    throw new InvalidOperationException($"No value found on {nodeUI.node.type} with id {valueData.id}.");

                this.value = value;

                value.onConnectionChanged += OnConnectionChanged;
                SetUpSocketByValue();

                _input.onValueChanged.AddListener(OnInputValueChanged);
            }
        }

        private void OnInputValueChanged(string text)
        {
            // TODO: Fix this after spec change
            //value.property = Helpers.CreateProperty(typeof(float), text);
        }

        private void OnConnectionChanged(Value v)
        {
            SetUpSocketByValue();
        }

        private void SetUpSocketByValue()
        {
            var showLiteralInput = value.node == null;

            _input.gameObject.SetActive(showLiteralInput);
            _dot.enabled = showLiteralInput;
            _line.enabled = showLiteralInput;

            if(showLiteralInput)
                _input.text = value.property.ToString();
        }

        protected override void ConnectSockets(SocketUI input, SocketUI output)
        {
            var i = input as ValueSocketUI;
            var o = output as ValueSocketUI;

            i.nodeUI.node.TryChangeValueToReference(i.valueData.id, o.nodeUI.node, o.valueData.id, out _);
        }

        private void OnDestroy()
        {
            if(value != null)
                value.onConnectionChanged -= OnConnectionChanged;
        }
    }
}