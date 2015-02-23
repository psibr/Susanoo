#region

using Susanoo.Pipeline.Command.ResultSets.Mapping;
using Susanoo.Pipeline.Command.ResultSets.Mapping.Properties;
using Susanoo.Pipeline.Command.ResultSets.Processing;
using System;
using System.Collections.Generic;
using System.Numerics;

#endregion

namespace Susanoo.Pipeline.Command.ResultSets
{
    /// <summary>
    /// Base implementation for Command Results.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    public abstract class CommandResultCommon<TFilter> : ICommandResultMappingExport, IFluentPipelineFragment
    {
        private readonly ICommandResultImplementor<TFilter> _implementor;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResultCommon{TFilter}" /> class.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="implementor">The implementor.</param>
        internal CommandResultCommon(ICommandExpressionInfo<TFilter> command, ICommandResultImplementor<TFilter> implementor)
        {
            _implementor = implementor;

            CommandExpression = command;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResultCommon{TFilter}" /> class.
        /// </summary>
        /// <param name="command">The command.</param>
        protected CommandResultCommon(ICommandExpressionInfo<TFilter> command)
            : this(command, new CommandResultImplementor<TFilter>())
        {
        }

        /// <summary>
        /// Gets the command expression.
        /// </summary>
        /// <value>The command expression.</value>
        public virtual ICommandExpressionInfo<TFilter> CommandExpression { get; private set; }

        /// <summary>
        /// Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public virtual BigInteger CacheHash
        {
            get { return _implementor.CacheHash ^ (CommandExpression.CacheHash * 31); }
        }

        /// <summary>
        /// Gets the implementor of the Commandresult functionality.
        /// </summary>
        /// <value>The implementor.</value>
        protected virtual ICommandResultImplementor<TFilter> Implementor
        {
            get { return _implementor; }
        }

        /// <summary>
        /// Exports a results mappings for processing.
        /// </summary>
        /// <param name="resultType">Type of the result.</param>
        /// <returns>IDictionary&lt;System.String, IPropertyMapping&gt;.</returns>
        public IDictionary<string, IPropertyMapping> Export(Type resultType)
        {
            return Implementor.Export(resultType);
        }

        /// <summary>
        /// Converts to a single result expression.
        /// </summary>
        /// <typeparam name="TSingle">The type of the single.</typeparam>
        /// <returns>ICommandResultExpression&lt;TFilter, TSingle&gt;.</returns>
        public virtual ICommandResultExpression<TFilter, TSingle> ToSingleResult<TSingle>()
        {
            return new CommandResultExpression<TFilter, TSingle>(CommandExpression, Implementor);
        }
    }

    /// <summary>
    /// Provides methods for customizing how results are handled and compiling result mappings.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public class CommandResultExpression<TFilter, TResult> : CommandResultCommon<TFilter>,
        ICommandResultExpression<TFilter, TResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResultExpression{TFilter, TResult}" /> class.
        /// </summary>
        /// <param name="command">The command.</param>
        public CommandResultExpression(ICommandExpressionInfo<TFilter> command)
            : base(command)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResultExpression{TFilter, TResult}" /> class.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="implementor">The implementor.</param>
        internal CommandResultExpression(ICommandExpressionInfo<TFilter> command,
            ICommandResultImplementor<TFilter> implementor)
            : base(command, implementor)
        {
        }

        /// <summary>
        /// Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public override BigInteger CacheHash
        {
            get { return (base.CacheHash * 31) ^ typeof(TResult).AssemblyQualifiedName.GetHashCode() 
                ^ this.GetType().AssemblyQualifiedName.GetHashCode(); }
        }

        /// <summary>
        /// Builds the or regens a command processor from cache.
        /// </summary>
        /// <param name="commandResultExpression">The command result expression.</param>
        /// <param name="name">The name.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult&gt;.</returns>
        public static ICommandProcessor<TFilter, TResult> BuildOrRegenCommandProcessor(
            ICommandResultExpression<TFilter, TResult> commandResultExpression, string name = null)
        {
            ICommandProcessorWithResults instance;
            SingleResultSetCommandProcessor<TFilter, TResult> result = null;

            if (name == null)
            {
                if (CommandManager.TryGetCommandProcessor(commandResultExpression.CacheHash, out instance))
                    result = (SingleResultSetCommandProcessor<TFilter, TResult>)instance;
            }
            else
            {
                if (CommandManager.TryGetCommandProcessor(name, out instance))
                    result = (SingleResultSetCommandProcessor<TFilter, TResult>)instance;
            }

            return result ??
                   new SingleResultSetCommandProcessor<TFilter, TResult>(commandResultExpression, name);
        }

        /// <summary>
        /// Provide mapping actions and options for a result set
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
        /// Realizes the pipeline and compiles result mappings.
        /// </summary>
        /// <param name="name">The name of the processor.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult&gt;.</returns>
        public ICommandProcessor<TFilter, TResult> Realize(string name = null)
        {
            return BuildOrRegenCommandProcessor(this, name);
        }

        /// <summary>
        /// To the single result.
        /// </summary>
        /// <typeparam name="TSingle">The type of the single.</typeparam>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult&gt;.</returns>
        public override ICommandResultExpression<TFilter, TSingle> ToSingleResult<TSingle>()
        {
            return new CommandResultExpression<TFilter, TSingle>(CommandExpression);
        }
    }

    /// <summary>
    /// Provides methods for customizing how results are handled and compiling result mappings.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the 1st result.</typeparam>
    /// <typeparam name="TResult2">The type of the 2nd result.</typeparam>
    public class CommandResultExpression<TFilter, TResult1, TResult2> : CommandResultCommon<TFilter>,
        ICommandResultExpression<TFilter, TResult1, TResult2>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResultExpression{TFilter, TResult1, TResult2}" /> class.
        /// </summary>
        /// <param name="command">The command.</param>
        public CommandResultExpression(ICommandExpressionInfo<TFilter> command)
            : base(command)
        {
        }

        /// <summary>
        /// Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public override BigInteger CacheHash
        {
            get
            {
                return (base.CacheHash * 31)
                       ^ typeof(TResult1).AssemblyQualifiedName.GetHashCode()
                       ^ typeof(TResult2).AssemblyQualifiedName.GetHashCode()
                       ^ this.GetType().AssemblyQualifiedName.GetHashCode();
            }
        }

        /// <summary>
        /// Provide mapping actions and options for a result set
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
        /// Realizes the pipeline and compiles result mappings.
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
    /// Provides methods for customizing how results are handled and compiling result mappings.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the 1st result.</typeparam>
    /// <typeparam name="TResult2">The type of the 2nd result.</typeparam>
    /// <typeparam name="TResult3">The type of the 3rd result.</typeparam>
    public class CommandResultExpression<TFilter, TResult1, TResult2, TResult3> : CommandResultCommon<TFilter>,
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResultExpression{TFilter, TResult1, TResult2, TResult3}" />
        /// class.
        /// </summary>
        /// <param name="command">The command.</param>
        public CommandResultExpression(ICommandExpressionInfo<TFilter> command)
            : base(command)
        {
        }

        /// <summary>
        /// Gets the hash code used for caching result mapping compilations.
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
                       ^ this.GetType().AssemblyQualifiedName.GetHashCode();
            }
        }

        /// <summary>
        /// Provide mapping actions and options for a result set
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
        /// Realizes the pipeline and compiles result mappings.
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
    /// Provides methods for customizing how results are handled and compiling result mappings.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the 1st result.</typeparam>
    /// <typeparam name="TResult2">The type of the 2nd result.</typeparam>
    /// <typeparam name="TResult3">The type of the 3rd result.</typeparam>
    /// <typeparam name="TResult4">The type of the 4th result.</typeparam>
    public class CommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4> : CommandResultCommon<TFilter>,
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4>
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="CommandResultExpression{TFilter, TResult1, TResult2, TResult3, TResult4}" /> class.
        /// </summary>
        /// <param name="command">The command.</param>
        public CommandResultExpression(ICommandExpressionInfo<TFilter> command)
            : base(command)
        {
        }

        /// <summary>
        /// Gets the hash code used for caching result mapping compilations.
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
                       ^ this.GetType().AssemblyQualifiedName.GetHashCode();
            }
        }

        /// <summary>
        /// Provide mapping actions and options for a result set
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
        /// Realizes the pipeline and compiles result mappings.
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
    /// Provides methods for customizing how results are handled and compiling result mappings.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the 1st result.</typeparam>
    /// <typeparam name="TResult2">The type of the 2nd result.</typeparam>
    /// <typeparam name="TResult3">The type of the 3rd result.</typeparam>
    /// <typeparam name="TResult4">The type of the 4th result.</typeparam>
    /// <typeparam name="TResult5">The type of the 5th result.</typeparam>
    public class CommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5> :
        CommandResultCommon<TFilter>,
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5>
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="CommandResultExpression{TFilter, TResult1, TResult2, TResult3, TResult4, TResult5}" /> class.
        /// </summary>
        /// <param name="command">The command.</param>
        public CommandResultExpression(ICommandExpressionInfo<TFilter> command)
            : base(command)
        {
        }

        /// <summary>
        /// Gets the hash code used for caching result mapping compilations.
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
                       ^ this.GetType().AssemblyQualifiedName.GetHashCode();
            }
        }

        /// <summary>
        /// Provide mapping actions and options for a result set
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
        /// Realizes the pipeline and compiles result mappings.
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
    /// Provides methods for customizing how results are handled and compiling result mappings.
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
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="CommandResultExpression{TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6}" /> class.
        /// </summary>
        /// <param name="command">The command.</param>
        public CommandResultExpression(ICommandExpressionInfo<TFilter> command)
            : base(command)
        {
        }

        /// <summary>
        /// Gets the hash code used for caching result mapping compilations.
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
                       ^ this.GetType().AssemblyQualifiedName.GetHashCode();
            }
        }

        /// <summary>
        /// Provide mapping actions and options for a result set
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
        /// Realizes the pipeline and compiles result mappings.
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
    /// Provides methods for customizing how results are handled and compiling result mappings.
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
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="CommandResultExpression{TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7}" />
        /// class.
        /// </summary>
        /// <param name="command">The command.</param>
        public CommandResultExpression(ICommandExpressionInfo<TFilter> command)
            : base(command)
        {
        }

        /// <summary>
        /// Gets the hash code used for caching result mapping compilations.
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
                       ^ this.GetType().AssemblyQualifiedName.GetHashCode();
            }
        }

        /// <summary>
        /// Provide mapping actions and options for a result set
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
        /// Realizes the pipeline and compiles result mappings.
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