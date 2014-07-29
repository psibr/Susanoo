using System;
using System.Collections.Generic;
using System.Numerics;

namespace Susanoo
{
    /// <summary>
    /// Base implementation for Command Results.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    public abstract class CommandResultCommon<TFilter> : IFluentPipelineFragment
    {
        private readonly ICommandResultImplementor<TFilter> _Implementor;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResultCommon{TFilter}"/> class.
        /// </summary>
        /// <param name="command">The command.</param>
        public CommandResultCommon(ICommandExpression<TFilter> command)
            : this(command, new CommandResultImplementor<TFilter>()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResultCommon{TFilter}"/> class.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="implementor">The implementor.</param>
        internal CommandResultCommon(ICommandExpression<TFilter> command, ICommandResultImplementor<TFilter> implementor)
        {
            _Implementor = implementor;

            this.CommandExpression = command;
        }

        /// <summary>
        /// Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public virtual BigInteger CacheHash
        {
            get
            {
                return _Implementor.CacheHash;
            }
        }

        /// <summary>
        /// Gets the command expression.
        /// </summary>
        /// <value>The command expression.</value>
        public virtual ICommandExpression<TFilter> CommandExpression { get; private set; }

        /// <summary>
        /// Gets the implementor of the Commandresult functionality.
        /// </summary>
        /// <value>The implementor.</value>
        protected virtual ICommandResultImplementor<TFilter> Implementor
        {
            get { return _Implementor; }
        }

        /// <summary>
        /// Exports this instance to property mappings.
        /// </summary>
        /// <typeparam name="TResultType">The type of the t result type.</typeparam>
        /// <returns>IDictionary&lt;System.String, IPropertyMappingConfiguration&lt;System.Data.IDataRecord&gt;&gt;.</returns>
        public virtual IDictionary<string, IPropertyMapping> Export<TResultType>()
            where TResultType : new()
        {
            return this.Implementor.Export<TResultType>();
        }

        public virtual ICommandResultExpression<TFilter, TSingle> ToSingleResult<TSingle>()
            where TSingle : new()
        {
            return new CommandResultExpression<TFilter, TSingle>(this.CommandExpression, this.Implementor);
        }
    }

    /// <summary>
    /// Provides methods for customizing how results are handled and compiling result mappings.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public class CommandResultExpression<TFilter, TResult> : CommandResultCommon<TFilter>,
        ICommandResultExpression<TFilter, TResult>, IFluentPipelineFragment
            where TResult : new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResultExpression{TFilter, TResult}"/> class.
        /// </summary>
        /// <param name="command">The command.</param>
        public CommandResultExpression(ICommandExpression<TFilter> command)
            : base(command) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResultExpression{TFilter, TResult}"/> class.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="implementor">The implementor.</param>
        internal CommandResultExpression(ICommandExpression<TFilter> command, ICommandResultImplementor<TFilter> implementor)
            : base(command, implementor) { }

        /// <summary>
        /// Provide mapping actions and options for a result set
        /// </summary>
        /// <param name="mappings">The mappings.</param>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult&gt;.</returns>
        public ICommandResultExpression<TFilter, TResult> ForResults(Action<IResultMappingExpression<TFilter, TResult>> mappings)
        {
            Implementor.StoreMapping<TResult>(mappings);

            return this;
        }

        /// <summary>
        /// Finalizes the pipeline and compiles result mappings.
        /// </summary>
        /// <returns>ICommandProcessor&lt;TFilter, TResult&gt;.</returns>
        public ICommandProcessor<TFilter, TResult> Finalize()
        {
            return new SingleResultSetCommandProcessor<TFilter, TResult>(this);
        }

        /// <summary>
        /// To the single result.
        /// </summary>
        /// <typeparam name="TSingle">The type of the single.</typeparam>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult&gt;.</returns>
        public override ICommandResultExpression<TFilter, TSingle> ToSingleResult<TSingle>()
        {
            return this as ICommandResultExpression<TFilter, TSingle>;
        }
    }
    /// <summary>
    /// Provides methods for customizing how results are handled and compiling result mappings.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult1">The type of the 1st result.</typeparam>
    /// <typeparam name="TResult2">The type of the 2nd result.</typeparam>
    public class CommandResultExpression<TFilter, TResult1, TResult2> : CommandResultCommon<TFilter>,
        ICommandResultExpression<TFilter, TResult1, TResult2>, IFluentPipelineFragment
        where TResult1 : new()
        where TResult2 : new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResultExpression{TFilter, TResult1, TResult2}"/> class.
        /// </summary>
        /// <param name="command">The command.</param>
        public CommandResultExpression(ICommandExpression<TFilter> command)
            : base(command) { }

        /// <summary>
        /// Provide mapping actions and options for a result set
        /// </summary>
        /// <typeparam name="TResultType">The type of the result.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult1, TResult2&gt;.</returns>
        public ICommandResultExpression<TFilter, TResult1, TResult2> ForResultsOfType<TResultType>(Action<IResultMappingExpression<TFilter, TResultType>> mappings)
            where TResultType : new()
        {
            Implementor.StoreMapping<TResultType>(mappings);

            return this;
        }

        /// <summary>
        /// Finalizes the pipeline and compiles result mappings.
        /// </summary>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2&gt;.</returns>
        public ICommandProcessor<TFilter, TResult1, TResult2> Finalize()
        {
            return new MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2>(this);
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
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3>, IFluentPipelineFragment
        where TResult1 : new()
        where TResult2 : new()
        where TResult3 : new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResultExpression{TFilter, TResult1, TResult2, TResult3}"/> class.
        /// </summary>
        /// <param name="command">The command.</param>
        public CommandResultExpression(ICommandExpression<TFilter> command)
            : base(command) { }

        /// <summary>
        /// Provide mapping actions and options for a result set
        /// </summary>
        /// <typeparam name="TResultType">The type of the result.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult1, TResult2, TResult3&gt;.</returns>
        public ICommandResultExpression<TFilter, TResult1, TResult2, TResult3> ForResultsOfType<TResultType>(
            Action<IResultMappingExpression<TFilter, TResultType>> mappings)
                where TResultType : new()
        {
            Implementor.StoreMapping<TResultType>(mappings);

            return this;
        }

        /// <summary>
        /// Finalizes the pipeline and compiles result mappings.
        /// </summary>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2, TResult3&gt;.</returns>
        public ICommandProcessor<TFilter, TResult1, TResult2, TResult3> Finalize()
        {
            return new MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2, TResult3>(this);
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
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4>, IFluentPipelineFragment
        where TResult1 : new()
        where TResult2 : new()
        where TResult3 : new()
        where TResult4 : new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResultExpression{TFilter, TResult1, TResult2, TResult3, TResult4}"/> class.
        /// </summary>
        /// <param name="command">The command.</param>
        public CommandResultExpression(ICommandExpression<TFilter> command)
            : base(command) { }

        /// <summary>
        /// Provide mapping actions and options for a result set
        /// </summary>
        /// <typeparam name="TResultType">The type of the result.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult1, TResult2, TResult3, TResult4&gt;.</returns>
        public ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4> ForResultsOfType<TResultType>(
            Action<IResultMappingExpression<TFilter, TResultType>> mappings)
                where TResultType : new()
        {
            Implementor.StoreMapping<TResultType>(mappings);

            return this;
        }

        /// <summary>
        /// Finalizes the pipeline and compiles result mappings.
        /// </summary>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2, TResult3, TResult4&gt;.</returns>
        public ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4> Finalize()
        {
            return new MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4>(this);
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
    public class CommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5> : CommandResultCommon<TFilter>,
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5>, IFluentPipelineFragment
        where TResult1 : new()
        where TResult2 : new()
        where TResult3 : new()
        where TResult4 : new()
        where TResult5 : new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResultExpression{TFilter, TResult1, TResult2, TResult3, TResult4, TResult5}"/> class.
        /// </summary>
        /// <param name="command">The command.</param>
        public CommandResultExpression(ICommandExpression<TFilter> command)
            : base(command) { }

        /// <summary>
        /// Provide mapping actions and options for a result set
        /// </summary>
        /// <typeparam name="TResultType">The type of the result.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5&gt;.</returns>
        public ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5> ForResultsOfType<TResultType>(
            Action<IResultMappingExpression<TFilter, TResultType>> mappings)
                where TResultType : new()
        {
            Implementor.StoreMapping<TResultType>(mappings);

            return this;
        }

        /// <summary>
        /// Finalizes the pipeline and compiles result mappings.
        /// </summary>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5&gt;.</returns>
        public ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5> Finalize()
        {
            return new MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5>(this);
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
    public class CommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6> : CommandResultCommon<TFilter>,
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>, IFluentPipelineFragment
        where TResult1 : new()
        where TResult2 : new()
        where TResult3 : new()
        where TResult4 : new()
        where TResult5 : new()
        where TResult6 : new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResultExpression{TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6}"/> class.
        /// </summary>
        /// <param name="command">The command.</param>
        public CommandResultExpression(ICommandExpression<TFilter> command)
            : base(command) { }

        /// <summary>
        /// Provide mapping actions and options for a result set
        /// </summary>
        /// <typeparam name="TResultType">The type of the result.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6&gt;.</returns>
        public ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6> ForResultsOfType<TResultType>(
            Action<IResultMappingExpression<TFilter, TResultType>> mappings)
                where TResultType : new()
        {
            Implementor.StoreMapping<TResultType>(mappings);

            return this;
        }

        /// <summary>
        /// Finalizes the pipeline and compiles result mappings.
        /// </summary>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6&gt;.</returns>
        public ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6> Finalize()
        {
            return new MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>(this);
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
    public class CommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7> : CommandResultCommon<TFilter>,
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>, IFluentPipelineFragment
        where TResult1 : new()
        where TResult2 : new()
        where TResult3 : new()
        where TResult4 : new()
        where TResult5 : new()
        where TResult6 : new()
        where TResult7 : new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResultExpression{TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7}"/> class.
        /// </summary>
        /// <param name="command">The command.</param>
        public CommandResultExpression(ICommandExpression<TFilter> command)
            : base(command) { }

        /// <summary>
        /// Provide mapping actions and options for a result set
        /// </summary>
        /// <typeparam name="TResultType">The type of the result.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7&gt;.</returns>
        public ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7> ForResultsOfType<TResultType>(
            Action<IResultMappingExpression<TFilter, TResultType>> mappings)
                where TResultType : new()
        {
            Implementor.StoreMapping<TResultType>(mappings);

            return this;
        }

        /// <summary>
        /// Finalizes the pipeline and compiles result mappings.
        /// </summary>
        /// <returns>ICommandProcessor&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7&gt;.</returns>
        public ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7> Finalize()
        {
            return new MultipleResultSetCommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>(this);
        }
    }
}