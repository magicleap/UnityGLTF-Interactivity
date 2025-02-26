using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class PointerResolver
    {
        private readonly List<NodePointers> _nodePointers = new();
        private readonly List<MaterialPointers> _materialPointers = new();
        private readonly ScenePointers _scenePointers;
        private readonly ActiveCameraPointers _activeCameraPointers = ActiveCameraPointers.CreatePointers();

        public ReadOnlyCollection<NodePointers> nodePointers { get; private set; }

        private readonly Dictionary<string, IPointer> _pointerCache = new();

        private static readonly Regex _variableRegex = new("{(.*?)}");

        public PointerResolver(GLTFSceneImporter importer)
        {
            RegisterNodes(importer);
            RegisterMaterials(importer);
            _scenePointers = new ScenePointers(importer);
        }

        private void RegisterNodes(GLTFSceneImporter importer)
        {
            var nodeSchemas = importer.Root.Nodes;
            var nodeGameObjects = importer.NodeCache;

            for (int i = 0; i < nodeGameObjects.Length; i++)
            {
                Util.Log($"Registered Node Pointer {i}", nodeGameObjects[i]);
                _nodePointers.Add(new NodePointers(nodeGameObjects[i], nodeSchemas[i]));
            }

            nodePointers = new(_nodePointers);
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

            var matches = _variableRegex.Matches(str);

            var values = engineNode.values;

            foreach (Match match in matches)
            {
                str = str.Replace(match.Value, engine.ParseValue(values[match.Groups[1].Value]).ToString());
            }

            if (_pointerCache.TryGetValue(str, out IPointer pointer))
                return pointer;

            pointer = GetPointer(str);

            _pointerCache.Add(str, pointer);

            return pointer;
        }

        public NodePointers PointersOf(GameObject go)
        {
            for (int i = 0; i < _nodePointers.Count; i++)
            {
                if (_nodePointers[i].gameObject == go)
                    return _nodePointers[i];
            }

            throw new InvalidOperationException($"No node pointers found for {go.name}!");
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
            Util.Log($"Getting pointer: {pointerString}");

            var path = pointerString.Split("/");

            switch (path[1])
            {
                case "nodes":
                    return ProcessNodePointer(path);

                case "materials":
                    return ProcessMaterialPointer(path);

                case "activeCamera":
                    return ProcessCameraPointer(path);

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

        private IPointer ProcessCameraPointer(string[] path)
        {
            var property = path[2];

            switch (property)
            {
                case "translation":
                    return _activeCameraPointers.translation;

                case "rotation":
                    return _activeCameraPointers.rotation;
            }

            throw new InvalidOperationException($"Active Camera Property {property} is unsupported at this time!");
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

                case "extensions":
                    return ProcessExtensionPointer(path, nodePointer);
            }

            throw new InvalidOperationException($"Property {property} is unsupported at this time!");
        }

        private IPointer ProcessExtensionPointer(string[] path, NodePointers nodePointer)
        {
            var subProperty = path[4];

            switch(subProperty)
            {
                // TODO: Handle these properly via extensions in UnityGLTF?
                case "KHR_node_selectability":
                    return nodePointer.selectability;
                case "KHR_node_visibility":
                    return nodePointer.visibility;
            }

            throw new InvalidOperationException($"Extension {subProperty} is unsupported at this time!");
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

                case "baseColorTexture":
                    return ProcessBaseColorTexturePointer(path, matPointer);

                case "metallicRoughnessTexture":
                    return ProcessMetallRoughnessTexturePointer(path, matPointer);

                case "metallicFactor":
                    return matPointer.metallicFactor;

                case "roughnessFactor":
                    return matPointer.roughnessFactor;
            }

            throw new InvalidOperationException($"No valid subproperty {subProperty} found for PBR material.");
        }

        private IPointer ProcessBaseColorTexturePointer(string[] path, MaterialPointers matPointer)
        {
            // TODO: These come in the form of baseColorTexture/extensions/KHR_texture_transform/{PROPERTY}
            // Don't skip the extensions/KHR_texture_transform bit.
            var subProperty = path[7];

            switch (subProperty)
            {
                case "offset":
                    return matPointer.baseOffset;

                case "rotation":
                    throw new NotImplementedException();

                case "scale":
                    return matPointer.baseScale;
            }

            throw new InvalidOperationException($"No valid subproperty {subProperty} found for texture transform.");
        }

        private IPointer ProcessMetallRoughnessTexturePointer(string[] path, MaterialPointers matPointer)
        {
            // TODO: These come in the form of metallicRoughnessTexture/extensions/KHR_texture_transform/{PROPERTY}
            // Don't skip the extensions/KHR_texture_transform bit.
            var subProperty = path[7];

            switch (subProperty)
            {
                case "offset":
                    return matPointer.metallicRoughnessOffset;

                case "rotation":
                    throw new NotImplementedException();

                case "scale":
                    return matPointer.metallicRoughnessScale;
            }

            throw new InvalidOperationException($"No valid subproperty {subProperty} found for texture transform.");
        }

        private IPointer ProcessOcclusionMapPointer(string[] path, MaterialPointers matPointer)
        {
            var subProperty = path[4];

            switch (subProperty)
            {
                case "strength":
                    return matPointer.occlusionTextureStrength;
            }

            throw new InvalidOperationException($"No valid subproperty {subProperty} found for occlusion material.");
        }

        private IPointer ProcessNormalMapPointer(string[] path, MaterialPointers matPointer)
        {
            var subProperty = path[4];

            switch (subProperty)
            {
                case "scale":
                    return matPointer.normalTextureScale;
            }

            throw new InvalidOperationException($"No valid subproperty {subProperty} found for normal material.");
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