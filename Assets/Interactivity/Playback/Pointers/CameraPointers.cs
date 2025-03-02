using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public struct CameraPointers
    {
        public Pointer<float> orthographicXMag;
        public Pointer<float> orthographicYMag;

        public Pointer<float> perspectiveAspectRatio;
        public Pointer<float> perspectiveYFov;

        public Pointer<float> zFar;
        public Pointer<float> zNear;

        public CameraPointers(Camera cam)
        {
            // Unity does not allow you to set the width of the orthographic window directly.
            // cam.orthographicSize is the YMag and the width is then that value multiplied by your aspect ratio.
            orthographicXMag = new Pointer<float>()
            {
                setter = (v) => cam.orthographicSize = v / cam.aspect,
                getter = () => cam.orthographicSize * cam.aspect,
                evaluator = (a, b, t) => Mathf.Lerp(a, b, t)
            };

            orthographicYMag = new Pointer<float>()
            {
                setter = (v) => cam.orthographicSize = v,
                getter = () => cam.orthographicSize,
                evaluator = (a, b, t) => Mathf.Lerp(a, b, t)
            };

            perspectiveAspectRatio = new Pointer<float>()
            {
                setter = (v) => cam.aspect = v,
                getter = () => cam.aspect,
                evaluator = (a, b, t) => Mathf.Lerp(a, b, t)
            };

            perspectiveYFov = new Pointer<float>()
            {
                setter = (v) => cam.fieldOfView = v,
                getter = () => cam.fieldOfView,
                evaluator = (a, b, t) => Mathf.Lerp(a, b, t)
            };

            zNear = new Pointer<float>()
            {
                setter = (v) => cam.nearClipPlane = v,
                getter = () => cam.nearClipPlane,
                evaluator = (a, b, t) => Mathf.Lerp(a, b, t)
            };

            zFar = new Pointer<float>()
            {
                setter = (v) => cam.farClipPlane = v,
                getter = () => cam.farClipPlane,
                evaluator = (a, b, t) => Mathf.Lerp(a, b, t)
            };
        }

        public static IPointer ProcessCameraPointer(string[] path, List<CameraPointers> pointers)
        {
            var index = int.Parse(path[2]);
            var pointer = pointers[index];
            var property = path[3];

            switch (property)
            {
                case "orthographic":
                    return ProcessOrthographicPointer(path, pointer);

                case "perspective":
                    return ProcessPerspectivePointer(path, pointer);
            }

            throw new InvalidOperationException($"Property {property} is unsupported at this time!");
        }

        private static IPointer ProcessPerspectivePointer(string[] path, CameraPointers pointer)
        {
            var subProperty = path[4];

            switch (subProperty)
            {
                case "aspectRatio":
                    return pointer.perspectiveAspectRatio;

                case "yfov":
                    return pointer.perspectiveYFov;

                case "zfar":
                    return pointer.zFar;

                case "znear":
                    return pointer.zNear;
            }

            throw new InvalidOperationException($"No valid subproperty {subProperty} found for perspective camera.");
        }

        private static IPointer ProcessOrthographicPointer(string[] path, CameraPointers pointer)
        {
            var subProperty = path[4];

            switch (subProperty)
            {
                case "xmag":
                    return pointer.orthographicXMag;

                case "ymag":
                    return pointer.orthographicYMag;

                case "zfar":
                    return pointer.zFar;

                case "znear":
                    return pointer.zNear;
            }

            throw new InvalidOperationException($"No valid subproperty {subProperty} found for orthographic camera.");
        }
    }
}