using GLTF.Schema;
using UnityGLTF.Plugins;

namespace UnityGLTF.Interactivity
{
    public class VideoImportPlugin : GLTFImportPlugin
    { 
        public override string DisplayName => "Video_Importer";
        public override string Description => "Imports GOOG compliant video graphs";

        private VideoImportContext _context;

        public override GLTFImportPluginContext CreateInstance(GLTFImportContext context)
        {
            _context = new VideoImportContext(this);

            GLTFProperty.RegisterExtension(new VideoGraphFactory());

            return _context;
        }

    }
}
