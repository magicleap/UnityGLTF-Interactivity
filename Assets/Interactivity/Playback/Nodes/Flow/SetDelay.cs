using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class FlowSetDelay : BehaviourEngineNode
    {
        //seconds to delay actions by
        private float _duration;
        private int _lastDelayIndex = -1;
        private const int MAXIMUM_SIMULTANEOUS_DELAYS = 99;

        public FlowSetDelay(BehaviourEngine engine, Node node) : base(engine, node)
        {
            TryGetConfig(ConstStrings.DURATION, out _duration);
        }

        protected override void Execute(string socket, ValidationResult validationResult)
        {
            switch (socket)
            {
                case ConstStrings.CANCEL:
                    engine.nodeDelayManager.CancelDelaysFromNode(_lastDelayIndex);
                    _lastDelayIndex = -1;
                    break;
                case ConstStrings.IN:
                    if (double.IsNaN(_duration) || double.IsInfinity(_duration) || _duration < 0)
                        TryExecuteFlow(ConstStrings.ERR);
                    else if (node.flows.Count >= MAXIMUM_SIMULTANEOUS_DELAYS)
                    {
                        TryExecuteFlow(ConstStrings.ERR);
                        break;
                    }
                    else
                    {
                        //throw an error if the time to delay by is 1 hour or greater
                        if (_duration >= 3600)
                        {
                            TryExecuteFlow(ConstStrings.ERR);
                            break;
                        }
                        _lastDelayIndex = engine.nodeDelayManager.currentDelayIndex;

                        //NodeDelayData temp;
                        var temp = new NodeDelayData(_lastDelayIndex, node.flows, _duration, Time.timeAsDouble);
                        engine.nodeDelayManager.AddDelayNode(ref temp);

                        Util.Log($"Set delay for {node.flows} flows is scheduled");
                        TryExecuteFlow(ConstStrings.DONE);
                    }

                    TryExecuteFlow(ConstStrings.OUT);
                    break;
                default:
                    throw new InvalidOperationException($"Socket {socket} is not a valid input on this SetDelay node!");
            }
        }

        public override IProperty GetOutputValue(string socket)
        {
            return new Property<int>(_lastDelayIndex);
        }
    }
}