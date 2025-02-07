using System;
using System.Collections.Generic;

namespace UnityGLTF.Interactivity
{
    public class EventReceive : BehaviourEngineNode
    {
        private Dictionary<string, IProperty> _outValues = new();

        public EventReceive(BehaviourEngine engine, Node node) : base(engine, node)
        {
            engine.onCustomEventFired += OnEventFired;
        }

        public override IProperty GetOutputValue(string socket)
        {
            if (!_outValues.TryGetValue(socket, out IProperty outValue))
                throw new ArgumentException($"No output value found for socket {socket}");

            return outValue;
        }

        private void OnEventFired(int eventIndex, Dictionary<string, IProperty> outValues)
        {
            if (!TryGetConfig(ConstStrings.EVENT, out int eventToListenFor))
                throw new InvalidOperationException();

            if (eventIndex != eventToListenFor)
                return;

            _outValues = outValues;

            TryExecuteFlow(ConstStrings.OUT);
        }
    }
}