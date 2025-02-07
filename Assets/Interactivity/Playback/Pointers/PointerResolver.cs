using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class PointerResolver
    {
        private readonly List<NodePointers> _nodePointers = new();
        private readonly List<MaterialPointers> _materialPointers = new();
        private readonly ScenePointers _scenePointers;

        public PointerResolver(GLTFSceneImporter importer)
        {
            RegisterNodes(importer);
            RegisterMaterials(importer);
            _scenePointers = new ScenePointers(importer);
        }

        private void RegisterNodes(GLTFSceneImporter importer)
        {
            var nodes = importer.NodeCache;
            for (int i = 0; i < nodes.Length; i++)
            {
                _nodePointers.Add(new NodePointers(nodes[i]));
            }
        }

        private void RegisterMaterials(GLTFSceneImporter importer)
        {
            var materials = importer.MaterialCache;
            for (int i = 0; i < materials.Length; i++)
            {
                _materialPointers.Add(new MaterialPointers(materials[i].UnityMaterialWithVertexColor));
            }
        }

        public IPointer GetPointer(string pointerString, BehaviourEngineNode engineNode, BehaviourEngine engine)
        {
            var str = pointerString;

            var regex = new Regex("{(.*?)}");
            var matches = regex.Matches(str);

            var values = engineNode.values;

            foreach (Match match in matches)
            {
                str = str.Replace(match.Value, engine.ParseValue(values[match.Groups[1].Value]).ToString());
            }

            return GetPointer(str);
        }

        public int IndexOf(GameObject go)
        {
            for (int i = 0; i < _nodePointers.Count; i++)
            {
                if (_nodePointers[i].gameObject == go)
                    return i;
            }

            return -1;
        }

        private IPointer GetPointer(string pointerString)
        {
            Debug.Log($"Getting pointer: {pointerString}");

            var path = pointerString.Split("/");

            switch (path[1])
            {
                case "nodes":
                    return ProcessNodePointer(path);

                case "materials":
                    return ProcessMaterialPointer(path);

                case Pointers.ANIMATIONS_LENGTH:
                    return _scenePointers.animationsLength;

                case Pointers.MATERIALS_LENGTH:
                    return _scenePointers.materialsLength;

                case Pointers.MESHES_LENGTH:
                    return _scenePointers.meshesLength;

                case Pointers.NODES_LENGTH:
                    return _scenePointers.nodesLength;
            }

            throw new InvalidOperationException("No valid pointer found.");
        }

        private IPointer ProcessNodePointer(string[] path)
        {
            var nodeIndex = int.Parse(path[2]);
            var nodePointer = GetNodePointer(nodeIndex);
            var property = path[3];

            switch (property)
            {
                case "translation":
                    return nodePointer.translation;

                case "rotation":
                    return nodePointer.rotation;

                case "scale":
                    return nodePointer.scale;

            }

            throw new InvalidOperationException("No valid property found for node.");
        }

        private IPointer ProcessMaterialPointer(string[] path)
        {
            var matIndex = int.Parse(path[2]);
            var matPointer = GetMaterialPointer(matIndex);
            var property = path[3];

            switch (property)
            {
                case "alphaCutoff":
                    return matPointer.alphaCutoff;

                case "emissiveFactor":
                    return matPointer.emissiveFactor;

                case "normalTexture":
                    return ProcessNormalMapPointer(path, matPointer);

                case "occlusionTexture":
                    return ProcessOcclusionMapPointer(path, matPointer);

                case "pbrMetallicRoughness":
                    return ProcessPBRMetallicRoughnessPointer(path, matPointer);
            }

            throw new InvalidOperationException("No valid property found for material.");
        }

        private IPointer ProcessPBRMetallicRoughnessPointer(string[] path, MaterialPointers matPointer)
        {
            var subProperty = path[4];

            switch(subProperty)
            {
                case "baseColorFactor":
                    return matPointer.baseColorFactor;

                case "metallicFactor":
                    return matPointer.metallicFactor;

                case "roughnessFactor":
                    return matPointer.roughnessFactor;
            }

            throw new InvalidOperationException("No valid subproperty found for material.");
        }

        private IPointer ProcessOcclusionMapPointer(string[] path, MaterialPointers matPointer)
        {
            var subProperty = path[4];

            switch (subProperty)
            {
                case "strength":
                    return matPointer.occlusionTextureStrength;
            }

            throw new InvalidOperationException("No valid subproperty found for material.");
        }

        private IPointer ProcessNormalMapPointer(string[] path, MaterialPointers matPointer)
        {
            var subProperty = path[4];

            switch (subProperty)
            {
                case "scale":
                    return matPointer.normalTextureScale;
            }

            throw new InvalidOperationException("No valid subproperty found for material.");
        }

        private NodePointers GetNodePointer(int nodeIndex)
        {
            return _nodePointers[nodeIndex];
        }

        private MaterialPointers GetMaterialPointer(int matIndex)
        {
            return _materialPointers[matIndex];
        }
    }
}