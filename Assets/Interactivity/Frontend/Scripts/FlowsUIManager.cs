using System.Collections.Generic;
using UnityEngine;
using UnityGLTF.Interactivity.Frontend.Extensions;

namespace UnityGLTF.Interactivity.Frontend
{
    public class FlowsUIManager : MonoBehaviour
    {
        [Header("Asset References")]
        [SerializeField] private GameObject _flowCurvePrefab;

        [field: Header("Self References")]
        [field: SerializeField] public Transform container { get; private set; }
        [SerializeField] private Transform _tempFlowPoint;

        private GraphUI _graphUI;

        private Dictionary<Node, FlowsInNodeUI> _nodes = new();

        private Graph _graph;

        private FlowUI _tempFlowUI;

        public void SetData(GraphUI graphUI)
        {
            GameObjectPoolManager.CreatePool(_flowCurvePrefab);

            _graphUI = graphUI;
            _graphUI.onGraphUIReady += OnGraphLoaded;
        }

        private void OnGraphLoaded()
        {
            if (_graph != null)
            {
                _graph.onNodeAdded -= OnNodeAdded;
                _graph.onNodeRemoved -= OnNodeRemoved;
            }

            _graph = _graphUI.graph;

            DestroyExistingFlows();

            for (int i = 0; i < _graph.nodes.Count; i++)
            {
                var node = _graph.nodes[i];
                _nodes.Add(node, new(node, _graphUI, _flowCurvePrefab, container));
            }

            _graph.onNodeAdded += OnNodeAdded;
            _graph.onNodeRemoved += OnNodeRemoved;
        }

        private void DestroyExistingFlows()
        {
            foreach (var node in _nodes.Values)
            {
                node.DestroyExistingFlows();
            }

            _nodes.Clear();
        }

        private void OnNodeAdded(Node node)
        {
            _nodes.Add(node, new(node, _graphUI, _flowCurvePrefab, container));
        }

        private void OnNodeRemoved(Node node)
        {
            _nodes[node].OnBackingNodeRemoved();
            _nodes.Remove(node);
        }

        public FlowUI ShowTempFlow(Transform from, Vector3 to, SocketSide socketSide)
        {
            GameObjectPoolManager.TryGet(_flowCurvePrefab, out GameObject obj);
            obj.transform.SetParent(container);
            obj.transform.Reset();
            _tempFlowUI = obj.GetComponent<FlowUI>();

            _tempFlowPoint.transform.position = to;

            if(socketSide == SocketSide.Output)
                _tempFlowUI.SetData(from, _tempFlowPoint);
            else
                _tempFlowUI.SetData(_tempFlowPoint, from);

            return _tempFlowUI;
        }

        public void MoveTempFlowTargetPoint(Vector3 to)
        {
            _tempFlowPoint.transform.position = to;
        }

        public void HideTempFlow()
        {
            GameObjectPoolManager.TryRelease(_flowCurvePrefab, _tempFlowUI.gameObject);
        }
    }
}