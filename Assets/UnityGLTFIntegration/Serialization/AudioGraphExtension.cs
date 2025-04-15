using GLTF.Schema;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class AudioGraphExtension<T> : IExtension
    {
        public const string KHR_EXTENSION_NAME = "KHR_audio_emitter";
        public const string GOOG_EXTENSION_NAME = "GOOG_audio_emitter";

        public KHR_ExtensionGraph extensionData { get; private set; }

        private readonly GraphSerializer _serializer = new();

        private T _t;
        public AudioGraphExtension(T t,  KHR_ExtensionGraph extensionData = null)
        {
            _t = t;
            this.extensionData = extensionData;
        }

        public IExtension Clone(GLTFRoot root)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(JProperty extensionToken)
        {
            if (extensionToken.Name.Equals(KHR_EXTENSION_NAME) || extensionToken.Name.Equals(GOOG_EXTENSION_NAME))
                extensionData = _serializer.Deserialize(extensionToken.Value.ToString());
        }

        public JProperty Serialize()
        {
            try
            {
                var json = _serializer.Serialize(extensionData);

                JObject jobject = JObject.Parse(json);
                if (_t is KHR_AudioType)
                    return new JProperty(KHR_EXTENSION_NAME, jobject);
                else if (_t is GOOG_EXTENSION_NAME)
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