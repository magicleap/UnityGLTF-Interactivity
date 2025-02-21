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
        public readonly AnimationWrapper animationWrapper;

        public event Action onStart;
        public event Action<GameObject, int> onSelect;
        public event Action onTick;
        public event Action<int, Dictionary<string, IProperty>> onCustomEventFired;

        public readonly PointerResolver pointerResolver;

        private CancellationTokenSource _cancellationToken = new();

        public BehaviourEngine(Graph graph, PointerResolver pointerResolver, AnimationWrapper animationWrapper)
        {
            this.graph = graph;
            this.pointerResolver = pointerResolver;
            this.animationWrapper = animationWrapper;

            for (int i = 0; i < graph.nodes.Count; i++)
            {
                engineNodes.Add(graph.nodes[i], CreateBehaviourEngineNode(graph.nodes[i]));
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

        public void Select(GameObject go)
        {
            var nodeIndex = pointerResolver.IndexOf(go);
            onSelect?.Invoke(go, nodeIndex);
        }

        private BehaviourEngineNode CreateBehaviourEngineNode(Node node)
        {
            var instantiateFunc = NodeRegistry.nodeTypes[node.type];

            return instantiateFunc(this, node);
        }

        public void ExecuteFlow(Flow flow)
        {
            Assert.IsNotNull(flow.toNode);

            var node = engineNodes[flow.toNode];

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
                pointer = pointerResolver.GetPointer(pointerString, engineNode, this);
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