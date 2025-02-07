using GLTF.Schema;
using UnityEngine;
using UnityGLTF.Plugins;

namespace UnityGLTF.Interactivity
{

    public class InteractivityExportContext : GLTFExportPluginContext
    {
        internal readonly InteractivityExportPlugin settings;

        public InteractivityExportContext(InteractivityExportPlugin interactivityLoader)
        {
            settings = interactivityLoader;
        }

        public override void AfterMaterialExport(GLTFSceneExporter exporter, GLTFRoot gltfRoot, Material material, GLTFMaterial materialNode)
        {
            Debug.Log($"InteractivityExportContext::AfterMaterialExport ");
        }
        public override void AfterMeshExport(GLTFSceneExporter exporter, Mesh mesh, GLTFMesh gltfMesh, int index)
        {
            Debug.Log($"InteractivityExportContext::AfterMeshExport ");
        }
        public override void AfterNodeExport(GLTFSceneExporter exporter, GLTFRoot gltfRoot, Transform transform, GLTF.Schema.Node node)
        {
            Debug.Log($"InteractivityExportContext::AfterNodeExport ");
        }
        public override void AfterPrimitiveExport(GLTFSceneExporter exporter, Mesh mesh, MeshPrimitive primitive, int index)
        {
            Debug.Log($"InteractivityExportContext::AfterPrimitiveExport ");
        }
        public override void AfterSceneExport(GLTFSceneExporter exporter, GLTFRoot gltfRoot)
        {
            Debug.Log($"InteractivityExportContext::AfterSceneExport ");
        }
        public override void AfterTextureExport(GLTFSceneExporter exporter, GLTFSceneExporter.UniqueTexture texture, int index, GLTFTexture tex)
        {
            Debug.Log($"InteractivityExportContext::AfterTextureExport ");
        }
        public override bool BeforeMaterialExport(GLTFSceneExporter exporter, GLTFRoot gltfRoot, Material material, GLTFMaterial materialNode)
        {
            Debug.Log($"InteractivityExportContext::BeforeMaterialExport ");
            return false;
        }
        public override void BeforeNodeExport(GLTFSceneExporter exporter, GLTFRoot gltfRoot, Transform transform, GLTF.Schema.Node node)
        {
            Debug.Log($"InteractivityExportContext::BeforeNodeExport ");
        }
        public override void BeforeSceneExport(GLTFSceneExporter exporter, GLTFRoot gltfRoot)
        {
            Debug.Log($"InteractivityExportContext::BeforeSceneExport ");
            exporter.DeclareExtensionUsage(ConstStrings.EXTENSION_NAME, true);
            gltfRoot.AddExtension(ConstStrings.EXTENSION_NAME, new InteractivityGraphExtension(settings.graph));
        }
        public override void BeforeTextureExport(GLTFSceneExporter exporter, ref GLTFSceneExporter.UniqueTexture texture, string textureSlot)
        {
            Debug.Log($"InteractivityExportContext::BeforeTextureExport ");
        }
    }

}