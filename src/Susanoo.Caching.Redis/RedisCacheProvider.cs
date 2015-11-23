using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using Susanoo.Proxies.Caching;

namespace Susanoo.Caching.Redis
{
    /// <summary>
    /// Susanoo Cache provider for Redis using StackExchange.Redis and JSON.NET
    /// </summary>
    public class RedisCacheProvider
            : ICacheProvider
    {
        private readonly IDatabase _client;
        private readonly ConnectionMultiplexer _connection;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the 
        ///<see cref="RedisCacheProvider"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="database">The database.</param>
        public RedisCacheProvider(
            ConnectionMultiplexer connection, int database)
        {
            _client = connection.GetDatabase(database);
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="RedisCacheProvider"/> class.
        /// </summary>
        /// <param name="endpoints">The endpoints.</param>
        /// <param name="database">The database.</param>
        public RedisCacheProvider(
            IEnumerable<Tuple<string, int>> endpoints,
            int database)
        {
            var config = new ConfigurationOptions();

            var endPoints = config.EndPoints;

            foreach (var endpoint in endpoints)
            {
                endPoints.Add(endpoint.Item1, endpoint.Item2);
            }

            _connection = ConnectionMultiplexer.Connect(config);

            _client = _connection.GetDatabase(database);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing,
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing && !isDisposed)
            {
                _connection?.Dispose();

                isDisposed = true;

                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Removes the specified item from the cache.
        /// </summary>
        /// <param name="key">The identifier for the item to delete.</param>
        /// <returns>
        /// true if the item was successfully removed from the cache;
        /// false otherwise.
        /// </returns>
        public bool RemoveValue(string key)
        {
            return _client.KeyDelete(key);
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
            var json = _client.StringGet(key);

            var value =
                typeof(T).GenericTypeArguments.First() == typeof(object)
                    ? JsonConvert.DeserializeObject<IEnumerable<JObject>>(json)
                        ?.Select(o => o.ToObject<ExpandoObject>()) as T
                    : JsonConvert.DeserializeObject<T>(json);

            return value;
        }

        /// <summary>
        /// Sets an item into the cache at the cache key specified regardless
        /// if it already exists or not.
        /// </summary>
        public bool SetValue<T>(string key, T value)
            where T : class
        {
            var json = JsonConvert.SerializeObject(value);

            return _client.StringSet(key, json);
        }

        /// <summary>
        /// Sets an item into the cache at the cache key specified regardless
        /// if it already exists or not.
        /// </summary>
        public bool SetValue<T>(string key, T value, TimeSpan expiresIn)
            where T : class
        {
            var json = JsonConvert.SerializeObject(value);

            return _client.StringSet(key, json, expiresIn);
        }

        /// <summary>
        /// Invalidates all data on the cache.
        /// </summary>
        public void FlushAll()
        {
            _client.Multiplexer.GetEndPoints()
                .ToList()
                .ForEach(e =>
                    _client.Multiplexer
                        .GetServer(e)
                        .FlushDatabase(_client.Database));
        }
    }
}
