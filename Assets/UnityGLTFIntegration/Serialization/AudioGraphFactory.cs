using GLTF.Schema;
using Newtonsoft.Json.Linq;

namespace UnityGLTF.Interactivity
{
    public class AudioGraphFactory<T> : ExtensionFactory
    {
        public T _t;
        public AudioGraphFactory(T t)
        {
            _t = t;
            if (_t is KHR_AudioType)
            {
                ExtensionName = AudioGraphExtension<KHR_AudioType>.KHR_EXTENSION_NAME;
            }
            else if (_t is GOOG_AudioType)
            {
                ExtensionName = AudioGraphExtension<GOOG_AudioType>.GOOG_EXTENSION_NAME;
            }
        }

        public override IExtension Deserialize(GLTFRoot root, JProperty extensionToken)
        {
            if (extensionToken == null)
                return null;

            if (_t is KHR_AudioType) {
                var graph = new AudioGraphExtension<KHR_AudioType>(_t as KHR_AudioType);
                graph.Deserialize(extensionToken);
                return graph;
            }
            else if (_t is GOOG_AudioType)
            {
                var graph = new AudioGraphExtension<GOOG_AudioType>(_t as GOOG_AudioType);
                graph.Deserialize(extensionToken);
                return graph;
            }
            return null;
        }
    }
}