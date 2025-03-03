using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace UnityGLTF.Interactivity
{
    public class BehaviourEngine
    {
        public readonly Graph graph;
        public readonly Dictionary<Node, BehaviourEngineNode> engineNodes = new();
        public AnimationWrapper animationWrapper { get; set; }

        public event Action onStart;

        public event Action<Ray, RaycastHit, RaycastHit[]> onSelect;
        public event Action onTick;
        public event Action<int, Dictionary<string, IProperty>> onCustomEventFired;
        public event Action<Flow> onFlowTriggered;

        public readonly PointerResolver pointerResolver;

        private CancellationTokenSource _cancellationToken = new();

        public BehaviourEngine(Graph graph, PointerResolver pointerResolver)
        {
            this.graph = graph;
            this.pointerResolver = pointerResolver;

            for (int i = 0; i < graph.nodes.Count; i++)
            {
                engineNodes.Add(graph.nodes[i], NodeRegistry.CreateBehaviourEngineNode(this, graph.nodes[i]));
            }
        }

        public void StartPlayback()
        {
            onStart?.Invoke();
        }

        public void Tick()
        {
            onTick?.Invoke();
        }

        public void Select(in Ray ray, in RaycastHit hit, RaycastHit[] otherHits)
        {
            onSelect?.Invoke(ray, hit, otherHits);
        }


        public void ExecuteFlow(Flow flow)
        {
            Assert.IsNotNull(flow.toNode);

            var node = engineNodes[flow.toNode];

            onFlowTriggered?.Invoke(flow);

            node.ValidateAndExecute(flow.toSocket, _cancellationToken.Token);
        }

        public void FireCustomEvent(int eventIndex, Dictionary<string, IProperty> outValues = null)
        {
            if (eventIndex < 0 || eventIndex >= graph.customEvents.Count)
                return; // TODO: Add error handling.

            onCustomEventFired?.Invoke(eventIndex, outValues);
        }

        public IProperty ParseValue(Value v)
        {
            if (v.node == null)
                return v.property;

            var node = engineNodes[v.node];
            return node.GetOutputValue(v.socket);
        }

        public IProperty GetVariableProperty(int variableIndex)
        {
            return graph.variables[variableIndex].property;
        }

        public bool TryGetPointer(string pointerString, BehaviourEngineNode engineNode, out IPointer pointer)
        {
            try
            {
                pointer = pointerResolver.GetPointer(pointerString, engineNode);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            pointer = default;
            return false;
        }

        public async Task PlayAnimationAsync(int animationIndex, float startTime, float endTime, float speed, CancellationToken cancellationToken)
        {
            if (animationWrapper == null)
            {
                Util.LogWarning("Tried to play an animation on a glb that has no animations.");
                return;
            }

            await animationWrapper.PlayAnimationAsync(animationIndex, startTime, endTime, speed, cancellationToken);
        }

        public void CancelExecution()
        {
            _cancellationToken.Cancel();
            _cancellationToken = new();
        }
    }
}