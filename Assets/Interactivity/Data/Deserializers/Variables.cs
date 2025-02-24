using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityGLTF.Interactivity
{
    public static class VariablesDeserializer
    {
        public static List<Variable> GetVariables(JObject jObj, List<Type> types)
        {
            var jVariables = jObj[ConstStrings.VARIABLES].Children();

            var variables = new List<Variable>(jVariables.Count());

            foreach (var v in jVariables)
            {
                variables.Add(CreateVariable(v, types));
            }

            return variables;
        }

        private static Variable CreateVariable(JToken token, List<Type> types)
        {
            var id = token[ConstStrings.ID].Value<string>();
            var typeIndex = token[ConstStrings.TYPE].Value<int>();
            var valueArray = token[ConstStrings.VALUE] as JArray;

            return new Variable()
            {
                id = id,
                property = Helpers.CreateProperty(types[typeIndex], valueArray),
                initialValue = Helpers.CreateProperty(types[typeIndex], valueArray),
            };
        }
    }
}