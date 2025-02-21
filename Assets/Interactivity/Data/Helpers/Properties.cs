using System;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public static partial class Helpers
    {
        public static Type GetSystemType(InteractivityType type)
        {
            return GetSystemTypeBySignature(type.signature);
        }

        public static Type GetSystemTypeBySignature(string signature)
        {
            switch (signature)
            {
                case "bool": return typeof(bool);
                case "int": return typeof(int);
                case "float": return typeof(float);
                case "float2": return typeof(Vector2);
                case "float3": return typeof(Vector3);
                case "float4": return typeof(Vector4);
                case "float4x4": return typeof(Matrix4x4);
                case "int[]": return typeof(int[]);
                default: return typeof(string);
            }
        }

        public static string GetSignatureBySystemType(Type type)
        {
            if (type == typeof(bool)) return "bool";
            if (type == typeof(int)) return "int";
            if (type == typeof(float)) return "float";
            if (type == typeof(Vector2)) return "float2";
            if (type == typeof(Vector3)) return "float3";
            if (type == typeof(Vector4)) return "float4";
            if (type == typeof(Matrix4x4)) return "float4x4";
            if (type == typeof(int[])) return "int[]";
            throw new InvalidOperationException($"Invalid type {type} used!");
        }

        public static bool TryCreateProperty(Type type, object value, out IProperty property)
        {
            try
            {
                property = CreateProperty(type, value);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                property = default;
                return false;
            }
        }

        public static IProperty CreateProperty(Type type, object value)
        {
            if (type == typeof(int))
            {
                return new Property<int>(Parser.ToInt(value));
            }
            else if (type == typeof(float))
            {
                return new Property<float>(Parser.ToFloat(value));
            }
            else if (type == typeof(bool))
            {
                return new Property<bool>(Parser.ToBool(value));
            }
            else if (type == typeof(Vector2))
            {
                return new Property<Vector2>(Parser.ToVector2(value));
            }
            else if (type == typeof(Vector3))
            {
                return new Property<Vector3>(Parser.ToVector3(value));
            }
            else if (type == typeof(Vector4))
            {
                return new Property<Vector4>(Parser.ToVector4(value));
            }
            else if (type == typeof(Matrix4x4))
            {
                return new Property<Matrix4x4>(Parser.ToMatrix4x4(value));
            }
            else if (type == typeof(int[]))
            {
                return new Property<int[]>(Parser.ToIntArray(value));
            }
            else if (type == typeof(string))
            {
                return new Property<string>(value.ToString());
            }

            throw new InvalidOperationException($"Type {type} is unsupported in this spec.");
        }

        public static T GetPropertyValue<T>(IProperty property)
        {
            if (property is not Property<T> typedProperty)
                throw new InvalidCastException($"Property is not of type {typeof(T)}");

            return typedProperty.value;
        }
    }
}