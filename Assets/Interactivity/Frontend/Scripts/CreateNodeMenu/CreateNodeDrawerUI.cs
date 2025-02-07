using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UnityGLTF.Interactivity.Frontend
{
    public class CreateNodeDrawerUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _categoryTitle;
        [SerializeField] private RectTransform _arrowTransform;
        [SerializeField] private GameObject _nodeEntryPrefab;
        [SerializeField] private Button _button;

        private Quaternion _arrowDownRotation;

        public CreateNodeMenuUI menuUI { get; private set; }
        public string category { get; private set; }
        private NodeEntryUI[] _nodeEntries;

        private bool _isExpanded;

        public void SetData(CreateNodeMenuUI menuUI, string category, List<string> nodes)
        {
            this.menuUI = menuUI;
            this.category = category;

            _categoryTitle.text = category;

            _button.onClick.AddListener(OnClick);
            _arrowDownRotation = _arrowTransform.rotation;

            _nodeEntries = new NodeEntryUI[nodes.Count];

            for (int i = 0; i < _nodeEntries.Length; i++)
            {
                var obj = Instantiate(_nodeEntryPrefab, menuUI.nodeContainer);
                var entry = obj.GetComponent<NodeEntryUI>();

                entry.SetData(this, nodes[i]);

                _nodeEntries[i] = entry;
            }

            ShowEntries(false);
        }

        private void OnClick()
        {
            _isExpanded = !_isExpanded;
            ShowEntries(_isExpanded);
        }

        public void ShowEntries(bool show)
        {
            _arrowTransform.rotation = show ? Quaternion.identity : _arrowDownRotation;

            for (int i = 0; i < _nodeEntries.Length; i++)
            {
                _nodeEntries[i].gameObject.SetActive(show);
            }
        }
    }
}