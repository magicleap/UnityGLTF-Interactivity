using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityGLTF.Interactivity.Frontend
{
    public class VariableUI : MonoBehaviour, IDragHandler, IEndDragHandler
    {
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _typeText;

        private VariableMenuUI _menu;
        private Variable _variable;

        public void OnDrag(PointerEventData eventData)
        {
        }

        public void OnEndDrag(PointerEventData eventData)
        {

        }

        internal void SetData(VariableMenuUI menu, Variable variable)
        {
            _menu = menu;
            _variable = variable;

            _nameText.text = variable.id;
            _typeText.text = variable.property.GetTypeSignature();
        }
    }
}