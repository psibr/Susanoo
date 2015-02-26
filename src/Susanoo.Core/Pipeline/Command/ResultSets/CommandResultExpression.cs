#region

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Numerics;
using Susanoo.Pipeline.Command.ResultSets.Mapping;
using Susanoo.Pipeline.Command.ResultSets.Mapping.Properties;
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
        ICommandResultExpression<TFilter, TResult>,
        ICommandResultInfo<TFilter>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CommandResultExpression{TFilter, TResult}" /> class.
        /// </summary>
        /// <param name="command">The command.</param>
        public CommandResultExpression(ICommand<TFilter> command)
            : base(command)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CommandResultExpression{TFilter, TResult}" /> class.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="implementor">The implementor.</param>
        internal CommandResultExpression(ICommand<TFilter> command,
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
                return (base.CacheHash * 31) ^ typeof(TResult).AssemblyQualifiedName.GetHashCode()
                       ^ GetType().AssemblyQualifiedName.GetHashCode();
            }
        }

        private IDictionary<string, object> _optionsObject = null;

        /// <summary>
        /// Builds the where filter.
        /// </summary>
        /// <param name="optionsObject">The options object.</param>
        /// <returns>ICommandExpression&lt;TFilter&gt;.</returns>
        public ICommandResultExpression<TFilter, TResult> BuildWhereFilter(object optionsObject = null)
        {
            _optionsObject = optionsObject != null ? optionsObject.ToExpando() : new ExpandoObject();

            return this;
        }

        /// <summary>
        /// Builds the where filter implementation.
        /// </summary>
        /// <param name="optionsObject">The options object.</param>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult&gt;.</returns>
        protected virtual void BuildWhereFilterImplementation(IDictionary<string, object> optionsObject)
        {
            const string format =
                @"SELECT *
FROM (
    {0}
) susanoo_query_wrapper
WHERE 1=1";

            var mappings = Export(typeof(TFilter));
            object compareOption;
            var options = mappings.ToDictionary(
                propertyMapping =>
                    new KeyValuePair<string, string>(propertyMapping.Key, propertyMapping.Value.ActiveAlias),
                propertyMapping => _optionsObject.TryGetValue(propertyMapping.Key, out compareOption)
                    ? compareOption
                    : GetDefaultCompareMethod(propertyMapping.Value.PropertyMetadata.PropertyType));

            Command.CommandText = string.Format(format, Command.CommandText) + string.Concat(options.Select(o =>
            {
                var compareFormat = string.Empty;
                if (o.Value is CompareMethod)
                    compareFormat = Comparison.GetComparisonFormat((CompareMethod)o.Value);

                var value = o.Value as Comparison.ComparisonOverride;
                compareFormat = value != null ? value.OverrideText : compareFormat;

                if (compareFormat.Contains('{'))
                    compareFormat = string.Format(compareFormat, o.Key.Value,o.Key.Value);

                return compareFormat;
            }));

            //Hash will have changed since we modified command text.
            Command.RecomputeCacheHash();
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
            if(_optionsObject != null)
                BuildWhereFilterImplementation(_optionsObject);

            return BuildOrRegenCommandProcessor(this, name);
        }

        /// <summary>
        ///     Gets the command information.
        /// </summary>
        /// <returns>ICommandInfo&lt;TFilter&gt;.</returns>
        public ICommandInfo<TFilter> GetCommandInfo()
        {
            return Command;
        }

        /// <summary>
        ///     Gets the mappings exporter.
        /// </summary>
        /// <returns>ICommandResultMappingExport.</returns>
        public ICommandResultMappingExport GetExporter()
        {
            return this;
        }

        /// <summary>
        ///     Builds the or regens a command processor from cache.
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
                if (CommandManager.TryGetCommandProcessor(commandResultInfo.CacheHash, out instance))
                    result = (SingleResultSetCommandProcessor<TFilter, TResult>)instance;
            }
            else
            {
                if (CommandManager.TryGetCommandProcessor(name, out instance))
                    result = (SingleResultSetCommandProcessor<TFilter, TResult>)instance;
            }

            return result ??
                   new SingleResultSetCommandProcessor<TFilter, TResult>(commandResultInfo, name);
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
        public CommandResultExpression(ICommand<TFilter> command)
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
                       ^ typeof(TResult1).AssemblyQualifiedName.GetHashCode()
                       ^ typeof(TResult2).AssemblyQualifiedName.GetHashCode()
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

        /// <summary>
        ///     Gets the command information.
        /// </summary>
        /// <returns>ICommandInfo&lt;TFilter&gt;.</returns>
        public ICommandInfo<TFilter> GetCommandInfo()
        {
            return Command;
        }

        /// <summary>
        ///     Gets the exporter.
        /// </summary>
        /// <returns>ICommandResultMappingExport.</returns>
        public ICommandResultMappingExport GetExporter()
        {
            return this;
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
        public CommandResultExpression(ICommand<TFilter> command)
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
                       ^ typeof(TResult1).AssemblyQualifiedName.GetHashCode()
                       ^ typeof(TResult2).AssemblyQualifiedName.GetHashCode()
                       ^ typeof(TResult3).AssemblyQualifiedName.GetHashCode()
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

        /// <summary>
        ///     Gets the command information.
        /// </summary>
        /// <returns>ICommandInfo&lt;TFilter&gt;.</returns>
        public ICommandInfo<TFilter> GetCommandInfo()
        {
            return Command;
        }

        /// <summary>
        ///     Gets the exporter.
        /// </summary>
        /// <returns>ICommandResultMappingExport.</returns>
        public ICommandResultMappingExport GetExporter()
        {
            return this;
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
        public CommandResultExpression(ICommand<TFilter> command)
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
                       ^ typeof(TResult1).AssemblyQualifiedName.GetHashCode()
                       ^ typeof(TResult2).AssemblyQualifiedName.GetHashCode()
                       ^ typeof(TResult3).AssemblyQualifiedName.GetHashCode()
                       ^ typeof(TResult4).AssemblyQualifiedName.GetHashCode()
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

        /// <summary>
        ///     Gets the command information.
        /// </summary>
        /// <returns>ICommandInfo&lt;TFilter&gt;.</returns>
        public ICommandInfo<TFilter> GetCommandInfo()
        {
            return Command;
        }

        /// <summary>
        ///     Gets the exporter.
        /// </summary>
        /// <returns>ICommandResultMappingExport.</returns>
        public ICommandResultMappingExport GetExporter()
        {
            return this;
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
        public CommandResultExpression(ICommand<TFilter> command)
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
                       ^ typeof(TResult1).AssemblyQualifiedName.GetHashCode()
                       ^ typeof(TResult2).AssemblyQualifiedName.GetHashCode()
                       ^ typeof(TResult3).AssemblyQualifiedName.GetHashCode()
                       ^ typeof(TResult4).AssemblyQualifiedName.GetHashCode()
                       ^ typeof(TResult5).AssemblyQualifiedName.GetHashCode()
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

        /// <summary>
        ///     Gets the command information.
        /// </summary>
        /// <returns>ICommandInfo&lt;TFilter&gt;.</returns>
        public ICommandInfo<TFilter> GetCommandInfo()
        {
            return Command;
        }

        /// <summary>
        ///     Gets the exporter.
        /// </summary>
        /// <returns>ICommandResultMappingExport.</returns>
        public ICommandResultMappingExport GetExporter()
        {
            return this;
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
        public CommandResultExpression(ICommand<TFilter> command)
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
                       ^ typeof(TResult1).AssemblyQualifiedName.GetHashCode()
                       ^ typeof(TResult2).AssemblyQualifiedName.GetHashCode()
                       ^ typeof(TResult3).AssemblyQualifiedName.GetHashCode()
                       ^ typeof(TResult4).AssemblyQualifiedName.GetHashCode()
                       ^ typeof(TResult5).AssemblyQualifiedName.GetHashCode()
                       ^ typeof(TResult6).AssemblyQualifiedName.GetHashCode()
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

        /// <summary>
        ///     Gets the command information.
        /// </summary>
        /// <returns>ICommandInfo&lt;TFilter&gt;.</returns>
        public ICommandInfo<TFilter> GetCommandInfo()
        {
            return Command;
        }

        /// <summary>
        ///     Gets the exporter.
        /// </summary>
        /// <returns>ICommandResultMappingExport.</returns>
        public ICommandResultMappingExport GetExporter()
        {
            return this;
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
        public CommandResultExpression(ICommand<TFilter> command)
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
                       ^ typeof(TResult1).AssemblyQualifiedName.GetHashCode()
                       ^ typeof(TResult2).AssemblyQualifiedName.GetHashCode()
                       ^ typeof(TResult3).AssemblyQualifiedName.GetHashCode()
                       ^ typeof(TResult4).AssemblyQualifiedName.GetHashCode()
                       ^ typeof(TResult5).AssemblyQualifiedName.GetHashCode()
                       ^ typeof(TResult6).AssemblyQualifiedName.GetHashCode()
                       ^ typeof(TResult7).AssemblyQualifiedName.GetHashCode()
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

        /// <summary>
        ///     Gets the command information.
        /// </summary>
        /// <returns>ICommandInfo&lt;TFilter&gt;.</returns>
        public ICommandInfo<TFilter> GetCommandInfo()
        {
            return Command;
        }

        /// <summary>
        ///     Gets the exporter.
        /// </summary>
        /// <returns>ICommandResultMappingExport.</returns>
        public ICommandResultMappingExport GetExporter()
        {
            return this;
        }
    }
}