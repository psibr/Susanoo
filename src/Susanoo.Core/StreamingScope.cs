using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Security;
using System.Threading;

namespace Susanoo
{
    /// <summary>
    /// Thread static scope for handling out of context DbDataReaders and allowing commands to stream results safely.
    /// </summary>
    public sealed class StreamingScope
        : IDisposable
    {
        private readonly bool _requireThreadAffinity;
        private bool _disposed;
        private readonly Queue<WeakReference<IDataReader>> _instances;
        private readonly StreamingScope _parent;

        private static readonly ThreadLocal<StreamingScope> Head = new ThreadLocal<StreamingScope>();

        /// <summary>
        /// Begins a new streaming scope letting commands with datareaders know to stream results instead of buffer.
        /// </summary>
        /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
        public StreamingScope(bool requireThreadAffinity = true)
        {
            _requireThreadAffinity = requireThreadAffinity;

            if(_requireThreadAffinity)
                Thread.BeginThreadAffinity();

            _instances = new Queue<WeakReference<IDataReader>>();

            _parent = Head.Value;
            Head.Value = this;
        }

        /// <summary>
        /// Gets the current StreamingScope if one is available.
        /// </summary>
        /// <value>The current StreamingScope.</value>
        public static StreamingScope Current => Head.Value;

        /// <summary>
        /// Enlists the specified target in scoped disposal.
        /// </summary>
        /// <param name="target">The target IDisposable.</param>
        public void Enlist(IDataReader target)
        {
            _instances.Enqueue(new WeakReference<IDataReader>(target));
        }

        /// <summary>
        /// Disposes this instance and all enlisted disposables.
        /// </summary>
        /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                Head.Value = _parent;

                if(_requireThreadAffinity)
                    Thread.EndThreadAffinity();

                foreach (var weakReference in _instances)
                {
                    IDataReader disposable;
                    if (weakReference.TryGetTarget(out disposable))
                        disposable.Dispose();
                }
            }
        }
    }
}