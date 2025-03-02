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
        private readonly List<CameraPointers> _cameraPointers = new();
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

                if (nodeGameObjects[i].TryGetComponent(out Camera cam))
                {
                    cam.enabled = true;
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
                    return NodePointers.ProcessNodePointer(path, _nodePointers);

                case "materials":
                    return MaterialPointers.ProcessMaterialPointer(path, _materialPointers);

                case "activeCamera":
                    return _activeCameraPointers.ProcessActiveCameraPointer(path[2]);

                case "cameras":
                    return CameraPointers.ProcessCameraPointer(path, _cameraPointers);

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
    }
}