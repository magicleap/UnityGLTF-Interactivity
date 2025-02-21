using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace UnityGLTF.Interactivity.Frontend
{
    public class GraphUI : MonoBehaviour
    {
        [Header("Asset References")]
        [SerializeField] private GameObject _nodePrefab;

        [Header("Self References")]
        [SerializeField] private Transform _nodesContainer;

        [field: Header("Scene References")]
        [field: SerializeField] public CreateNodeMenuUI createNodeMenuUI { get; private set; }
        [field: SerializeField] public InteractivityManager loader { get; private set; }
        [field: SerializeField] public FlowsUIManager flowsManager { get; private set; }
        [field: SerializeField] public VariableMenuUI variablesMenu { get; private set; }


        private readonly Dictionary<Node, NodeUI> _nodes = new();
        public ReadOnlyDictionary<Node, NodeUI> nodes;

        public Graph graph { get; private set; }

        public Action onGraphUIReady;
        public Action<NodeUI> onNodeUICreated;
        public Action<NodeUI> onNodeUIDestroyed;

        private void Awake()
        {
            nodes = new(_nodes);
            flowsManager.SetData(this);
            createNodeMenuUI.SetData(this);
            variablesMenu.SetData(this);
            loader.onExtensionLoadComplete += OnGraphLoaded;
        }

        private void OnGraphLoaded(KHR_interactivity extensionData)
        {
            // TODO: Support multiple graphs
            this.graph = extensionData.graphs[0];

            DestroyExistingNodes();

            for (int i = 0; i < graph.nodes.Count; i++)
            {
                CreateNodeUI(graph, graph.nodes[i]);
            }

            graph.onNodeAdded += OnNodeAdded;
            graph.onNodeRemoved += OnNodeRemoved;

            onGraphUIReady?.Invoke();
        }

        private void CreateNodeUI(Graph graph, Node node)
        {
            var obj = Instantiate(_nodePrefab, _nodesContainer);
            var nodeUI = obj.GetComponent<NodeUI>();

            nodeUI.SetData(this, node);
            _nodes.Add(node, nodeUI);

            onNodeUICreated?.Invoke(nodeUI);
        }

        private void OnNodeAdded(Node node)
        {
            CreateNodeUI(graph, node);
        }

        private void OnNodeRemoved(Node node)
        {
            _nodes[node].OnBackingNodeRemoved();
            _nodes.Remove(node);
        }

        private void DestroyExistingNodes()
        {
            foreach (var node in _nodes.Values)
            {
                Destroy(node.gameObject);
            }

            _nodes.Clear();
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Space))
            {
                createNodeMenuUI.ShowMenu(true);
            }
            else if (Input.GetKeyUp(KeyCode.G))
            {
                Debug.Log("Removing node");
                graph.RemoveNode(graph.nodes[0]);
            }
            else if (Input.GetKeyUp(KeyCode.H))
            {
                Debug.Log("Removing flow");

                graph.nodes[0].RemoveFlow(graph.nodes[0].flows[0]);
            }
        }

        internal Vector2 ConvertMousePosToAnchoredPosition()
        {
            var rectTransform = transform as RectTransform;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, Camera.main, out Vector2 local);
            return local;
        }
    }
}