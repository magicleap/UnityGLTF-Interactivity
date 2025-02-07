using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public struct ScenePointers
    {
        public ReadOnlyPointer<int> animationsLength;
        public ReadOnlyPointer<int> materialsLength;
        public ReadOnlyPointer<int> meshesLength;
        public ReadOnlyPointer<int> nodesLength;

        public ScenePointers(GLTFSceneImporter importer)
        {
            animationsLength = new ReadOnlyPointer<int>()
            {
                getter = () => importer.AnimationCache.Length,
            };

            materialsLength = new ReadOnlyPointer<int>()
            {
                getter = () => importer.MaterialCache.Length,
            };

            meshesLength = new ReadOnlyPointer<int>()
            {
                getter = () => importer.MeshCache.Length,
            };

            nodesLength = new ReadOnlyPointer<int>()
            {
                getter = () => importer.NodeCache.Length,
            };
        }
    }
}