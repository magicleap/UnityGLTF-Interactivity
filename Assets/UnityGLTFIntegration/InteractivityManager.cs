using GLTF.Schema;
using System;
using System.Collections.Generic;
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
            public KHR_ExtensionGraph extensionData;
        }

        [SerializeField] private string _modelName = "interactive";
        [SerializeField] private string _saveToFile = "saveTest.glb";
        [SerializeField] private int _loadTimeout = 8;
        [SerializeField] private GLTFSettings _settings;

        private InteractiveGLBLoader _loader;

        private LoadPacket _lastLoadPacket;
        private BehaviourEngine _behaviourEngine;

        public event Action onModelLoadComplete;
        public event Action<KHR_ExtensionGraph> onExtensionLoadComplete;

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

            List<GraphData> gdList = GetInteractivityGraphsSorted(extensions);
            if (gdList == null || gdList.Count <= 0)
                return;
//            if (extensionValue is not InteractivityGraphExtension interactivityGraph)
 //               return;

            try
            {
                //var defaultGraphIndex = interactivityGraph.extensionData.defaultGraphIndex;
                //// Can be used to inject a graph created from code in a hacky way for testing.
                ////interactivityGraph.extensionData.graphs[defaultGraphIndex] = TestGraph.CreateTestGraph();
                //var defaultGraph = interactivityGraph.extensionData.graphs[defaultGraphIndex];
                List<Graph> graphList = new();
                gdList.ForEach(r => graphList.Add(r.graph)); 
                _behaviourEngine = new BehaviourEngine(graphList, importer);
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
            List<KHR_ExtensionGraph> extensionList = new();
            gdList.ForEach(r => extensionList.Add(r.extension));
            eventWrapper.SetData(_behaviourEngine, extensionList);

            _lastLoadPacket = new LoadPacket()
            {
                importer = importer,
                extensionData = extensionList[0]
            };

            onExtensionLoadComplete?.Invoke(extensionList[0]);
        }

        private class GraphData
        {
            public KHR_ExtensionGraph extension;
            public Graph graph;
        };

        private List<GraphData> GetInteractivityGraphsSorted(Dictionary<string, IExtension> extensions)
        {
            var graphs = new List<GraphData>();

            var extensionValues = new List<IExtension>();
            if (!extensions.TryGetValue(InteractivityGraphExtension.EXTENSION_NAME, out IExtension interactivityExtensionValue))
                return null;
            extensionValues.Add(interactivityExtensionValue);
            if (extensions.TryGetValue(AudioGraphExtension.EXTENSION_NAME, out IExtension audioExtensionValue))
                extensionValues.Add(audioExtensionValue);

            bool found = false;
            foreach(var v in extensionValues)
            {
                switch(v)
                {
                    case InteractivityGraphExtension interactivityGraph:
                        found = true;
                        var defaultGraphIndex = interactivityGraph.extensionData.defaultGraphIndex;
                        graphs.Add(new GraphData() { extension = interactivityGraph.extensionData, graph = interactivityGraph.extensionData.graphs[defaultGraphIndex] });
                        break;
                    case AudioGraphExtension audioGraph:
                        var audioGraphIndex = audioGraph.extensionData.defaultGraphIndex;
                        graphs.Add(new GraphData() { extension = audioGraph.extensionData, graph = audioGraph.extensionData.graphs[audioGraphIndex] });
                        break;
                    default:
                        Debug.LogError($"{this.name} : Undefined InteractivityGraph type");
                        break;
                }               
            }
            if (!found) 
                return null;

            return graphs;
        }

        public void SaveModel()
        {
            _loader.SaveModel(_saveToFile, _settings, _lastLoadPacket.importer, _lastLoadPacket.extensionData);
        }
    }
}
