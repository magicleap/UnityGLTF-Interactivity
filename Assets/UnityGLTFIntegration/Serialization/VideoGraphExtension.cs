using GLTF.Schema;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class VideoGraphExtension : IExtension
    {
        public const string GOOG_EXTENSION_NAME = "GOOG_video";

        public KHR_ExtensionGraph extensionData { get; private set; }

        private readonly GraphSerializer _serializer = new();

        public VideoGraphExtension(KHR_ExtensionGraph extensionData = null)
        {
            this.extensionData = extensionData;
        }

        public IExtension Clone(GLTFRoot root)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(JProperty extensionToken)
        {
            if (extensionToken.Name.Equals(GOOG_EXTENSION_NAME))
                extensionData = _serializer.Deserialize(extensionToken.Value.ToString());
        }

        public JProperty Serialize()
        {
            try
            {
                var json = _serializer.Serialize(extensionData);

                JObject jobject = JObject.Parse(json);
                return new JProperty(GOOG_EXTENSION_NAME, jobject);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            return null;
        }
    }
}