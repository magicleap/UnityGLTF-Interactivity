using GLTF.Schema;
using UnityGLTF.Plugins;

namespace UnityGLTF.Interactivity
{
    public class AudioImportPlugin : GLTFImportPlugin
    { 
        public override string DisplayName => "Audio_Emitter_Importer";
        public override string Description => "Imports KHR or GOOG compliant interactivity graphs";

        private AudioImportContext _context;

        public override GLTFImportPluginContext CreateInstance(GLTFImportContext context)
        {
            _context = new AudioImportContext(this);

            GLTFProperty.RegisterExtension(new AudioGraphFactory<KHR_AudioType>(new KHR_AudioType()));
            GLTFProperty.RegisterExtension(new AudioGraphFactory<GOOG_AudioType>(new GOOG_AudioType()));

            return _context;
        }

    }
}
