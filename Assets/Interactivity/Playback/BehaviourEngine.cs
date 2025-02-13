using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class BehaviourEngine
    {
        public readonly Graph graph;
        public readonly Dictionary<Node, BehaviourEngineNode> engineNodes = new();
        public readonly AnimationWrapper animationWrapper;

        public event Action onStart;
        public event Action<int> onSelect;
        public event Action onTick;
        public event Action<int, Dictionary<string, IProperty>> onCustomEventFired;

        public readonly PointerResolver pointerResolver;

        private Queue<Flow> _flowsToExecute = new();
        private LinkedList<BehaviourEngineNode> _nodesRunning = new();

        private bool _paused = false;

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

        public void StopPlayback()
        {
            Cancel();
        }

        public void Tick()
        {
            if(_paused)
            {
                return;
            }

            while(_flowsToExecute.Count > 0)
            {
                var f = _flowsToExecute.Dequeue();
                ExecuteFlow(f);
            }
            onTick?.Invoke();
        }

        public void Pause()
        {
            if(_paused)
            {
                return;
            }
            _paused = true;

            Debug.LogWarning("ENGINE PAUSED");

            foreach(var n in _nodesRunning)
            {
                n.Pause();
            }
        }

        public bool IsPaused()
        {
            return _paused;
        }

        public void Resume()
        {
            if(_paused == false)
            {
                return;
            }
            _paused = false;

            Debug.LogWarning("ENGINE RESUMED");

            foreach(var n in _nodesRunning)
            {
                n.Resume();
            }
        }

        public void Cancel()
        {
            Debug.LogWarning($"ENGINE CANCELED ({_nodesRunning.Count} objects running)");
            foreach(var n in _nodesRunning)
            {
                n.Cancel();
            }
        }

        public void Select(GameObject go)
        {
            var nodeIndex = pointerResolver.IndexOf(go);
            onSelect?.Invoke(nodeIndex);
        }

        private BehaviourEngineNode CreateBehaviourEngineNode(Node node)
        {
            var instantiateFunc = NodeRegistry.nodeTypes[node.type];

            return instantiateFunc(this, node);
        }

        private void ExecuteFlow(Flow flow)
        {
            if (!Application.isPlaying)
                return;

            if (flow.toNode == null)
                return;

            var node = engineNodes[flow.toNode];
            node.ValidateAndExecute(flow.toSocket);
        }

        public void PushFlowForExecution(Flow flow)
        {
            _flowsToExecute.Enqueue(flow);
        }

        public void FireCustomEvent(int eventIndex, Dictionary<string, IProperty> outValues = null)
        {
            if (eventIndex < 0 || eventIndex >= graph.customEvents.Count)
                return; // TODO: Add error handling.

            onCustomEventFired?.Invoke(eventIndex, outValues);
        }

        public IProperty ParseValue(Value v)
        {
            if (v.node != null)
            {
                var node = engineNodes[v.node];
                return node.GetOutputValue(v.socket);
            }

            return v.property;
        }

        public T ParseValueProperty<T>(Value v)
        {
            var property = ParseValue(v);

            return ((Property<T>)property).value;
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

        public async Task PlayAnimationAsync(int animationIndex, float startTime, float endTime, float speed)
        {
            if (animationWrapper == null)
            {
                Debug.LogWarning("Tried to play an animation on a glb that has no animations.");
                return;
            }

            await animationWrapper.PlayAnimationAsync(animationIndex, startTime, endTime, speed);
        }

        public void StartLongOperation(BehaviourEngineNode n)
        {
            _nodesRunning.AddLast(n);
        }

        public void EndLongOperation(BehaviourEngineNode n)
        {
            _nodesRunning.Remove(n);
        }
    }
}