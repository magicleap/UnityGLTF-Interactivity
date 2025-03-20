using System;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Pool;
using UnityGLTF.Interactivity.Frontend;

namespace UnityGLTF.Interactivity
{
    public struct NodeDelayData
    {
        public int delayIndex { get; }
        public List<Flow> outputFlows;
        public double durationInMs;
        public double startTimeInMs;
        public double scheduledActivationTime;

        public NodeDelayData(int _lastDelayIndex, List<Flow> _outputFlows, double _duration, double _startTime)
        {
            delayIndex = _lastDelayIndex;
            outputFlows = _outputFlows;
            durationInMs = _duration * 1000;
            startTimeInMs = _startTime * 1000;
            scheduledActivationTime = startTimeInMs + durationInMs;
        }
    }

    public class NodeDelayManager
    {
        public List<NodeDelayData> delayedNodes = new();
        public List<NodeDelayData> delayNodeToRemove = new();
        public List<Flow> flowsToExecute { get; protected set; }
        //use this to identify flows canceled by cancelDelay
        public List<Flow> canceledFlows { get; protected set; }
        public int currentDelayIndex = 0;

        public void OnTick()
        {
            var temp = ListPool<NodeDelayData>.Get();
            var flows = ListPool<Flow>.Get();

            try
            {
                foreach (var nodeData in delayedNodes)
                {
                    if (!delayNodeToRemove.Contains(nodeData))
                        temp.Add(nodeData);
                }
            }
            finally
            {
                foreach (var delay in temp)
                {
                    if (Time.timeAsDouble >= delay.scheduledActivationTime)
                    {
                        foreach (var flow in delay.outputFlows)
                        {
                            if (!canceledFlows.Contains(flow))
                                flows.Add(flow);
                        }
                    }
                }

                delayedNodes = temp;
                flowsToExecute = flows;
                ListPool<Flow>.Release(flows);
                ListPool<NodeDelayData>.Release(temp);
            }
        }

        public void AddDelayNode(ref NodeDelayData data)
        {
            currentDelayIndex++;
            delayedNodes.Add(data);
        }

        public void CancelDelaysFromNode(int delayNodeIndex)
        {
            foreach (var delay in delayedNodes)
            {
                if (delay.delayIndex == delayNodeIndex)
                {
                    delayNodeToRemove.Add(delay);
                    Util.Log($"Removing nodes delayed by {delayNodeIndex} from the NodeDelayManager");
                    break;
                }
            }
        }

        public void CancelSingleDelay(int cancelIndex)
        {
            //TODO for use with CancelDelay nodes
        }
    }
}