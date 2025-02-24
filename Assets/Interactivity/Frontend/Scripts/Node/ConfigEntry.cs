using System;
using TMPro;
using UnityEngine;

namespace UnityGLTF.Interactivity.Frontend
{
    public class ConfigEntry : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TMP_InputField _inputField;

        private NodeUI _nodeUI;
        private Configuration _configuration;
        private NodeConfiguration _configData;

        private bool _valueInvalid;

        public void SetData(NodeUI nodeUI, NodeConfiguration configData)
        {
            _configData = configData;
            _configuration = GetConfigurationFromNode(nodeUI.node, configData.id);
            _nodeUI = nodeUI;
            _title.text = configData.id;
            _inputField.text = _configuration.value.ToString();
            _inputField.onValueChanged.AddListener(OnInputFieldChanged);
        }

        private void OnInputFieldChanged(string text)
        {
            try
            {
                object value = text;

                var type = _configData.type;

                if (type == typeof(float))
                    value = float.Parse(text);
                else if (type == typeof(int))
                    value = int.Parse(text);
                else if (type == typeof(bool))
                    value = bool.Parse(text);
                else
                    value = text;

                // TODO: Fix this after change to config data model
                //_configuration.value = value;
                _valueInvalid = false;
            }
            catch
            {
                _valueInvalid = true;
            }
        }

        private static Configuration GetConfigurationFromNode(Node node, string id)
        {
            Configuration config;

            for (int i = 0; i < node.configuration.Count; i++)
            {
                config = node.configuration[i];

                if (config.id == id)
                    return config;
            }

            return null;
        }
    }
}