using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public class GLTFBinaryData
    {
        public const uint JSON_CHUNK = 0x4E4F534A;
        public const uint BIN_CHUNK = 0x004E4942;

        public struct Header
        {
            public uint magic;
            public uint version;
            public uint length;
        }

        public struct Chunk
        {
            public uint chunkLength;
            public uint chunkType;
            public byte[] data;
        }
    }
}
