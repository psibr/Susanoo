using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Numerics;
using Susanoo.Deserialization;
using Susanoo.Exceptions;
using Susanoo.ResultSets;

namespace Susanoo.Processing
{
    /// <summary>
    /// A fully built and ready to be executed CommandBuilder expression with appropriate mapping expressions compiled and a
    /// filter parameter.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <remarks>Appropriate mapping expressions are compiled at the point this interface becomes available.</remarks>
    public class MultipleResultSetCommandProcessor<TFilter> :
        CommandProcessorWithResults<TFilter>, IMultipleResultSetCommandProcessor<TFilter>
    {

        private readonly IDeserializer[] _mappers;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="MultipleResultSetCommandProcessor{TFilter}" />
        /// class.
        /// </summary>
        /// <param name="commandResultInfo">The CommandBuilder result information.</param>
        /// <param name="name">The name.</param>
        /// <param name="resultTypes">The result types.</param>
        public MultipleResultSetCommandProcessor(ICommandResultInfo<TFilter> commandResultInfo, string name, Type[] resultTypes)
            : base(commandResultInfo)
        {
            var resolver = CommandManager.Instance.Bootstrapper
                .ResolveDependency<IDeserializerResolver>();

            _mappers = resultTypes
                .Select(t => resolver.ResolveDeserializer(t, commandResultInfo.GetExporter()))
                .ToArray();

            CommandManager.Instance.RegisterCommandProcessor(this, name);
        }

        /// <summary>
        /// Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public override BigInteger CacheHash =>
            CommandResultInfo.CacheHash 
                + HashBuilder.Compute(_mappers.Aggregate(string.Empty, (s, deserializer) 
                    => s + deserializer.DeserializationType.AssemblyQualifiedName));

        /// <summary>
        /// Executes the CommandBuilder using a provided database manager and optionally a filter to read parameters from and explicit
        /// parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;&gt;.</returns>
        public IResultSetReader Execute(IDatabaseManager databaseManager,
            TFilter filter, object parameterObject, params DbParameter[] explicitParameters)
        {
            var results = new IEnumerable[_mappers.Length];

            var parameters = CommandBuilderInfo.BuildParameters(databaseManager, filter, parameterObject,
                explicitParameters);

            var cachedItemPresent = false;
            var hashCode = BigInteger.Zero;
            IResultSetReader finalResults = null;

            if (ResultCachingEnabled)
            {
                var parameterAggregate = parameters.Aggregate(string.Empty,
                    (p, c) => p + (c.ParameterName + (c.Value ?? string.Empty).ToString()));

                hashCode = HashBuilder.Compute(parameterAggregate);

                object value;
                TryRetrieveCacheResult(hashCode, out value);

                finalResults = value as IResultSetReader;

                cachedItemPresent = finalResults != null;
            }

            if (!cachedItemPresent)
            {
                try
                {
                    using (var record = databaseManager
                        .ExecuteDataReader(
                            CommandBuilderInfo.CommandText,
                            CommandBuilderInfo.DbCommandType,
                            Timeout,
                            parameters))
                    {
                        int ix = 0;

                        do
                        {
                            results[ix] = _mappers[ix].Deserialize(record, RetrieveColumnIndexInfo());
                            ix++;
                        } while (ix < results.Length && record.NextResult());
                    }
                }
                catch (Exception ex)
                {
                    throw new SusanooExecutionException(ex, CommandBuilderInfo, Timeout, parameters);
                }

                finalResults = new ResultSetReader(results);

                if (ResultCachingEnabled)
                    ResultCacheContainer.TryAdd(hashCode,
                        new CacheItem(finalResults, ResultCachingMode, ResultCachingInterval));
            }
            return finalResults;
        }

        /// <summary>
        /// Executes the CommandBuilder using a provided database manager and optionally parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;&gt;.</returns>
        public IResultSetReader Execute(IDatabaseManager databaseManager, params DbParameter[] explicitParameters)
        {
            return Execute(databaseManager, default(TFilter), explicitParameters);
        }

        /// <summary>
        /// Executes the CommandBuilder using a provided database manager and optionally parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;&gt;.</returns>
        public IResultSetReader Execute(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters)
        {
            return Execute(databaseManager, filter, null, explicitParameters);
        }

        /// <summary>
        /// Enables result caching.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="interval">The interval.</param>
        /// <returns>INoResultCommandProcessor&lt;TFilter, TResult1, TResult2&gt;.</returns>
        /// <exception cref="ArgumentException">
        ///     Calling EnableResultCaching with CacheMode None effectively would disable caching,
        ///     this is confusing and therefor is not allowed.
        /// </exception>
        public IMultipleResultSetCommandProcessor<TFilter>
            EnableResultCaching(CacheMode mode = CacheMode.Permanent, double? interval = null)
        {
            ActivateResultCaching(mode, interval);

            return this;
        }
    }
}