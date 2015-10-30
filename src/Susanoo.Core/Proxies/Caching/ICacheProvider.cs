using System;

namespace Susanoo.Proxies.Caching
{
    /// <summary>
    /// A common interface implementation that is implemented by most cache providers
    /// </summary>
    public interface ICacheProvider
        : IDisposable
    {
        /// <summary>
        /// Removes the specified item from the cache.
        /// </summary>
        /// <param name="key">The identifier for the item to delete.</param>
        /// <returns>
        /// true if the item was successfully removed from the cache; false otherwise.
        /// </returns>
        bool RemoveValue(string key);

        /// <summary>
        /// Retrieves the specified item from the cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The identifier for the item to retrieve.</param>
        /// <returns>
        /// The retrieved item, or <value>null</value> if the key was not found.
        /// </returns>
        T GetValue<T>(string key) where T : class;

        /// <summary>
        /// Sets an item into the cache at the cache key specified regardless if it already exists or not.
        /// </summary>
        bool SetValue<T>(string key, T value) where T : class;

        /// <summary>
        /// Sets an item into the cache at the cache key specified regardless if it already exists or not.
        /// </summary>
        bool SetValue<T>(string key, T value, TimeSpan expiresIn) where T : class;

        /// <summary>
        /// Invalidates all data on the cache.
        /// </summary>
        void FlushAll();
    }
}