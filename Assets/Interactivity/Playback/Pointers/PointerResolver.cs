using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityGLTF.Interactivity.Materials;

namespace UnityGLTF.Interactivity
{
    public class PointerResolver
    {
        private readonly List<NodePointers> _nodePointers = new();
        private readonly List<MaterialPointers> _materialPointers = new();
        private readonly List<CameraPointers> _cameraPointers = new();
        private readonly ScenePointers _scenePointers;
        private readonly ActiveCameraPointers _activeCameraPointers = ActiveCameraPointers.CreatePointers();

        public ReadOnlyCollection<NodePointers> nodePointers { get; private set; }

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

                if (nodeGameObjects[i].TryGetComponent(out Camera cam))
                {
                    Util.Log($"Registered Camera", nodeGameObjects[i]);
                    _cameraPointers.Add(new CameraPointers(cam));
                }
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

        public IPointer GetPointer(string pointerString, BehaviourEngineNode engineNode)
        {
            Util.Log($"Getting pointer: {pointerString}");

            var reader = new StringSpanReader(pointerString);

            reader.Slice('/', '/');

            return reader.AsReadOnlySpan() switch
            {
                var a when a.SequenceEqual("nodes".AsSpan()) => NodePointers.ProcessNodePointer(reader, engineNode, _nodePointers),
                var a when a.SequenceEqual("materials".AsSpan()) => MaterialPointers.ProcessMaterialPointer(reader, engineNode, _materialPointers),
                var a when a.SequenceEqual("activeCamera".AsSpan()) => _activeCameraPointers.ProcessActiveCameraPointer(reader),
                var a when a.SequenceEqual("cameras".AsSpan()) => CameraPointers.ProcessCameraPointer(reader, engineNode, _cameraPointers),
                var a when a.SequenceEqual(Pointers.ANIMATIONS_LENGTH.AsSpan()) => _scenePointers.animationsLength,
                var a when a.SequenceEqual(Pointers.MATERIALS_LENGTH.AsSpan()) => _scenePointers.materialsLength,
                var a when a.SequenceEqual(Pointers.MESHES_LENGTH.AsSpan()) => _scenePointers.meshesLength,
                var a when a.SequenceEqual(Pointers.NODES_LENGTH.AsSpan()) => _scenePointers.nodesLength,
                _ => throw new InvalidOperationException($"No valid pointer found with name {reader.ToString()}"),
            };
        }

        public static int GetNodeIndexFromArgument(StringSpanReader reader, BehaviourEngineNode engineNode)
        {
            int nodeIndex;

            if (reader[0] == '{')
            {
                reader.Slice('{', '}');
                // Can't access the values dictionary with a Span, prevents this from being 0 allocation.
                var property = (Property<int>)engineNode.engine.ParseValue(engineNode.values[reader.ToString()]);
                nodeIndex = property.value;
            }
            else
            {
                nodeIndex = int.Parse(reader.AsReadOnlySpan());
            }

            return nodeIndex;
        }
    }
}