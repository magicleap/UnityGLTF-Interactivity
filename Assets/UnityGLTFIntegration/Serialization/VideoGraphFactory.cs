using GLTF.Schema;
using Newtonsoft.Json.Linq;

namespace UnityGLTF.Interactivity
{
    public class VideoGraphFactory: ExtensionFactory
    {
        public VideoGraphFactory()
        {
            ExtensionName = VideoGraphExtension.GOOG_EXTENSION_NAME;
        }

        public override IExtension Deserialize(GLTFRoot root, JProperty extensionToken)
        {
            if (extensionToken == null)
                return null;

            var graph = new VideoGraphExtension();
            graph.Deserialize(extensionToken);
            return graph;
        }
    }
}