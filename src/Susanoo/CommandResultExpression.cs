using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Susanoo
{
    public abstract class CommandResultBridge<TFilter>
    {
        private readonly ICommandResultImplementor<TFilter> _Implementor;

        protected virtual ICommandResultImplementor<TFilter> Implementor
        {
            get { return _Implementor; }
        }

        public virtual ICommandExpression<TFilter> CommandExpression { get; private set; }

        public CommandResultBridge(ICommandExpression<TFilter> command)
        {
            _Implementor = new CommandResultImplementor<TFilter>();

            this.CommandExpression = command;
        }

        public IDictionary<string, IPropertyMappingConfiguration<System.Data.IDataRecord>> Export<TResultType>()
            where TResultType : new()
        {
            return this.Implementor.Export<TResultType>();
        }
    }

    public class CommandResultExpression<TFilter, TResult> : CommandResultBridge<TFilter>,
        ICommandResultExpression<TFilter, TResult>
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
        ICommandResultExpression<TFilter, TResult1, TResult2>
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
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3>
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
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4>
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
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5>
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
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>
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
        ICommandResultExpression<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>
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
