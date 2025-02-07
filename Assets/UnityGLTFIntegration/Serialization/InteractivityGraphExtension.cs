using GLTF.Schema;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class InteractivityGraphExtension : IExtension
    {
        public Graph graph { get; private set; }

        private readonly GraphSerializer _serializer = new();

        public InteractivityGraphExtension(Graph graph = null)
        {
            this.graph = graph;
        }

        public IExtension Clone(GLTFRoot root)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(JProperty extensionToken)
        {
            if (!extensionToken.Name.Equals(ConstStrings.EXTENSION_NAME))
                return;

            graph = _serializer.Deserialize(extensionToken.Value.ToString());
        }

        public JProperty Serialize()
        {
            try
            {
                var json = _serializer.Serialize(graph);

                JObject jobject = JObject.Parse(json);

                return new JProperty(ConstStrings.EXTENSION_NAME, jobject);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            return null;
        }
    }
}