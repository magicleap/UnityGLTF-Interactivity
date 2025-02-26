using GLTF.Schema;
using System;
using System.Runtime.ExceptionServices;
using UnityEngine;
using UnityGLTF.Interactivity.Extensions;

namespace UnityGLTF.Interactivity
{
    public class InteractivityManager : MonoBehaviour
    {
        private const int MAX_RAYCAST_HITS = 32;

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

        private static readonly RaycastHit[] _raycastHits = new RaycastHit[MAX_RAYCAST_HITS];
        private static readonly RaycastHit[] _selectableHits = new RaycastHit[MAX_RAYCAST_HITS];

        private void Start()
        {
            _loader = new();
            _ = _loader.LoadModelAsync(_modelName, OnLoadComplete, _loadTimeout);
        }

        private void Update()
        {
            CheckForObjectSelect(_behaviourEngine);
        }

        private static void CheckForObjectSelect(BehaviourEngine engine)
        {
            if (!Input.GetMouseButtonDown(0))
                return;

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            var hitCount = Physics.RaycastNonAlloc(ray, _raycastHits);

            if (hitCount <= 0)
                return;

            NodePointers pointers;
            GameObject go;
            var selectableCount = 0;
            RaycastHit closestHit = default;
            float closestHitDistance = float.MaxValue;

            for (int i = 0; i < hitCount; i++)
            {
                go = _raycastHits[i].transform.gameObject;
                pointers = engine.pointerResolver.PointersOf(go);

                if (pointers.selectability.getter())
                {
                    _selectableHits[selectableCount++] = _raycastHits[i];

                    if(_raycastHits[i].distance < closestHitDistance)
                    {
                        closestHit = _raycastHits[i];
                        closestHitDistance = _raycastHits[i].distance;
                    }
                }
            }

            if (selectableCount <= 0)
                return;

            var root = closestHit.transform.root.gameObject;

            if (!root.TryGetComponent(out EventWrapper wrapper))
                return;

            wrapper.Select(closestHit, _selectableHits);

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

            try
            {
                var defaultGraphIndex = interactivityGraph.extensionData.defaultGraphIndex;
                var defaultGraph = interactivityGraph.extensionData.graphs[defaultGraphIndex];
                _behaviourEngine = new BehaviourEngine(defaultGraph, new PointerResolver(importer));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return;
            }

            AnimationWrapper animationWrapper = null;

            if (!importer.AnimationCache.IsNullOrEmpty())
            {
                animationWrapper = importer.SceneParent.gameObject.AddComponent<AnimationWrapper>();
                animationWrapper.SetData(importer.LastLoadedScene.GetComponents<Animation>()[0]);
                _behaviourEngine.animationWrapper = animationWrapper;
            }

            var eventWrapper = importer.SceneParent.gameObject.AddComponent<EventWrapper>();

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
            // Can be null if an exception is thrown or load does not complete before it's assigned.
            _behaviourEngine?.CancelExecution();
        }
    }
}
