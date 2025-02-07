using System.Collections.Generic;
using UnityEngine;

namespace UnityGLTF.Interactivity.Frontend
{
    public class FlowsInNodeUI
    {
        private Dictionary<Flow, FlowUI> _flowUIs = new();
        private Dictionary<Value, FlowUI> _valueUIs = new();

        private GameObject _prefab;
        private Transform _container;
        private Node _node;
        private GraphUI _graphUI;

        public FlowsInNodeUI(Node node, GraphUI graphUI, GameObject prefab, Transform container)
        {
            _node = node;
            _graphUI = graphUI;
            _prefab = prefab;
            _container = container;

            node.onFlowAdded += OnFlowAdded;
            node.onFlowRemoved += OnFlowRemoved;

            for (int i = 0; i < node.values.Count; i++)
            {
                // Need this closure otherwise value will be equal to whatever the last value iterated through was when OnValueConnectionChanged is fired.
                var value = node.values[i];
                value.onConnectionChanged += OnValueConnectionChanged;
            }

            CreateConnectionLines();
        }

        private void CreateConnectionLines()
        {
            for (int i = 0; i < _node.flows.Count; i++)
            {
                CreateFlowUI(_node.flows[i]);
            }

            for (int i = 0; i < _node.values.Count; i++)
            {
                var value = _node.values[i];

                if (value.node == null)
                    continue;

                CreateValueUI(value);
            }
        }

        private void CreateFlowUI(Flow flow)
        {
            var obj = GameObject.Instantiate(_prefab, _container);
            var flowUI = obj.GetComponent<FlowUI>();
            flowUI.SetData(_graphUI.nodes[_node].outputs[flow.fromSocket].socketPoint, _graphUI.nodes[flow.toNode].inputs[flow.toSocket].socketPoint);
            _flowUIs.Add(flow, flowUI);
        }

        private void CreateValueUI(Value value)
        {
            var obj = GameObject.Instantiate(_prefab, _container);
            var flowUI = obj.GetComponent<FlowUI>();
            flowUI.SetData(_graphUI.nodes[value.node].outputs[value.socket].socketPoint, _graphUI.nodes[_node].inputs[value.id].socketPoint);
            _valueUIs.Add(value, flowUI);
        }

        private void OnValueConnectionChanged(Value value)
        {
            var hasFlowUI = _valueUIs.TryGetValue(value, out FlowUI flowUI);

            Debug.Log($"On connection changed for node {_node.type}'s value {value.id}. Has existing UI? {hasFlowUI}, Node is null? {value.node == null}");

            if (hasFlowUI)
            {
                if (value.node == null)
                {
                    _valueUIs.Remove(value);
                    GameObject.Destroy(flowUI.gameObject);
                }
            }
            else
            {
                if (value.node != null)
                {
                    CreateValueUI(value);
                }
            }
        }

        private void OnFlowAdded(Flow flow)
        {
            CreateFlowUI(flow);
        }

        private void OnFlowRemoved(Flow flow)
        {
            DestroyFlowUI(flow);
        }

        public void DestroyExistingFlows()
        {
            foreach (var flowUI in _flowUIs.Values)
            {
                GameObject.Destroy(flowUI.gameObject);
            }

            _flowUIs.Clear();

            foreach (var valueUI in _valueUIs.Values)
            {
                GameObject.Destroy(valueUI.gameObject);
            }

            _valueUIs.Clear();
        }

        private void DestroyFlowUI(Flow flow)
        {
            if (!_flowUIs.TryGetValue(flow, out FlowUI flowUI))
            { 
                Debug.LogWarning($"No flow found for {flow.fromSocket}");
                return;
            }

            GameObject.Destroy(flowUI.gameObject);

            _flowUIs.Remove(flow);
        }

        internal void OnBackingNodeRemoved()
        {
            DestroyExistingFlows();
        }
    }
}