using GLTF.Schema;
using UnityEngine;
using UnityGLTF.Plugins;

namespace UnityGLTF.Interactivity
{
    public class InteractivityImportContext : GLTFImportPluginContext
    {
        internal readonly InteractivityImportPlugin settings;

        public InteractivityImportContext(InteractivityImportPlugin interactivityLoader)
        {
            settings = interactivityLoader;
        }

        /// <summary>
        /// Called before import starts
        /// </summary>
        public override void OnBeforeImport()
        {
            Debug.Log($"InteractivityImportContext::OnBeforeImport Complete");
        }

        public override void OnBeforeImportRoot()
        {
            Debug.Log($"InteractivityImportContext::OnBeforeImportRoot Complete");
        }

        /// <summary>
        /// Called when the GltfRoot has been deserialized
        /// </summary>
        public override void OnAfterImportRoot(GLTFRoot gltfRoot)
        {
            Debug.Log($"InteractivityImportContext::OnAfterImportRoot Complete: {gltfRoot.ToString()}");
        }

        public override void OnBeforeImportScene(GLTFScene scene)
        {
            Debug.Log($"InteractivityImportContext::OnBeforeImportScene Complete: {scene.ToString()}");
        }

        public override void OnAfterImportNode(GLTF.Schema.Node node, int nodeIndex, GameObject nodeObject)
        {
            Debug.Log($"InteractivityImportContext::OnAfterImportNode Complete: {node.ToString()}");
        }

        public override void OnAfterImportMaterial(GLTFMaterial material, int materialIndex, Material materialObject)
        {
            Debug.Log($"InteractivityImportContext::OnAfterImportMaterial Complete: {material.ToString()}");
        }

        public override void OnAfterImportTexture(GLTFTexture texture, int textureIndex, Texture textureObject)
        {
            Debug.Log($"InteractivityImportContext::OnAfterImportTexture Complete: {texture.ToString()}");
        }

        public override void OnAfterImportScene(GLTFScene scene, int sceneIndex, GameObject sceneObject)
        {
            Debug.Log($"InteractivityImportContext::OnAfterImportScene Complete: {scene.Extensions}");
        }

        public override void OnAfterImport()
        {
            Debug.Log($"InteractivityImportContext::OnAfterImport Complete");
        }
    }

}