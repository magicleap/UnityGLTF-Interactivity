using GLTF.Interactivity.Frontend;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UnityGLTF.Interactivity.Frontend
{
    public class NodeUI : MonoBehaviour
    {
        [Header("Asset References")]
        [SerializeField] private GameObject _inputFlowPrefab;
        [SerializeField] private GameObject _inputValuePrefab;
        [SerializeField] private GameObject _outputFlowPrefab;
        [SerializeField] private GameObject _outputValuePrefab;
        [SerializeField] private GameObject _configPrefab;

        [Header("Internal References")]
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private Transform _inputsContainer;
        [SerializeField] private Transform _outputsContainer;
        [SerializeField] private Transform _configContainer;
        [SerializeField] private DraggableUI _draggableUI;

        public Node node { get; private set; }
        public GraphUI graphUI { get; private set; }
        public RectTransform rectTransform { get; private set; }
        private NodeSpecifications _nodeUIData;

        private Dictionary<string, SocketUI> _inputs = new();
        private Dictionary<string, SocketUI> _outputs = new();
        private List<ConfigEntry> _configurations = new();

        public ReadOnlyDictionary<string, SocketUI> inputs;
        public ReadOnlyDictionary<string, SocketUI> outputs;

        internal void SetData(GraphUI graphUI, Node node)
        {
            inputs = new(_inputs);
            outputs = new(_outputs);

            this.node = node;
            this.graphUI = graphUI;

            _title.text = node.type;
            rectTransform = transform as RectTransform;
            rectTransform.anchoredPosition = new Vector2((float)node.metadata.positionX, (float)node.metadata.positionY);
            _draggableUI.onDragEnd += RecordPositionToMetadata;

            _nodeUIData = NodeRegistry.nodeSpecs[node.type];

            GenerateUIElements(_nodeUIData);
        }

        private void GenerateUIElements(NodeSpecifications nodeUIData)
        {
            var configuration = nodeUIData.GetConfiguration();
            var inputs = nodeUIData.GetInputs();
            var outputs = nodeUIData.GetOutputs();

            if (inputs.flows != null)
            {
                for (int i = 0; i < inputs.flows.Length; i++)
                {
                    CreateInputFlow(inputs.flows[i]);
                }
            }

            if (inputs.values != null)
            {
                for (int i = 0; i < inputs.values.Length; i++)
                {
                    CreateInputValue(inputs.values[i]);
                }
            }

            if (outputs.flows != null)
            {
                for (int i = 0; i < outputs.flows.Length; i++)
                {
                    CreateOutputFlow(outputs.flows[i]);
                }
            }

            if (outputs.values != null)
            {
                for (int i = 0; i < outputs.values.Length; i++)
                {
                    CreateOutputValue(outputs.values[i]);
                }
            }

            if (configuration != null)
            {
                for (int i = 0; i < configuration.Length; i++)
                {
                    CreateConfig(configuration[i]);
                }
            }
        }

        private ConfigEntry CreateConfig(NodeConfiguration nodeConfiguration)
        {
            var obj = Instantiate(_configPrefab, _configContainer);
            var config = obj.GetComponent<ConfigEntry>();
            config.SetData(this, nodeConfiguration);
            _configurations.Add(config);

            return config;
        }

        private SocketUI CreateSocket(SocketType socketType, NodeValue value, NodeFlow flow, GameObject prefab, Transform container)
        {
            var obj = Instantiate(prefab, container);

            switch (socketType)
            {
                case SocketType.Flow:
                    var flowSocket = obj.GetComponent<FlowSocketUI>();
                    flowSocket.SetData(this);
                    flowSocket.SetFlow(flow);
                    return flowSocket;
                case SocketType.Value:
                    var valueSocket = obj.GetComponent<ValueSocketUI>();
                    valueSocket.SetData(this);
                    valueSocket.SetValue(value);
                    return valueSocket;
                default:
                    throw new InvalidOperationException();
            }

        }

        private void CreateInputFlow(NodeFlow flow)
        {
            CreateInput(_inputFlowPrefab, flow.id, SocketType.Flow, default, flow);
        }
        private void CreateInputValue(NodeValue value)
        {
            CreateInput(_inputValuePrefab, value.id, SocketType.Value, value, default);
        }

        private void CreateOutputFlow(NodeFlow flow)
        {
            CreateOutput(_outputFlowPrefab, flow.id, SocketType.Flow, default, flow);
        }
        private void CreateOutputValue(NodeValue value)
        {
            CreateOutput(_outputValuePrefab, value.id, SocketType.Value, value, default);
        }

        private void CreateInput(GameObject prefab, string socketName, SocketType socketType, NodeValue value, NodeFlow flow)
        {
            var socket = CreateSocket(socketType, value, flow, prefab, _inputsContainer);
            _inputs.Add(socketName, socket);
        }

        private void CreateOutput(GameObject prefab, string socketName, SocketType socketType, NodeValue value, NodeFlow flow)
        {
            var socket = CreateSocket(socketType, value, flow, prefab, _outputsContainer);
            _outputs.Add(socketName, socket);
        }

        private void RecordPositionToMetadata()
        {
            var pos = rectTransform.anchoredPosition;
            node.metadata.positionX = pos.x;
            node.metadata.positionY = pos.y;
        }

        internal void OnBackingNodeRemoved()
        {
            Destroy(gameObject);
        }
    }
}