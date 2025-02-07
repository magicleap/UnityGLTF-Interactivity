using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UnityGLTF.Interactivity.Frontend
{
    public class ViewJsonUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _json;
        [SerializeField] private GameObject _menu;
        [SerializeField] private Button _button;
        [SerializeField] private GraphUI _graphUI;

        private GraphSerializer _serializer = new(Newtonsoft.Json.Formatting.Indented);

        private void Awake()
        {
            _button.onClick.AddListener(ToggleMenu);
            gameObject.SetActive(false);
        }

        private void ToggleMenu()
        {
            var setActive = !_menu.activeSelf;

            if (setActive)
            {
                _json.text = _serializer.Serialize(_graphUI.graph);
            }

            _menu.SetActive(setActive);
        }
    }
}