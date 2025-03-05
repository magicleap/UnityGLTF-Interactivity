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
            ["math/asr"] = (engine, node) => new MathAsr(engine, node),
            ["math/cbrt"] = (engine, node) => new MathCbrt(engine, node),
            ["math/clamp"] = (engine, node) => new MathClamp(engine, node),
            ["math/clz"] = (engine, node) => new MathClz(engine, node),
            ["math/ctz"] = (engine, node) => new MathCtz(engine, node),
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
            ["math/lsl"] = (engine, node) => new MathLsl(engine, node),
            ["math/lt"] = (engine, node) => new MathLt(engine, node),
            ["math/mix"] = (engine, node) => new MathMix(engine, node),
            ["math/mul"] = (engine, node) => new MathMul(engine, node),
            ["math/neg"] = (engine, node) => new MathNeg(engine, node),
            ["math/not"] = (engine, node) => new MathNot(engine, node),
            ["math/popcnt"] = (engine, node) => new MathPopcnt(engine, node),
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
            ["math/acos"] = (engine, node) => new MathACos(engine, node),
            ["math/acosh"] = (engine, node) => new MathACosH(engine, node),
            ["math/asin"] = (engine, node) => new MathASin(engine, node),
            ["math/asinh"] = (engine, node) => new MathASinH(engine, node),
            ["math/atan"] = (engine, node) => new MathATan(engine, node),
            ["math/atan2"] = (engine, node) => new MathATan2(engine, node),
            ["math/atanh"] = (engine, node) => new MathATanH(engine, node),
            ["math/ceil"] = (engine, node) => new MathCeil(engine, node),
            ["math/cosh"] = (engine, node) => new MathCosH(engine, node),
            ["math/cross"] = (engine, node) => new MathCross(engine, node),
            ["math/determinant"] = (engine, node) => new MathDeterminant(engine, node),
            ["math/dot"] = (engine, node) => new MathDot(engine, node),
            ["math/exp"] = (engine, node) => new MathExp(engine, node),
            ["math/fract"] = (engine, node) => new MathFract(engine, node),
            ["math/inf"] = (engine, node) => new MathInf(engine, node),
            ["math/inverse"] = (engine, node) => new MathInverse(engine, node),
            ["math/isinf"] = (engine, node) => new MathIsInf(engine, node),
            ["math/log"] = (engine, node) => new MathLog(engine, node),
            ["math/log10"] = (engine, node) => new MathLog10(engine, node),
            ["math/log2"] = (engine, node) => new MathLog2(engine, node),
            ["math/matcompose"] = (engine, node) => new MathMatCompose(engine, node),
            ["math/matdecompose"] = (engine, node) => new MathMatDecompose(engine, node),
            ["math/matmul"] = (engine, node) => new MathMatMul(engine, node),
            ["math/min"] = (engine, node) => new MathMin(engine, node),
            ["math/max"] = (engine, node) => new MathMax(engine, node),
            ["math/nan"] = (engine, node) => new MathNaN(engine, node),
            ["math/normalize"] = (engine, node) => new MathNormalize(engine, node),
            ["math/or"] = (engine, node) => new MathOr(engine, node),
            ["math/pi"] = (engine, node) => new MathPi(engine, node),
            ["math/rotate2d"] = (engine, node) => new MathRotate2D(engine, node),
            ["math/rotate3d"] = (engine, node) => new MathRotate3D(engine, node),
            ["math/round"] = (engine, node) => new MathRound(engine, node),
            ["math/sign"] = (engine, node) => new MathSign(engine, node),
            ["math/sinh"] = (engine, node) => new MathSinH(engine, node),
            ["math/tanh"] = (engine, node) => new MathTanH(engine, node),
            ["math/transform"] = (engine, node) => new MathTransform(engine, node),
            ["math/transpose"] = (engine, node) => new MathTranspose(engine, node),
            ["math/trunc"] = (engine, node) => new MathTrunc(engine, node),
            ["math/xor"] = (engine, node) => new MathXor(engine, node),

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