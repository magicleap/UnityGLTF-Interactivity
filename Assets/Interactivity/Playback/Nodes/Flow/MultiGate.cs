using System;
using System.Collections.Generic;
using System.Threading;

namespace UnityGLTF.Interactivity
{
    //TODO test
    public class FlowMultiGate : BehaviourEngineNode
    {
        private bool isRandom;
        private bool isLoop;
        private int rand;
        //Maximum reps before the do-while loop automatically exits
        private int maxReps = 99;

        public FlowMultiGate(BehaviourEngine engine, Node node) : base(engine, node)
        {

        }

        protected override void Execute(string socket, ValidationResult validationResult, CancellationToken cancellationToken)
        {
            List<Flow> executionSequence = node.flows;
            do
            {
                if (isRandom)
                {
                    Random r = new Random();
                    for (int i = node.flows.Count; i > 0; --i)
                    {
                        rand = r.Next(i + 1);
                        //swap values using tuple
                        (executionSequence[rand], executionSequence[i]) = (executionSequence[i], executionSequence[rand]);
                    }
                }

                foreach (Flow output in executionSequence)
                {
                    engine.ExecuteFlow(output);
                }
                maxReps--;
            } while (isLoop && maxReps > 0);

            TryExecuteFlow(ConstStrings.COMPLETED);
        }

        public override bool ValidateValues(string socket)
        {
            if (!TryEvaluateValue(ConstStrings.CONDITION, out bool condition))
                return false;

            return true;
        }
    }
}