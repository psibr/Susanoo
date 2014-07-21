using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Susanoo
{
    public abstract class CommandResultBridge<TFilter> : IFluentPipelineFragment
    {
        private readonly ICommandResultImplementor<TFilter> _Implementor;

        public virtual BigInteger CacheHash
        {
            get
            {
                return _Implementor.CacheHash;
            }
        }

        /// <summary>
        /// Gets the implementor of the Commandresult functionality.
        /// </summary>
        /// <value>The implementor.</value>
        protected virtual ICommandResultImplementor<TFilter> Implementor
        {
            get { return _Implementor; }
        }

        /// <summary>
        /// Gets the command expression.
        /// </summary>
        /// <value>The command expression.</value>
        public virtual ICommandExpression<TFilter> CommandExpression { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResultBridge{TFilter}"/> class.
        /// </summary>
        /// <param name="command">The command.</param>
        public CommandResultBridge(ICommandExpression<TFilter> command)
        {
            _Implementor = new CommandResultImplementor<TFilter>();

            this.CommandExpression = command;
        }

        /// <summary>
        /// Exports this instance to property mappings.
        /// </summary>
        /// <typeparam name="TResultType">The type of the t result type.</typeparam>
        /// <returns>IDictionary&lt;System.String, IPropertyMappingConfiguration&lt;System.Data.IDataRecord&gt;&gt;.</returns>
        public virtual IDictionary<string, IPropertyMappingConfiguration<System.Data.IDataRecord>> Export<TResultType>()
            where TResultType : new()
        {
            return this.Implementor.Export<TResultType>();
        }
    }

    public class CommandResultExpression<TFilter, TResult> : CommandResultBridge<TFilter>,
        ICommandResultExpression<TFilter, TResult>, IFluentPipelineFragment
            where TResult : new()
    {
        public CommandResultExpression(ICommandExpression<TFilter> command)
            : base(command) { }

        /// <summary>
        /// Provide mapping actions and options for a result set
        /// </summary>
        /// <typeparam name="TResultType">The type of the result.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult&gt;.</returns>
        public ICommandResultExpression<TFilter, TResult> ForResultSet(Action<IResultMappingExpression<TFilter, TResult>> mappings)
        {
            Implementor.StoreMapping<TResult>(mappings);

            return this;
        }

        public ICommandProcessor<TFilter, TResult> Finalize()
        {
            return new SingleResultSetCommandProcessor<TFilter, TResult>(this);
        }
    }

    public class CommandResultExpression<TFilter, TResult1, TResult2> : CommandResultBridge<TFilter>,
        ICommandResultExpression<TFilter, TResult1, TResult2>, IFluentPipelineFragment
        where TResult1 : new()
        where TResult2 : new()
    {
        public CommandResultExpression(ICommandExpression<TFilter> command)
            : base(command) { }

        /// <summary>
        /// Provide mapping actions and options for a result set
        /// </summary>
        /// <typeparam name="TResultType">The type of the result.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult1, TResult2&gt;.</returns>
        public ICommandResultExpression<TFilter, TResult1, TResult2> ForResultSet<TResultType>(Action<IResultMappingExpression<TFilter, TResultType>> mappings)
            where TResultType : new()
        {
            Implementor.StoreMapping<TResultType>(mappings);

            return this;
        }
    }

    public class CommandResultExpression<TFilter, TResult1, TResult2, TResult3> : CommandResultBridge<TFilter>,
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3>, IFluentPipelineFragment
        where TResult1 : new()
        where TResult2 : new()
        where TResult3 : new()
    {
        public CommandResultExpression(ICommandExpression<TFilter> command)
            : base(command) { }

        /// <summary>
        /// Provide mapping actions and options for a result set
        /// </summary>
        /// <typeparam name="TResultType">The type of the result.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult1, TResult2, TResult3&gt;.</returns>
        public ICommandResultExpression<TFilter, TResult1, TResult2, TResult3> ForResultSet<TResultType>(
            Action<IResultMappingExpression<TFilter, TResultType>> mappings)
                where TResultType : new()
        {
            Implementor.StoreMapping<TResultType>(mappings);

            return this;
        }
    }

    public class CommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4> : CommandResultBridge<TFilter>,
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4>, IFluentPipelineFragment
        where TResult1 : new()
        where TResult2 : new()
        where TResult3 : new()
        where TResult4 : new()
    {
        public CommandResultExpression(ICommandExpression<TFilter> command)
            : base(command) { }

        /// <summary>
        /// Provide mapping actions and options for a result set
        /// </summary>
        /// <typeparam name="TResultType">The type of the result.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult1, TResult2, TResult3, TResult4&gt;.</returns>
        public ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4> ForResultSet<TResultType>(
            Action<IResultMappingExpression<TFilter, TResultType>> mappings)
                where TResultType : new()
        {
            Implementor.StoreMapping<TResultType>(mappings);

            return this;
        }
    }

    public class CommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5> : CommandResultBridge<TFilter>,
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5>, IFluentPipelineFragment
        where TResult1 : new()
        where TResult2 : new()
        where TResult3 : new()
        where TResult4 : new()
        where TResult5 : new()
    {
        public CommandResultExpression(ICommandExpression<TFilter> command)
            : base(command) { }

        /// <summary>
        /// Provide mapping actions and options for a result set
        /// </summary>
        /// <typeparam name="TResultType">The type of the result.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5&gt;.</returns>
        public ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5> ForResultSet<TResultType>(
            Action<IResultMappingExpression<TFilter, TResultType>> mappings)
                where TResultType : new()
        {
            Implementor.StoreMapping<TResultType>(mappings);

            return this;
        }
    }

    public class CommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6> : CommandResultBridge<TFilter>,
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>, IFluentPipelineFragment
        where TResult1 : new()
        where TResult2 : new()
        where TResult3 : new()
        where TResult4 : new()
        where TResult5 : new()
        where TResult6 : new()
    {
        public CommandResultExpression(ICommandExpression<TFilter> command)
            : base(command) { }

        /// <summary>
        /// Provide mapping actions and options for a result set
        /// </summary>
        /// <typeparam name="TResultType">The type of the result.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6&gt;.</returns>
        public ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6> ForResultSet<TResultType>(
            Action<IResultMappingExpression<TFilter, TResultType>> mappings)
                where TResultType : new()
        {
            Implementor.StoreMapping<TResultType>(mappings);

            return this;
        }
    }

    public class CommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7> : CommandResultBridge<TFilter>,
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>, IFluentPipelineFragment
        where TResult1 : new()
        where TResult2 : new()
        where TResult3 : new()
        where TResult4 : new()
        where TResult5 : new()
        where TResult6 : new()
        where TResult7 : new()
    {
        public CommandResultExpression(ICommandExpression<TFilter> command)
            : base(command) { }

        /// <summary>
        /// Provide mapping actions and options for a result set
        /// </summary>
        /// <typeparam name="TResultType">The type of the result.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <returns>ICommandResultExpression&lt;TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7&gt;.</returns>
        public ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7> ForResultSet<TResultType>(
            Action<IResultMappingExpression<TFilter, TResultType>> mappings)
                where TResultType : new()
        {
            Implementor.StoreMapping<TResultType>(mappings);

            return this;
        }
    }
}
