using System;
using System.Threading;
using UnityEngine;

namespace UnityGLTF.Interactivity
{
    public interface ICancelToken
    {
        public bool isCancelled { get; }
    }

    public struct InterpolateCancelToken : ICancelToken
    {
        public CancellationToken globalToken;
        public CancellationToken localToken;

        public InterpolateCancelToken(CancellationToken globalToken, CancellationToken localToken)
        {
            this.globalToken = globalToken;
            this.localToken = localToken;
        }

        public bool isCancelled => globalToken.IsCancellationRequested || localToken.IsCancellationRequested;
    }

    public struct CancelToken : ICancelToken
    {
        public CancellationToken globalToken;

        public CancelToken(CancellationToken globalToken)
        {
            this.globalToken = globalToken;
        }

        public bool isCancelled => globalToken.IsCancellationRequested;

        public static implicit operator CancellationToken(CancelToken d) => d.globalToken;
        public static implicit operator CancelToken(CancellationToken d) => new CancelToken(d);
    }
}
