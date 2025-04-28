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

        [SerializeField]
        private RenderTexture _rt;
        [SerializeField]
        private GameObject _quad;

        private Vector3 _lastQuadPosition = Vector3.zero;

        private void Start()
        {
            _loader = new();
            _ = _loader.LoadModelAsync($"{_modelName}.glb", OnLoadComplete, _loadTimeout);

            _quad.SetActive(false);
            _lastQuadPosition = new Vector3(0f, 0f, 3.0f);

            Camera.main.transform.position = new Vector3(0, 0, 0);
            Camera.main.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        //private bool _loadedAudioFile = false;
        //private UnityEngine.AudioSource _loadingAudioSource = null;
        private AudioWrapper _audioWrapper = null;
        private int _audioIndex = 0;
        private VideoWrapper _videoWrapper = null;

        private void Update()
        {
            //if (_loadedAudioFile)
            //{
            //    if (_loadingAudioSource.playOnAwake)
            //        _loadingAudioSource.Play();
            //    _loadedAudioFile = false;
            //}
            //if (_loadedVideoFile && _loadedVideoPlayer != null)
            //{
            //    if (_loadedVideoPlayer.playOnAwake)
            //        _loadedVideoPlayer.Play();
            //    _videoWrapper.AddVideoSource(0, new VideoPlayerData() { index = 0, player = _loadedVideoPlayer });
            //    _loadedVideoPlayer = null;
            //    _loadedVideoFile = false;
            //}
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
                    _videoWrapper = importer.SceneParent.gameObject.GetComponent<VideoWrapper>();
                    if (_videoWrapper == null)
                    {
                        _videoWrapper = importer.SceneParent.gameObject.AddComponent<VideoWrapper>();
                        _behaviourEngine.SetVideoWrapper(_videoWrapper);
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
                        case KHR_ExtensionGraph.GraphType.GOOG_Video:
                            var videoSrcs = gdList.FindAll(r => (r.graph.videoSource != null) && (r.graph.videoSource.Count> 0));
                            if (videoSrcs != null && videoSrcs.Count > 0)
                                AddGOOGVideoSource(importer, videoSrcs);
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

        public void AddGOOGVideoSource(GLTFSceneImporter importer, List<GraphData> videoGraphs)
        {
            if (_videoWrapper == null)
                return;

            //loop through all of the audio graphs. Could be more than one conceivably. Practically speaking though, 
            // one model should have one audio graph with audio, emitters and sources
            foreach (var videoGraph in videoGraphs)
            {
                Graph graph = videoGraph.graph;

                int idx = 0;

                // each audio
                foreach (var video in graph.videoSource)
                {
                    VideoData e = GetVideoData(idx, graph.videoData);

                    if (e == null)
                    {
                        return;
                    }

                    string path = string.Empty;
                    string finalFileName = String.Empty;
                    string finalDir = String.Empty;
                    if (video.uri != null && !string.IsNullOrEmpty(video.uri))
                    {
                        video.uri = video.uri;
                        path = Path.GetDirectoryName(_modelName);
                        if (!string.IsNullOrEmpty(path))
                        {
                            path += Path.DirectorySeparatorChar + video.uri;
                        }
                        else
                        {
                            path = video.uri;
                        }
                        var (directory, fileName) = Helpers.GetFilePath(path);
                        finalFileName = fileName;
                        finalDir = directory;
                    }
                    else
                    {
                        /// get index into buffer
                        BufferView bv = importer.Root.BufferViews[video.bufferView];
                        var offset = bv.ByteOffset;
                        var length = bv.ByteLength;

                        byte[] videoArray = GetBufferArrayFromGLB(_modelName, offset, length);
                        string tempFile = WriteVideoToTempFile(videoArray, length);

                        var (directory, fileName) = Helpers.GetFilePath(tempFile);
                        finalFileName = fileName;
                        finalDir = directory;
                    }

                    if (!string.IsNullOrEmpty(finalFileName) && !string.IsNullOrEmpty(finalDir))
                    {
                        var loadedVideoPlayer = importer.SceneParent.gameObject.AddComponent<UnityEngine.Video.VideoPlayer>();
                        SetGOOGVideoValues(video, e, ref loadedVideoPlayer);
                        string path1 = finalDir + Path.DirectorySeparatorChar + finalFileName; // Application.streamingAssetsPath + Path.DirectorySeparatorChar + video.uri;
                        loadedVideoPlayer.url = path1;

                        // duplicate quad and assign movie
                        GameObject newQuad = Instantiate(_quad, this.transform);
                        _lastQuadPosition += new Vector3(.3f, 0, 0);
                        newQuad.SetActive(true);
                        newQuad.transform.position += _lastQuadPosition;
                        RenderTexture newRT = Instantiate(_rt);
                        newQuad.GetComponent<Renderer>().material.mainTexture = newRT;
                        loadedVideoPlayer.targetTexture = newRT;

                        if (loadedVideoPlayer.playOnAwake)
                            loadedVideoPlayer.Play();
                        _videoWrapper.AddVideoSource(idx, new VideoPlayerData() { index = idx, player = loadedVideoPlayer });
                    }
                    idx++;
                }
            }
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

                        byte[] audioArray = GetBufferArrayFromGLB(_modelName, offset, length);
                        string tempFile = WriteAudioToTempFile(audioArray, length);

                        var (directory, fileName) = Helpers.GetFilePath(tempFile);
                        finalFileName = fileName;
                        finalDir = directory;
                    }

                    if (!string.IsNullOrEmpty(finalFileName) && !string.IsNullOrEmpty(finalDir))
                    {
//                        if (_loadingAudioSource == null)
                        var loadingAudioSource = importer.SceneParent.gameObject.AddComponent<UnityEngine.AudioSource>();
                        _audioWrapper.AddAudioSource(_audioIndex, new AudioPlayData() { index = _audioIndex, source = loadingAudioSource });
                        SetGOOGAudioValues(audio, e, ref loadingAudioSource);
                        StartCoroutine(LoadAudioSourceClipFromFile(_audioIndex, finalDir + Path.DirectorySeparatorChar + finalFileName, Path.GetFileNameWithoutExtension(audio.uri), e.autoPlay));
                        _audioIndex++;
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

                        byte[] audioArray = GetBufferArrayFromGLB(_modelName, offset, length);
                        string tempFile =  WriteAudioToTempFile(audioArray, length);

                        var (directory, fileName) = Helpers.GetFilePath(tempFile);
                        finalFileName = fileName;
                        finalDir = directory;
                    }

                    if (!string.IsNullOrEmpty(finalFileName) && !string.IsNullOrEmpty(finalDir))
                    {
                        var loadingAudioSource = importer.SceneParent.gameObject.AddComponent<UnityEngine.AudioSource>();
                        _audioWrapper.AddAudioSource(_audioIndex, new AudioPlayData() { index = _audioIndex, source = loadingAudioSource });
                        SetAudioValues(a, e, ref loadingAudioSource);
                        StartCoroutine(LoadAudioSourceClipFromFile(_audioIndex, finalDir + Path.DirectorySeparatorChar + finalFileName, a.sourceName, e.autoPlay));
                        _audioIndex++;
                    }

                    idx++;
                }
            }
        }

        private const string AUDIO_TEMP_FILE_PATH = "./audio/temp.mp3";
        private const string VIDEO_TEMP_FILE_PATH = "./audio/temp.mp4";

        private string WriteVideoToTempFile(byte[] arr, uint length)
        {
            var (directory, fileName) = Helpers.GetFilePath(VIDEO_TEMP_FILE_PATH);
            FileStream fsOut = new FileStream(directory + Path.DirectorySeparatorChar + fileName, FileMode.OpenOrCreate);
            BinaryWriter bw = new BinaryWriter(fsOut);
            bw.Write(arr, 0, (int)length);
            bw.Flush();
            bw.Close();
            fsOut.Close();

            return VIDEO_TEMP_FILE_PATH;
        }

        private string WriteAudioToTempFile(byte[] arr, uint length)
        {
            var (directory, fileName) = Helpers.GetFilePath(AUDIO_TEMP_FILE_PATH);
            FileStream fsOut = new FileStream(directory + Path.DirectorySeparatorChar + fileName, FileMode.OpenOrCreate);
            BinaryWriter bw = new BinaryWriter(fsOut);
            bw.Write(arr, 0, (int)length);
            bw.Flush();
            bw.Close();
            fsOut.Close();

            return AUDIO_TEMP_FILE_PATH;
        }

        private byte[] GetBufferArrayFromGLB(string modelName, uint offset, uint length)
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

        private void SetGOOGVideoValues(VideoSource source, VideoData data, ref UnityEngine.Video.VideoPlayer videoPlayer)
        {
            if (data != null)
            {
                videoPlayer.playOnAwake = data.autoPlay;
                videoPlayer.isLooping = data.loop;
                videoPlayer.playbackSpeed = data.speed;
            }
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

        //private IEnumerator LoadVideoSourceClipFromFile(string pathFileName, string clipName)
        //{
        //    UnityWebRequest www = UnityEngine.Video.VideoPlayer.

        //}

        private IEnumerator LoadAudioSourceClipFromFile(int audioIndex, string pathFileName, string clipName, bool autoPlay)
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
                var loadingAudioSource = _audioWrapper.GetAudioSource(audioIndex).source;
                loadingAudioSource.clip = clip;
                loadingAudioSource.clip.name = clipName;

                if (autoPlay)
                    loadingAudioSource.Play();
//                _loadedAudioFile = true;
                yield return loadingAudioSource;
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
        
        // in case we ever have an array of video[]
        internal VideoData GetVideoData(int idx, List<VideoData> videoDatas)
        {
            foreach(VideoData videoData in videoDatas)
            {
                if (videoData.video == idx)
                {
                    return new VideoData()
                    {
                        autoPlay = videoData.autoPlay,
                        loop = videoData.loop,
                        name = videoData.name,
                        speed = videoData.speed,
                        video = videoData.video
                    };
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
            if (extensions.TryGetValue(VideoGraphExtension.GOOG_EXTENSION_NAME, out IExtension videoExtensionValue))
                extensionValues.Add(videoExtensionValue);
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
                    case VideoGraphExtension GOOGvideoGraph:
                        var videoGraphIndex = GOOGvideoGraph.extensionData.defaultGraphIndex;
                        graphs.Add(new GraphData() { extension = GOOGvideoGraph.extensionData, graph = GOOGvideoGraph.extensionData.graphs[videoGraphIndex] });
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
