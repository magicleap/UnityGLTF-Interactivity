using GLTF.Schema;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
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

        private const int GLTF_BIN_DATA_INDEX = 1;

        private void Start()
        {
            _loader = new();
            _ = _loader.LoadModelAsync($"{_modelName}.glb", OnLoadComplete, _loadTimeout);
        }

        private bool _loadedAudioFile = false;
        private UnityEngine.AudioSource _loadingAudioSource = null;
        private AudioWrapper _audioWrapper = null;

        private void Update()
        {
            if (_loadedAudioFile && _loadingAudioSource != null)
            {
                if (_loadingAudioSource.playOnAwake)
                    _loadingAudioSource.Play();
                _audioWrapper.AddAudioSource(0, new AudioPlayData() { index = 0, source = _loadingAudioSource });
                _loadingAudioSource = null;
                _loadedAudioFile = false;
            }
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

            try
            {
                _behaviourEngine = new BehaviourEngine(gdList, importer);
                
                foreach(var graph in gdList)
                {
                    _audioWrapper = importer.SceneParent.gameObject.GetComponent<AudioWrapper>();
                    if (_audioWrapper == null)
                    {
                        _audioWrapper = importer.SceneParent.gameObject.AddComponent<AudioWrapper>();
                        _behaviourEngine.SetAudioWrapper(_audioWrapper);
                    }
                    switch (graph.extension.type)
                    {
                        case KHR_ExtensionGraph.GraphType.KHR_Audio:
                            var audioSrcs = gdList.FindAll(r => (r.graph.audioSources != null) && (r.graph.audioSources.Count > 0));
                            if (audioSrcs != null && audioSrcs.Count > 0)          
                                AddAudioSources(importer, audioSrcs);
                            break;
                        case KHR_ExtensionGraph.GraphType.GOOG_Audio:
                            audioSrcs = gdList.FindAll(r => (r.graph.audio != null) && (r.graph.audio.Count > 0));
                            if (graph.graph.audio != null && graph.graph.audio.Count > 0)
                                AddGOOGAudioSource(importer, audioSrcs);
                            break;
                        default:
                            break;
                    }
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

        // dpq - need to genercize this with the method below.        
        private void AddGOOGAudioSource(GLTFSceneImporter importer, List<GraphData> audioGraphs)
        {
            if (_audioWrapper == null)
                return;

            //loop through all of the audio graphs. Could be more than one conceivably. Practically speaking though, 
            // one model should have one audio graph with audio, emitters and sources
            foreach (var audioGraph in audioGraphs)
            {
                Graph graph = audioGraph.graph;

                int idx = 0;

                // each audio
                foreach (var audio in graph.audio)
                {
                    AudioEmitterPartial e = GetAudioEmitterPartial<GOOG_AudioType>(new GOOG_AudioType(), idx, graph.audioEmitter);

                    string path = string.Empty;
                    string finalFileName = String.Empty;
                    string finalDir = String.Empty;
                    if (audio.uri != null && !string.IsNullOrEmpty(audio.uri))
                    {
                        audio.uri = audio.uri;
                        path = Path.GetDirectoryName(_modelName);
                        if (!string.IsNullOrEmpty(path))
                        {
                            path += Path.DirectorySeparatorChar + audio.uri;
                        }
                        else
                        {
                            path = audio.uri;
                        }
                        var (directory, fileName) = Helpers.GetFilePath(path);
                        finalFileName = fileName;
                        finalDir = directory;
                    }
                    else
                    {
                        /// get index into buffer
                        BufferView bv = importer.Root.BufferViews[audio.bufferView];
                        var offset = bv.ByteOffset;
                        var length = bv.ByteLength;

                        byte[] audioArray = GetAudioBufferArrayFromGLB(_modelName, offset, length);
                        string tempFile = WriteToTempFile(audioArray, length);

                        var (directory, fileName) = Helpers.GetFilePath(tempFile);
                        finalFileName = fileName;
                        finalDir = directory;
                    }

                    if (!string.IsNullOrEmpty(finalFileName) && !string.IsNullOrEmpty(finalDir))
                    {
                        if (_loadingAudioSource == null)
                            _loadingAudioSource = importer.SceneParent.gameObject.AddComponent<UnityEngine.AudioSource>();
                        SetGOOGAudioValues(audio, e, ref _loadingAudioSource);
                        StartCoroutine(LoadAudioSourceClipFromFile(finalDir + Path.DirectorySeparatorChar + finalFileName, Path.GetFileNameWithoutExtension(audio.uri)));
                    }

                    idx++;
                }
            }
        }

        private void AddAudioSources(GLTFSceneImporter importer, List<GraphData> audioGraphs)
        {
            if (_audioWrapper == null)
                return;

            //loop through all of the audio graphs. Could be more than one conceivably. Practically speaking though, 
            // one model should have one audio graph with audio, emitters and sources
            foreach(var audioGraph in audioGraphs)
            {
                Graph graph = audioGraph.graph;
                UnityEngine.AudioSource audioSourceScene = null;

                int idx = 0;

                // each audio
                foreach(var audio in graph.audio)
                {
                    AudioSource a = GetAudioSource(idx, graph.audioSources);
                    AudioEmitterPartial e = GetAudioEmitterPartial<KHR_AudioType>(new KHR_AudioType(), idx, graph.audioEmitter);

                    string path = string.Empty;
                    string finalFileName = String.Empty;
                    string finalDir = String.Empty;
                    if (audio.uri != null && !string.IsNullOrEmpty(audio.uri))
                    {
                        audio.uri = audio.uri;
                        path = Path.GetDirectoryName(_modelName);
                        if (!string.IsNullOrEmpty(path))
                        {
                            path += Path.DirectorySeparatorChar + audio.uri;
                        }
                        else
                        {
                            path = audio.uri;
                        }
                        var (directory, fileName) = Helpers.GetFilePath(path);
                        finalFileName = fileName;
                        finalDir = directory;
                    }
                    else
                    {
                        /// get index into buffer
                        BufferView bv = importer.Root.BufferViews[audio.bufferView];
                        var offset = bv.ByteOffset;
                        var length = bv.ByteLength;

                        byte[] audioArray = GetAudioBufferArrayFromGLB(_modelName, offset, length);
                        string tempFile =  WriteToTempFile(audioArray, length);

                        var (directory, fileName) = Helpers.GetFilePath(tempFile);
                        finalFileName = fileName;
                        finalDir = directory;
                    }

                    if (!string.IsNullOrEmpty(finalFileName) && !string.IsNullOrEmpty(finalDir))
                    {
                        if (_loadingAudioSource == null)
                            _loadingAudioSource = importer.SceneParent.gameObject.AddComponent<UnityEngine.AudioSource>();
                        SetAudioValues(a, e, ref _loadingAudioSource);

                        StartCoroutine(LoadAudioSourceClipFromFile(finalDir + Path.DirectorySeparatorChar + finalFileName, a.sourceName));

                    }

                    idx++;
                }
            }
        }

        private const string TEMP_FILE_PATH = "./audio/temp.mp3";
        private string WriteToTempFile(byte[] arr, uint length)
        {
            var (directory, fileName) = Helpers.GetFilePath(TEMP_FILE_PATH);
            FileStream fsOut = new FileStream(directory + Path.DirectorySeparatorChar + fileName, FileMode.OpenOrCreate);
            BinaryWriter bw = new BinaryWriter(fsOut);
            bw.Write(arr, 0, (int)length);
            bw.Flush();
            bw.Close();
            fsOut.Close();

            return TEMP_FILE_PATH;
        }

        private byte[] GetAudioBufferArrayFromGLB(string modelName, uint offset, uint length)
        {
            var (directory, fileName) = Helpers.GetFilePath($"{modelName}.glb");

            FileStream fs = new FileStream(directory + Path.DirectorySeparatorChar + fileName, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            long totalLengthRead = 0;

            GLTFBinaryData.Header header = new();
            header.magic = br.ReadUInt32();
            header.version = br.ReadUInt32();
            header.length = br.ReadUInt32();
            totalLengthRead += 12;

            List<GLTFBinaryData.Chunk> chunks = new();

            while (totalLengthRead < fs.Length)
            {

                GLTFBinaryData.Chunk chunk = new();
                chunk.chunkLength = br.ReadUInt32();
                chunk.chunkType = br.ReadUInt32();
                totalLengthRead += 8;
                chunk.data = br.ReadBytes((int)chunk.chunkLength);
                totalLengthRead += (long)chunk.chunkLength;

                chunks.Add(chunk);
            }

            byte[] arr = new byte[length];
            Array.Copy(chunks[GLTF_BIN_DATA_INDEX].data, offset, arr, 0, length);

            return arr;
        }

        private void SetGOOGAudioValues(Audio source, AudioEmitterPartial emitter, ref UnityEngine.AudioSource audioSource)
        {
            if (emitter != null)
            {
                audioSource.volume = emitter.gain;
                audioSource.loop = emitter.loop;
                audioSource.playOnAwake = emitter.autoPlay;
            }
            //string fileName = Path.GetFileNameWithoutExtension(source.uri);
            //audioSource.clip.name = fileName;
        }

        private void SetAudioValues(AudioSource source, AudioEmitterPartial emitter, ref UnityEngine.AudioSource audioSource)
        {
            if (source != null)
            {
                audioSource.volume = source.gain;
                audioSource.loop = source.loop;
            }
            if (emitter != null)
            {
                audioSource.minDistance = emitter.positional.minDistance;
                audioSource.maxDistance = emitter.positional.maxDistance;
            }
//            audioSource.clip.name = source.sourceName;
        }

        private IEnumerator LoadAudioSourceClipFromFile(string pathFileName, string clipName)
        {
            UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(pathFileName, AudioType.MPEG);
            var download = www.SendWebRequest();
            yield return null;

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + www.error);
            }
            else
            {
                var clip = DownloadHandlerAudioClip.GetContent(www);
                _loadingAudioSource.clip = clip;
                _loadingAudioSource.clip.name = clipName;
                _loadedAudioFile = true;
                yield return _loadingAudioSource;
            }
            yield return null;
        }

        internal AudioSource GetAudioSource(int index, List<AudioSource> audioSources)
        {
            foreach (AudioSource audioSource in audioSources)
            {
                if (audioSource.audio == index)
                {
                    return audioSource;
                }
            }
            return null;
        }

        internal AudioEmitterPartial GetAudioEmitterPartial<T>(T t, int index, List<AudioEmitter> audioEmitters)
        {
                foreach (AudioEmitter emitter in audioEmitters)
                {
                if (t is KHR_AudioType)
                {
                    for (int i = 0; i < emitter.sources.Count; i++)
                    {
                        if ((emitter.sources[i] == index) && (emitter.positional.Count == emitter.sources.Count))
                        {
                            return new AudioEmitterPartial()
                            {
                                gain = emitter.gain,
                                name = emitter.name,
                                source = emitter.sources[i],
                                type = emitter.type,
                                positional = emitter.positional[i]
                            };
                        }
                    }
                }
                else if (t is GOOG_AudioType)
                {
                    return new AudioEmitterPartial()
                    {
                        gain = emitter.gain,
                        audio = emitter.audio,
                        autoPlay = emitter.autoPlay,
                        loop = emitter.loop
                    };
                }
            }
            return null;
        }

        private List<GraphData> GetInteractivityGraphsSorted(Dictionary<string, IExtension> extensions)
        {
            var graphs = new List<GraphData>();

            var extensionValues = new List<IExtension>();
            if (!extensions.TryGetValue(InteractivityGraphExtension.EXTENSION_NAME, out IExtension interactivityExtensionValue))
                return null;
            extensionValues.Add(interactivityExtensionValue);
            if (extensions.TryGetValue(AudioGraphExtension<KHR_AudioType>.KHR_EXTENSION_NAME, out IExtension audioExtensionValue))
                extensionValues.Add(audioExtensionValue);
            else if (extensions.TryGetValue(AudioGraphExtension<KHR_AudioType>.GOOG_EXTENSION_NAME, out IExtension GOOGaudioExtensionValue))
                extensionValues.Add(GOOGaudioExtensionValue);


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
                    case AudioGraphExtension<GOOG_AudioType> GOOGaudioGraph:
                        var audioGraphIndex = GOOGaudioGraph.extensionData.defaultGraphIndex;
                        graphs.Add(new GraphData() { extension = GOOGaudioGraph.extensionData, graph = GOOGaudioGraph.extensionData.graphs[audioGraphIndex] });
                        break;
                    case AudioGraphExtension<KHR_AudioType> KHRaudioGraph:
                        var KHRaudioGraphIndex = KHRaudioGraph.extensionData.defaultGraphIndex;
                        graphs.Add(new GraphData() { extension = KHRaudioGraph.extensionData, graph = KHRaudioGraph.extensionData.graphs[KHRaudioGraphIndex] });
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
