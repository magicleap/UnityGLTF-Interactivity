using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace UnityGLTF.Interactivity.Frontend
{
    public class NodeEntryUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private Button _button;

        private CreateNodeDrawerUI _drawer;
        private string _nodeName;

        public void SetData(CreateNodeDrawerUI drawer, string nodeName)
        {
            _title.text = nodeName;

            _drawer = drawer;
            _nodeName = nodeName;

            _button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            var anchoredPos = _drawer.menuUI.graphUI.ConvertMousePosToAnchoredPosition();
            var node = _drawer.menuUI.graphUI.graph.CreateNode($"{_drawer.category}/{_nodeName}", anchoredPos);
            _drawer.menuUI.ShowMenu(false);
        }
    }
}