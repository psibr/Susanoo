#region

using System;
using System.Collections.Concurrent;
using System.Numerics;
using Susanoo.Command;
using Susanoo.ResultSets;

#endregion

namespace Susanoo.Processing
{
    /// <summary>
    ///     Common components between CommandProcessors with ResultSets
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    public abstract class CommandProcessorWithResults<TFilter> :
        ICommandProcessorWithResults<TFilter>,
        ICommandProcessor<TFilter>
    {
        /// <summary>
        /// Initializes pipeline components of the result processor
        /// </summary>
        /// <param name="commandResultInfo">The CommandBuilder result information.</param>
        protected CommandProcessorWithResults(ICommandResultInfo<TFilter> commandResultInfo)
            : this()
        {
            CommandBuilderInfo = commandResultInfo.GetCommandInfo();
            CommandResultInfo = commandResultInfo;
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="CommandProcessorWithResults{TFilter}"/> class from being created.
        /// </summary>
        private CommandProcessorWithResults() { ResultCacheContainer = new ConcurrentDictionary<BigInteger, CacheItem>(); }

        /// <summary>
        ///     Gets the result cache container.
        /// </summary>
        /// <value>The result cache container.</value>
        protected ConcurrentDictionary<BigInteger, CacheItem> ResultCacheContainer { get; }

        /// <summary>
        ///     Gets a value indicating whether [result caching enabled].
        /// </summary>
        /// <value><c>true</c> if [result caching enabled]; otherwise, <c>false</c>.</value>
        protected bool ResultCachingEnabled { get; private set; }

        /// <summary>
        ///     Gets the result caching interval.
        /// </summary>
        /// <value>The result caching interval.</value>
        protected double ResultCachingInterval { get; private set; }

        /// <summary>
        ///     Gets the result caching mode.
        /// </summary>
        /// <value>The result caching mode.</value>
        protected CacheMode ResultCachingMode { get; private set; } = CacheMode.None;

        /// <summary>
        ///     Gets the CommandBuilder expression.
        /// </summary>
        /// <value>The CommandBuilder expression.</value>
        public ICommandBuilderInfo<TFilter> CommandBuilderInfo { get; protected set; }

        /// <summary>
        /// Gets or sets the timeout of a command execution.
        /// </summary>
        /// <value>The timeout.</value>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>
        ///     Gets the mapping expressions.
        /// </summary>
        /// <value>The mapping expressions.</value>
        public ICommandResultInfo<TFilter> CommandResultInfo { get; protected set; }

        /// <summary>
        ///     Clears any column index information that may have been cached.
        /// </summary>
        public virtual void ClearColumnIndexInfo()
        {
        }

        /// <summary>
        ///     Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public virtual BigInteger CacheHash => 
            CommandResultInfo.CacheHash;

        /// <summary>
        ///     Flushes the cache.
        /// </summary>
        public void FlushCache()
        {
            ResultCacheContainer.Clear();
        }

        /// <summary>
        ///     Updates the column index information.
        /// </summary>
        /// <param name="info">The information.</param>
        public virtual void UpdateColumnIndexInfo(ColumnChecker info)
        {
        }

        /// <summary>
        ///     Retrieves a copy of the column index information.
        /// </summary>
        /// <returns>ColumnChecker.</returns>
        public virtual ColumnChecker RetrieveColumnIndexInfo()
        {
            return new ColumnChecker();
        }

        /// <summary>
        ///     Activates the result caching.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="interval">The interval.</param>
        /// <exception cref="System.ArgumentException">
        ///     @Calling EnableResultCaching with CacheMode None effectively would disable caching,
        ///     this is confusing and therefor is not allowed.;mode
        /// </exception>
        protected void ActivateResultCaching(CacheMode mode = CacheMode.Permanent, double? interval = null)
        {
            if (mode == CacheMode.None)
                throw new ArgumentException(
                    @"Calling EnableResultCaching with CacheMode.None effectively would disable caching, this is confusing and therefor is not allowed.",
                    nameof(mode));

            ResultCachingEnabled = true;
            ResultCachingMode = mode;
            ResultCachingInterval = interval != null && mode != CacheMode.Permanent ? interval.Value : 0d;
        }

        /// <summary>
        ///     Retrieves a cached result.
        /// </summary>
        /// <param name="hashCode">The hash code.</param>
        /// <param name="value">The value.</param>
        /// <returns>INoResultCommandProcessor&lt;TFilter, TResult&gt;.</returns>
        public bool TryRetrieveCacheResult(BigInteger hashCode, out object value)
        {
            CacheItem cache;
            var result = false;
            if (ResultCacheContainer.TryGetValue(hashCode, out cache))
            {
                if (cache.CachingMode == CacheMode.Permanent
                    ||
                    cache.CachingMode == CacheMode.TimeSpan && cache.TimeStamp.AddSeconds(cache.Interval) > DateTime.Now
                    || cache.CachingMode == CacheMode.RepeatedRequestLimit && cache.CallCount < cache.Interval)
                {
                    result = true;
                }
                else
                {
                    CacheItem trash;
                    ResultCacheContainer.TryRemove(hashCode, out trash);
                }
            }

            value = cache?.Item;

            return result;
        }
    }
}