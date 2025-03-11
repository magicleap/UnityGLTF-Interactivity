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

        [SerializeField] private string _modelName = "interactive";
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
            _ = _loader.LoadModelAsync($"{_modelName}.glb", OnLoadComplete, _loadTimeout);
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
                // Can be used to inject a graph created from code in a hacky way for testing.
                //interactivityGraph.extensionData.graphs[defaultGraphIndex] = TestGraph.CreateTestGraph();
                var defaultGraph = interactivityGraph.extensionData.graphs[defaultGraphIndex];
                _behaviourEngine = new BehaviourEngine(defaultGraph, importer);
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
                _behaviourEngine.SetAnimationWrapper(animationWrapper, importer.LastLoadedScene.GetComponents<Animation>()[0]);
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
    }
}
