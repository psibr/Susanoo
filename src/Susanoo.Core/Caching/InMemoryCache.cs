using System;
using System.Collections.Concurrent;
using Susanoo.Proxies.Caching;

namespace Susanoo.Caching
{
    /// <summary>
    /// A simple in-memory cache provider.
    /// </summary>
    public class InMemoryCache 
        : ICacheProvider
    {
        private readonly ConcurrentDictionary<string, CacheItem<object>> _cacheItems
            = new ConcurrentDictionary<string,CacheItem<object>>();

        private static readonly object SyncRoot = new object();

        private static InMemoryCache _instance;

        /// <summary>
        /// Gets the shared instance.
        /// </summary>
        /// <value>The instance.</value>
        public static InMemoryCache Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if(_instance == null)
                            _instance = new InMemoryCache();
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _cacheItems.Clear();
        }

        /// <summary>
        /// Removes the specified item from the cache.
        /// </summary>
        /// <param name="key">The identifier for the item to delete.</param>
        /// <returns>
        /// true if the item was successfully removed from the cache; false otherwise.
        /// </returns>
        public bool RemoveValue(string key)
        {
            CacheItem<object> item;
            return _cacheItems.TryRemove(key, out item);
        }

        /// <summary>
        /// Retrieves the specified item from the cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The identifier for the item to retrieve.</param>
        /// <returns>
        /// The retrieved item, or <value>null</value> if the key was not found.
        /// </returns>
        public T GetValue<T>(string key)
            where T : class
        {
            CacheItem<object> value;
            if (_cacheItems.TryGetValue(key, out value))
            {
                if (value.InvalidationDateTime == null || value.InvalidationDateTime > DateTime.Now)
                    return (T) value.Value;

                _cacheItems.TryRemove(key, out value);
            }

            return default(T);
        }

        /// <summary>
        /// Sets an item into the cache at the cache key specified regardless if it already exists or not.
        /// </summary>
        public bool SetValue<T>(string key, T value)
            where T : class
        {
            var cacheItem = new CacheItem<object> { Value = value };

            _cacheItems
                .AddOrUpdate(key, cacheItem,
                    (s, item) => cacheItem);

            return true;
        }

        /// <summary>
        /// Sets an item into the cache at the cache key specified regardless if it already exists or not.
        /// </summary>
        public bool SetValue<T>(string key, T value, TimeSpan expiresIn)
            where T : class 
        {
            var cacheItem = new CacheItem<object> { Value = value, InvalidationDateTime = DateTime.Now + expiresIn };

            _cacheItems
                .AddOrUpdate(key, cacheItem,
                    (s, item) => cacheItem);

            return true;
        }

        /// <summary>
        /// Invalidates all data on the cache.
        /// </summary>
        public void FlushAll()
        {
            _cacheItems.Clear();
        }

        class CacheItem<T>
        {
            public DateTime? InvalidationDateTime { get; set; }

            public T Value { get; set; }
        }
    }
}
