using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Susanoo.Command;
using Susanoo.Mapping.Properties;
using Susanoo.Pipeline;

namespace Susanoo.ResultSets
{
    /// <summary>
    /// Base implementation for CommandBuilder Results.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    public abstract class CommandResultCommon<TFilter> : ICommandResultInfo<TFilter>, ICommandResultMappingExport, IFluentPipelineFragment
    {
        private readonly ICommandResultImplementor<TFilter> _implementor;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResultCommon{TFilter}" /> class.
        /// </summary>
        /// <param name="command">The CommandBuilder.</param>
        /// <param name="implementor">The implementor.</param>
        internal CommandResultCommon(ICommandBuilderInfo<TFilter> command, ICommandResultImplementor<TFilter> implementor)
        {
            _implementor = implementor;

            Command = command;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResultCommon{TFilter}" /> class.
        /// </summary>
        /// <param name="command">The CommandBuilder.</param>
        protected CommandResultCommon(ICommandBuilderInfo<TFilter> command)
            : this(command, new CommandResultImplementor<TFilter>())
        {
        }

        /// <summary>
        /// Gets the type argument hash code.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>BigInteger.</returns>
        internal static BigInteger GetTypeArgumentHashCode(Type type)
        {
            return type
                .GetGenericArguments()
                .Aggregate(new BigInteger(0), (p, arg) => (p * 31) ^ arg.AssemblyQualifiedName.GetHashCode());
        }

        /// <summary>
        /// Gets the CommandBuilder expression.
        /// </summary>
        /// <value>The CommandBuilder expression.</value>
        public virtual ICommandBuilderInfo<TFilter> Command { get; private set; }

        /// <summary>
        /// Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public virtual BigInteger CacheHash => 
            (_implementor.CacheHash ^ (Command.CacheHash * 31));

        /// <summary>
        /// Gets the implementor of the Commandresult functionality.
        /// </summary>
        /// <value>The implementor.</value>
        protected virtual ICommandResultImplementor<TFilter> Implementor => 
            _implementor;

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
            return new CommandResultExpression<TFilter, TSingle>(Command, Implementor);
        }

        /// <summary>
        /// Gets or sets the CommandBuilder information.
        /// </summary>
        /// <returns>ICommandBuilderInfo&lt;TFilter&gt;.</returns>
        /// <value>The CommandBuilder information.</value>
        public ICommandBuilderInfo<TFilter> GetCommandInfo()
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


    }
}