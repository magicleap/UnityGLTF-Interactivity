using GLTF.Schema;
using UnityGLTF.Plugins;

namespace UnityGLTF.Interactivity
{
    public class AudioImportPlugin : GLTFImportPlugin
    {
        public override string DisplayName => "KHR_Audio_Emitter_Importer";
        public override string Description => "Imports KHR compliant interactivity graphs";

        private AudioImportContext _context;

        public override GLTFImportPluginContext CreateInstance(GLTFImportContext context)
        {
            _context = new AudioImportContext(this);
            GLTFProperty.RegisterExtension(new AudioGraphFactory());

            return _context;
        }

    }
}
