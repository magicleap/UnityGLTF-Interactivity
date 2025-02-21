using GLTF.Schema;
using System;
using System.Runtime.ExceptionServices;
using UnityEngine;
using UnityGLTF.Interactivity.Extensions;

namespace UnityGLTF.Interactivity
{
    public class InteractivityManager : MonoBehaviour
    {
        private struct LoadPacket
        {
            public GLTFSceneImporter importer;
            public KHR_interactivity extensionData;
        }

        [SerializeField] private string _modelName = "interactive.glb";
        [SerializeField] private string _saveToFile = "saveTest.glb";
        [SerializeField] private int _loadTimeout = 8;

        private InteractiveGLBLoader _loader;

        private LoadPacket _lastLoadPacket;
        private BehaviourEngine _behaviourEngine;

        public event Action onModelLoadComplete;
        public event Action<KHR_interactivity> onExtensionLoadComplete;

        private void Start()
        {
            _loader = new();
            _ = _loader.LoadModelAsync(_modelName, OnLoadComplete, _loadTimeout);
        }

        private void Update()
        {
            CheckForObjectSelect();
        }

        private static void CheckForObjectSelect()
        {
            if (!Input.GetMouseButtonDown(0))
                return;

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(ray, out RaycastHit hit))
                return;

            var root = hit.collider.transform.root.gameObject;

            if (!root.TryGetComponent(out EventWrapper wrapper))
                return;

            wrapper.Select(hit.transform.gameObject);
        }

        private void OnLoadComplete(GameObject obj, ExceptionDispatchInfo exceptionDispatchInfo, GLTFSceneImporter importer)
        {
            onModelLoadComplete?.Invoke();

            var extensions = importer.Root?.Extensions;

            if (extensions == null)
                return;

            if (!extensions.TryGetValue(ConstStrings.EXTENSION_NAME, out IExtension extensionValue))
                return;

            if (extensionValue is not InteractivityGraphExtension interactivityGraph)
                return;

            AnimationWrapper animationWrapper = null;

            if (!importer.AnimationCache.IsNullOrEmpty())
            {
                animationWrapper = importer.SceneParent.gameObject.AddComponent<AnimationWrapper>();
                animationWrapper.SetData(importer.LastLoadedScene.GetComponents<Animation>()[0]);
            }

            var defaultGraphIndex = interactivityGraph.extensionData.defaultGraphIndex;
            var defaultGraph = interactivityGraph.extensionData.graphs[defaultGraphIndex];
            var eventWrapper = importer.SceneParent.gameObject.AddComponent<EventWrapper>();

            _behaviourEngine = new BehaviourEngine(defaultGraph, new PointerResolver(importer), animationWrapper);
            eventWrapper.SetData(_behaviourEngine);

            _lastLoadPacket = new LoadPacket()
            {
                importer = importer,
                extensionData = interactivityGraph.extensionData
            };

            onExtensionLoadComplete?.Invoke(interactivityGraph.extensionData);
        }

        public void SaveModel()
        {
            _loader.SaveModel(_saveToFile, _lastLoadPacket.importer, _lastLoadPacket.extensionData);
        }

        public void OnDestroy()
        {
            _behaviourEngine.CancelExecution();
        }
    }
}
