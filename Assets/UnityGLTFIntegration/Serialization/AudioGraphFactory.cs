using GLTF.Schema;
using Newtonsoft.Json.Linq;

namespace UnityGLTF.Interactivity
{
    public class AudioGraphFactory : ExtensionFactory
    {
        public AudioGraphFactory()
        {
            ExtensionName = AudioGraphExtension.EXTENSION_NAME;
        }

        public override IExtension Deserialize(GLTFRoot root, JProperty extensionToken)
        {
            if (extensionToken == null)
                return null;
            
            var graph = new AudioGraphExtension();
            graph.Deserialize(extensionToken);
            return graph;
        }
    }
}