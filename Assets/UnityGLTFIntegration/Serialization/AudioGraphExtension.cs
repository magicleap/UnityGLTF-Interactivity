using GLTF.Schema;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class AudioGraphExtension : IExtension
    {
        public const string EXTENSION_NAME = "KHR_audio_emitter";

        public KHR_ExtensionGraph extensionData { get; private set; }

        private readonly GraphSerializer _serializer = new();

        public AudioGraphExtension(KHR_ExtensionGraph extensionData = null)
        {
            this.extensionData = extensionData;
        }

        public IExtension Clone(GLTFRoot root)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(JProperty extensionToken)
        {
            if (!extensionToken.Name.Equals(EXTENSION_NAME))
                return;

            extensionData = _serializer.Deserialize(extensionToken.Value.ToString());
        }

        public JProperty Serialize()
        {
            try
            {
                var json = _serializer.Serialize(extensionData);

                JObject jobject = JObject.Parse(json);

                return new JProperty(EXTENSION_NAME, jobject);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            return null;
        }
    }
}