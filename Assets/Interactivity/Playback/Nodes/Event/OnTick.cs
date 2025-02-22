using System;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class EventOnTick : BehaviourEngineNode
    {
        private float _startTime = -9999f;
        private float _lastTickTime = 0;

        public EventOnTick(BehaviourEngine engine, Node node) : base(engine, node)
        {
            engine.onStart += OnStart;
            engine.onTick += OnTick;
        }

        private void OnStart()
        {
            _startTime = Time.time;
        }

        private void OnTick()
        {
            _lastTickTime = Time.time;

            //Util.Log($"Time Since Start: {Time.time - _startTime}");

            TryExecuteFlow(ConstStrings.OUT);
        }

        public override IProperty GetOutputValue(string id)
        {
            return id switch
            {
                "timeSinceStart" => new Property<float>(Time.time - _startTime),
                "timeSinceLastTick" => new Property<float>(Time.time - _lastTickTime),
                _ => throw new InvalidOperationException($"No valid output with name {id}"),
            };
        }
    }
}