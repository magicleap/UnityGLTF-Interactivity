using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class DebugLog : BehaviourEngineNode
    {
        private string _message;
        private static readonly Regex _variableRegex = new("{(.*?)}");

        public DebugLog(BehaviourEngine engine, Node node) : base(engine, node)
        {
            if (!TryGetConfig(ConstStrings.MESSAGE, out _message))
                _message = "";
        }

        protected override void Execute(string socket, ValidationResult validationResult)
        {
            var formatted = FormatString(_message);

            Debug.Log(formatted);

            TryExecuteFlow(ConstStrings.OUT);
        }

        private string FormatString(string str)
        {
            var matches = _variableRegex.Matches(str);

            foreach (Match match in matches)
            {
                if (!TryEvaluateValue(match.Groups[1].Value, out IProperty value))
                    continue;

                str = str.Replace(match.Value, value.ToString());
            }

            return str;
        }
    }
}