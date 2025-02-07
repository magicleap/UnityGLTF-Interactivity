using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UnityGLTF.Interactivity.Frontend
{
    public class CreateNodeMenuUI : MonoBehaviour
    {
        [Header("Asset References")]
        [SerializeField] private GameObject _nodeDrawerPrefab;

        [Header("Self References")]
        [field: SerializeField] public Transform nodeContainer;
        [SerializeField] private TMP_InputField _searchInput;

        public GraphUI graphUI { get; private set; }

        private CreateNodeDrawerUI[] _drawers;

        public void SetData(GraphUI graphUI)
        {
            this.graphUI = graphUI;
        }

        private void Awake()
        {
            var nodesByCategory = new Dictionary<string, List<string>>();
            string[] key;
            foreach (var availableNode in NodeRegistry.nodeSpecs)
            {
                key = availableNode.Key.Split('/');
                var category = key[0];
                var nodeName = key[1];

                if (!nodesByCategory.TryGetValue(category, out List<string> nodeList))
                {
                    nodeList = new List<string>();
                    nodesByCategory.Add(category, nodeList);
                }

                nodeList.Add(nodeName);
            }

            _drawers = new CreateNodeDrawerUI[nodesByCategory.Count];
            var i = 0;
            foreach (var set in nodesByCategory)
            {
                var obj = Instantiate(_nodeDrawerPrefab, nodeContainer);

                var nodeDrawer = obj.GetComponent<CreateNodeDrawerUI>();
                nodeDrawer.SetData(this, set.Key, set.Value);

                _drawers[i++] = nodeDrawer;
            }

            ShowMenu(false);
        }

        public void ShowMenu(bool show)
        {
            gameObject.SetActive(show);

            if (show)
            {
                for (int i = 0; i < _drawers.Length; i++)
                {
                    _drawers[i].ShowEntries(false);
                }

                var rectTransform = transform as RectTransform;
                rectTransform.anchoredPosition = graphUI.ConvertMousePosToAnchoredPosition();
            }
        }
    }
}