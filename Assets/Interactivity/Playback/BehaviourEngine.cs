using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using static UnityGLTF.Interactivity.InteractivityManager;

namespace UnityGLTF.Interactivity
{
    public class BehaviourEngine
    {
        public readonly List<GraphData> graphData;
        public readonly Dictionary<Node, BehaviourEngineNode> engineNodes = new();
        public AnimationWrapper animationWrapper { get; private set; }
        public AudioWrapper audioWrapper { get; private set; }
        public readonly PointerInterpolationManager pointerInterpolationManager = new();
        public readonly VariableInterpolationManager variableInterpolationManager = new();

        public event Action onStart;

        public event Action<RayArgs> onSelect;
        public event Action<RayArgs> onHoverIn;
        public event Action<RayArgs> onHoverOut;
        public event Action onTick;
        public event Action<int, Dictionary<string, IProperty>> onCustomEventFired;
        public event Action<Flow> onFlowTriggered;

        public readonly PointerResolver pointerResolver;

        public BehaviourEngine(List<GraphData> graphData, GLTFSceneImporter importer)
        {
            this.graphData = graphData;
            pointerResolver = new PointerResolver(importer);

            for (int j = 0; j < graphData.Count; j++)
            {
                if (graphData[j].extension is KHR_ExtensionGraph extensionGraph)
                {
                    Graph graph = graphData[j].graph;
                    if (extensionGraph.type == KHR_ExtensionGraph.GraphType.Interactivity)
                    {
                        for (int i = 0; i < graph.nodes.Count; i++)
                        {
                            engineNodes.Add(graph.nodes[i], NodeRegistry.CreateBehaviourEngineNode(this, graph.nodes[i]));
                        }
                    }
                    // has other graphs such as an audio emitter extension.
                    else if (extensionGraph.type == KHR_ExtensionGraph.GraphType.Audio)
                    {
                        // do audio specific stuff here
                    }
                }
            }
        }

        public void StartPlayback()
        {
            onStart?.Invoke();
        }

        public void Tick()
        {
            pointerInterpolationManager.OnTick();
            variableInterpolationManager.OnTick();
            onTick?.Invoke();
        }

        public void Select(in RayArgs args)
        {
            onSelect?.Invoke(args);
        }

        public void HoverIn(in RayArgs args)
        {
            onHoverIn?.Invoke(args);
        }

        public void HoverOut(in RayArgs args)
        {
            onHoverOut?.Invoke(args);
        }

        public void ExecuteFlow(Flow flow)
        {
            Assert.IsNotNull(flow.toNode);

            var node = engineNodes[flow.toNode];

            onFlowTriggered?.Invoke(flow);

            node.ValidateAndExecute(flow.toSocket);
        }

        public void FireCustomEvent(int eventIndex, Dictionary<string, IProperty> outValues = null)
        {
            if (eventIndex < 0 || eventIndex >= graphData[0].graph.customEvents.Count)
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
            return graphData[0].graph.variables[variableIndex].property;
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

        public void SetAnimationWrapper(AnimationWrapper wrapper, Animation animation)
        {
            animationWrapper = wrapper;
            wrapper.SetData(this, animation);
            pointerResolver.RegisterAnimations(wrapper);
        }

        public void SetAudioWrapper (AudioWrapper wrapper)
        {
            audioWrapper =wrapper;
            wrapper.SetData(this);
        }
        public void PlayAudio(AudioPlayData data)
        {
            if (!HasAudioWrapper())
                return;

            audioWrapper.PlayAudio(data.index);
        }

        public void StopAudio(AudioPlayData data)
        {
            if (!HasAudioWrapper())
                return;

            audioWrapper.StopAudio(data.index);
        }

        public void PauseAudio(AudioPlayData data)
        {
            if (!HasAudioWrapper())
                return;

            audioWrapper.PauseAudio(data.index);
        }

        public void UnPauseAudio(AudioPlayData data)
        {
            if (!HasAudioWrapper())
                return;

            audioWrapper.UnPauseAudio(data.index);
        }

        public void PlayAnimation(in AnimationPlayData data)
        {
            if (!HasAnimationWrapper())
                return;

            animationWrapper.PlayAnimation(data);
        }

        public void StopAnimation(int index)
        {
            if (!HasAnimationWrapper())
                return;

            animationWrapper.StopAnimation(index);
        }

        public void StopAnimationAt(int index, float stopTime, Action callback)
        {
            if (!HasAnimationWrapper())
                return;

            animationWrapper.StopAnimationAt(index, stopTime, callback);
        }

        public bool HasAnimationWrapper()
        {
            if (animationWrapper == null)
            {
                Util.LogWarning("Tried to play an animation on a glb that has no animations.");
                return false;
            }

            return true;
        }

        public bool HasAudioWrapper()
        {
            if (audioWrapper == null)
            {
                Util.LogWarning("Tried to play an audio on a glb that has no audio.");
                return false;
            }

            return true;
        }
    }
}