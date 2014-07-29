using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Susanoo
{
    /// <summary>
    /// A fully built and ready to be executed command expression with appropriate mapping expressions compiled and a filter parameter.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the result1.</typeparam>
    /// <typeparam name="TResult2">The type of the result2.</typeparam>
    /// <remarks>Appropriate mapping expressions are compiled at the point this interface becomes available.</remarks>
    public class MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2>
        : ICommandProcessor<TFilter, TResult1, TResult2>, IFluentPipelineFragment
        where TResult1 : new()
        where TResult2 : new()
    {
        private readonly ICommandResultExpression<TFilter, TResult1, TResult2> _CommandResultExpression;

        private readonly ICommandExpression<TFilter> _CommandExpression;

        private readonly IResultMapper<TResult1> _Item1Mapper;
        private readonly IResultMapper<TResult2> _Item2Mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleResultSetCommandProcessor{TFilter, TResult1, TResult2}"/> class.
        /// </summary>
        /// <param name="commandResultExpression">The command result expression.</param>
        public MultipleResultSetCommandProcessor(ICommandResultExpression<TFilter, TResult1, TResult2> commandResultExpression)
        {
            this._CommandExpression = commandResultExpression.CommandExpression;
            this._CommandResultExpression = commandResultExpression;

            _Item1Mapper = new SingleResultSetCommandProcessor<TFilter, TResult1>(CommandResultExpression.ToSingleResult<TResult1>());
            _Item2Mapper = new SingleResultSetCommandProcessor<TFilter, TResult2>(CommandResultExpression.ToSingleResult<TResult2>());
        }

        /// <summary>
        /// Gets the command result expression.
        /// </summary>
        /// <value>The command result expression.</value>
        public ICommandResultExpression<TFilter, TResult1, TResult2> CommandResultExpression
        {
            get { return this._CommandResultExpression; }
        }

        /// <summary>
        /// Gets the command expression.
        /// </summary>
        /// <value>The command expression.</value>
        public ICommandExpression<TFilter> CommandExpression
        {
            get { return this._CommandExpression; }
        }

        /// <summary>
        /// Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public System.Numerics.BigInteger CacheHash
        {
            get { return (_Item1Mapper.CacheHash * 31) ^ _Item2Mapper.CacheHash; }
        }

        public Tuple<IEnumerable<TResult1>, IEnumerable<TResult2>> Execute(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters)
        {
            IEnumerable<TResult1> results1 = null;
            IEnumerable<TResult2> results2 = null;

            ICommandExpression<TFilter> commandExpression = this.CommandExpression;

            using (IDataReader record = databaseManager
                .ExecuteDataReader(
                    commandExpression.CommandText,
                    commandExpression.DBCommandType,
                    null,
                    commandExpression.BuildParameters(databaseManager, filter, explicitParameters)))
            {
                results1 = _Item1Mapper.MapResult(record);

                if (record.NextResult())
                {
                    results2 = _Item2Mapper.MapResult(record);
                }
            }

            return new Tuple<IEnumerable<TResult1>, IEnumerable<TResult2>>(results1, results2);
        }

        public Tuple<IEnumerable<TResult1>, IEnumerable<TResult2>> Execute(IDatabaseManager databaseManager, params DbParameter[] explicitParameters)
        {
            return this.Execute(databaseManager, default(TFilter), explicitParameters);
        }
    }

    /// <summary>
    /// A fully built and ready to be executed command expression with appropriate mapping expressions compiled and a filter parameter.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the result1.</typeparam>
    /// <typeparam name="TResult2">The type of the result2.</typeparam>
    /// <typeparam name="TResult3">The type of the result3.</typeparam>
    /// <remarks>Appropriate mapping expressions are compiled at the point this interface becomes available.</remarks>
    public class MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2, TResult3>
    : ICommandProcessor<TFilter, TResult1, TResult2, TResult3>, IFluentPipelineFragment
        where TResult1 : new()
        where TResult2 : new()
        where TResult3 : new()
    {
        private readonly ICommandResultExpression<TFilter, TResult1, TResult2, TResult3> _CommandResultExpression;

        private readonly ICommandExpression<TFilter> _CommandExpression;

        private readonly IResultMapper<TResult1> _Item1Mapper;
        private readonly IResultMapper<TResult2> _Item2Mapper;
        private readonly IResultMapper<TResult3> _Item3Mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleResultSetCommandProcessor{TFilter, TResult1, TResult2, TResult3}"/> class.
        /// </summary>
        /// <param name="commandResultExpression">The command result expression.</param>
        public MultipleResultSetCommandProcessor(ICommandResultExpression<TFilter, TResult1, TResult2, TResult3> commandResultExpression)
        {
            this._CommandExpression = commandResultExpression.CommandExpression;
            this._CommandResultExpression = commandResultExpression;

            _Item1Mapper = new SingleResultSetCommandProcessor<TFilter, TResult1>(CommandResultExpression.ToSingleResult<TResult1>());
            _Item2Mapper = new SingleResultSetCommandProcessor<TFilter, TResult2>(CommandResultExpression.ToSingleResult<TResult2>());
            _Item3Mapper = new SingleResultSetCommandProcessor<TFilter, TResult3>(CommandResultExpression.ToSingleResult<TResult3>());
        }

        /// <summary>
        /// Gets the command result expression.
        /// </summary>
        /// <value>The command result expression.</value>
        public ICommandResultExpression<TFilter, TResult1, TResult2, TResult3> CommandResultExpression
        {
            get { return this._CommandResultExpression; }
        }

        /// <summary>
        /// Gets the command expression.
        /// </summary>
        /// <value>The command expression.</value>
        public ICommandExpression<TFilter> CommandExpression
        {
            get { return this._CommandExpression; }
        }

        /// <summary>
        /// Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public System.Numerics.BigInteger CacheHash
        {
            get { return (_Item1Mapper.CacheHash * 31) ^ _Item2Mapper.CacheHash; }
        }

        public Tuple<IEnumerable<TResult1>, IEnumerable<TResult2>, IEnumerable<TResult3>>
            Execute(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters)
        {
            IEnumerable<TResult1> results1 = null;
            IEnumerable<TResult2> results2 = null;
            IEnumerable<TResult3> results3 = null;

            ICommandExpression<TFilter> commandExpression = this.CommandExpression;

            using (IDataReader record = databaseManager
                .ExecuteDataReader(
                    commandExpression.CommandText,
                    commandExpression.DBCommandType,
                    null,
                    commandExpression.BuildParameters(databaseManager, filter, explicitParameters)))
            {
                results1 = _Item1Mapper.MapResult(record);

                if (record.NextResult())
                {
                    results2 = _Item2Mapper.MapResult(record);

                    if (record.NextResult())
                    {
                        results3 = _Item3Mapper.MapResult(record);
                    }
                }
            }

            return new Tuple<IEnumerable<TResult1>, IEnumerable<TResult2>, IEnumerable<TResult3>>(results1, results2, results3);
        }

        public Tuple<IEnumerable<TResult1>, IEnumerable<TResult2>, IEnumerable<TResult3>> Execute(IDatabaseManager databaseManager, params DbParameter[] explicitParameters)
        {
            return this.Execute(databaseManager, default(TFilter), explicitParameters);
        }
    }

    /// <summary>
    /// A fully built and ready to be executed command expression with appropriate mapping expressions compiled and a filter parameter.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the result1.</typeparam>
    /// <typeparam name="TResult2">The type of the result2.</typeparam>
    /// <typeparam name="TResult3">The type of the result3.</typeparam>
    /// <typeparam name="TResult4">The type of the result4.</typeparam>
    /// <remarks>Appropriate mapping expressions are compiled at the point this interface becomes available.</remarks>
    public class MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4>
    : ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4>, IFluentPipelineFragment
        where TResult1 : new()
        where TResult2 : new()
        where TResult3 : new()
        where TResult4 : new()
    {
        private readonly ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4> _CommandResultExpression;

        private readonly ICommandExpression<TFilter> _CommandExpression;

        private readonly IResultMapper<TResult1> _Item1Mapper;
        private readonly IResultMapper<TResult2> _Item2Mapper;
        private readonly IResultMapper<TResult3> _Item3Mapper;
        private readonly IResultMapper<TResult4> _Item4Mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleResultSetCommandProcessor{TFilter, TResult1, TResult2, TResult3, TResult4}"/> class.
        /// </summary>
        /// <param name="commandResultExpression">The command result expression.</param>
        public MultipleResultSetCommandProcessor(ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4>
            commandResultExpression)
        {
            this._CommandExpression = commandResultExpression.CommandExpression;
            this._CommandResultExpression = commandResultExpression;

            _Item1Mapper = new SingleResultSetCommandProcessor<TFilter, TResult1>(CommandResultExpression.ToSingleResult<TResult1>());
            _Item2Mapper = new SingleResultSetCommandProcessor<TFilter, TResult2>(CommandResultExpression.ToSingleResult<TResult2>());
            _Item3Mapper = new SingleResultSetCommandProcessor<TFilter, TResult3>(CommandResultExpression.ToSingleResult<TResult3>());
            _Item4Mapper = new SingleResultSetCommandProcessor<TFilter, TResult4>(CommandResultExpression.ToSingleResult<TResult4>());
        }

        /// <summary>
        /// Gets the command result expression.
        /// </summary>
        /// <value>The command result expression.</value>
        public ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4> CommandResultExpression
        {
            get { return this._CommandResultExpression; }
        }

        /// <summary>
        /// Gets the command expression.
        /// </summary>
        /// <value>The command expression.</value>
        public ICommandExpression<TFilter> CommandExpression
        {
            get { return this._CommandExpression; }
        }

        /// <summary>
        /// Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public System.Numerics.BigInteger CacheHash
        {
            get { return (_Item1Mapper.CacheHash * 31) ^ _Item2Mapper.CacheHash; }
        }

        public Tuple<IEnumerable<TResult1>,
                IEnumerable<TResult2>,
                IEnumerable<TResult3>,
                IEnumerable<TResult4>>
            Execute(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters)
        {
            IEnumerable<TResult1> results1 = null;
            IEnumerable<TResult2> results2 = null;
            IEnumerable<TResult3> results3 = null;
            IEnumerable<TResult4> results4 = null;

            ICommandExpression<TFilter> commandExpression = this.CommandExpression;

            using (IDataReader record = databaseManager
                .ExecuteDataReader(
                    commandExpression.CommandText,
                    commandExpression.DBCommandType,
                    null,
                    commandExpression.BuildParameters(databaseManager, filter, explicitParameters)))
            {
                results1 = _Item1Mapper.MapResult(record);

                if (record.NextResult())
                {
                    results2 = _Item2Mapper.MapResult(record);

                    if (record.NextResult())
                    {
                        results3 = _Item3Mapper.MapResult(record);

                        if (record.NextResult())
                        {
                            results4 = _Item4Mapper.MapResult(record);
                        }
                    }
                }
            }

            return new Tuple<IEnumerable<TResult1>,
                IEnumerable<TResult2>,
                IEnumerable<TResult3>,
                IEnumerable<TResult4>>(results1, results2, results3, results4);
        }

        public Tuple<IEnumerable<TResult1>,
                IEnumerable<TResult2>,
                IEnumerable<TResult3>,
                IEnumerable<TResult4>> Execute(IDatabaseManager databaseManager, params DbParameter[] explicitParameters)
        {
            return this.Execute(databaseManager, default(TFilter), explicitParameters);
        }
    }

    /// <summary>
    /// A fully built and ready to be executed command expression with appropriate mapping expressions compiled and a filter parameter.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the result1.</typeparam>
    /// <typeparam name="TResult2">The type of the result2.</typeparam>
    /// <typeparam name="TResult3">The type of the result3.</typeparam>
    /// <typeparam name="TResult4">The type of the result4.</typeparam>
    /// <typeparam name="TResult5">The type of the result5.</typeparam>
    /// <remarks>Appropriate mapping expressions are compiled at the point this interface becomes available.</remarks>
    public class MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5>
    : ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5>, IFluentPipelineFragment
        where TResult1 : new()
        where TResult2 : new()
        where TResult3 : new()
        where TResult4 : new()
        where TResult5 : new()
    {
        private readonly ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5> _CommandResultExpression;

        private readonly ICommandExpression<TFilter> _CommandExpression;

        private readonly IResultMapper<TResult1> _Item1Mapper;
        private readonly IResultMapper<TResult2> _Item2Mapper;
        private readonly IResultMapper<TResult3> _Item3Mapper;
        private readonly IResultMapper<TResult4> _Item4Mapper;
        private readonly IResultMapper<TResult5> _Item5Mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleResultSetCommandProcessor{TFilter, TResult1, TResult2, TResult3, TResult4, TResult5}"/> class.
        /// </summary>
        /// <param name="commandResultExpression">The command result expression.</param>
        public MultipleResultSetCommandProcessor(ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5>
            commandResultExpression)
        {
            this._CommandExpression = commandResultExpression.CommandExpression;
            this._CommandResultExpression = commandResultExpression;

            _Item1Mapper = new SingleResultSetCommandProcessor<TFilter, TResult1>(CommandResultExpression.ToSingleResult<TResult1>());
            _Item2Mapper = new SingleResultSetCommandProcessor<TFilter, TResult2>(CommandResultExpression.ToSingleResult<TResult2>());
            _Item3Mapper = new SingleResultSetCommandProcessor<TFilter, TResult3>(CommandResultExpression.ToSingleResult<TResult3>());
            _Item4Mapper = new SingleResultSetCommandProcessor<TFilter, TResult4>(CommandResultExpression.ToSingleResult<TResult4>());
            _Item5Mapper = new SingleResultSetCommandProcessor<TFilter, TResult5>(CommandResultExpression.ToSingleResult<TResult5>());
        }

        /// <summary>
        /// Gets the command result expression.
        /// </summary>
        /// <value>The command result expression.</value>
        public ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5> CommandResultExpression
        {
            get { return this._CommandResultExpression; }
        }

        /// <summary>
        /// Gets the command expression.
        /// </summary>
        /// <value>The command expression.</value>
        public ICommandExpression<TFilter> CommandExpression
        {
            get { return this._CommandExpression; }
        }

        /// <summary>
        /// Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public System.Numerics.BigInteger CacheHash
        {
            get { return (_Item1Mapper.CacheHash * 31) ^ _Item2Mapper.CacheHash; }
        }

        public Tuple<IEnumerable<TResult1>,
                IEnumerable<TResult2>,
                IEnumerable<TResult3>,
                IEnumerable<TResult4>,
                IEnumerable<TResult5>>
            Execute(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters)
        {
            IEnumerable<TResult1> results1 = null;
            IEnumerable<TResult2> results2 = null;
            IEnumerable<TResult3> results3 = null;
            IEnumerable<TResult4> results4 = null;
            IEnumerable<TResult5> results5 = null;

            ICommandExpression<TFilter> commandExpression = this.CommandExpression;

            using (IDataReader record = databaseManager
                .ExecuteDataReader(
                    commandExpression.CommandText,
                    commandExpression.DBCommandType,
                    null,
                    commandExpression.BuildParameters(databaseManager, filter, explicitParameters)))
            {
                results1 = _Item1Mapper.MapResult(record);

                if (record.NextResult())
                {
                    results2 = _Item2Mapper.MapResult(record);

                    if (record.NextResult())
                    {
                        results3 = _Item3Mapper.MapResult(record);

                        if (record.NextResult())
                        {
                            results4 = _Item4Mapper.MapResult(record);

                            if (record.NextResult())
                            {
                                results5 = _Item5Mapper.MapResult(record);
                            }
                        }
                    }
                }
            }

            return new Tuple<IEnumerable<TResult1>,
                IEnumerable<TResult2>,
                IEnumerable<TResult3>,
                IEnumerable<TResult4>,
                IEnumerable<TResult5>>(results1, results2, results3, results4, results5);
        }

        public Tuple<IEnumerable<TResult1>,
                IEnumerable<TResult2>,
                IEnumerable<TResult3>,
                IEnumerable<TResult4>,
                IEnumerable<TResult5>> Execute(IDatabaseManager databaseManager, params DbParameter[] explicitParameters)
        {
            return this.Execute(databaseManager, default(TFilter), explicitParameters);
        }
    }

    /// <summary>
    /// A fully built and ready to be executed command expression with appropriate mapping expressions compiled and a filter parameter.
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
    : ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>, IFluentPipelineFragment
        where TResult1 : new()
        where TResult2 : new()
        where TResult3 : new()
        where TResult4 : new()
        where TResult5 : new()
        where TResult6 : new()
    {
        private readonly ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6> _CommandResultExpression;

        private readonly ICommandExpression<TFilter> _CommandExpression;

        private readonly IResultMapper<TResult1> _Item1Mapper;
        private readonly IResultMapper<TResult2> _Item2Mapper;
        private readonly IResultMapper<TResult3> _Item3Mapper;
        private readonly IResultMapper<TResult4> _Item4Mapper;
        private readonly IResultMapper<TResult5> _Item5Mapper;
        private readonly IResultMapper<TResult6> _Item6Mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleResultSetCommandProcessor{TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6}"/> class.
        /// </summary>
        /// <param name="commandResultExpression">The command result expression.</param>
        public MultipleResultSetCommandProcessor(ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>
            commandResultExpression)
        {
            this._CommandExpression = commandResultExpression.CommandExpression;
            this._CommandResultExpression = commandResultExpression;

            _Item1Mapper = new SingleResultSetCommandProcessor<TFilter, TResult1>(CommandResultExpression.ToSingleResult<TResult1>());
            _Item2Mapper = new SingleResultSetCommandProcessor<TFilter, TResult2>(CommandResultExpression.ToSingleResult<TResult2>());
            _Item3Mapper = new SingleResultSetCommandProcessor<TFilter, TResult3>(CommandResultExpression.ToSingleResult<TResult3>());
            _Item4Mapper = new SingleResultSetCommandProcessor<TFilter, TResult4>(CommandResultExpression.ToSingleResult<TResult4>());
            _Item5Mapper = new SingleResultSetCommandProcessor<TFilter, TResult5>(CommandResultExpression.ToSingleResult<TResult5>());
            _Item6Mapper = new SingleResultSetCommandProcessor<TFilter, TResult6>(CommandResultExpression.ToSingleResult<TResult6>());
        }

        public ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6> CommandResultExpression
        {
            get { return this._CommandResultExpression; }
        }

        /// <summary>
        /// Gets the command expression.
        /// </summary>
        /// <value>The command expression.</value>
        public ICommandExpression<TFilter> CommandExpression
        {
            get { return this._CommandExpression; }
        }

        /// <summary>
        /// Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public System.Numerics.BigInteger CacheHash
        {
            get { return (_Item1Mapper.CacheHash * 31) ^ _Item2Mapper.CacheHash; }
        }

        public Tuple<IEnumerable<TResult1>,
                IEnumerable<TResult2>,
                IEnumerable<TResult3>,
                IEnumerable<TResult4>,
                IEnumerable<TResult5>,
                IEnumerable<TResult6>>
            Execute(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters)
        {
            IEnumerable<TResult1> results1 = null;
            IEnumerable<TResult2> results2 = null;
            IEnumerable<TResult3> results3 = null;
            IEnumerable<TResult4> results4 = null;
            IEnumerable<TResult5> results5 = null;
            IEnumerable<TResult6> results6 = null;

            ICommandExpression<TFilter> commandExpression = this.CommandExpression;

            using (IDataReader record = databaseManager
                .ExecuteDataReader(
                    commandExpression.CommandText,
                    commandExpression.DBCommandType,
                    null,
                    commandExpression.BuildParameters(databaseManager, filter, explicitParameters)))
            {
                results1 = _Item1Mapper.MapResult(record);

                if (record.NextResult())
                {
                    results2 = _Item2Mapper.MapResult(record);

                    if (record.NextResult())
                    {
                        results3 = _Item3Mapper.MapResult(record);

                        if (record.NextResult())
                        {
                            results4 = _Item4Mapper.MapResult(record);

                            if (record.NextResult())
                            {
                                results5 = _Item5Mapper.MapResult(record);

                                if (record.NextResult())
                                {
                                    results6 = _Item6Mapper.MapResult(record);
                                }
                            }
                        }
                    }
                }
            }

            return new Tuple<IEnumerable<TResult1>,
                IEnumerable<TResult2>,
                IEnumerable<TResult3>,
                IEnumerable<TResult4>,
                IEnumerable<TResult5>,
                IEnumerable<TResult6>>(results1, results2, results3, results4, results5, results6);
        }

        public Tuple<IEnumerable<TResult1>,
                IEnumerable<TResult2>,
                IEnumerable<TResult3>,
                IEnumerable<TResult4>,
                IEnumerable<TResult5>,
                IEnumerable<TResult6>> Execute(IDatabaseManager databaseManager, params DbParameter[] explicitParameters)
        {
            return this.Execute(databaseManager, default(TFilter), explicitParameters);
        }
    }

    /// <summary>
    /// A fully built and ready to be executed command expression with appropriate mapping expressions compiled and a filter parameter.
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
    public class MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>
        : ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>, IFluentPipelineFragment
        where TResult1 : new()
        where TResult2 : new()
        where TResult3 : new()
        where TResult4 : new()
        where TResult5 : new()
        where TResult6 : new()
        where TResult7 : new()
    {
        private readonly ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7> _CommandResultExpression;

        private readonly ICommandExpression<TFilter> _CommandExpression;

        private readonly IResultMapper<TResult1> _Item1Mapper;
        private readonly IResultMapper<TResult2> _Item2Mapper;
        private readonly IResultMapper<TResult3> _Item3Mapper;
        private readonly IResultMapper<TResult4> _Item4Mapper;
        private readonly IResultMapper<TResult5> _Item5Mapper;
        private readonly IResultMapper<TResult6> _Item6Mapper;
        private readonly IResultMapper<TResult7> _Item7Mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleResultSetCommandProcessor{TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7}"/> class.
        /// </summary>
        /// <param name="commandResultExpression">The command result expression.</param>
        public MultipleResultSetCommandProcessor(ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>
            commandResultExpression)
        {
            this._CommandExpression = commandResultExpression.CommandExpression;
            this._CommandResultExpression = commandResultExpression;

            _Item1Mapper = new SingleResultSetCommandProcessor<TFilter, TResult1>(CommandResultExpression.ToSingleResult<TResult1>());
            _Item2Mapper = new SingleResultSetCommandProcessor<TFilter, TResult2>(CommandResultExpression.ToSingleResult<TResult2>());
            _Item3Mapper = new SingleResultSetCommandProcessor<TFilter, TResult3>(CommandResultExpression.ToSingleResult<TResult3>());
            _Item4Mapper = new SingleResultSetCommandProcessor<TFilter, TResult4>(CommandResultExpression.ToSingleResult<TResult4>());
            _Item5Mapper = new SingleResultSetCommandProcessor<TFilter, TResult5>(CommandResultExpression.ToSingleResult<TResult5>());
            _Item6Mapper = new SingleResultSetCommandProcessor<TFilter, TResult6>(CommandResultExpression.ToSingleResult<TResult6>());
            _Item7Mapper = new SingleResultSetCommandProcessor<TFilter, TResult7>(CommandResultExpression.ToSingleResult<TResult7>());
        }

        /// <summary>
        /// Gets the command result expression.
        /// </summary>
        /// <value>The command result expression.</value>
        public ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7> CommandResultExpression
        {
            get { return this._CommandResultExpression; }
        }

        /// <summary>
        /// Gets the command expression.
        /// </summary>
        /// <value>The command expression.</value>
        public ICommandExpression<TFilter> CommandExpression
        {
            get { return this._CommandExpression; }
        }

        /// <summary>
        /// Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public System.Numerics.BigInteger CacheHash
        {
            get { return (_Item1Mapper.CacheHash * 31) ^ _Item2Mapper.CacheHash; }
        }

        public Tuple<IEnumerable<TResult1>,
                IEnumerable<TResult2>,
                IEnumerable<TResult3>,
                IEnumerable<TResult4>,
                IEnumerable<TResult5>,
                IEnumerable<TResult6>,
                IEnumerable<TResult7>>
            Execute(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters)
        {
            IEnumerable<TResult1> results1 = null;
            IEnumerable<TResult2> results2 = null;
            IEnumerable<TResult3> results3 = null;
            IEnumerable<TResult4> results4 = null;
            IEnumerable<TResult5> results5 = null;
            IEnumerable<TResult6> results6 = null;
            IEnumerable<TResult7> results7 = null;

            ICommandExpression<TFilter> commandExpression = this.CommandExpression;

            using (IDataReader record = databaseManager
                .ExecuteDataReader(
                    commandExpression.CommandText,
                    commandExpression.DBCommandType,
                    null,
                    commandExpression.BuildParameters(databaseManager, filter, explicitParameters)))
            {
                results1 = _Item1Mapper.MapResult(record);

                if (record.NextResult())
                {
                    results2 = _Item2Mapper.MapResult(record);

                    if (record.NextResult())
                    {
                        results3 = _Item3Mapper.MapResult(record);

                        if (record.NextResult())
                        {
                            results4 = _Item4Mapper.MapResult(record);

                            if (record.NextResult())
                            {
                                results5 = _Item5Mapper.MapResult(record);

                                if (record.NextResult())
                                {
                                    results6 = _Item6Mapper.MapResult(record);

                                    if (record.NextResult())
                                    {
                                        results7 = _Item7Mapper.MapResult(record);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return new Tuple<IEnumerable<TResult1>,
                IEnumerable<TResult2>,
                IEnumerable<TResult3>,
                IEnumerable<TResult4>,
                IEnumerable<TResult5>,
                IEnumerable<TResult6>,
                IEnumerable<TResult7>>(results1, results2, results3, results4, results5, results6, results7);
        }

        public Tuple<IEnumerable<TResult1>,
                IEnumerable<TResult2>,
                IEnumerable<TResult3>,
                IEnumerable<TResult4>,
                IEnumerable<TResult5>,
                IEnumerable<TResult6>,
                IEnumerable<TResult7>> Execute(IDatabaseManager databaseManager, params DbParameter[] explicitParameters)
        {
            return this.Execute(databaseManager, default(TFilter), explicitParameters);
        }
    }
}