#region

using System;
using System.Collections.Generic;
using System.Numerics;

#endregion

namespace Susanoo
{
    /// <summary>
    ///     Base implementation for Command Results.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    public abstract class CommandResultCommon<TFilter> : IFluentPipelineFragment
    {
        private readonly ICommandResultImplementor<TFilter> _implementor;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CommandResultCommon{TFilter}" /> class.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="implementor">The implementor.</param>
        internal CommandResultCommon(ICommandExpression<TFilter> command, ICommandResultImplementor<TFilter> implementor)
        {
            _implementor = implementor;

            CommandExpression = command;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CommandResultCommon{TFilter}" /> class.
        /// </summary>
        /// <param name="command">The command.</param>
        protected CommandResultCommon(ICommandExpression<TFilter> command)
            : this(command, new CommandResultImplementor<TFilter>())
        {
        }

        /// <summary>
        ///     Gets the command expression.
        /// </summary>
        /// <value>The command expression.</value>
        public virtual ICommandExpression<TFilter> CommandExpression { get; private set; }

        /// <summary>
        ///     Gets the implementor of the Commandresult functionality.
        /// </summary>
        /// <value>The implementor.</value>
        protected virtual ICommandResultImplementor<TFilter> Implementor
        {
            get { return _implementor; }
        }

        /// <summary>
        ///     Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public virtual BigInteger CacheHash
        {
            get { return _implementor.CacheHash ^ (CommandExpression.CacheHash * 31); }
        }

        /// <summary>
        ///     Exports this instance to property mappings.
        /// </summary>
        /// <typeparam name="TResultType">The type of the t result type.</typeparam>
        /// <returns>IDictionary&lt;System.String, IPropertyMappingConfiguration&lt;System.Data.IDataRecord&gt;&gt;.</returns>
        public virtual IDictionary<string, IPropertyMapping> Export<TResultType>()
            where TResultType : new()
        {
            return Implementor.Export<TResultType>();
        }

        /// <summary>
        ///     Converts to a single result expression.
        /// </summary>
        /// <typeparam name="TSingle">The type of the single.</typeparam>
        /// <returns>ICommandResultExpression&lt;TFilter, TSingle&gt;.</returns>
        public virtual ICommandResultExpression<TFilter, TSingle> ToSingleResult<TSingle>()
            where TSingle : new()
        {
            return new CommandResultExpression<TFilter, TSingle>(CommandExpression, Implementor);
        }
    }

    /// <summary>
    ///     Provides methods for customizing how results are handled and compiling result mappings.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public class CommandResultExpression<TFilter, TResult> : CommandResultCommon<TFilter>,
        ICommandResultExpression<TFilter, TResult>
        where TResult : new()
    {
        /// <summary>
        /// Builds the or regens a command processor from cache.
        /// </summary>
        /// <param name="commandResultExpression">The command result expression.</param>
        /// <param name="name">The name.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult&gt;.</returns>
        public static ICommandProcessor<TFilter, TResult> BuildOrRegenCommandProcessor(ICommandResultExpression<TFilter, TResult> commandResultExpression, string name = null)
        {
            CommandProcessorCommon instance;
            SingleResultSetCommandProcessor<TFilter, TResult> result = null;

            if (name == null)
            {
                if (CommandManager.TryGetCommandProcessor(commandResultExpression.CacheHash, out instance))
                    result = (SingleResultSetCommandProcessor<TFilter, TResult>)instance;
            }
            else
            {
                if(CommandManager.TryGetCommandProcessor(name, out instance))
                    result = (SingleResultSetCommandProcessor<TFilter, TResult>)instance;
            }

            if(result == null)
                result = new SingleResultSetCommandProcessor<TFilter, TResult>(commandResultExpression, name);

            return result;
        }
        /// <summary>
        ///     Initializes a new instance of the <see cref="CommandResultExpression{TFilter, TResult}" /> class.
        /// </summary>
        /// <param name="command">The command.</param>
        public CommandResultExpression(ICommandExpression<TFilter> command)
            : base(command)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CommandResultExpression{TFilter, TResult}" /> class.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="implementor">The implementor.</param>
        internal CommandResultExpression(ICommandExpression<TFilter> command,
            ICommandResultImplementor<TFilter> implementor)
            : base(command, implementor)
        {
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
        /// Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public override BigInteger CacheHash
        {
            get
            {
                return (base.CacheHash * 31) ^ typeof(TResult).AssemblyQualifiedName.GetHashCode();
            }
        }

        /// <summary>
        ///     Realizes the pipeline and compiles result mappings.
        /// </summary>
        /// <param name="name">The name of the processor.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult&gt;.</returns>
        public ICommandProcessor<TFilter, TResult> Realize(string name = null)
        {
            return BuildOrRegenCommandProcessor(this, name);
        }

        /// <summary>
        ///     To the single result.
        /// </summary>
        /// <typeparam name="TSingle">The type of the single.</typeparam>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult&gt;.</returns>
        public override ICommandResultExpression<TFilter, TSingle> ToSingleResult<TSingle>()
        {
            return new CommandResultExpression<TFilter, TSingle>(this.CommandExpression);
        }
    }

    /// <summary>
    ///     Provides methods for customizing how results are handled and compiling result mappings.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the 1st result.</typeparam>
    /// <typeparam name="TResult2">The type of the 2nd result.</typeparam>
    public class CommandResultExpression<TFilter, TResult1, TResult2> : CommandResultCommon<TFilter>,
        ICommandResultExpression<TFilter, TResult1, TResult2>
        where TResult1 : new()
        where TResult2 : new()
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CommandResultExpression{TFilter, TResult1, TResult2}" /> class.
        /// </summary>
        /// <param name="command">The command.</param>
        public CommandResultExpression(ICommandExpression<TFilter> command)
            : base(command)
        {
        }

        /// <summary>
        ///     Provide mapping actions and options for a result set
        /// </summary>
        /// <typeparam name="TResultType">The type of the result.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult1, TResult2&gt;.</returns>
        public ICommandResultExpression<TFilter, TResult1, TResult2> ForResultsOfType<TResultType>(
            Action<IResultMappingExpression<TFilter, TResultType>> mappings)
            where TResultType : new()
        {
            Implementor.StoreMapping(mappings);

            return this;
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
                    ^ typeof(TResult2).AssemblyQualifiedName.GetHashCode();
            }
        }

        /// <summary>
        ///     Realizes the pipeline and compiles result mappings.
        /// </summary>
        /// <param name="name">The name of the processor.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2&gt;.</returns>
        public ICommandProcessor<TFilter, TResult1, TResult2> Realize(string name = null)
        {
            CommandProcessorCommon instance;
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
    public class CommandResultExpression<TFilter, TResult1, TResult2, TResult3> : CommandResultCommon<TFilter>,
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3>
        where TResult1 : new()
        where TResult2 : new()
        where TResult3 : new()
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CommandResultExpression{TFilter, TResult1, TResult2, TResult3}" />
        ///     class.
        /// </summary>
        /// <param name="command">The command.</param>
        public CommandResultExpression(ICommandExpression<TFilter> command)
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
                    ^ HashBuilder.Compute(typeof(TResult1).AssemblyQualifiedName)
                    ^ HashBuilder.Compute(typeof(TResult2).AssemblyQualifiedName)
                    ^ HashBuilder.Compute(typeof(TResult3).AssemblyQualifiedName);
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
            where TResultType : new()
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
            CommandProcessorCommon instance;
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
    public class CommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4> : CommandResultCommon<TFilter>,
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4>
        where TResult1 : new()
        where TResult2 : new()
        where TResult3 : new()
        where TResult4 : new()
    {
        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="CommandResultExpression{TFilter, TResult1, TResult2, TResult3, TResult4}" /> class.
        /// </summary>
        /// <param name="command">The command.</param>
        public CommandResultExpression(ICommandExpression<TFilter> command)
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
                    ^ HashBuilder.Compute(typeof(TResult1).AssemblyQualifiedName)
                    ^ HashBuilder.Compute(typeof(TResult2).AssemblyQualifiedName)
                    ^ HashBuilder.Compute(typeof(TResult3).AssemblyQualifiedName)
                    ^ HashBuilder.Compute(typeof(TResult4).AssemblyQualifiedName);
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
            where TResultType : new()
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
            CommandProcessorCommon instance;
            MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4> result;

            if (CommandManager.TryGetCommandProcessor(CacheHash, out instance))
                result = (MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4>)instance;
            else
                result = new MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4>(this, name);

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
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5>
        where TResult1 : new()
        where TResult2 : new()
        where TResult3 : new()
        where TResult4 : new()
        where TResult5 : new()
    {
        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="CommandResultExpression{TFilter, TResult1, TResult2, TResult3, TResult4, TResult5}" /> class.
        /// </summary>
        /// <param name="command">The command.</param>
        public CommandResultExpression(ICommandExpression<TFilter> command)
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
                    ^ HashBuilder.Compute(typeof(TResult1).AssemblyQualifiedName)
                    ^ HashBuilder.Compute(typeof(TResult2).AssemblyQualifiedName)
                    ^ HashBuilder.Compute(typeof(TResult3).AssemblyQualifiedName)
                    ^ HashBuilder.Compute(typeof(TResult4).AssemblyQualifiedName)
                    ^ HashBuilder.Compute(typeof(TResult5).AssemblyQualifiedName);
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
            where TResultType : new()
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
            CommandProcessorCommon instance;
            MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5> result;

            if (CommandManager.TryGetCommandProcessor(CacheHash, out instance))
                result = (MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5>)instance;
            else
                result = new MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5>(this, name);

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
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>
        where TResult1 : new()
        where TResult2 : new()
        where TResult3 : new()
        where TResult4 : new()
        where TResult5 : new()
        where TResult6 : new()
    {
        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="CommandResultExpression{TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6}" /> class.
        /// </summary>
        /// <param name="command">The command.</param>
        public CommandResultExpression(ICommandExpression<TFilter> command)
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
                    ^ HashBuilder.Compute(typeof(TResult1).AssemblyQualifiedName)
                    ^ HashBuilder.Compute(typeof(TResult2).AssemblyQualifiedName)
                    ^ HashBuilder.Compute(typeof(TResult3).AssemblyQualifiedName)
                    ^ HashBuilder.Compute(typeof(TResult4).AssemblyQualifiedName)
                    ^ HashBuilder.Compute(typeof(TResult5).AssemblyQualifiedName)
                    ^ HashBuilder.Compute(typeof(TResult6).AssemblyQualifiedName);
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
            where TResultType : new()
        {
            Implementor.StoreMapping(mappings);

            return this;
        }

        /// <summary>
        ///     Realizes the pipeline and compiles result mappings.
        /// </summary>
        /// <param name="name">The name of the processor.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6&gt;.</returns>
        public ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6> Realize(string name = null)
        {
            CommandProcessorCommon instance;
            MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6> result;

            if (CommandManager.TryGetCommandProcessor(CacheHash, out instance))
                result = (MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>)instance;
            else
                result = new MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>(this, name);

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
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>
        where TResult1 : new()
        where TResult2 : new()
        where TResult3 : new()
        where TResult4 : new()
        where TResult5 : new()
        where TResult6 : new()
        where TResult7 : new()
    {
        /// <summary>
        ///     Initializes a new instance of the
        ///     <see cref="CommandResultExpression{TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7}" />
        ///     class.
        /// </summary>
        /// <param name="command">The command.</param>
        public CommandResultExpression(ICommandExpression<TFilter> command)
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
                    ^ HashBuilder.Compute(typeof(TResult1).AssemblyQualifiedName)
                    ^ HashBuilder.Compute(typeof(TResult2).AssemblyQualifiedName)
                    ^ HashBuilder.Compute(typeof(TResult3).AssemblyQualifiedName)
                    ^ HashBuilder.Compute(typeof(TResult4).AssemblyQualifiedName)
                    ^ HashBuilder.Compute(typeof(TResult5).AssemblyQualifiedName)
                    ^ HashBuilder.Compute(typeof(TResult6).AssemblyQualifiedName)
                    ^ HashBuilder.Compute(typeof(TResult7).AssemblyQualifiedName);
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
            where TResultType : new()
        {
            Implementor.StoreMapping(mappings);

            return this;
        }

        /// <summary>
        ///     Realizes the pipeline and compiles result mappings.
        /// </summary>
        /// <param name="name">The name of the processor.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7&gt;.</returns>
        public ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7> Realize(string name = null)
        {
            CommandProcessorCommon instance;
            MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7> result;

            if (CommandManager.TryGetCommandProcessor(CacheHash, out instance))
                result = (MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>)instance;
            else
                result = new MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>(this, name);

            return result;
        }
    }
}