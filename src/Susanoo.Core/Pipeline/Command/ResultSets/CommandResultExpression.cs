#region

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Numerics;
using System.Runtime.Remoting.Messaging;
using Susanoo.Pipeline.Command.ResultSets.Mapping;
using Susanoo.Pipeline.Command.ResultSets.Processing;

#endregion

namespace Susanoo.Pipeline.Command.ResultSets
{
    /// <summary>
    ///     Provides methods for customizing how results are handled and compiling result mappings.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public class CommandResultExpression<TFilter, TResult> :
        CommandResultCommon<TFilter>,
        ICommandResultExpression<TFilter, TResult>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CommandResultExpression{TFilter, TResult}" /> class.
        /// </summary>
        /// <param name="command">The command.</param>
        public CommandResultExpression(ICommandInfo<TFilter> command)
            : base(command)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CommandResultExpression{TFilter, TResult}" /> class.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="implementor">The implementor.</param>
        internal CommandResultExpression(ICommandInfo<TFilter> command,
            ICommandResultImplementor<TFilter> implementor)
            : base(command, implementor)
        {
        }



        /// <summary>
        ///     Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public override BigInteger CacheHash
        {
            get
            {
                return (base.CacheHash * 31) ^ (GetTypeArgumentHashCode(this.GetType()) * 31)
                       ^ GetType().AssemblyQualifiedName.GetHashCode();
            }
        }

        /// <summary>
        /// Gets the where filter options. Null if no where filter.
        /// </summary>
        /// <value>The where filter options.</value>
        public IDictionary<string, object> WhereFilterOptions { get; private set; }

        /// <summary>
        /// Builds the where filter.
        /// </summary>
        /// <param name="optionsObject">The options object.</param>
        /// <returns>ICommandExpression&lt;TFilter&gt;.</returns>
        public ICommandResultExpression<TFilter, TResult> BuildWhereFilter(object optionsObject = null)
        {
            WhereFilterOptions = optionsObject != null ? optionsObject.ToExpando() : new ExpandoObject();

            //Make sure the command is wrapped in a new SELECT for simplicity.
            Command.AddQueryWrapper();

            var whereFilterModifier = new CommandModifier
            {
                Description = "WhereFilter",
                Priority = 900,
                ModifierFunc = BuildWhereFilterImplementation
            };

            whereFilterModifier.CacheHash =
                HashBuilder.Compute(whereFilterModifier.Description + WhereFilterOptions.Aggregate(string.Empty,
                    (s, pair) => s + pair.Key + pair.Value));

            if (!Command.TryAddCommandModifier(whereFilterModifier))
                throw new Exception("Conflicting priorities for command modifiers");

            return this;
        }

        /// <summary>
        /// Builds the where filter.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <returns>ICommandExpression&lt;TFilter&gt;.</returns>
        /// <exception cref="Exception">Conflicting priorities for command modifiers</exception>
        public ICommandResultExpression<TFilter, TResult> AddOrderByExpression(string parameterName = "OrderBy")
        {
            if (parameterName == null)
                throw new ArgumentNullException("parameterName");

            var orderByModifier = new CommandModifier
            {
                Description = "OrderByExpression",
                Priority = 800,
                ModifierFunc = info =>
                {
                    var orderByParameter = info.Parameters.First(p => p.ParameterName == parameterName);

                    if (orderByParameter.Value == null
                        || !CommandManager.Bootstrapper.RetrieveOrderByRegex()
                            .IsMatch(orderByParameter.Value.ToString()))
                        throw new FormatException("Order By paramter either contains unsafe characters or a bad format");

                    return new ExecutableCommandInfo
                    {
                        CommandText = info.CommandText + "\r\nORDER BY " + orderByParameter.Value,
                        Parameters = info.Parameters.Where(p => p.ParameterName != parameterName).ToArray(),
                        DbCommandType = info.DbCommandType
                    };
                },
                CacheHash = HashBuilder.Compute("ORDER BY @" + parameterName)
            };

            if (!Command.TryAddCommandModifier(orderByModifier))
                throw new Exception("Conflicting priorities for command modifiers");

            return this;
        }

        /// <summary>
        /// Builds the where filter implementation.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult&gt;.</returns>
        protected virtual IExecutableCommandInfo BuildWhereFilterImplementation(
            IExecutableCommandInfo info)
        {
            var mappings = info.Parameters
                .Join(Export(typeof(TFilter)), parameter => parameter.SourceColumn, pair => pair.Key,
                    (parameter, pair) =>
                        new Tuple<string, Type, string, string>(
                            pair.Key,                                 //Property Name
                            pair.Value.PropertyMetadata.PropertyType, //Property Type
                            parameter.ParameterName,                  //Parameter Name
                            pair.Value.ActiveAlias                    //Result Column Name
                            ))
                .GroupJoin(WhereFilterOptions, tuple => tuple.Item1, pair => pair.Key,
                    (tuple, pairs) => new { tuple, comparer = pairs.Select(kvp => kvp.Value).FirstOrDefault() })
                .Select(o => new Tuple<string, Type, string, string, object>(
                    o.tuple.Item1,                                          //Property Name
                    o.tuple.Item2,                                          //Property Type
                    o.tuple.Item3,                                          //Parameter Name
                    o.tuple.Item4,                                          //Result Column Name
                    o.comparer ?? GetDefaultCompareMethod(o.tuple.Item2)    //Comparer
                    ));

            return new ExecutableCommandInfo
            {
                CommandText = info.CommandText + string.Concat(mappings.Select(o =>
                {
                    var compareFormat = string.Empty;
                    if (o.Item5 is CompareMethod)
                        compareFormat = Comparison.GetComparisonFormat((CompareMethod)o.Item5);

                    var value = o.Item5 as ComparisonOverride;
                    compareFormat = value != null ? value.OverrideText : compareFormat;

                    if (compareFormat.Contains('{'))
                        compareFormat = string.Format(compareFormat, o.Item3, "[" + o.Item4 + "]");

                    return compareFormat;
                })),
                DbCommandType = info.DbCommandType,
                Parameters = info.Parameters
            };
        }

        /// <summary>
        /// Gets the default compare method.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>CompareMethod.</returns>
        private static CompareMethod GetDefaultCompareMethod(Type type)
        {
            var result = Comparison.Equal;
            if (type == typeof(string))
                result = CompareMethod.Contains;
            else if (type == typeof(DateTime) || CommandManager.GetDbType(type) == null)
                result = CompareMethod.Ignore;

            return result;
        }

        /// <summary>
        ///     Provide mapping actions and options for a result set
        /// </summary>
        /// <param name="mappings">The mappings.</param>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult&gt;.</returns>
        public ICommandResultExpression<TFilter, TResult> ForResults(
            Action<IResultMappingExpression<TFilter, TResult>> mappings)
        {
            Implementor.StoreMapping(mappings);

            return this;
        }

        /// <summary>
        ///     Realizes the pipeline and compiles result mappings.
        /// </summary>
        /// <param name="name">The name of the processor.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult&gt;.</returns>
        public ICommandProcessor<TFilter, TResult> Realize(string name = null)
        {
            var processor = BuildOrRegenCommandProcessor(this, name);

            return processor;
        }

        /// <summary>
        ///     Builds the or regenerates a command processor from cache.
        /// </summary>
        /// <param name="commandResultInfo">The command result information.</param>
        /// <param name="name">The name.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult&gt;.</returns>
        public static ICommandProcessor<TFilter, TResult> BuildOrRegenCommandProcessor(
            ICommandResultInfo<TFilter> commandResultInfo, string name = null)
        {
            ICommandProcessorWithResults instance;
            SingleResultSetCommandProcessor<TFilter, TResult> result = null;

            if (name == null)
            {
                var hash = (commandResultInfo.CacheHash * 31) ^
                           GetTypeArgumentHashCode(typeof(SingleResultSetCommandProcessor<TFilter, TResult>));

                if (CommandManager.TryGetCommandProcessor(hash, out instance))
                    result = (SingleResultSetCommandProcessor<TFilter, TResult>)instance;
            }
            else
            {
                if (CommandManager.TryGetCommandProcessor(name, out instance))
                    result = (SingleResultSetCommandProcessor<TFilter, TResult>)instance;
            }

            return result ??
                   new SingleResultSetCommandProcessor<TFilter, TResult>(commandResultInfo, commandResultInfo.GetCommandInfo().CommandModifiers, name);
        }

        /// <summary>
        ///     To the single result.
        /// </summary>
        /// <typeparam name="TSingle">The type of the single.</typeparam>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult&gt;.</returns>
        public override ICommandResultExpression<TFilter, TSingle> ToSingleResult<TSingle>()
        {
            return new CommandResultExpression<TFilter, TSingle>(Command);
        }
    }

    /// <summary>
    ///     Provides methods for customizing how results are handled and compiling result mappings.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the 1st result.</typeparam>
    /// <typeparam name="TResult2">The type of the 2nd result.</typeparam>
    public class CommandResultExpression<TFilter, TResult1, TResult2> :
        CommandResultCommon<TFilter>,
        ICommandResultExpression<TFilter, TResult1, TResult2>,
        ICommandResultInfo<TFilter>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CommandResultExpression{TFilter, TResult1, TResult2}" /> class.
        /// </summary>
        /// <param name="command">The command.</param>
        public CommandResultExpression(ICommandInfo<TFilter> command)
            : base(command)
        {
        }

        /// <summary>
        ///     Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public override BigInteger CacheHash
        {
            get
            {
                return (base.CacheHash * 31)
                       ^ (GetTypeArgumentHashCode(this.GetType()) * 31)
                       ^ GetType().AssemblyQualifiedName.GetHashCode();
            }
        }

        /// <summary>
        ///     Provide mapping actions and options for a result set
        /// </summary>
        /// <typeparam name="TResultType">The type of the result.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult1, TResult2&gt;.</returns>
        public ICommandResultExpression<TFilter, TResult1, TResult2> ForResultsOfType<TResultType>(
            Action<IResultMappingExpression<TFilter, TResultType>> mappings)
        {
            Implementor.StoreMapping(mappings);

            return this;
        }

        /// <summary>
        ///     Realizes the pipeline and compiles result mappings.
        /// </summary>
        /// <param name="name">The name of the processor.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2&gt;.</returns>
        public ICommandProcessor<TFilter, TResult1, TResult2> Realize(string name = null)
        {
            ICommandProcessorWithResults instance;
            MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2> result;

            if (CommandManager.TryGetCommandProcessor(CacheHash, out instance))
                result = (MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2>)instance;
            else
                result = new MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2>(this, name);

            return result;
        }
    }

    /// <summary>
    ///     Provides methods for customizing how results are handled and compiling result mappings.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the 1st result.</typeparam>
    /// <typeparam name="TResult2">The type of the 2nd result.</typeparam>
    /// <typeparam name="TResult3">The type of the 3rd result.</typeparam>
    public class CommandResultExpression<TFilter, TResult1, TResult2, TResult3> :
        CommandResultCommon<TFilter>,
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3>,
        ICommandResultInfo<TFilter>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CommandResultExpression{TFilter, TResult1, TResult2, TResult3}" />
        ///     class.
        /// </summary>
        /// <param name="command">The command.</param>
        public CommandResultExpression(ICommandInfo<TFilter> command)
            : base(command)
        {
        }

        /// <summary>
        ///     Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public override BigInteger CacheHash
        {
            get
            {
                return (base.CacheHash * 31)
                       ^ (GetTypeArgumentHashCode(this.GetType()) * 31)
                       ^ GetType().AssemblyQualifiedName.GetHashCode();
            }
        }

        /// <summary>
        ///     Provide mapping actions and options for a result set
        /// </summary>
        /// <typeparam name="TResultType">The type of the result.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult1, TResult2, TResult3&gt;.</returns>
        public ICommandResultExpression<TFilter, TResult1, TResult2, TResult3> ForResultsOfType<TResultType>(
            Action<IResultMappingExpression<TFilter, TResultType>> mappings)
        {
            Implementor.StoreMapping(mappings);

            return this;
        }

        /// <summary>
        ///     Realizes the pipeline and compiles result mappings.
        /// </summary>
        /// <param name="name">The name of the processor.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2, TResult3&gt;.</returns>
        public ICommandProcessor<TFilter, TResult1, TResult2, TResult3> Realize(string name = null)
        {
            ICommandProcessorWithResults instance;
            MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2, TResult3> result;

            if (CommandManager.TryGetCommandProcessor(CacheHash, out instance))
                result = (MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2, TResult3>)instance;
            else
                result = new MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2, TResult3>(this, name);

            return result;
        }
    }

    /// <summary>
    ///     Provides methods for customizing how results are handled and compiling result mappings.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the 1st result.</typeparam>
    /// <typeparam name="TResult2">The type of the 2nd result.</typeparam>
    /// <typeparam name="TResult3">The type of the 3rd result.</typeparam>
    /// <typeparam name="TResult4">The type of the 4th result.</typeparam>
    public class CommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4> :
        CommandResultCommon<TFilter>,
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4>,
        ICommandResultInfo<TFilter>
    {
        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="CommandResultExpression{TFilter, TResult1, TResult2, TResult3, TResult4}" /> class.
        /// </summary>
        /// <param name="command">The command.</param>
        public CommandResultExpression(ICommandInfo<TFilter> command)
            : base(command)
        {
        }

        /// <summary>
        ///     Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public override BigInteger CacheHash
        {
            get
            {
                return (base.CacheHash * 31)
                       ^ (GetTypeArgumentHashCode(this.GetType()) * 31)
                       ^ GetType().AssemblyQualifiedName.GetHashCode();
            }
        }

        /// <summary>
        ///     Provide mapping actions and options for a result set
        /// </summary>
        /// <typeparam name="TResultType">The type of the result.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult1, TResult2, TResult3, TResult4&gt;.</returns>
        public ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4> ForResultsOfType<TResultType>(
            Action<IResultMappingExpression<TFilter, TResultType>> mappings)
        {
            Implementor.StoreMapping(mappings);

            return this;
        }

        /// <summary>
        ///     Realizes the pipeline and compiles result mappings.
        /// </summary>
        /// <param name="name">The name of the processor.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2, TResult3, TResult4&gt;.</returns>
        public ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4> Realize(string name = null)
        {
            ICommandProcessorWithResults instance;
            MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4> result;

            if (CommandManager.TryGetCommandProcessor(CacheHash, out instance))
                result = (MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4>)instance;
            else
                result = new MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4>(this,
                    name);

            return result;
        }
    }

    /// <summary>
    ///     Provides methods for customizing how results are handled and compiling result mappings.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the 1st result.</typeparam>
    /// <typeparam name="TResult2">The type of the 2nd result.</typeparam>
    /// <typeparam name="TResult3">The type of the 3rd result.</typeparam>
    /// <typeparam name="TResult4">The type of the 4th result.</typeparam>
    /// <typeparam name="TResult5">The type of the 5th result.</typeparam>
    public class CommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5> :
        CommandResultCommon<TFilter>,
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5>,
        ICommandResultInfo<TFilter>
    {
        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="CommandResultExpression{TFilter, TResult1, TResult2, TResult3, TResult4, TResult5}" /> class.
        /// </summary>
        /// <param name="command">The command.</param>
        public CommandResultExpression(ICommandInfo<TFilter> command)
            : base(command)
        {
        }

        /// <summary>
        ///     Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public override BigInteger CacheHash
        {
            get
            {
                return (base.CacheHash * 31)
                       ^ (GetTypeArgumentHashCode(this.GetType()) * 31)
                       ^ GetType().AssemblyQualifiedName.GetHashCode();
            }
        }

        /// <summary>
        ///     Provide mapping actions and options for a result set
        /// </summary>
        /// <typeparam name="TResultType">The type of the result.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5&gt;.</returns>
        public ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5> ForResultsOfType
            <TResultType>(
            Action<IResultMappingExpression<TFilter, TResultType>> mappings)
        {
            Implementor.StoreMapping(mappings);

            return this;
        }

        /// <summary>
        ///     Realizes the pipeline and compiles result mappings.
        /// </summary>
        /// <param name="name">The name of the processor.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5&gt;.</returns>
        public ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5> Realize(string name = null)
        {
            ICommandProcessorWithResults instance;
            MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5> result;

            if (CommandManager.TryGetCommandProcessor(CacheHash, out instance))
                result =
                    (MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5>)
                        instance;
            else
                result =
                    new MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5>(
                        this, name);

            return result;
        }
    }

    /// <summary>
    ///     Provides methods for customizing how results are handled and compiling result mappings.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the 1st result.</typeparam>
    /// <typeparam name="TResult2">The type of the 2nd result.</typeparam>
    /// <typeparam name="TResult3">The type of the 3rd result.</typeparam>
    /// <typeparam name="TResult4">The type of the 4th result.</typeparam>
    /// <typeparam name="TResult5">The type of the 5th result.</typeparam>
    /// <typeparam name="TResult6">The type of the 6th result.</typeparam>
    public class CommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6> :
        CommandResultCommon<TFilter>,
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>,
        ICommandResultInfo<TFilter>
    {
        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="CommandResultExpression{TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6}" /> class.
        /// </summary>
        /// <param name="command">The command.</param>
        public CommandResultExpression(ICommandInfo<TFilter> command)
            : base(command)
        {
        }

        /// <summary>
        ///     Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public override BigInteger CacheHash
        {
            get
            {
                return (base.CacheHash * 31)
                       ^ (GetTypeArgumentHashCode(this.GetType()) * 31)
                       ^ GetType().AssemblyQualifiedName.GetHashCode();
            }
        }

        /// <summary>
        ///     Provide mapping actions and options for a result set
        /// </summary>
        /// <typeparam name="TResultType">The type of the result.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6&gt;.</returns>
        public ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>
            ForResultsOfType<TResultType>(
            Action<IResultMappingExpression<TFilter, TResultType>> mappings)
        {
            Implementor.StoreMapping(mappings);

            return this;
        }

        /// <summary>
        ///     Realizes the pipeline and compiles result mappings.
        /// </summary>
        /// <param name="name">The name of the processor.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6&gt;.</returns>
        public ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6> Realize(
            string name = null)
        {
            ICommandProcessorWithResults instance;
            MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>
                result;

            if (CommandManager.TryGetCommandProcessor(CacheHash, out instance))
                result =
                    (
                        MultipleResultSetCommandProcessor
                            <TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>)instance;
            else
                result =
                    new MultipleResultSetCommandProcessor
                        <TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>(this, name);

            return result;
        }
    }

    /// <summary>
    ///     Provides methods for customizing how results are handled and compiling result mappings.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the 1st result.</typeparam>
    /// <typeparam name="TResult2">The type of the 2nd result.</typeparam>
    /// <typeparam name="TResult3">The type of the 3rd result.</typeparam>
    /// <typeparam name="TResult4">The type of the 4th result.</typeparam>
    /// <typeparam name="TResult5">The type of the 5th result.</typeparam>
    /// <typeparam name="TResult6">The type of the 6th result.</typeparam>
    /// <typeparam name="TResult7">The type of the 7th result.</typeparam>
    public class CommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7> :
        CommandResultCommon<TFilter>,
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>,
        ICommandResultInfo<TFilter>
    {
        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="CommandResultExpression{TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7}" />
        ///     class.
        /// </summary>
        /// <param name="command">The command.</param>
        public CommandResultExpression(ICommandInfo<TFilter> command)
            : base(command)
        {
        }

        /// <summary>
        ///     Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public override BigInteger CacheHash
        {
            get
            {
                return (base.CacheHash * 31)
                       ^ (GetTypeArgumentHashCode(this.GetType()) * 31)
                       ^ GetType().AssemblyQualifiedName.GetHashCode();
            }
        }

        /// <summary>
        ///     Provide mapping actions and options for a result set
        /// </summary>
        /// <typeparam name="TResultType">The type of the result.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7&gt;.</returns>
        public ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>
            ForResultsOfType<TResultType>(
            Action<IResultMappingExpression<TFilter, TResultType>> mappings)
        {
            Implementor.StoreMapping(mappings);

            return this;
        }

        /// <summary>
        ///     Realizes the pipeline and compiles result mappings.
        /// </summary>
        /// <param name="name">The name of the processor.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7&gt;.</returns>
        public ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7> Realize(
            string name = null)
        {
            ICommandProcessorWithResults instance;
            MultipleResultSetCommandProcessor
                <TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7> result;

            if (CommandManager.TryGetCommandProcessor(CacheHash, out instance))
                result =
                    (
                        MultipleResultSetCommandProcessor
                            <TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>)instance;
            else
                result =
                    new MultipleResultSetCommandProcessor
                        <TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>(this, name);

            return result;
        }
    }
}