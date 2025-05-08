using System.Net.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityGLTF.Interactivity.Playback.Frontend
{
    public enum SocketSide
    {
        Input = 0,
        Output = 1,
    }

    public enum SocketType
    {
        Flow = 1,
        Value = 2
    }

    public class SocketUIPoint : MonoBehaviour
    {
        [field: SerializeField] public SocketUI socket { get; private set; }     
    }
}