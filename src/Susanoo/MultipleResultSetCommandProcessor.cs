#region

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Numerics;

#endregion

namespace Susanoo
{
    /// <summary>
    ///     A fully built and ready to be executed command expression with appropriate mapping expressions compiled and a
    ///     filter parameter.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the result1.</typeparam>
    /// <typeparam name="TResult2">The type of the result2.</typeparam>
    /// <remarks>Appropriate mapping expressions are compiled at the point this interface becomes available.</remarks>
    public class MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2>
        : CommandProcessorCommon, ICommandProcessor<TFilter, TResult1, TResult2>
        where TResult1 : new()
        where TResult2 : new()
    {
        private readonly ICommandExpression<TFilter> _commandExpression;
        private readonly ICommandResultExpression<TFilter, TResult1, TResult2> _commandResultExpression;

        private readonly IResultMapper<TResult1> _item1Mapper;
        private readonly IResultMapper<TResult2> _item2Mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleResultSetCommandProcessor{TFilter, TResult1, TResult2}" />
        /// class.
        /// </summary>
        /// <param name="commandResultExpression">The command result expression.</param>
        /// <param name="name">The name.</param>
        public MultipleResultSetCommandProcessor(
            ICommandResultExpression<TFilter, TResult1, TResult2> commandResultExpression, string name)
            : base(name)
        {
            _commandExpression = commandResultExpression.CommandExpression;
            _commandResultExpression = commandResultExpression;

            _item1Mapper =
                new SingleResultSetCommandProcessor<TFilter, TResult1>(
                    CommandResultExpression.ToSingleResult<TResult1>(), null);
            _item2Mapper =
                new SingleResultSetCommandProcessor<TFilter, TResult2>(
                    CommandResultExpression.ToSingleResult<TResult2>(), null);

            CommandManager.RegisterCommandProcessor(this, name);
        }

        /// <summary>
        ///     Gets the command result expression.
        /// </summary>
        /// <value>The command result expression.</value>
        public ICommandResultExpression<TFilter, TResult1, TResult2> CommandResultExpression
        {
            get { return _commandResultExpression; }
        }

        /// <summary>
        ///     Gets the command expression.
        /// </summary>
        /// <value>The command expression.</value>
        public ICommandExpression<TFilter> CommandExpression
        {
            get { return _commandExpression; }
        }

        /// <summary>
        ///     Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public override BigInteger CacheHash
        {
            get
            {
                return (_item1Mapper.CacheHash * 31)
                     ^ (_item2Mapper.CacheHash * 31);
            }
        }

        /// <summary>
        ///     Executes the command using a provided database manager and optionally a filter to read parameters from and explicit
        ///     parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;IEnumerable&lt;TResult1&gt;, IEnumerable&lt;TResult2&gt;&gt;.</returns>
        public Tuple<IEnumerable<TResult1>, IEnumerable<TResult2>> Execute(IDatabaseManager databaseManager,
            TFilter filter, params DbParameter[] explicitParameters)
        {
            IEnumerable<TResult1> results1 = null;
            IEnumerable<TResult2> results2 = null;

            ICommandExpression<TFilter> commandExpression = CommandExpression;
            var parameters = commandExpression.BuildParameters(databaseManager, filter, explicitParameters);

            bool cachedItemPresent = false;
            BigInteger hashCode = BigInteger.Zero;
            Tuple<IEnumerable<TResult1>, IEnumerable<TResult2>> finalResults = null;
            if (ResultCachingEnabled)
            {
                var parameterAggregate = parameters.Aggregate(string.Empty, (p, c) => p + (c.ParameterName + (c.Value ?? string.Empty).ToString()));

                hashCode = FnvHash.GetHash(parameterAggregate, 128);

                object value = null;
                TryRetrieveCacheResult(hashCode, out value);

                finalResults = value as Tuple<IEnumerable<TResult1>, IEnumerable<TResult2>>;
                cachedItemPresent = finalResults != null;
            }

            if (!cachedItemPresent)
            {
                using (IDataReader record = databaseManager
                    .ExecuteDataReader(
                        commandExpression.CommandText,
                        commandExpression.DbCommandType,
                        parameters))
                {
                    results1 = _item1Mapper.MapResult(record);

                    if (record.NextResult())
                    {
                        results2 = _item2Mapper.MapResult(record);
                    }
                }

                finalResults = new Tuple<IEnumerable<TResult1>, IEnumerable<TResult2>>(results1, results2);

                if (ResultCachingEnabled)
                    ResultCacheContainer.TryAdd(hashCode, new CacheItem(finalResults, ResultCachingMode, ResultCachingInterval));
            }

            return finalResults;
        }

        /// <summary>
        ///     Executes the command using a provided database manager and optionally parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;IEnumerable&lt;TResult1&gt;, IEnumerable&lt;TResult2&gt;&gt;.</returns>
        public Tuple<IEnumerable<TResult1>, IEnumerable<TResult2>> Execute(IDatabaseManager databaseManager,
            params DbParameter[] explicitParameters)
        {
            return Execute(databaseManager, default(TFilter), explicitParameters);
        }

        /// <summary>
        /// Enables the result caching.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="interval">The interval.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2&gt;.</returns>
        public ICommandProcessor<TFilter, TResult1, TResult2> EnableResultCaching(CacheMode mode = CacheMode.Permanent, double? interval = null)
        {
            ActivateResultCaching(mode, interval);

            return this;
        }
    }

    /// <summary>
    ///     A fully built and ready to be executed command expression with appropriate mapping expressions compiled and a
    ///     filter parameter.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the result1.</typeparam>
    /// <typeparam name="TResult2">The type of the result2.</typeparam>
    /// <typeparam name="TResult3">The type of the result3.</typeparam>
    /// <remarks>Appropriate mapping expressions are compiled at the point this interface becomes available.</remarks>
    public class MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2, TResult3>
        : CommandProcessorCommon, ICommandProcessor<TFilter, TResult1, TResult2, TResult3>
        where TResult1 : new()
        where TResult2 : new()
        where TResult3 : new()
    {
        private readonly ICommandExpression<TFilter> _commandExpression;
        private readonly ICommandResultExpression<TFilter, TResult1, TResult2, TResult3> _commandResultExpression;

        private readonly IResultMapper<TResult1> _item1Mapper;
        private readonly IResultMapper<TResult2> _item2Mapper;
        private readonly IResultMapper<TResult3> _item3Mapper;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="MultipleResultSetCommandProcessor{TFilter, TResult1, TResult2, TResult3}" /> class.
        /// </summary>
        /// <param name="commandResultExpression">The command result expression.</param>
        /// <param name="name">The name.</param>
        public MultipleResultSetCommandProcessor(
            ICommandResultExpression<TFilter, TResult1, TResult2, TResult3> commandResultExpression, string name)
            : base(name)
        {
            _commandExpression = commandResultExpression.CommandExpression;
            _commandResultExpression = commandResultExpression;

            _item1Mapper =
                new SingleResultSetCommandProcessor<TFilter, TResult1>(
                    CommandResultExpression.ToSingleResult<TResult1>(), null);
            _item2Mapper =
                new SingleResultSetCommandProcessor<TFilter, TResult2>(
                    CommandResultExpression.ToSingleResult<TResult2>(), null);
            _item3Mapper =
                new SingleResultSetCommandProcessor<TFilter, TResult3>(
                    CommandResultExpression.ToSingleResult<TResult3>(), null);

            CommandManager.RegisterCommandProcessor(this, name);
        }

        /// <summary>
        ///     Gets the command result expression.
        /// </summary>
        /// <value>The command result expression.</value>
        public ICommandResultExpression<TFilter, TResult1, TResult2, TResult3> CommandResultExpression
        {
            get { return _commandResultExpression; }
        }

        /// <summary>
        ///     Gets the command expression.
        /// </summary>
        /// <value>The command expression.</value>
        public ICommandExpression<TFilter> CommandExpression
        {
            get { return _commandExpression; }
        }
        /// <summary>
        ///     Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public override BigInteger CacheHash
        {
            get
            {
                return (_item1Mapper.CacheHash * 31)
                     ^ (_item2Mapper.CacheHash * 31)
                     ^ (_item3Mapper.CacheHash * 31);
            }
        }

        /// <summary>
        ///     Executes the command using a provided database manager and optionally a filter to read parameters from and explicit
        ///     parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;IEnumerable&lt;TResult1&gt;, IEnumerable&lt;TResult2&gt;, IEnumerable&lt;TResult3&gt;&gt;.</returns>
        public Tuple<IEnumerable<TResult1>, IEnumerable<TResult2>, IEnumerable<TResult3>>
            Execute(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters)
        {
            IEnumerable<TResult1> results1;
            IEnumerable<TResult2> results2 = null;
            IEnumerable<TResult3> results3 = null;

            ICommandExpression<TFilter> commandExpression = CommandExpression;
            var parameters = commandExpression.BuildParameters(databaseManager, filter, explicitParameters);

            bool cachedItemPresent = false;
            BigInteger hashCode = BigInteger.Zero;
            Tuple<IEnumerable<TResult1>, IEnumerable<TResult2>, IEnumerable<TResult3>> finalResults = null;
            if (ResultCachingEnabled)
            {
                var parameterAggregate = parameters.Aggregate(string.Empty, (p, c) => p + (c.ParameterName + (c.Value ?? string.Empty).ToString()));

                hashCode = FnvHash.GetHash(parameterAggregate, 128);

                object value = null;
                TryRetrieveCacheResult(hashCode, out value);

                finalResults = value as Tuple<IEnumerable<TResult1>, IEnumerable<TResult2>, IEnumerable<TResult3>>;
                cachedItemPresent = finalResults != null;
            }

            if (!cachedItemPresent)
            {
                using (IDataReader record = databaseManager
                    .ExecuteDataReader(
                        commandExpression.CommandText,
                        commandExpression.DbCommandType,
                        parameters))
                {
                    results1 = _item1Mapper.MapResult(record);

                    if (record.NextResult())
                    {
                        results2 = _item2Mapper.MapResult(record);

                        if (record.NextResult())
                        {
                            results3 = _item3Mapper.MapResult(record);
                        }
                    }
                }

                finalResults = new Tuple<IEnumerable<TResult1>, IEnumerable<TResult2>, IEnumerable<TResult3>>(results1, results2, results3);

                if (ResultCachingEnabled)
                    ResultCacheContainer.TryAdd(hashCode, new CacheItem(finalResults, ResultCachingMode, ResultCachingInterval));
            }

            return finalResults;
        }

        /// <summary>
        ///     Executes the command using a provided database manager and optionally parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;IEnumerable&lt;TResult1&gt;, IEnumerable&lt;TResult2&gt;, IEnumerable&lt;TResult3&gt;&gt;.</returns>
        public Tuple<IEnumerable<TResult1>, IEnumerable<TResult2>, IEnumerable<TResult3>> Execute(
            IDatabaseManager databaseManager,
            params DbParameter[] explicitParameters)
        {
            return Execute(databaseManager, default(TFilter), explicitParameters);
        }

        /// <summary>
        /// Enables the result caching.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="interval">The interval.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2, TResult3&gt;.</returns>
        public ICommandProcessor<TFilter, TResult1, TResult2, TResult3> EnableResultCaching(CacheMode mode = CacheMode.Permanent, double? interval = null)
        {
            ActivateResultCaching(mode, interval);

            return this;
        }
    }

    /// <summary>
    ///     A fully built and ready to be executed command expression with appropriate mapping expressions compiled and a
    ///     filter parameter.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the result1.</typeparam>
    /// <typeparam name="TResult2">The type of the result2.</typeparam>
    /// <typeparam name="TResult3">The type of the result3.</typeparam>
    /// <typeparam name="TResult4">The type of the result4.</typeparam>
    /// <remarks>Appropriate mapping expressions are compiled at the point this interface becomes available.</remarks>
    public class MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4>
        : CommandProcessorCommon, ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4>
        where TResult1 : new()
        where TResult2 : new()
        where TResult3 : new()
        where TResult4 : new()
    {
        private readonly ICommandExpression<TFilter> _commandExpression;

        private readonly ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4>
            _commandResultExpression;

        private readonly IResultMapper<TResult1> _item1Mapper;
        private readonly IResultMapper<TResult2> _item2Mapper;
        private readonly IResultMapper<TResult3> _item3Mapper;
        private readonly IResultMapper<TResult4> _item4Mapper;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="MultipleResultSetCommandProcessor{TFilter, TResult1, TResult2, TResult3, TResult4}" /> class.
        /// </summary>
        /// <param name="commandResultExpression">The command result expression.</param>
        /// <param name="name">The name.</param>
        public MultipleResultSetCommandProcessor(
            ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4>
                commandResultExpression, string name)
            : base(name)
        {
            _commandExpression = commandResultExpression.CommandExpression;
            _commandResultExpression = commandResultExpression;

            _item1Mapper =
                new SingleResultSetCommandProcessor<TFilter, TResult1>(
                    CommandResultExpression.ToSingleResult<TResult1>(), null);
            _item2Mapper =
                new SingleResultSetCommandProcessor<TFilter, TResult2>(
                    CommandResultExpression.ToSingleResult<TResult2>(), null);
            _item3Mapper =
                new SingleResultSetCommandProcessor<TFilter, TResult3>(
                    CommandResultExpression.ToSingleResult<TResult3>(), null);
            _item4Mapper =
                new SingleResultSetCommandProcessor<TFilter, TResult4>(
                    CommandResultExpression.ToSingleResult<TResult4>(), null);

            CommandManager.RegisterCommandProcessor(this, name);
        }

        /// <summary>
        ///     Gets the command result expression.
        /// </summary>
        /// <value>The command result expression.</value>
        public ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4> CommandResultExpression
        {
            get { return _commandResultExpression; }
        }

        /// <summary>
        ///     Gets the command expression.
        /// </summary>
        /// <value>The command expression.</value>
        public ICommandExpression<TFilter> CommandExpression
        {
            get { return _commandExpression; }
        }

        /// <summary>
        ///     Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public override BigInteger CacheHash
        {
            get
            {
                return (_item1Mapper.CacheHash * 31)
                     ^ (_item2Mapper.CacheHash * 31)
                     ^ (_item3Mapper.CacheHash * 31)
                     ^ (_item4Mapper.CacheHash * 31);
            }
        }

        /// <summary>
        ///     Executes the command using a provided database manager and optionally a filter to read parameters from and explicit
        ///     parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>
        ///     Tuple&lt;IEnumerable&lt;TResult1&gt;, IEnumerable&lt;TResult2&gt;, IEnumerable&lt;TResult3&gt;, IEnumerable
        ///     &lt;TResult4&gt;&gt;.
        /// </returns>
        public Tuple<IEnumerable<TResult1>,
            IEnumerable<TResult2>,
            IEnumerable<TResult3>,
            IEnumerable<TResult4>>
            Execute(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters)
        {
            IEnumerable<TResult1> results1;
            IEnumerable<TResult2> results2 = null;
            IEnumerable<TResult3> results3 = null;
            IEnumerable<TResult4> results4 = null;

            ICommandExpression<TFilter> commandExpression = CommandExpression;
            var parameters = commandExpression.BuildParameters(databaseManager, filter, explicitParameters);

            bool cachedItemPresent = false;
            BigInteger hashCode = BigInteger.Zero;
            Tuple<IEnumerable<TResult1>,
                IEnumerable<TResult2>,
                IEnumerable<TResult3>,
                IEnumerable<TResult4>> finalResults = null;
            if (ResultCachingEnabled)
            {
                var parameterAggregate = parameters.Aggregate(string.Empty, (p, c) => p + (c.ParameterName + (c.Value ?? string.Empty).ToString()));

                hashCode = FnvHash.GetHash(parameterAggregate, 128);

                object value = null;
                TryRetrieveCacheResult(hashCode, out value);

                finalResults = value as Tuple<IEnumerable<TResult1>,
                                                IEnumerable<TResult2>,
                                                IEnumerable<TResult3>,
                                                IEnumerable<TResult4>>;
                cachedItemPresent = finalResults != null;
            }

            if (!cachedItemPresent)
            {
                using (IDataReader record = databaseManager
                    .ExecuteDataReader(
                        commandExpression.CommandText,
                        commandExpression.DbCommandType,
                        parameters))
                {
                    //This can be replaced with a loop once there is a non-generic IResultMapper interface, I think.

                    results1 = _item1Mapper.MapResult(record);

                    if (record.NextResult())
                    {
                        results2 = _item2Mapper.MapResult(record);

                        if (record.NextResult())
                        {
                            results3 = _item3Mapper.MapResult(record);

                            if (record.NextResult())
                            {
                                results4 = _item4Mapper.MapResult(record);
                            }
                        }
                    }
                }

                finalResults = new Tuple<IEnumerable<TResult1>,
            IEnumerable<TResult2>,
            IEnumerable<TResult3>,
            IEnumerable<TResult4>>(results1, results2, results3, results4);

                if (ResultCachingEnabled)
                    ResultCacheContainer.TryAdd(hashCode, new CacheItem(finalResults, ResultCachingMode, ResultCachingInterval));
            }

            return finalResults;
        }

        /// <summary>
        ///     Executes the command using a provided database manager and optionally parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>
        ///     Tuple&lt;IEnumerable&lt;TResult1&gt;, IEnumerable&lt;TResult2&gt;, IEnumerable&lt;TResult3&gt;, IEnumerable
        ///     &lt;TResult4&gt;&gt;.
        /// </returns>
        public Tuple<IEnumerable<TResult1>,
            IEnumerable<TResult2>,
            IEnumerable<TResult3>,
            IEnumerable<TResult4>> Execute(IDatabaseManager databaseManager, params DbParameter[] explicitParameters)
        {
            return Execute(databaseManager, default(TFilter), explicitParameters);
        }

        /// <summary>
        /// Enables the result caching.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="interval">The interval.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2, TResult3, TResult4&gt;.</returns>
        public ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4> EnableResultCaching(CacheMode mode = CacheMode.Permanent, double? interval = null)
        {
            ActivateResultCaching(mode, interval);

            return this;
        }
    }

    /// <summary>
    ///     A fully built and ready to be executed command expression with appropriate mapping expressions compiled and a
    ///     filter parameter.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the result1.</typeparam>
    /// <typeparam name="TResult2">The type of the result2.</typeparam>
    /// <typeparam name="TResult3">The type of the result3.</typeparam>
    /// <typeparam name="TResult4">The type of the result4.</typeparam>
    /// <typeparam name="TResult5">The type of the result5.</typeparam>
    /// <remarks>Appropriate mapping expressions are compiled at the point this interface becomes available.</remarks>
    public class MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5>
        : CommandProcessorCommon, ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5>
        where TResult1 : new()
        where TResult2 : new()
        where TResult3 : new()
        where TResult4 : new()
        where TResult5 : new()
    {
        private readonly ICommandExpression<TFilter> _commandExpression;

        private readonly ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5>
            _commandResultExpression;

        private readonly IResultMapper<TResult1> _item1Mapper;
        private readonly IResultMapper<TResult2> _item2Mapper;
        private readonly IResultMapper<TResult3> _item3Mapper;
        private readonly IResultMapper<TResult4> _item4Mapper;
        private readonly IResultMapper<TResult5> _item5Mapper;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="MultipleResultSetCommandProcessor{TFilter, TResult1, TResult2, TResult3, TResult4, TResult5}" /> class.
        /// </summary>
        /// <param name="commandResultExpression">The command result expression.</param>
        /// <param name="name">The name.</param>
        public MultipleResultSetCommandProcessor(
            ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5>
                commandResultExpression, string name)
            : base(name)
        {
            _commandExpression = commandResultExpression.CommandExpression;
            _commandResultExpression = commandResultExpression;

            _item1Mapper =
                new SingleResultSetCommandProcessor<TFilter, TResult1>(
                    CommandResultExpression.ToSingleResult<TResult1>(), null);
            _item2Mapper =
                new SingleResultSetCommandProcessor<TFilter, TResult2>(
                    CommandResultExpression.ToSingleResult<TResult2>(), null);
            _item3Mapper =
                new SingleResultSetCommandProcessor<TFilter, TResult3>(
                    CommandResultExpression.ToSingleResult<TResult3>(), null);
            _item4Mapper =
                new SingleResultSetCommandProcessor<TFilter, TResult4>(
                    CommandResultExpression.ToSingleResult<TResult4>(), null);
            _item5Mapper =
                new SingleResultSetCommandProcessor<TFilter, TResult5>(
                    CommandResultExpression.ToSingleResult<TResult5>(), null);

            CommandManager.RegisterCommandProcessor(this, name);
        }

        /// <summary>
        ///     Gets the command result expression.
        /// </summary>
        /// <value>The command result expression.</value>
        public ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5>
            CommandResultExpression
        {
            get { return _commandResultExpression; }
        }

        /// <summary>
        ///     Gets the command expression.
        /// </summary>
        /// <value>The command expression.</value>
        public ICommandExpression<TFilter> CommandExpression
        {
            get { return _commandExpression; }
        }

        /// <summary>
        ///     Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public override BigInteger CacheHash
        {
            get
            {
                return (_item1Mapper.CacheHash * 31)
                     ^ (_item2Mapper.CacheHash * 31)
                     ^ (_item3Mapper.CacheHash * 31)
                     ^ (_item4Mapper.CacheHash * 31)
                     ^ (_item5Mapper.CacheHash * 31);
            }
        }

        /// <summary>
        ///     Executes the command using a provided database manager and optionally a filter to read parameters from and explicit
        ///     parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>
        ///     Tuple&lt;
        ///     IEnumerable&lt;TResult1&gt;,
        ///     IEnumerable&lt;TResult2&gt;,
        ///     IEnumerable&lt;TResult3&gt;,
        ///     IEnumerable&lt;TResult4&gt;,
        ///     IEnumerable&lt;TResult5&gt;&gt;.
        /// </returns>
        public Tuple<IEnumerable<TResult1>,
            IEnumerable<TResult2>,
            IEnumerable<TResult3>,
            IEnumerable<TResult4>,
            IEnumerable<TResult5>>
            Execute(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters)
        {
            IEnumerable<TResult1> results1;
            IEnumerable<TResult2> results2 = null;
            IEnumerable<TResult3> results3 = null;
            IEnumerable<TResult4> results4 = null;
            IEnumerable<TResult5> results5 = null;

            ICommandExpression<TFilter> commandExpression = CommandExpression;
            var parameters = commandExpression.BuildParameters(databaseManager, filter, explicitParameters);

            bool cachedItemPresent = false;
            BigInteger hashCode = BigInteger.Zero;
            Tuple<IEnumerable<TResult1>,
                IEnumerable<TResult2>,
                IEnumerable<TResult3>,
                IEnumerable<TResult4>,
                IEnumerable<TResult5>> finalResults = null;
            if (ResultCachingEnabled)
            {
                var parameterAggregate = parameters.Aggregate(string.Empty, (p, c) => p + (c.ParameterName + (c.Value ?? string.Empty).ToString()));

                hashCode = FnvHash.GetHash(parameterAggregate, 128);

                object value = null;
                TryRetrieveCacheResult(hashCode, out value);

                finalResults = value as Tuple<IEnumerable<TResult1>,
                                                IEnumerable<TResult2>,
                                                IEnumerable<TResult3>,
                                                IEnumerable<TResult4>,
                                                IEnumerable<TResult5>>;
                cachedItemPresent = finalResults != null;
            }

            if (!cachedItemPresent)
            {
                using (IDataReader record = databaseManager
                    .ExecuteDataReader(
                        commandExpression.CommandText,
                        commandExpression.DbCommandType,
                        parameters))
                {
                    //This can be replaced with a loop once there is a non-generic IResultMapper interface, I think.

                    results1 = _item1Mapper.MapResult(record);

                    if (record.NextResult())
                    {
                        results2 = _item2Mapper.MapResult(record);

                        if (record.NextResult())
                        {
                            results3 = _item3Mapper.MapResult(record);

                            if (record.NextResult())
                            {
                                results4 = _item4Mapper.MapResult(record);

                                if (record.NextResult())
                                {
                                    results5 = _item5Mapper.MapResult(record);
                                }
                            }
                        }
                    }
                }

                finalResults = new Tuple<IEnumerable<TResult1>,
            IEnumerable<TResult2>,
            IEnumerable<TResult3>,
            IEnumerable<TResult4>,
            IEnumerable<TResult5>>(results1, results2, results3, results4, results5);

                if (ResultCachingEnabled)
                    ResultCacheContainer.TryAdd(hashCode, new CacheItem(finalResults, ResultCachingMode, ResultCachingInterval));
            }

            return finalResults;
        }

        /// <summary>
        ///     Executes the command using a provided database manager and optionally parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>
        ///     Tuple&lt;
        ///     IEnumerable&lt;TResult1&gt;,
        ///     IEnumerable&lt;TResult2&gt;,
        ///     IEnumerable&lt;TResult3&gt;,
        ///     IEnumerable&lt;TResult4&gt;,
        ///     IEnumerable&lt;TResult5&gt;&gt;.
        /// </returns>
        public Tuple<IEnumerable<TResult1>,
            IEnumerable<TResult2>,
            IEnumerable<TResult3>,
            IEnumerable<TResult4>,
            IEnumerable<TResult5>> Execute(IDatabaseManager databaseManager, params DbParameter[] explicitParameters)
        {
            return Execute(databaseManager, default(TFilter), explicitParameters);
        }

        /// <summary>
        /// Enables the result caching.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="interval">The interval.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5&gt;.</returns>
        public ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5> EnableResultCaching(CacheMode mode = CacheMode.Permanent, double? interval = null)
        {
            ActivateResultCaching(mode, interval);

            return this;
        }
    }

    /// <summary>
    ///     A fully built and ready to be executed command expression with appropriate mapping expressions compiled and a
    ///     filter parameter.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the result1.</typeparam>
    /// <typeparam name="TResult2">The type of the result2.</typeparam>
    /// <typeparam name="TResult3">The type of the result3.</typeparam>
    /// <typeparam name="TResult4">The type of the result4.</typeparam>
    /// <typeparam name="TResult5">The type of the result5.</typeparam>
    /// <typeparam name="TResult6">The type of the result6.</typeparam>
    /// <remarks>Appropriate mapping expressions are compiled at the point this interface becomes available.</remarks>
    public class MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>
        : CommandProcessorCommon, ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>
        where TResult1 : new()
        where TResult2 : new()
        where TResult3 : new()
        where TResult4 : new()
        where TResult5 : new()
        where TResult6 : new()
    {
        private readonly ICommandExpression<TFilter> _commandExpression;

        private readonly ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>
            _commandResultExpression;

        private readonly IResultMapper<TResult1> _item1Mapper;
        private readonly IResultMapper<TResult2> _item2Mapper;
        private readonly IResultMapper<TResult3> _item3Mapper;
        private readonly IResultMapper<TResult4> _item4Mapper;
        private readonly IResultMapper<TResult5> _item5Mapper;
        private readonly IResultMapper<TResult6> _item6Mapper;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="MultipleResultSetCommandProcessor{TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6}" />
        /// class.
        /// </summary>
        /// <param name="commandResultExpression">The command result expression.</param>
        /// <param name="name">The name.</param>
        public MultipleResultSetCommandProcessor(
            ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>
                commandResultExpression, string name)
            : base(name)
        {
            _commandExpression = commandResultExpression.CommandExpression;
            _commandResultExpression = commandResultExpression;

            _item1Mapper =
                new SingleResultSetCommandProcessor<TFilter, TResult1>(
                    CommandResultExpression.ToSingleResult<TResult1>(), null);
            _item2Mapper =
                new SingleResultSetCommandProcessor<TFilter, TResult2>(
                    CommandResultExpression.ToSingleResult<TResult2>(), null);
            _item3Mapper =
                new SingleResultSetCommandProcessor<TFilter, TResult3>(
                    CommandResultExpression.ToSingleResult<TResult3>(), null);
            _item4Mapper =
                new SingleResultSetCommandProcessor<TFilter, TResult4>(
                    CommandResultExpression.ToSingleResult<TResult4>(), null);
            _item5Mapper =
                new SingleResultSetCommandProcessor<TFilter, TResult5>(
                    CommandResultExpression.ToSingleResult<TResult5>(), null);
            _item6Mapper =
                new SingleResultSetCommandProcessor<TFilter, TResult6>(
                    CommandResultExpression.ToSingleResult<TResult6>(), null);

            CommandManager.RegisterCommandProcessor(this, name);
        }

        /// <summary>
        ///     Gets the command result expression.
        /// </summary>
        /// <value>The command result expression.</value>
        public ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>
            CommandResultExpression
        {
            get { return _commandResultExpression; }
        }

        /// <summary>
        ///     Gets the command expression.
        /// </summary>
        /// <value>The command expression.</value>
        public ICommandExpression<TFilter> CommandExpression
        {
            get { return _commandExpression; }
        }

        /// <summary>
        ///     Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public override BigInteger CacheHash
        {
            get
            {
                return (_item1Mapper.CacheHash * 31)
                     ^ (_item2Mapper.CacheHash * 31)
                     ^ (_item3Mapper.CacheHash * 31)
                     ^ (_item4Mapper.CacheHash * 31)
                     ^ (_item5Mapper.CacheHash * 31)
                     ^ (_item6Mapper.CacheHash * 31);
            }
        }

        /// <summary>
        ///     Executes the command using a provided database manager
        ///     and optionally a filter to read parameters from and explicit parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>
        ///     Tuple&lt;
        ///     IEnumerable&lt;TResult1&gt;,
        ///     IEnumerable&lt;TResult2&gt;,
        ///     IEnumerable&lt;TResult3&gt;,
        ///     IEnumerable&lt;TResult4&gt;,
        ///     IEnumerable&lt;TResult5&gt;,
        ///     IEnumerable&lt;TResult6&gt;&gt;.
        /// </returns>
        public Tuple<IEnumerable<TResult1>,
            IEnumerable<TResult2>,
            IEnumerable<TResult3>,
            IEnumerable<TResult4>,
            IEnumerable<TResult5>,
            IEnumerable<TResult6>>
            Execute(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters)
        {
            IEnumerable<TResult1> results1;
            IEnumerable<TResult2> results2 = null;
            IEnumerable<TResult3> results3 = null;
            IEnumerable<TResult4> results4 = null;
            IEnumerable<TResult5> results5 = null;
            IEnumerable<TResult6> results6 = null;

            ICommandExpression<TFilter> commandExpression = CommandExpression;
            var parameters = commandExpression.BuildParameters(databaseManager, filter, explicitParameters);

            bool cachedItemPresent = false;
            BigInteger hashCode = BigInteger.Zero;
            Tuple<IEnumerable<TResult1>,
                IEnumerable<TResult2>,
                IEnumerable<TResult3>,
                IEnumerable<TResult4>,
                IEnumerable<TResult5>,
                IEnumerable<TResult6>> finalResults = null;
            if (ResultCachingEnabled)
            {
                var parameterAggregate = parameters.Aggregate(string.Empty, (p, c) => p + (c.ParameterName + (c.Value ?? string.Empty).ToString()));

                hashCode = FnvHash.GetHash(parameterAggregate, 128);

                object value = null;
                TryRetrieveCacheResult(hashCode, out value);

                finalResults = value as Tuple<IEnumerable<TResult1>,
                                                IEnumerable<TResult2>,
                                                IEnumerable<TResult3>,
                                                IEnumerable<TResult4>,
                                                IEnumerable<TResult5>,
                                                IEnumerable<TResult6>>;
                cachedItemPresent = finalResults != null;
            }

            if (!cachedItemPresent)
            {
                using (IDataReader record = databaseManager
                    .ExecuteDataReader(
                        commandExpression.CommandText,
                        commandExpression.DbCommandType,
                        parameters))
                {
                    //This can be replaced with a loop once there is a non-generic IResultMapper interface, I think.

                    results1 = _item1Mapper.MapResult(record);

                    if (record.NextResult())
                    {
                        results2 = _item2Mapper.MapResult(record);

                        if (record.NextResult())
                        {
                            results3 = _item3Mapper.MapResult(record);

                            if (record.NextResult())
                            {
                                results4 = _item4Mapper.MapResult(record);

                                if (record.NextResult())
                                {
                                    results5 = _item5Mapper.MapResult(record);

                                    if (record.NextResult())
                                    {
                                        results6 = _item6Mapper.MapResult(record);
                                    }
                                }
                            }
                        }
                    }
                }

                finalResults = new Tuple<IEnumerable<TResult1>,
            IEnumerable<TResult2>,
            IEnumerable<TResult3>,
            IEnumerable<TResult4>,
            IEnumerable<TResult5>,
            IEnumerable<TResult6>>(results1, results2, results3, results4, results5, results6);

                if (ResultCachingEnabled)
                    ResultCacheContainer.TryAdd(hashCode, new CacheItem(finalResults, ResultCachingMode, ResultCachingInterval));
            }

            return finalResults;
        }

        /// <summary>
        ///     Executes the command using a provided database manager and optionally parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>
        ///     Tuple&lt;
        ///     IEnumerable&lt;TResult1&gt;,
        ///     IEnumerable&lt;TResult2&gt;,
        ///     IEnumerable&lt;TResult3&gt;,
        ///     IEnumerable&lt;TResult4&gt;,
        ///     IEnumerable&lt;TResult5&gt;,
        ///     IEnumerable&lt;TResult6&gt;&gt;.
        /// </returns>
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
        /// Enables the result caching.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="interval">The interval.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6&gt;.</returns>
        public ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6> EnableResultCaching(CacheMode mode = CacheMode.Permanent, double? interval = null)
        {
            ActivateResultCaching(mode, interval);

            return this;
        }
    }

    /// <summary>
    ///     A fully built and ready to be executed command expression with appropriate mapping expressions compiled and a
    ///     filter parameter.
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
        : CommandProcessorCommon, ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>
        where TResult1 : new()
        where TResult2 : new()
        where TResult3 : new()
        where TResult4 : new()
        where TResult5 : new()
        where TResult6 : new()
        where TResult7 : new()
    {
        private readonly ICommandExpression<TFilter> _commandExpression;

        private readonly
            ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>
            _commandResultExpression;

        private readonly IResultMapper<TResult1> _item1Mapper;
        private readonly IResultMapper<TResult2> _item2Mapper;
        private readonly IResultMapper<TResult3> _item3Mapper;
        private readonly IResultMapper<TResult4> _item4Mapper;
        private readonly IResultMapper<TResult5> _item5Mapper;
        private readonly IResultMapper<TResult6> _item6Mapper;
        private readonly IResultMapper<TResult7> _item7Mapper;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="MultipleResultSetCommandProcessor{TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7}" />
        /// class.
        /// </summary>
        /// <param name="commandResultExpression">The command result expression.</param>
        /// <param name="name">The name.</param>
        public MultipleResultSetCommandProcessor(
            ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>
                commandResultExpression, string name)
            : base(name)
        {
            _commandExpression = commandResultExpression.CommandExpression;
            _commandResultExpression = commandResultExpression;

            _item1Mapper =
                new SingleResultSetCommandProcessor<TFilter, TResult1>(
                    CommandResultExpression.ToSingleResult<TResult1>(), null);
            _item2Mapper =
                new SingleResultSetCommandProcessor<TFilter, TResult2>(
                    CommandResultExpression.ToSingleResult<TResult2>(), null);
            _item3Mapper =
                new SingleResultSetCommandProcessor<TFilter, TResult3>(
                    CommandResultExpression.ToSingleResult<TResult3>(), null);
            _item4Mapper =
                new SingleResultSetCommandProcessor<TFilter, TResult4>(
                    CommandResultExpression.ToSingleResult<TResult4>(), null);
            _item5Mapper =
                new SingleResultSetCommandProcessor<TFilter, TResult5>(
                    CommandResultExpression.ToSingleResult<TResult5>(), null);
            _item6Mapper =
                new SingleResultSetCommandProcessor<TFilter, TResult6>(
                    CommandResultExpression.ToSingleResult<TResult6>(), null);
            _item7Mapper =
                new SingleResultSetCommandProcessor<TFilter, TResult7>(
                    CommandResultExpression.ToSingleResult<TResult7>(), null);

            CommandManager.RegisterCommandProcessor(this, name);
        }

        /// <summary>
        ///     Gets the command result expression.
        /// </summary>
        /// <value>The command result expression.</value>
        public ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>
            CommandResultExpression
        {
            get { return _commandResultExpression; }
        }

        /// <summary>
        ///     Gets the command expression.
        /// </summary>
        /// <value>The command expression.</value>
        public ICommandExpression<TFilter> CommandExpression
        {
            get { return _commandExpression; }
        }

        /// <summary>
        ///     Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public override BigInteger CacheHash
        {
            get
            {
                return (_item1Mapper.CacheHash * 31)
                     ^ (_item2Mapper.CacheHash * 31)
                     ^ (_item3Mapper.CacheHash * 31)
                     ^ (_item4Mapper.CacheHash * 31)
                     ^ (_item5Mapper.CacheHash * 31)
                     ^ (_item6Mapper.CacheHash * 31)
                     ^ (_item7Mapper.CacheHash * 31);
            }
        }

        /// <summary>
        ///     Executes the command using a provided database manager and optionally a filter to read parameters from and explicit
        ///     parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>
        ///     Tuple&lt;
        ///     IEnumerable&lt;TResult1&gt;,
        ///     IEnumerable&lt;TResult2&gt;,
        ///     IEnumerable&lt;TResult3&gt;,
        ///     IEnumerable&lt;TResult4&gt;,
        ///     IEnumerable&lt;TResult5&gt;,
        ///     IEnumerable&lt;TResult6&gt;,
        ///     IEnumerable&lt;TResult7&gt;&gt;.
        /// </returns>
        public Tuple<IEnumerable<TResult1>,
            IEnumerable<TResult2>,
            IEnumerable<TResult3>,
            IEnumerable<TResult4>,
            IEnumerable<TResult5>,
            IEnumerable<TResult6>,
            IEnumerable<TResult7>>
            Execute(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters)
        {
            IEnumerable<TResult1> results1;
            IEnumerable<TResult2> results2 = null;
            IEnumerable<TResult3> results3 = null;
            IEnumerable<TResult4> results4 = null;
            IEnumerable<TResult5> results5 = null;
            IEnumerable<TResult6> results6 = null;
            IEnumerable<TResult7> results7 = null;

            ICommandExpression<TFilter> commandExpression = CommandExpression;
            var parameters = commandExpression.BuildParameters(databaseManager, filter, explicitParameters);

            bool cachedItemPresent = false;
            BigInteger hashCode = BigInteger.Zero;
            Tuple<IEnumerable<TResult1>,
                IEnumerable<TResult2>,
                IEnumerable<TResult3>,
                IEnumerable<TResult4>,
                IEnumerable<TResult5>,
                IEnumerable<TResult6>,
                IEnumerable<TResult7>> finalResults = null;
            if (ResultCachingEnabled)
            {
                var parameterAggregate = parameters.Aggregate(string.Empty, (p, c) => p + (c.ParameterName + (c.Value ?? string.Empty).ToString()));

                hashCode = FnvHash.GetHash(parameterAggregate, 128);

                object value = null;
                TryRetrieveCacheResult(hashCode, out value);

                finalResults = value as Tuple<IEnumerable<TResult1>,
                                                IEnumerable<TResult2>,
                                                IEnumerable<TResult3>,
                                                IEnumerable<TResult4>,
                                                IEnumerable<TResult5>,
                                                IEnumerable<TResult6>,
                                                IEnumerable<TResult7>>;
                cachedItemPresent = finalResults != null;
            }

            if (!cachedItemPresent)
            {
                using (IDataReader record = databaseManager
                    .ExecuteDataReader(
                        commandExpression.CommandText,
                        commandExpression.DbCommandType,
                        parameters))
                {
                    //This can be replaced with a loop once there is a non-generic IResultMapper interface, I think.

                    results1 = _item1Mapper.MapResult(record);

                    if (record.NextResult())
                    {
                        results2 = _item2Mapper.MapResult(record);

                        if (record.NextResult())
                        {
                            results3 = _item3Mapper.MapResult(record);

                            if (record.NextResult())
                            {
                                results4 = _item4Mapper.MapResult(record);

                                if (record.NextResult())
                                {
                                    results5 = _item5Mapper.MapResult(record);

                                    if (record.NextResult())
                                    {
                                        results6 = _item6Mapper.MapResult(record);

                                        if (record.NextResult())
                                        {
                                            results7 = _item7Mapper.MapResult(record);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                finalResults = new Tuple<IEnumerable<TResult1>,
            IEnumerable<TResult2>,
            IEnumerable<TResult3>,
            IEnumerable<TResult4>,
            IEnumerable<TResult5>,
            IEnumerable<TResult6>,
            IEnumerable<TResult7>>(results1, results2, results3, results4, results5, results6, results7);

                if (ResultCachingEnabled)
                    ResultCacheContainer.TryAdd(hashCode, new CacheItem(finalResults, ResultCachingMode, ResultCachingInterval));
            }

            return finalResults;
        }

        /// <summary>
        ///     Executes the command using a provided database manager and optionally parameters.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>
        ///     Tuple&lt;
        ///     IEnumerable&lt;TResult1&gt;,
        ///     IEnumerable&lt;TResult2&gt;,
        ///     IEnumerable&lt;TResult3&gt;,
        ///     IEnumerable&lt;TResult4&gt;,
        ///     IEnumerable&lt;TResult5&gt;,
        ///     IEnumerable&lt;TResult6&gt;,
        ///     IEnumerable&lt;TResult7&gt;&gt;.
        /// </returns>
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
        /// Enables the result caching.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="interval">The interval.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7&gt;.</returns>
        public ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7> EnableResultCaching(CacheMode mode = CacheMode.Permanent, double? interval = null)
        {
            ActivateResultCaching(mode, interval);

            return this;
        }
    }
}