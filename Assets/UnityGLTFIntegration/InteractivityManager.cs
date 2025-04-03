using GLTF.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.ExceptionServices;
using UnityEngine;
using UnityEngine.Networking;
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
//                List<Graph> graphList = new();
//                gdList.ForEach(r => graphList.Add(r.graph)); 
                _behaviourEngine = new BehaviourEngine(gdList, importer);

                var audio = gdList.FindAll(r => (r.graph.audioSources != null) && (r.graph.audioSources.Count > 0));
                if (audio != null && audio.Count > 0)
                {
                    //found audio. add audio wrapper if it does not already exist.
                    AudioWrapper audioWrapper = importer.SceneParent.gameObject.AddComponent<AudioWrapper>();
                    _behaviourEngine.SetAudioWrapper(audioWrapper);

                    AddAudioSources(importer, audioWrapper, audio);
                }
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


        private void AddAudioSources(GLTFSceneImporter importer, AudioWrapper wrapper, List<GraphData> audioGraphs)
        {
            if (wrapper == null)
                return;

            foreach(var audioGraph in audioGraphs)
            {
                Graph graph = audioGraph.graph;

                int idx = 0;

                // each audio
                foreach(var audio in graph.audio)
                {
                    if (audio.uri != null && !string.IsNullOrEmpty(audio.uri))
                    {
                        audio.uri = audio.uri;
                        string path = Path.GetDirectoryName(_modelName);
                        if (!string.IsNullOrEmpty(path))
                        {
                            path += Path.DirectorySeparatorChar + audio.uri;
                        }
                        else
                        {
                            path = audio.uri;
                        }
                        try
                        {

                            var (directory, fileName) = Helpers.GetFilePath(path);
                            UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(directory + Path.DirectorySeparatorChar + fileName, AudioType.MPEG);
                            www.SendWebRequest();

                            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                            {
                                Debug.LogError("Error: " + www.error);
                            }
                            else
                            {
                                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);

                                UnityEngine.AudioSource audioSourceScene = importer.SceneParent.gameObject.AddComponent<UnityEngine.AudioSource>();
                                audioSourceScene.clip = clip;
                                audioSourceScene.clip.name = Path.GetFileNameWithoutExtension(fileName);
                                audioSourceScene.volume = graph.audioSources[0].gain;
                                audioSourceScene.minDistance = graph.audioEmitter[0].positional[0].minDistance;
                                audioSourceScene.maxDistance = graph.audioEmitter[0].positional[0].maxDistance;
                                audioSourceScene.loop = graph.audioSources[0].loop;
                                audioSourceScene.Play();

                                wrapper.AddAudioSource(0, new AudioPlayData() { index = 0, source = audioSourceScene });
                            }
                        }
                        catch (Exception ex)
                        {

                        }

                    }
                    else
                    {
                        BufferView bv = importer.Root.BufferViews[audio.bufferView];
                        var offset = bv.ByteOffset;
                        var length = bv.ByteLength;
                        var id = bv.Buffer;
                        var buf = importer.Root.Buffers[(int)id.Id];

                        GLTFBuffer v = importer.Root.Buffers[0];
                        //                        MemoryStream ms = new MemoryStream()
                        audio.uri = null;
                        
                    }
                    idx++;
                }
            }
        }

        //private float[] ConvertByteToFloat(byte[] array)
        //{
        //    float[] floatArr = new float[array.Length / 4];
        //    for (int i = 0; i < floatArr.Length; i++)
        //    {
        //        if (BitConverter.IsLittleEndian)
        //        {
        //            Array.Reverse(array, i * 4, 4);
        //        }
        //        floatArr[i] = BitConverter.ToSingle(array, i * 4) / 0x80000000;
        //    }
        //    return floatArr;
        //}

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
