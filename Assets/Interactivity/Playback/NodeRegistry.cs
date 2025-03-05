using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public static class NodeRegistry
    {
        public static BehaviourEngineNode CreateBehaviourEngineNode(BehaviourEngine engine, Node node)
        {
            if (!nodeTypes.TryGetValue(node.type, out var creationMethod))
            {
                // Using Debug.Log here instead of Util.Log since this should be a message that shows up in prod.
                Debug.LogWarning($"No node found for operation {node.type}, creating a NoOp node.");
                return new NoOp(engine, node);
            }

            return creationMethod(engine, node);
        }

        private static readonly Dictionary<string, Func<BehaviourEngine, Node, BehaviourEngineNode>> nodeTypes = new()
        {
            ["animation/start"] = (engine, node) => new AnimationStart(engine, node),
            ["animation/stop"] = (engine, node) => new AnimationStop(engine, node),
            ["animation/stopAt"] = (engine, node) => new AnimationStopAt(engine, node),
            ["debug/log"] = (engine, node) => new DebugLog(engine, node),
            ["event/onStart"] = (engine, node) => new EventOnStart(engine, node),
            ["event/onSelect"] = (engine, node) => new EventOnSelect(engine, node),
            ["event/onTick"] = (engine, node) => new EventOnTick(engine, node),
            ["event/receive"] = (engine, node) => new EventReceive(engine, node),
            ["event/send"] = (engine, node) => new EventSend(engine, node),
            ["flow/branch"] = (engine, node) => new FlowBranch(engine, node),
            ["flow/for"] = (engine, node) => new FlowFor(engine, node),
            ["flow/sequence"] = (engine, node) => new FlowSequence(engine, node),
            ["flow/setDelay"] = (engine, node) => new FlowSetDelay(engine, node),
            ["flow/switch"] = (engine, node) => new FlowSwitch(engine, node),
            ["math/abs"] = (engine, node) => new MathAbs(engine, node),
            ["math/add"] = (engine, node) => new MathAdd(engine, node),
            ["math/and"] = (engine, node) => new MathAnd(engine, node),
            ["math/clamp"] = (engine, node) => new MathClamp(engine, node),
            ["math/combine2"] = (engine, node) => new MathCombine2(engine, node),
            ["math/combine3"] = (engine, node) => new MathCombine3(engine, node),
            ["math/combine4"] = (engine, node) => new MathCombine4(engine, node),
            ["math/cos"] = (engine, node) => new MathCos(engine, node),
            ["math/deg"] = (engine, node) => new MathDeg(engine, node),
            ["math/div"] = (engine, node) => new MathDiv(engine, node),
            ["math/e"] = (engine, node) => new MathE(engine, node),
            ["math/eq"] = (engine, node) => new MathEq(engine, node),
            ["math/extract2"] = (engine, node) => new MathExtract2(engine, node),
            ["math/extract3"] = (engine, node) => new MathExtract3(engine, node),
            ["math/extract4"] = (engine, node) => new MathExtract4(engine, node),
            ["math/floor"] = (engine, node) => new MathFloor(engine, node),
            ["math/ge"] = (engine, node) => new MathGe(engine, node),
            ["math/gt"] = (engine, node) => new MathGt(engine, node),
            ["math/isnan"] = (engine, node) => new MathIsNaN(engine, node),
            ["math/le"] = (engine, node) => new MathLe(engine, node),
            ["math/length"] = (engine, node) => new MathLength(engine, node),
            ["math/lt"] = (engine, node) => new MathLt(engine, node),
            ["math/mix"] = (engine, node) => new MathMix(engine, node),
            ["math/mul"] = (engine, node) => new MathMul(engine, node),
            ["math/neg"] = (engine, node) => new MathNeg(engine, node),
            ["math/not"] = (engine, node) => new MathNot(engine, node),
            ["math/pow"] = (engine, node) => new MathPow(engine, node),
            ["math/rad"] = (engine, node) => new MathRad(engine, node),
            ["math/random"] = (engine, node) => new MathRandom(engine, node),
            ["math/rem"] = (engine, node) => new MathRem(engine, node),
            ["math/saturate"] = (engine, node) => new MathSaturate(engine, node),
            ["math/select"] = (engine, node) => new MathSelect(engine, node),
            ["math/sin"] = (engine, node) => new MathSin(engine, node),
            ["math/sqrt"] = (engine, node) => new MathSqrt(engine, node),
            ["math/sub"] = (engine, node) => new MathSub(engine, node),
            ["math/tan"] = (engine, node) => new MathTan(engine, node),
            ["pointer/get"] = (engine, node) => new PointerGet(engine, node),
            ["pointer/interpolate"] = (engine, node) => new PointerInterpolate(engine, node),
            ["pointer/set"] = (engine, node) => new PointerSet(engine, node),
            ["type/boolToInt"] = (engine, node) => new TypeBoolToInt(engine, node),
            ["type/boolToFloat"] = (engine, node) => new TypeBoolToFloat(engine, node),
            ["type/floatToBool"] = (engine, node) => new TypeFloatToBool(engine, node),
            ["type/floatToInt"] = (engine, node) => new TypeFloatToInt(engine, node),
            ["type/intToBool"] = (engine, node) => new TypeIntToBool(engine, node),
            ["type/intToFloat"] = (engine, node) => new TypeIntToFloat(engine, node),
            ["variable/get"] = (engine, node) => new VariableGet(engine, node),
            ["variable/set"] = (engine, node) => new VariableSet(engine, node),
        };

        public static readonly Dictionary<string, NodeSpecifications> nodeSpecs = new()
        {
            ["event/onStart"] = new EventOnStartSpecs(),
            ["math/abs"] = new MathAbsSpec(),
            ["pointer/interpolate"] = new PointerInterpolateSpecs(),
            ["pointer/set"] = new PointerSetSpecs(),
        };
    }
}