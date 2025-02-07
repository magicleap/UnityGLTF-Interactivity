using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityGLTF.Interactivity.Frontend
{
    public class CloseCreateNodeMenuOnClick : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private CreateNodeMenuUI _menu;

        public void OnPointerClick(PointerEventData eventData)
        {
            _menu.ShowMenu(false);
        }
    }
}