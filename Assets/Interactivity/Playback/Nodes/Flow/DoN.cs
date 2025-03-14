using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class FlowDoN : BehaviourEngineNode
    {

        private int _nTimes;
        private int _currentCount;

        public FlowDoN(BehaviourEngine engine, Node node) : base(engine, node)
        {
            TryEvaluateValue(ConstStrings.N, out _nTimes);
        }

        protected override void Execute(string socket, ValidationResult validationResult)
        {
            Util.Log($"Starting a loop with doN {_nTimes}");

            switch (socket)
            {
                case ConstStrings.RESET:
                    _currentCount = 0;
                    break;
                case ConstStrings.IN:
                    if (_currentCount < _nTimes)
                    {
                        _currentCount++;
                        TryExecuteFlow(ConstStrings.OUT);
                    } 
                    break;
                default:
                    throw new InvalidOperationException($"Socket {socket} is not a valid input on this DoN node!");
            }
        }

        public override IProperty GetOutputValue(string socket)
        {
            return new Property<int>(_currentCount);
        }
    }
}