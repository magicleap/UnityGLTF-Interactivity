using System;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityGLTF.Loader;

namespace UnityGLTF.Interactivity
{
    public class InteractiveGLBLoader
    {
        private ImporterFactory _importerFactory;
        private ImportOptions _importOptions;

        public InteractiveGLBLoader()
        {
            _importerFactory = ScriptableObject.CreateInstance<DefaultImporterFactory>();
            _importOptions = new ImportOptions()
            {
                ImportNormals = GLTFImporterNormals.Import,
                ImportTangents = GLTFImporterNormals.Import,
            };
        }      

        public async Task<GLTFSceneImporter> LoadModelAsync(string filePath, Action<GameObject, ExceptionDispatchInfo, GLTFSceneImporter> onLoadComplete = null, int timeout = 8)
        {
            try
            {
                var (directory, fileName) = Helpers.GetFilePath(filePath);

                _importOptions.DataLoader = new UnityWebRequestLoader(directory);

                var importer = _importerFactory.CreateSceneImporter(
                    fileName,
                    _importOptions
                );

                var sceneParent = new GameObject(fileName).transform;

                importer.SceneParent = sceneParent;
                importer.Collider = GLTFSceneImporter.ColliderType.Box;
                importer.MaximumLod = 300;
                importer.Timeout = timeout;
                importer.IsMultithreaded = true;
                importer.CustomShaderName = null;

                // for logging progress
                await importer.LoadSceneAsync(
                    showSceneObj: true,
                    onLoadComplete: (go, e) => onLoadComplete(go, e, importer)
                );

                return importer;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return null;
            }
        }

        public void SaveModel(string fileName, GLTFSettings settings, GLTFSceneImporter importer, KHR_interactivity extensionData)
        {
            Util.Log($"Scene parent is {importer.SceneParent.name}", importer.SceneParent.gameObject);
            var sceneTransform = importer.SceneParent.GetChild(0);

            var rootTransforms = new Transform[sceneTransform.childCount];

            for (int i = 0; i < rootTransforms.Length; i++)
            {
                rootTransforms[i] = sceneTransform.GetChild(i);
            }

            var sceneExporter = new GLTFSceneExporter(rootTransforms, new ExportContext(settings));

            sceneExporter.SaveGLB(Application.streamingAssetsPath, fileName);
        }
    }
}
