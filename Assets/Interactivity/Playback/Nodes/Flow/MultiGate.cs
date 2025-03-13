using System;
using System.Linq;

namespace UnityGLTF.Interactivity
{
    public class FlowMultiGate : BehaviourEngineNode
    {
        private bool _isRandom = false;
        private bool _isLoop = false;
        private int _lastIndex = -1;
        private readonly Flow[] _outputFlows;
        private readonly bool[] _activated;

        private const int _maxOutputFlows = 99;

        public FlowMultiGate(BehaviourEngine engine, Node node) : base(engine, node)
        {
            TryGetConfig(ConstStrings.OUTPUT_FLOWS, out _outputFlows);
            _activated = Enumerable.Repeat(false, _outputFlows.Length).ToArray();
        }

        protected override void Execute(string socket, ValidationResult validationResult)
        {
            if (_outputFlows.Length > _maxOutputFlows)
                throw new InvalidOperationException("There are too many output flows to execute");

            switch (socket)
            {
                case ConstStrings.RESET:
                    _lastIndex = -1;
                    Array.Fill(_activated, false);
                    break;
                case ConstStrings.INPUT_FLOWS:
                    Span<int> outputs = stackalloc int[_outputFlows.Length];
                    if (_isRandom)
                    {
                        if (_activated.Contains(false))
                        {
                            Random rnd = new Random(DateTime.Now.Millisecond);
                            outputs = Enumerable.Range(0, outputs.Length).OrderBy(i => rnd.Next()).ToArray();
                        }
                    }

                    for (int i = 0; i < outputs.Length; i++)
                    {
                        _lastIndex = i;
                        _activated[outputs[_lastIndex]] = true;
                        engine.ExecuteFlow(node.flows[outputs[_lastIndex]]);
                    }

                    TryExecuteFlow(ConstStrings.COMPLETED);
                    break;
                default:
                    throw new InvalidOperationException("The input socket ran into an error");
            }
        }

        public override IProperty GetOutputValue(string socket)
        {
            return new Property<int>(_lastIndex);
        }

        public override bool ValidateConfiguration(string socket)
        {
            if (!TryGetConfig(ConstStrings.IS_LOOP, out _isLoop) || !TryGetConfig(ConstStrings.IS_RANDOM, out _isRandom))
            {
                _isLoop = _isRandom = false;
                return false;
            }

            return true;
        }
    }
}