using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityGLTF.Interactivity.Frontend.Extensions;

namespace UnityGLTF.Interactivity.Frontend
{
    public class VariableMenuUI : MonoBehaviour
    {
        [SerializeField] private GameObject _variablePrefab;
        [SerializeField] private Transform _container;

        private Dictionary<Variable, VariableUI> _variables = new();

        public GraphUI graphUI { get; private set; }

        private Graph _graph;

        private void Awake()
        {
            GameObjectPoolManager.CreatePool(_variablePrefab);
        }

        public void SetData(GraphUI graphUI)
        {
            this.graphUI = graphUI;
            graphUI.onGraphUIReady += OnGraphReady;
        }

        private void OnGraphReady()
        {
            if(_graph != null)
            {
                _graph.onVariableAdded -= OnVariableAddedToGraph;
                _graph.onVariableRemoved -= OnVariableRemovedFromGraph;
            }

            _graph = graphUI.graph;

            for (int i = 0; i < _graph.variables.Count; i++)
            {
                CreateVariableUI(_graph.variables[i]);
            }

            _graph.onVariableAdded += OnVariableAddedToGraph;
            _graph.onVariableRemoved += OnVariableRemovedFromGraph;
        }

        private void CreateVariableUI(Variable variable)
        {
            if (!GameObjectPoolManager.TryGet(_variablePrefab, out GameObject obj))
                throw new InvalidOperationException("No available variable object in pool!");

            obj.transform.SetParent(_container);
            obj.transform.Reset();
            var variableUI = obj.GetComponent<VariableUI>();

            variableUI.SetData(this, variable);
        }

        private void OnVariableAddedToGraph(Variable variable)
        {
            Debug.Log("Variable added to graph");
            CreateVariableUI(variable);
        }

        private void OnVariableRemovedFromGraph(Variable variable)
        {
            if (!_variables.TryGetValue(variable, out VariableUI variableUI))
                throw new InvalidOperationException("Variable was deleted from graph that had no UI representing it!");

            GameObjectPoolManager.TryRelease(_variablePrefab, variableUI.gameObject);
            _variables.Remove(variable);
        }
    }
}