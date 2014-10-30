using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Susanoo
{
    /// <summary>
    /// Common components between all CommandProcessors
    /// </summary>
    public abstract class CommandProcessorCommon
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandProcessorCommon"/> class.
        /// </summary>
        protected CommandProcessorCommon()
        {
            _resultCacheContainer = new ConcurrentDictionary<BigInteger, CacheItem>();

            CommandManager.RegisterCommandProcessor(this);
        }

        private bool _resultCachingEnabled = false;
        private CacheMode _resultCachingMode = CacheMode.None;
        private double _resultCachingInterval = 0d;

        private readonly ConcurrentDictionary<BigInteger, CacheItem> _resultCacheContainer;

        /// <summary>
        /// Gets the result cache container.
        /// </summary>
        /// <value>The result cache container.</value>
        protected ConcurrentDictionary<BigInteger, CacheItem> ResultCacheContainer {
            get { return _resultCacheContainer; }
        }

        /// <summary>
        /// Gets a value indicating whether [result caching enabled].
        /// </summary>
        /// <value><c>true</c> if [result caching enabled]; otherwise, <c>false</c>.</value>
        protected bool ResultCachingEnabled {
            get { return _resultCachingEnabled; }
        }

        /// <summary>
        /// Gets the result caching interval.
        /// </summary>
        /// <value>The result caching interval.</value>
        protected double ResultCachingInterval
        {
            get { return _resultCachingInterval; }
        }

        /// <summary>
        /// Gets the result caching mode.
        /// </summary>
        /// <value>The result caching mode.</value>
        protected CacheMode ResultCachingMode
        {
            get { return _resultCachingMode; }
        }

        /// <summary>
        /// Flushes the cache.
        /// </summary>
        public void FlushCache()
        {
            _resultCacheContainer.Clear();
        }

        /// <summary>
        /// Activates the result caching.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="interval">The interval.</param>
        /// <exception cref="System.ArgumentException">@Calling EnableResultCaching with CacheMode None effectively would disable caching,
        ///  this is confusing and therefor is not allowed.;mode</exception>
        protected void ActivateResultCaching(CacheMode mode = CacheMode.Permanent, double? interval = null)
        {
            if (mode == CacheMode.None)
                throw new ArgumentException(
                    @"Calling EnableResultCaching with CacheMode None effectively would disable caching, this is confusing and therefor is not allowed.",
                    "mode");

            _resultCachingEnabled = true;
            _resultCachingMode = mode;
            _resultCachingInterval = interval != null && mode != CacheMode.Permanent ? interval.Value : 0d;
        }

        /// <summary>
        /// Retrieves a cached result.
        /// </summary>
        /// <param name="hashCode">The hash code.</param>
        /// <param name="value">The value.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult&gt;.</returns>
        public bool TryRetrieveCacheResult(BigInteger hashCode, out object value)
        {
            CacheItem cache = null;
            bool result = false;
            if (_resultCacheContainer.TryGetValue(hashCode, out cache))
            {
                if (cache.CachingMode == CacheMode.Permanent
                    || cache.CachingMode == CacheMode.TimeSpan && cache.TimeStamp.AddSeconds(cache.Interval) <= DateTime.Now
                    || cache.CachingMode == CacheMode.RepeatedRequestLimit && cache.CallCount < cache.Interval)
                {
                    result = true;
                }
                else
                {
                    CacheItem trash;
                    _resultCacheContainer.TryRemove(hashCode, out trash);
                }
            }

            value = cache != null ? cache.Item : null;

            return result;
        }
    }
}
