using System;
using System.Collections.Generic;

namespace UnityGLTF.Interactivity
{
    public static class NodeRegistry
    {
        public static readonly Dictionary<string, Func<BehaviourEngine, Node, BehaviourEngineNode>> nodeTypes = new()
        {
            ["animation/start"] = (engine, node) => new AnimationStart(engine, node),
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
            ["math/abs"] = (engine, node) => new MathAbs(engine, node),
            ["math/add"] = (engine, node) => new MathAdd(engine, node),
            ["math/combine3"] = (engine, node) => new MathCombine3(engine, node),
            ["math/combine4"] = (engine, node) => new MathCombine4(engine, node),
            ["math/cos"] = (engine, node) => new MathCos(engine, node),
            ["math/deg"] = (engine, node) => new MathDeg(engine, node),
            ["math/div"] = (engine, node) => new MathDiv(engine, node),
            ["math/e"] = (engine, node) => new MathE(engine, node),
            ["math/eq"] = (engine, node) => new MathEq(engine, node),
            ["math/extract3"] = (engine, node) => new MathExtract3(engine, node),
            ["math/gt"] = (engine, node) => new MathGt(engine, node),
            ["math/isnan"] = (engine, node) => new MathIsNaN(engine, node),
            ["math/mul"] = (engine, node) => new MathMul(engine, node),
            ["math/neg"] = (engine, node) => new MathNeg(engine, node),
            ["math/pow"] = (engine, node) => new MathPow(engine, node),
            ["math/rad"] = (engine, node) => new MathRad(engine, node),
            ["math/select"] = (engine, node) => new MathSelect(engine, node),
            ["math/sin"] = (engine, node) => new MathSin(engine, node),
            ["math/sqrt"] = (engine, node) => new MathSqrt(engine, node),
            ["math/sub"] = (engine, node) => new MathSub(engine, node),
            ["math/tan"] = (engine, node) => new MathTan(engine, node),
            ["pointer/get"] = (engine, node) => new PointerGet(engine, node),
            ["pointer/interpolate"] = (engine, node) => new PointerInterpolate(engine, node),
            ["pointer/set"] = (engine, node) => new PointerSet(engine, node),
            ["type/intToFloat"] = (engine, node) => new TypeIntToFloat(engine, node),
            ["variable/get"] = (engine, node) => new VariableGet(engine, node),
            ["variable/set"] = (engine, node) => new VariableSet(engine, node),
        };

        public static readonly Dictionary<string, NodeSpecifications> nodeSpecs = new()
        {
            ["event/onStart"] = new EventOnStartSpecs(),
            ["math/abs"] = new MathAbsSpec(),
            ["pointer/interpolate"] = new PointerInterpolateSpecs(),
        };
    }
}