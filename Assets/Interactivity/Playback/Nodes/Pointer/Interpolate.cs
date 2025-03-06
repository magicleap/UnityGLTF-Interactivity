using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityGLTF.Interactivity.Extensions;

namespace UnityGLTF.Interactivity
{
    public class PointerInterpolate : BehaviourEngineNode
    {
        private IPointer _pointer;
        private IProperty _interpGoal;
        private float _duration;
        private Vector2 _p1, _p2;

        public PointerInterpolate(BehaviourEngine engine, Node node) : base(engine, node)
        {
        }

        protected override void Execute(string socket, ValidationResult validationResult)
        {
            if(validationResult != ValidationResult.Valid)
            {
                TryExecuteFlow(ConstStrings.ERR);
                return;
            }

            TryExecuteFlow(ConstStrings.OUT);

            var data = new InterpolateData()
            {
                pointer = _pointer,
                startTime = Time.time,
                duration = _duration,
                endValue = _interpGoal,
                cp1 = _p1,
                cp2 = _p2,
                done = () => TryExecuteFlow(ConstStrings.DONE)
            };

            engine.interpolationManager.StartInterpolation(ref data);
        }

        public override bool ValidateConfiguration(string socket)
        {
            return TryGetPointerFromConfiguration(out _pointer) && 
                _pointer is not IReadOnlyPointer;
        }

        public override bool ValidateValues(string socket)
        {
            return TryEvaluateValue(ConstStrings.VALUE, out _interpGoal) && 
                TryEvaluateValue(ConstStrings.DURATION, out _duration) &&
                TryEvaluateValue(ConstStrings.P1, out _p1) &&
                TryEvaluateValue(ConstStrings.P2, out _p2);
        }
    }
}