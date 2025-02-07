using UnityGLTF.Plugins;

namespace UnityGLTF.Interactivity
{
    public class InteractivityExportPlugin : GLTFExportPlugin
    {
        public override string DisplayName => "KHR_interactivity_Exporter";
        public override string Description => "Exports KHR compliant interactivity graphs";

        private InteractivityExportContext _context;
        public readonly Graph graph;

        public InteractivityExportPlugin(Graph graph)
        {
            this.graph = graph;
        }

        public override GLTFExportPluginContext CreateInstance(ExportContext context)
        {
            _context = new InteractivityExportContext(this);

            return _context;
        }
    }
}
