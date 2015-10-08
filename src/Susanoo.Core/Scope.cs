using System;
using System.Diagnostics;
using System.Security;
using System.Threading;

namespace Susanoo
{
    /// <summary>
    /// Example of Thread static scoping. May use this in the future for CommandBuilder batching.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal sealed class Scope<T> 
        : IDisposable where T: class
    {
        private bool _disposed;
        private readonly bool _ownsInstance;
        private readonly T _instance;
        private readonly Scope<T> _parent;

        [ThreadStatic]
        private static Scope<T> _head;

        /// <exception cref="SecurityException">The caller does not have the required permission. </exception>
        public Scope(T instance) : this(instance, true) { }

        /// <exception cref="SecurityException">The caller does not have the required permission. </exception>
        public Scope(T instance, bool ownsInstance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            _instance = instance;
            _ownsInstance = ownsInstance;

            Thread.BeginThreadAffinity();
            _parent = _head;
            _head = this;
        }

        public static T Current => _head?._instance;

        /// <exception cref="SecurityException">The caller does not have the required permission. </exception>
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                Debug.Assert(this == _head, "Disposed out of order.");
                _head = _parent;
                Thread.EndThreadAffinity();

                if (_ownsInstance)
                {
                    var disposable = _instance as IDisposable;
                    disposable?.Dispose();
                }
            }
        }
    }
}