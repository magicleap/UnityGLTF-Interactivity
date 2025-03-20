using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityGLTF.Interactivity;

namespace UnityGLTF.Interactivity.Tests
{
    public interface IAssertionByType
    {
        public void AreApproximatelyEqual(IProperty expectedProp, IProperty actualProp);
        public void AreEqual(IProperty expectedProp, IProperty actualProp);
        public void AreApproximatelyEqual(object expected, object actual);
        public void AreEqual(object expected, object actual);
    }

    public abstract class AssertionMethod<T> : IAssertionByType
    {
        protected T expected;
        protected T actual;

        public void SetData(object expected, object actual)
        {
            this.expected = (T)expected;
            this.actual = (T)actual;
        }

        public void AreApproximatelyEqual(IProperty expectedProp, IProperty actualProp)
        {
            var expected = ((Property<T>)expectedProp).value;
            var actual = ((Property<T>)actualProp).value;

            SetData(expected, actual);
            AreApproximatelyEqual();
        }

        public void AreEqual(IProperty expectedProp, IProperty actualProp)
        {
            var expected = ((Property<T>)expectedProp).value;
            var actual = ((Property<T>)actualProp).value;

            SetData(expected, actual);
            AreEqual();
        }

        public void AreApproximatelyEqual(object expected, object actual)
        {
            SetData(expected, actual);
            AreApproximatelyEqual();
        }

        public void AreEqual(object expected, object actual)
        {
            SetData(expected, actual);
            AreEqual();
        }

        protected abstract void AreApproximatelyEqual();
        protected abstract void AreEqual();

        protected static void ApproximatelyEqualNaNOrInfinity(float a, float b)
        {
            if (NaNOrInfinite(a, b))
                return;

            UnityEngine.Assertions.Assert.AreApproximatelyEqual(a, b);
        }

        protected static void EqualNaNOrInfinity(float a, float b)
        {
            if (NaNOrInfinite(a, b))
                return;

            UnityEngine.Assertions.Assert.AreEqual(a, b);
        }

        private static bool NaNOrInfinite(float a, float b)
        {
            if (float.IsInfinity(a) && float.IsInfinity(b))
                return true;

            if (float.IsNaN(a) && float.IsNaN(b))
                return true;

            return false;
        }
    }

    public class IntAssertion : AssertionMethod<int>
    {
        protected override void AreApproximatelyEqual()
        {
            UnityEngine.Assertions.Assert.AreApproximatelyEqual(expected, actual);
        }

        protected override void AreEqual()
        {
            UnityEngine.Assertions.Assert.AreEqual(expected, actual);
        }
    }

    public class IntArrayAssertion : AssertionMethod<int[]>
    {
        protected override void AreApproximatelyEqual()
        {
            UnityEngine.Assertions.Assert.AreEqual(expected.Length, actual.Length);

            for (int i = 0; i < expected.Length; i++)
            {
                UnityEngine.Assertions.Assert.AreApproximatelyEqual(expected[i], actual[i]);
            }
        }

        protected override void AreEqual()
        {
            UnityEngine.Assertions.Assert.AreEqual(expected.Length, actual.Length);

            for (int i = 0; i < expected.Length; i++)
            {
                UnityEngine.Assertions.Assert.AreEqual(expected[i], actual[i]);
            }
        }
    }


    public class FloatAssertion : AssertionMethod<float>
    {
        protected override void AreApproximatelyEqual()
        {
            ApproximatelyEqualNaNOrInfinity(expected, actual);
        }

        protected override void AreEqual()
        {
            EqualNaNOrInfinity(expected, actual);
        }
    }

    public class Float2Assertion : AssertionMethod<float2>
    {
        protected override void AreApproximatelyEqual()
        {
           ApproximatelyEqualNaNOrInfinity(expected.x, actual.x);
           ApproximatelyEqualNaNOrInfinity(expected.y, actual.y);
        }

        protected override void AreEqual()
        {
            EqualNaNOrInfinity(expected.x, actual.x);
            EqualNaNOrInfinity(expected.y, actual.y);
        }
    }

    public class Float3Assertion : AssertionMethod<float3>
    {
        protected override void AreApproximatelyEqual()
        {
           ApproximatelyEqualNaNOrInfinity(expected.x, actual.x);
           ApproximatelyEqualNaNOrInfinity(expected.y, actual.y);
           ApproximatelyEqualNaNOrInfinity(expected.z, actual.z);
        }

        protected override void AreEqual()
        {
            EqualNaNOrInfinity(expected.x, actual.x);
            EqualNaNOrInfinity(expected.y, actual.y);
            EqualNaNOrInfinity(expected.z, actual.z);
        }
    }

    public class Float4Assertion : AssertionMethod<float4>
    {
        protected override void AreApproximatelyEqual()
        {
           ApproximatelyEqualNaNOrInfinity(expected.x, actual.x);
           ApproximatelyEqualNaNOrInfinity(expected.y, actual.y);
           ApproximatelyEqualNaNOrInfinity(expected.z, actual.z);
           ApproximatelyEqualNaNOrInfinity(expected.w, actual.w);
        }

        protected override void AreEqual()
        {
            EqualNaNOrInfinity(expected.x, actual.x);
            EqualNaNOrInfinity(expected.y, actual.y);
            EqualNaNOrInfinity(expected.z, actual.z);
            EqualNaNOrInfinity(expected.w, actual.w);
        }
    }

    public class Float2x2Assertion : AssertionMethod<float2x2>
    {
        protected override void AreApproximatelyEqual()
        {
           ApproximatelyEqualNaNOrInfinity(expected.c0.x, actual.c0.x);
           ApproximatelyEqualNaNOrInfinity(expected.c0.y, actual.c0.y);
           ApproximatelyEqualNaNOrInfinity(expected.c1.x, actual.c1.x);
           ApproximatelyEqualNaNOrInfinity(expected.c1.y, actual.c1.y);

        }

        protected override void AreEqual()
        {
            EqualNaNOrInfinity(expected.c0.x, actual.c0.x);
            EqualNaNOrInfinity(expected.c0.y, actual.c0.y);
            EqualNaNOrInfinity(expected.c1.x, actual.c1.x);
            EqualNaNOrInfinity(expected.c1.y, actual.c1.y);
        }
    }

    public class Float3x3Assertion : AssertionMethod<float3x3>
    {
        protected override void AreApproximatelyEqual()
        {
           ApproximatelyEqualNaNOrInfinity(expected.c0.x, actual.c0.x);
           ApproximatelyEqualNaNOrInfinity(expected.c0.y, actual.c0.y);
           ApproximatelyEqualNaNOrInfinity(expected.c0.z, actual.c0.z);

           ApproximatelyEqualNaNOrInfinity(expected.c1.x, actual.c1.x);
           ApproximatelyEqualNaNOrInfinity(expected.c1.y, actual.c1.y);
           ApproximatelyEqualNaNOrInfinity(expected.c1.z, actual.c1.z);

           ApproximatelyEqualNaNOrInfinity(expected.c2.x, actual.c2.x);
           ApproximatelyEqualNaNOrInfinity(expected.c2.y, actual.c2.y);
           ApproximatelyEqualNaNOrInfinity(expected.c2.z, actual.c2.z);
        }

        protected override void AreEqual()
        {
            EqualNaNOrInfinity(expected.c0.x, actual.c0.x);
            EqualNaNOrInfinity(expected.c0.y, actual.c0.y);
            EqualNaNOrInfinity(expected.c0.z, actual.c0.z);

            EqualNaNOrInfinity(expected.c1.x, actual.c1.x);
            EqualNaNOrInfinity(expected.c1.y, actual.c1.y);
            EqualNaNOrInfinity(expected.c1.z, actual.c1.z);

            EqualNaNOrInfinity(expected.c2.x, actual.c2.x);
            EqualNaNOrInfinity(expected.c2.y, actual.c2.y);
            EqualNaNOrInfinity(expected.c2.z, actual.c2.z);
        }
    }

    public class Float4x4Assertion : AssertionMethod<float4x4>
    {
        protected override void AreApproximatelyEqual()
        {
           ApproximatelyEqualNaNOrInfinity(expected.c0.x, actual.c0.x);
           ApproximatelyEqualNaNOrInfinity(expected.c0.y, actual.c0.y);
           ApproximatelyEqualNaNOrInfinity(expected.c0.z, actual.c0.z);
           ApproximatelyEqualNaNOrInfinity(expected.c0.w, actual.c0.w);

           ApproximatelyEqualNaNOrInfinity(expected.c1.x, actual.c1.x);
           ApproximatelyEqualNaNOrInfinity(expected.c1.y, actual.c1.y);
           ApproximatelyEqualNaNOrInfinity(expected.c1.z, actual.c1.z);
           ApproximatelyEqualNaNOrInfinity(expected.c1.w, actual.c1.w);

           ApproximatelyEqualNaNOrInfinity(expected.c2.x, actual.c2.x);
           ApproximatelyEqualNaNOrInfinity(expected.c2.y, actual.c2.y);
           ApproximatelyEqualNaNOrInfinity(expected.c2.z, actual.c2.z);
           ApproximatelyEqualNaNOrInfinity(expected.c2.w, actual.c2.w);

           ApproximatelyEqualNaNOrInfinity(expected.c3.x, actual.c3.x);
           ApproximatelyEqualNaNOrInfinity(expected.c3.y, actual.c3.y);
           ApproximatelyEqualNaNOrInfinity(expected.c3.z, actual.c3.z);
           ApproximatelyEqualNaNOrInfinity(expected.c3.w, actual.c3.w);
        }

        protected override void AreEqual()
        {
            EqualNaNOrInfinity(expected.c0.x, actual.c0.x);
            EqualNaNOrInfinity(expected.c0.y, actual.c0.y);
            EqualNaNOrInfinity(expected.c0.z, actual.c0.z);
            EqualNaNOrInfinity(expected.c0.w, actual.c0.w);

            EqualNaNOrInfinity(expected.c1.x, actual.c1.x);
            EqualNaNOrInfinity(expected.c1.y, actual.c1.y);
            EqualNaNOrInfinity(expected.c1.z, actual.c1.z);
            EqualNaNOrInfinity(expected.c1.w, actual.c1.w);

            EqualNaNOrInfinity(expected.c2.x, actual.c2.x);
            EqualNaNOrInfinity(expected.c2.y, actual.c2.y);
            EqualNaNOrInfinity(expected.c2.z, actual.c2.z);
            EqualNaNOrInfinity(expected.c2.w, actual.c2.w);

            EqualNaNOrInfinity(expected.c3.x, actual.c3.x);
            EqualNaNOrInfinity(expected.c3.y, actual.c3.y);
            EqualNaNOrInfinity(expected.c3.z, actual.c3.z);
            EqualNaNOrInfinity(expected.c3.w, actual.c3.w);
        }
    }


    public class BoolAssertion : AssertionMethod<bool>
    {
        protected override void AreApproximatelyEqual()
        {
            UnityEngine.Assertions.Assert.AreEqual(expected, actual);
        }

        protected override void AreEqual()
        {
            UnityEngine.Assertions.Assert.AreEqual(expected, actual);
        }
    }

    public class StringAssertion : AssertionMethod<string>
    {
        protected override void AreApproximatelyEqual()
        {
            UnityEngine.Assertions.Assert.AreEqual(expected, actual);
        }

        protected override void AreEqual()
        {
            UnityEngine.Assertions.Assert.AreEqual(expected, actual);
        }
    }
}