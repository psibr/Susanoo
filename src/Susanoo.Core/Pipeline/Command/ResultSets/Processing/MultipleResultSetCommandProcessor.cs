using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Numerics;

namespace Susanoo.Pipeline.Command.ResultSets.Processing
{
    /// <summary>
    /// A fully built and ready to be executed command expression with appropriate mapping expressions compiled and a
    /// filter parameter.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the result1.</typeparam>
    /// <typeparam name="TResult2">The type of the result2.</typeparam>
    /// <remarks>Appropriate mapping expressions are compiled at the point this interface becomes available.</remarks>
    public class MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2> :
        CommandProcessorWithResults<TFilter>,
        ICommandProcessor<TFilter, TResult1, TResult2>
    {
        private const int ResultCount = 2;

        private readonly IResultMapper[] _mappers;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="MultipleResultSetCommandProcessor{TFilter, TResult1, TResult2}" />
        /// class.
        /// </summary>
        /// <param name="commandResultInfo">The command result information.</param>
        /// <param name="name">The name.</param>
        public MultipleResultSetCommandProcessor(ICommandResultInfo<TFilter> commandResultInfo, string name)
            : base(commandResultInfo)
        {

            _mappers = new IResultMapper[ResultCount];

            _mappers[0] = CommandResultInfo.GetProcessor<TFilter, TResult1>();
            _mappers[1] = CommandResultInfo.GetProcessor<TFilter, TResult2>();

            CommandManager.RegisterCommandProcessor(this, name);
        }

        /// <summary>
        /// Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public override BigInteger CacheHash
        {
            get { return CommandResultInfo.CacheHash; }
        }

        /// <summary>
        /// Executes the command using a provided database manager and optionally a filter to read parameters from and explicit
        /// parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;&gt;.</returns>
        public Tuple<IEnumerable<TResult1>,
            IEnumerable<TResult2>>
            Execute(IDatabaseManager databaseManager, TFilter filter, object parameterObject, params DbParameter[] explicitParameters)
        {
            var results = new object[ResultCount];

            var parameters = CommandInfo.BuildParameters(databaseManager, filter, parameterObject, explicitParameters);

            var cachedItemPresent = false;
            var hashCode = BigInteger.Zero;
            object finalResults = null;

            if (ResultCachingEnabled)
            {
                var parameterAggregate = parameters.Aggregate(string.Empty,
                    (p, c) => p + (c.ParameterName + (c.Value ?? string.Empty).ToString()));

                hashCode = HashBuilder.Compute(parameterAggregate);

                object value;
                TryRetrieveCacheResult(hashCode, out value);

                finalResults = value;

                cachedItemPresent = finalResults != null;
            }

            if (!cachedItemPresent)
            {
                try
                {
                    using (var record = databaseManager
                        .ExecuteDataReader(
                            CommandInfo.CommandText,
                            CommandInfo.DbCommandType,
                            parameters))
                    {
                        for (var i = 0; i < ResultCount; i++)
                        {
                            if (i > 0)
                                if (!record.NextResult())
                                    break;

                            results[i] = _mappers[i].MapResult(record);
                        }
                    }
                }
                catch (Exception ex)
                {
                    CommandManager.HandleExecutionException(CommandInfo, ex, parameters);
                }

                finalResults = new Tuple<IEnumerable<TResult1>,
                    IEnumerable<TResult2>>(
                    results[0] as IEnumerable<TResult1>,
                    results[1] as IEnumerable<TResult2>);

                if (ResultCachingEnabled)
                    ResultCacheContainer.TryAdd(hashCode,
                        new CacheItem(finalResults, ResultCachingMode, ResultCachingInterval));
            }
            return finalResults as Tuple<IEnumerable<TResult1>,
                IEnumerable<TResult2>>;
        }

        /// <summary>
        /// Executes the command using a provided database manager and optionally parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;&gt;.</returns>
        public Tuple<IEnumerable<TResult1>,
            IEnumerable<TResult2>> Execute(IDatabaseManager databaseManager, params DbParameter[] explicitParameters)
        {
            return Execute(databaseManager, default(TFilter), explicitParameters);
        }

        /// <summary>
        /// Executes the command using a provided database manager and optionally parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;&gt;.</returns>
        public Tuple<IEnumerable<TResult1>,
            IEnumerable<TResult2>> Execute(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters)
        {
            return Execute(databaseManager, filter, null, explicitParameters);
        }

        /// <summary>
        /// Enables result caching.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="interval">The interval.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2&gt;.</returns>
        public ICommandProcessor<TFilter, TResult1, TResult2>
            EnableResultCaching(CacheMode mode = CacheMode.Permanent, double? interval = null)
        {
            ActivateResultCaching(mode, interval);

            return this;
        }
    }

    /// <summary>
    /// A fully built and ready to be executed command expression with appropriate mapping expressions compiled and a
    /// filter parameter.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the result1.</typeparam>
    /// <typeparam name="TResult2">The type of the result2.</typeparam>
    /// <typeparam name="TResult3">The type of the result3.</typeparam>
    /// <remarks>Appropriate mapping expressions are compiled at the point this interface becomes available.</remarks>
    public class MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2, TResult3>
        : CommandProcessorWithResults<TFilter>,
            ICommandProcessor<TFilter, TResult1, TResult2, TResult3>
    {
        private const int ResultCount = 3;

        private readonly IResultMapper[] _mappers;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="MultipleResultSetCommandProcessor{TFilter, TResult1, TResult2, TResult3}" />
        /// class.
        /// </summary>
        /// <param name="commandResultInfo">The command result information.</param>
        /// <param name="name">The name.</param>
        public MultipleResultSetCommandProcessor(ICommandResultInfo<TFilter> commandResultInfo, string name)
            : base(commandResultInfo)
        {
            _mappers = new IResultMapper[ResultCount];

            _mappers[0] = CommandResultInfo.GetProcessor<TFilter, TResult1>();
            _mappers[1] = CommandResultInfo.GetProcessor<TFilter, TResult2>();
            _mappers[2] = CommandResultInfo.GetProcessor<TFilter, TResult3>();

            CommandManager.RegisterCommandProcessor(this, name);
        }

        /// <summary>
        /// Executes the command using a provided database manager and optionally a filter to read parameters from and explicit
        /// parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;,
        /// IEnumerable&lt;TResult3&gt;&gt;.</returns>
        public Tuple<IEnumerable<TResult1>,
            IEnumerable<TResult2>,
            IEnumerable<TResult3>>
            Execute(IDatabaseManager databaseManager, TFilter filter, object parameterObject, params DbParameter[] explicitParameters)
        {
            var results = new object[ResultCount];

            var commandExpression = CommandInfo;
            var parameters = commandExpression.BuildParameters(databaseManager, filter, parameterObject, explicitParameters);

            var cachedItemPresent = false;
            var hashCode = BigInteger.Zero;
            object finalResults = null;

            if (ResultCachingEnabled)
            {
                var parameterAggregate = parameters.Aggregate(string.Empty,
                    (p, c) => p + (c.ParameterName + (c.Value ?? string.Empty).ToString()));

                hashCode = HashBuilder.Compute(parameterAggregate);

                object value;
                TryRetrieveCacheResult(hashCode, out value);

                finalResults = value;

                cachedItemPresent = finalResults != null;
            }

            if (!cachedItemPresent)
            {
                try
                {
                    using (var record = databaseManager
                        .ExecuteDataReader(
                            commandExpression.CommandText,
                            commandExpression.DbCommandType,
                            parameters))
                    {
                        for (var i = 0; i < ResultCount; i++)
                        {
                            if (i > 0)
                                if (!record.NextResult())
                                    break;

                            results[i] = _mappers[i].MapResult(record);
                        }
                    }
                }
                catch (Exception ex)
                {
                    CommandManager.HandleExecutionException(CommandInfo, ex, parameters);
                }

                finalResults = new Tuple<IEnumerable<TResult1>,
                    IEnumerable<TResult2>,
                    IEnumerable<TResult3>>(
                    results[0] as IEnumerable<TResult1>,
                    results[1] as IEnumerable<TResult2>,
                    results[2] as IEnumerable<TResult3>);

                if (ResultCachingEnabled)
                    ResultCacheContainer.TryAdd(hashCode,
                        new CacheItem(finalResults, ResultCachingMode, ResultCachingInterval));
            }
            return finalResults as Tuple<IEnumerable<TResult1>,
                IEnumerable<TResult2>,
                IEnumerable<TResult3>>;
        }

        /// <summary>
        /// Executes the command using a provided database manager and optionally parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;,
        /// IEnumerable&lt;TResult3&gt;&gt;.</returns>
        public Tuple<IEnumerable<TResult1>,
            IEnumerable<TResult2>,
            IEnumerable<TResult3>> Execute(IDatabaseManager databaseManager, params DbParameter[] explicitParameters)
        {
            return Execute(databaseManager, default(TFilter), explicitParameters);
        }

        /// <summary>
        /// Executes the command using a provided database manager and optionally parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;,
        /// IEnumerable&lt;TResult3&gt;&gt;.</returns>
        public Tuple<IEnumerable<TResult1>,
            IEnumerable<TResult2>,
            IEnumerable<TResult3>> Execute(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters)
        {
            return Execute(databaseManager, filter, null, explicitParameters);
        }

        /// <summary>
        /// Enables result caching.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="interval">The interval.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2, TResult3&gt;.</returns>
        public ICommandProcessor<TFilter, TResult1, TResult2, TResult3>
            EnableResultCaching(CacheMode mode = CacheMode.Permanent, double? interval = null)
        {
            ActivateResultCaching(mode, interval);

            return this;
        }
    }

    /// <summary>
    /// A fully built and ready to be executed command expression with appropriate mapping expressions compiled and a
    /// filter parameter.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the result1.</typeparam>
    /// <typeparam name="TResult2">The type of the result2.</typeparam>
    /// <typeparam name="TResult3">The type of the result3.</typeparam>
    /// <typeparam name="TResult4">The type of the result4.</typeparam>
    /// <remarks>Appropriate mapping expressions are compiled at the point this interface becomes available.</remarks>
    public class MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4> :
        CommandProcessorWithResults<TFilter>,
        ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4>
    {
        private const int ResultCount = 4;

        private readonly IResultMapper[] _mappers;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="MultipleResultSetCommandProcessor{TFilter, TResult1, TResult2, TResult3, TResult4}" />
        /// class.
        /// </summary>
        /// <param name="commandResultInfo">The command result information.</param>
        /// <param name="name">The name.</param>
        public MultipleResultSetCommandProcessor(ICommandResultInfo<TFilter> commandResultInfo, string name)
            : base(commandResultInfo)
        {
            _mappers = new IResultMapper[ResultCount];

            _mappers[0] = CommandResultInfo.GetProcessor<TFilter, TResult1>();
            _mappers[1] = CommandResultInfo.GetProcessor<TFilter, TResult2>();
            _mappers[2] = CommandResultInfo.GetProcessor<TFilter, TResult3>();
            _mappers[3] = CommandResultInfo.GetProcessor<TFilter, TResult4>();

            CommandManager.RegisterCommandProcessor(this, name);
        }


        /// <summary>
        /// Executes the command using a provided database manager and optionally a filter to read parameters from and explicit
        /// parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;,
        /// IEnumerable&lt;TResult3&gt;,
        /// IEnumerable&lt;TResult4&gt;&gt;.</returns>
        public Tuple<IEnumerable<TResult1>,
            IEnumerable<TResult2>,
            IEnumerable<TResult3>,
            IEnumerable<TResult4>>
            Execute(IDatabaseManager databaseManager, TFilter filter, object parameterObject, params DbParameter[] explicitParameters)
        {
            var results = new object[ResultCount];

            var parameters = CommandInfo.BuildParameters(databaseManager, filter, parameterObject, explicitParameters);

            var cachedItemPresent = false;
            var hashCode = BigInteger.Zero;
            object finalResults = null;

            if (ResultCachingEnabled)
            {
                var parameterAggregate = parameters.Aggregate(string.Empty,
                    (p, c) => p + (c.ParameterName + (c.Value ?? string.Empty).ToString()));

                hashCode = HashBuilder.Compute(parameterAggregate);

                object value;
                TryRetrieveCacheResult(hashCode, out value);

                finalResults = value;

                cachedItemPresent = finalResults != null;
            }

            if (!cachedItemPresent)
            {
                try
                {
                    using (var record = databaseManager
                        .ExecuteDataReader(
                            CommandInfo.CommandText,
                            CommandInfo.DbCommandType,
                            parameters))
                    {
                        for (var i = 0; i < ResultCount; i++)
                        {
                            if (i > 0)
                                if (!record.NextResult())
                                    break;

                            results[i] = _mappers[i].MapResult(record);
                        }
                    }
                }
                catch (Exception ex)
                {
                    CommandManager.HandleExecutionException(CommandInfo, ex, parameters);
                }

                finalResults = new Tuple<IEnumerable<TResult1>,
                    IEnumerable<TResult2>,
                    IEnumerable<TResult3>,
                    IEnumerable<TResult4>>(
                    results[0] as IEnumerable<TResult1>,
                    results[1] as IEnumerable<TResult2>,
                    results[2] as IEnumerable<TResult3>,
                    results[3] as IEnumerable<TResult4>);

                if (ResultCachingEnabled)
                    ResultCacheContainer.TryAdd(hashCode,
                        new CacheItem(finalResults, ResultCachingMode, ResultCachingInterval));
            }
            return finalResults as Tuple<IEnumerable<TResult1>,
                IEnumerable<TResult2>,
                IEnumerable<TResult3>,
                IEnumerable<TResult4>>;
        }

        /// <summary>
        /// Executes the command using a provided database manager and optionally parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;,
        /// IEnumerable&lt;TResult3&gt;,
        /// IEnumerable&lt;TResult4&gt;;&gt;.</returns>
        public Tuple<IEnumerable<TResult1>,
            IEnumerable<TResult2>,
            IEnumerable<TResult3>,
            IEnumerable<TResult4>> Execute(IDatabaseManager databaseManager, params DbParameter[] explicitParameters)
        {
            return Execute(databaseManager, default(TFilter), explicitParameters);
        }

        /// <summary>
        /// Executes the command using a provided database manager and optionally parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;,
        /// IEnumerable&lt;TResult3&gt;,
        /// IEnumerable&lt;TResult4&gt;;&gt;.</returns>
        public Tuple<IEnumerable<TResult1>,
            IEnumerable<TResult2>,
            IEnumerable<TResult3>,
            IEnumerable<TResult4>> Execute(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters)
        {
            return Execute(databaseManager, filter, null, explicitParameters);
        }

        /// <summary>
        /// Enables result caching.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="interval">The interval.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2, TResult3, TResult4&gt;.</returns>
        public ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4>
            EnableResultCaching(CacheMode mode = CacheMode.Permanent, double? interval = null)
        {
            ActivateResultCaching(mode, interval);

            return this;
        }
    }

    /// <summary>
    /// A fully built and ready to be executed command expression with appropriate mapping expressions compiled and a
    /// filter parameter.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the result1.</typeparam>
    /// <typeparam name="TResult2">The type of the result2.</typeparam>
    /// <typeparam name="TResult3">The type of the result3.</typeparam>
    /// <typeparam name="TResult4">The type of the result4.</typeparam>
    /// <typeparam name="TResult5">The type of the result5.</typeparam>
    /// <remarks>Appropriate mapping expressions are compiled at the point this interface becomes available.</remarks>
    public class MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5> :
        CommandProcessorWithResults<TFilter>,
        ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5>
    {
        private const int ResultCount = 5;

        private readonly IResultMapper[] _mappers;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="MultipleResultSetCommandProcessor{TFilter, TResult1, TResult2, TResult3, TResult4, TResult5}" />
        /// class.
        /// </summary>
        /// <param name="commandResultInfo">The command result information.</param>
        /// <param name="name">The name.</param>
        public MultipleResultSetCommandProcessor(ICommandResultInfo<TFilter> commandResultInfo, string name)
            : base(commandResultInfo)
        {
            _mappers = new IResultMapper[ResultCount];

            _mappers[0] = CommandResultInfo.GetProcessor<TFilter, TResult1>();
            _mappers[1] = CommandResultInfo.GetProcessor<TFilter, TResult2>();
            _mappers[2] = CommandResultInfo.GetProcessor<TFilter, TResult3>();
            _mappers[3] = CommandResultInfo.GetProcessor<TFilter, TResult4>();
            _mappers[4] = CommandResultInfo.GetProcessor<TFilter, TResult5>();

            CommandManager.RegisterCommandProcessor(this, name);
        }

        /// <summary>
        /// Executes the command using a provided database manager and optionally a filter to read parameters from and explicit
        /// parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;,
        /// IEnumerable&lt;TResult3&gt;,
        /// IEnumerable&lt;TResult4&gt;,
        /// IEnumerable&lt;TResult5&gt;&gt;.</returns>
        public Tuple<IEnumerable<TResult1>,
            IEnumerable<TResult2>,
            IEnumerable<TResult3>,
            IEnumerable<TResult4>,
            IEnumerable<TResult5>>
            Execute(IDatabaseManager databaseManager, TFilter filter, object parameterObject, params DbParameter[] explicitParameters)
        {
            var results = new object[ResultCount];

            var parameters = CommandInfo.BuildParameters(databaseManager, filter, parameterObject, explicitParameters);

            var cachedItemPresent = false;
            var hashCode = BigInteger.Zero;
            object finalResults = null;

            if (ResultCachingEnabled)
            {
                var parameterAggregate = parameters.Aggregate(string.Empty,
                    (p, c) => p + (c.ParameterName + (c.Value ?? string.Empty).ToString()));

                hashCode = HashBuilder.Compute(parameterAggregate);

                object value;
                TryRetrieveCacheResult(hashCode, out value);

                finalResults = value;

                cachedItemPresent = finalResults != null;
            }

            if (!cachedItemPresent)
            {
                try
                {
                    using (var record = databaseManager
                        .ExecuteDataReader(
                            CommandInfo.CommandText,
                            CommandInfo.DbCommandType,
                            parameters))
                    {
                        for (var i = 0; i < ResultCount; i++)
                        {
                            if (i > 0)
                                if (!record.NextResult())
                                    break;

                            results[i] = _mappers[i].MapResult(record);
                        }
                    }
                }
                catch (Exception ex)
                {
                    CommandManager.HandleExecutionException(CommandInfo, ex, parameters);
                }

                finalResults = new Tuple<IEnumerable<TResult1>,
                    IEnumerable<TResult2>,
                    IEnumerable<TResult3>,
                    IEnumerable<TResult4>,
                    IEnumerable<TResult5>>(
                    results[0] as IEnumerable<TResult1>,
                    results[1] as IEnumerable<TResult2>,
                    results[2] as IEnumerable<TResult3>,
                    results[3] as IEnumerable<TResult4>,
                    results[4] as IEnumerable<TResult5>);

                if (ResultCachingEnabled)
                    ResultCacheContainer.TryAdd(hashCode,
                        new CacheItem(finalResults, ResultCachingMode, ResultCachingInterval));
            }
            return finalResults as Tuple<IEnumerable<TResult1>,
                IEnumerable<TResult2>,
                IEnumerable<TResult3>,
                IEnumerable<TResult4>,
                IEnumerable<TResult5>>;
        }

        /// <summary>
        /// Executes the command using a provided database manager and optionally parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;,
        /// IEnumerable&lt;TResult3&gt;,
        /// IEnumerable&lt;TResult4&gt;,
        /// IEnumerable&lt;TResult5&gt;&gt;.</returns>
        public Tuple<IEnumerable<TResult1>,
            IEnumerable<TResult2>,
            IEnumerable<TResult3>,
            IEnumerable<TResult4>,
            IEnumerable<TResult5>> Execute(IDatabaseManager databaseManager, params DbParameter[] explicitParameters)
        {
            return Execute(databaseManager, default(TFilter), explicitParameters);
        }

        /// <summary>
        /// Executes the command using a provided database manager and optionally parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;,
        /// IEnumerable&lt;TResult3&gt;,
        /// IEnumerable&lt;TResult4&gt;,
        /// IEnumerable&lt;TResult5&gt;&gt;.</returns>
        public Tuple<IEnumerable<TResult1>,
            IEnumerable<TResult2>,
            IEnumerable<TResult3>,
            IEnumerable<TResult4>,
            IEnumerable<TResult5>> Execute(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters)
        {
            return Execute(databaseManager, filter, null, explicitParameters);
        }

        /// <summary>
        /// Enables result caching.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="interval">The interval.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5&gt;.</returns>
        public ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5>
            EnableResultCaching(CacheMode mode = CacheMode.Permanent, double? interval = null)
        {
            ActivateResultCaching(mode, interval);

            return this;
        }
    }

    /// <summary>
    /// A fully built and ready to be executed command expression with appropriate mapping expressions compiled and a
    /// filter parameter.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the result1.</typeparam>
    /// <typeparam name="TResult2">The type of the result2.</typeparam>
    /// <typeparam name="TResult3">The type of the result3.</typeparam>
    /// <typeparam name="TResult4">The type of the result4.</typeparam>
    /// <typeparam name="TResult5">The type of the result5.</typeparam>
    /// <typeparam name="TResult6">The type of the result6.</typeparam>
    /// <remarks>Appropriate mapping expressions are compiled at the point this interface becomes available.</remarks>
    public class MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6> :
        CommandProcessorWithResults<TFilter>,
        ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>
    {
        private const int ResultCount = 6;

        private readonly IResultMapper[] _mappers;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="MultipleResultSetCommandProcessor{TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6}" />
        /// class.
        /// </summary>
        /// <param name="commandResultInfo">The command result information.</param>
        /// <param name="name">The name.</param>
        public MultipleResultSetCommandProcessor(ICommandResultInfo<TFilter> commandResultInfo, string name)
            : base(commandResultInfo)
        {
            _mappers = new IResultMapper[ResultCount];

            _mappers[0] = CommandResultInfo.GetProcessor<TFilter, TResult1>();
            _mappers[1] = CommandResultInfo.GetProcessor<TFilter, TResult2>();
            _mappers[2] = CommandResultInfo.GetProcessor<TFilter, TResult3>();
            _mappers[3] = CommandResultInfo.GetProcessor<TFilter, TResult4>();
            _mappers[4] = CommandResultInfo.GetProcessor<TFilter, TResult5>();
            _mappers[5] = CommandResultInfo.GetProcessor<TFilter, TResult6>();

            CommandManager.RegisterCommandProcessor(this, name);
        }

        /// <summary>
        /// Executes the command using a provided database manager and optionally a filter to read parameters from and explicit
        /// parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;,
        /// IEnumerable&lt;TResult3&gt;,
        /// IEnumerable&lt;TResult4&gt;,
        /// IEnumerable&lt;TResult5&gt;,
        /// IEnumerable&lt;TResult6&gt;&gt;.</returns>
        public Tuple<IEnumerable<TResult1>,
            IEnumerable<TResult2>,
            IEnumerable<TResult3>,
            IEnumerable<TResult4>,
            IEnumerable<TResult5>,
            IEnumerable<TResult6>>
            Execute(IDatabaseManager databaseManager, TFilter filter, object parameterObject, params DbParameter[] explicitParameters)
        {
            var results = new object[ResultCount];

            var parameters = CommandInfo.BuildParameters(databaseManager, filter, parameterObject, explicitParameters);

            var cachedItemPresent = false;
            var hashCode = BigInteger.Zero;
            object finalResults = null;

            if (ResultCachingEnabled)
            {
                var parameterAggregate = parameters.Aggregate(string.Empty,
                    (p, c) => p + (c.ParameterName + (c.Value ?? string.Empty).ToString()));

                hashCode = HashBuilder.Compute(parameterAggregate);

                object value;
                TryRetrieveCacheResult(hashCode, out value);

                finalResults = value;

                cachedItemPresent = finalResults != null;
            }

            if (!cachedItemPresent)
            {
                try
                {
                    using (var record = databaseManager
                        .ExecuteDataReader(
                            CommandInfo.CommandText,
                            CommandInfo.DbCommandType,
                            parameters))
                    {
                        for (var i = 0; i < ResultCount; i++)
                        {
                            if (i > 0)
                                if (!record.NextResult())
                                    break;

                            results[i] = _mappers[i].MapResult(record);
                        }
                    }
                }
                catch (Exception ex)
                {
                    CommandManager.HandleExecutionException(CommandInfo, ex, parameters);
                }

                finalResults = new Tuple<IEnumerable<TResult1>,
                    IEnumerable<TResult2>,
                    IEnumerable<TResult3>,
                    IEnumerable<TResult4>,
                    IEnumerable<TResult5>,
                    IEnumerable<TResult6>>(
                    results[0] as IEnumerable<TResult1>,
                    results[1] as IEnumerable<TResult2>,
                    results[2] as IEnumerable<TResult3>,
                    results[3] as IEnumerable<TResult4>,
                    results[4] as IEnumerable<TResult5>,
                    results[5] as IEnumerable<TResult6>);

                if (ResultCachingEnabled)
                    ResultCacheContainer.TryAdd(hashCode,
                        new CacheItem(finalResults, ResultCachingMode, ResultCachingInterval));
            }
            return finalResults as Tuple<IEnumerable<TResult1>,
                IEnumerable<TResult2>,
                IEnumerable<TResult3>,
                IEnumerable<TResult4>,
                IEnumerable<TResult5>,
                IEnumerable<TResult6>>;
        }

        /// <summary>
        /// Executes the command using a provided database manager and optionally parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;,
        /// IEnumerable&lt;TResult3&gt;,
        /// IEnumerable&lt;TResult4&gt;,
        /// IEnumerable&lt;TResult5&gt;,
        /// IEnumerable&lt;TResult6&gt;&gt;.</returns>
        public Tuple<IEnumerable<TResult1>,
            IEnumerable<TResult2>,
            IEnumerable<TResult3>,
            IEnumerable<TResult4>,
            IEnumerable<TResult5>,
            IEnumerable<TResult6>> Execute(IDatabaseManager databaseManager, params DbParameter[] explicitParameters)
        {
            return Execute(databaseManager, default(TFilter), explicitParameters);
        }

        /// <summary>
        /// Executes the command using a provided database manager and optionally parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;,
        /// IEnumerable&lt;TResult3&gt;,
        /// IEnumerable&lt;TResult4&gt;,
        /// IEnumerable&lt;TResult5&gt;,
        /// IEnumerable&lt;TResult6&gt;&gt;.</returns>
        public Tuple<IEnumerable<TResult1>,
            IEnumerable<TResult2>,
            IEnumerable<TResult3>,
            IEnumerable<TResult4>,
            IEnumerable<TResult5>,
            IEnumerable<TResult6>> Execute(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters)
        {
            return Execute(databaseManager, filter, null, explicitParameters);
        }

        /// <summary>
        /// Enables result caching.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="interval">The interval.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6&gt;.</returns>
        public ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>
            EnableResultCaching(CacheMode mode = CacheMode.Permanent, double? interval = null)
        {
            ActivateResultCaching(mode, interval);

            return this;
        }
    }

    /// <summary>
    /// A fully built and ready to be executed command expression with appropriate mapping expressions compiled and a
    /// filter parameter.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the result1.</typeparam>
    /// <typeparam name="TResult2">The type of the result2.</typeparam>
    /// <typeparam name="TResult3">The type of the result3.</typeparam>
    /// <typeparam name="TResult4">The type of the result4.</typeparam>
    /// <typeparam name="TResult5">The type of the result5.</typeparam>
    /// <typeparam name="TResult6">The type of the result6.</typeparam>
    /// <typeparam name="TResult7">The type of the result7.</typeparam>
    /// <remarks>Appropriate mapping expressions are compiled at the point this interface becomes available.</remarks>
    public class MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6,
        TResult7>
        : CommandProcessorWithResults<TFilter>,
            ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>
    {
        private const int ResultCount = 7;

        private readonly IResultMapper[] _mappers;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="MultipleResultSetCommandProcessor{TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7}" />
        /// class.
        /// </summary>
        /// <param name="commandResultInfo">The command result information.</param>
        /// <param name="name">The name.</param>
        public MultipleResultSetCommandProcessor(ICommandResultInfo<TFilter> commandResultInfo, string name)
            : base(commandResultInfo)
        {
            _mappers = new IResultMapper[ResultCount];

            _mappers[0] = CommandResultInfo.GetProcessor<TFilter, TResult1>();
            _mappers[1] = CommandResultInfo.GetProcessor<TFilter, TResult2>();
            _mappers[2] = CommandResultInfo.GetProcessor<TFilter, TResult3>();
            _mappers[3] = CommandResultInfo.GetProcessor<TFilter, TResult4>();
            _mappers[4] = CommandResultInfo.GetProcessor<TFilter, TResult5>();
            _mappers[5] = CommandResultInfo.GetProcessor<TFilter, TResult6>();
            _mappers[6] = CommandResultInfo.GetProcessor<TFilter, TResult7>();

            CommandManager.RegisterCommandProcessor(this, name);
        }

        /// <summary>
        /// Executes the command using a provided database manager and optionally a filter to read parameters from and explicit
        /// parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;,
        /// IEnumerable&lt;TResult3&gt;,
        /// IEnumerable&lt;TResult4&gt;,
        /// IEnumerable&lt;TResult5&gt;,
        /// IEnumerable&lt;TResult6&gt;,
        /// IEnumerable&lt;TResult7&gt;&gt;.</returns>
        public Tuple<IEnumerable<TResult1>,
            IEnumerable<TResult2>,
            IEnumerable<TResult3>,
            IEnumerable<TResult4>,
            IEnumerable<TResult5>,
            IEnumerable<TResult6>,
            IEnumerable<TResult7>>
            Execute(IDatabaseManager databaseManager, TFilter filter, object parameterObject, params DbParameter[] explicitParameters)
        {
            var results = new object[ResultCount];

            var parameters = CommandInfo.BuildParameters(databaseManager, filter, parameterObject, explicitParameters);

            var cachedItemPresent = false;
            var hashCode = BigInteger.Zero;
            object finalResults = null;

            if (ResultCachingEnabled)
            {
                var parameterAggregate = parameters.Aggregate(string.Empty,
                    (p, c) => p + (c.ParameterName + (c.Value ?? string.Empty).ToString()));

                hashCode = HashBuilder.Compute(parameterAggregate);

                object value;
                TryRetrieveCacheResult(hashCode, out value);

                finalResults = value;

                cachedItemPresent = finalResults != null;
            }

            if (!cachedItemPresent)
            {
                try
                {
                    using (var record = databaseManager
                        .ExecuteDataReader(
                            CommandInfo.CommandText,
                            CommandInfo.DbCommandType,
                            parameters))
                    {
                        for (var i = 0; i < ResultCount; i++)
                        {
                            if (i > 0)
                                if (!record.NextResult())
                                    break;

                            results[i] = _mappers[i].MapResult(record);
                        }
                    }
                }
                catch (Exception ex)
                {
                    CommandManager.HandleExecutionException(CommandInfo, ex, parameters);
                }

                finalResults = new Tuple<IEnumerable<TResult1>,
                    IEnumerable<TResult2>,
                    IEnumerable<TResult3>,
                    IEnumerable<TResult4>,
                    IEnumerable<TResult5>,
                    IEnumerable<TResult6>,
                    IEnumerable<TResult7>>(
                    results[0] as IEnumerable<TResult1>,
                    results[1] as IEnumerable<TResult2>,
                    results[2] as IEnumerable<TResult3>,
                    results[3] as IEnumerable<TResult4>,
                    results[4] as IEnumerable<TResult5>,
                    results[5] as IEnumerable<TResult6>,
                    results[6] as IEnumerable<TResult7>);

                if (ResultCachingEnabled)
                    ResultCacheContainer.TryAdd(hashCode,
                        new CacheItem(finalResults, ResultCachingMode, ResultCachingInterval));
            }

            return finalResults as Tuple<IEnumerable<TResult1>,
                IEnumerable<TResult2>,
                IEnumerable<TResult3>,
                IEnumerable<TResult4>,
                IEnumerable<TResult5>,
                IEnumerable<TResult6>,
                IEnumerable<TResult7>>;
        }

        /// <summary>
        /// Executes the command using a provided database manager and optionally parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;,
        /// IEnumerable&lt;TResult3&gt;,
        /// IEnumerable&lt;TResult4&gt;,
        /// IEnumerable&lt;TResult5&gt;,
        /// IEnumerable&lt;TResult6&gt;,
        /// IEnumerable&lt;TResult7&gt;&gt;.</returns>
        public Tuple<IEnumerable<TResult1>,
            IEnumerable<TResult2>,
            IEnumerable<TResult3>,
            IEnumerable<TResult4>,
            IEnumerable<TResult5>,
            IEnumerable<TResult6>,
            IEnumerable<TResult7>> Execute(IDatabaseManager databaseManager, params DbParameter[] explicitParameters)
        {
            return Execute(databaseManager, default(TFilter), explicitParameters);
        }

        /// <summary>
        /// Executes the command using a provided database manager and optionally parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;
        /// IEnumerable&lt;TResult1&gt;,
        /// IEnumerable&lt;TResult2&gt;,
        /// IEnumerable&lt;TResult3&gt;,
        /// IEnumerable&lt;TResult4&gt;,
        /// IEnumerable&lt;TResult5&gt;,
        /// IEnumerable&lt;TResult6&gt;,
        /// IEnumerable&lt;TResult7&gt;&gt;.</returns>
        public Tuple<IEnumerable<TResult1>,
            IEnumerable<TResult2>,
            IEnumerable<TResult3>,
            IEnumerable<TResult4>,
            IEnumerable<TResult5>,
            IEnumerable<TResult6>,
            IEnumerable<TResult7>> Execute(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters)
        {
            return Execute(databaseManager, filter, null, explicitParameters);
        }

        /// <summary>
        /// Enables result caching.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="interval">The interval.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7&gt;.</returns>
        public ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>
            EnableResultCaching(CacheMode mode = CacheMode.Permanent, double? interval = null)
        {
            ActivateResultCaching(mode, interval);

            return this;
        }
    }
}