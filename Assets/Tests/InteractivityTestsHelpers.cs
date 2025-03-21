using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityGLTF;
using UnityGLTF.Interactivity;
using UnityGLTF.Loader;

public class InteractivityTestsHelpers
{
    protected BehaviourEngine RunTestForGraph(Graph g, GLTFSceneImporter importer)
    {
        BehaviourEngine eng = new BehaviourEngine(g, importer);
        eng.StartPlayback();
        return eng;
    }

    protected async Task<GLTFSceneImporter> LoadTestModel(string modelName, Action<GameObject, ExceptionDispatchInfo, GLTFSceneImporter>  onLoadComplete = null)
    {
        ImporterFactory _importerFactory = ScriptableObject.CreateInstance<DefaultImporterFactory>();
        ImportOptions _importOptions = new ImportOptions()
        {
            ImportNormals = GLTFImporterNormals.Import,
            ImportTangents = GLTFImporterNormals.Import,
        };

        var (directory, fileName) = Helpers.GetFilePath(modelName);

        _importOptions.DataLoader = new UnityWebRequestLoader(directory);

        var importer = _importerFactory.CreateSceneImporter(
            modelName,
            _importOptions
        );

        var sceneParent = new GameObject(fileName).transform;

        importer.SceneParent = sceneParent;
        importer.Collider = GLTFSceneImporter.ColliderType.Box;
        importer.MaximumLod = 300;
        importer.Timeout = 8;
        importer.IsMultithreaded = true;
        importer.CustomShaderName = null;

        // for logging progress
        await importer.LoadSceneAsync(
            showSceneObj: true,
            onLoadComplete: (go, e) => onLoadComplete?.Invoke(go, e, importer)
        );

        return importer;
    }

}
