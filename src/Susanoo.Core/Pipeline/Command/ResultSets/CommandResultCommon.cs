using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Susanoo.Pipeline.Command.ResultSets.Mapping.Properties;

namespace Susanoo.Pipeline.Command.ResultSets
{
    /// <summary>
    /// Base implementation for Command Results.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    public abstract class CommandResultCommon<TFilter> : ICommandResultInfo<TFilter>, ICommandResultMappingExport, IFluentPipelineFragment
    {
        private readonly ICommandResultImplementor<TFilter> _implementor;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResultCommon{TFilter}" /> class.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="implementor">The implementor.</param>
        internal CommandResultCommon(ICommandInfo<TFilter> command, ICommandResultImplementor<TFilter> implementor)
        {
            _implementor = implementor;

            Command = command;
        }





        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResultCommon{TFilter}" /> class.
        /// </summary>
        /// <param name="command">The command.</param>
        protected CommandResultCommon(ICommandInfo<TFilter> command)
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
        /// Gets the command expression.
        /// </summary>
        /// <value>The command expression.</value>
        public virtual ICommandInfo<TFilter> Command { get; private set; }

        /// <summary>
        /// Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public virtual BigInteger CacheHash
        {
            get
            {
                return (_implementor.CacheHash ^ (Command.CacheHash * 31));
            }
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
            return new CommandResultExpression<TFilter, TSingle>(Command, Implementor);
        }



        /// <summary>
        /// Gets or sets the command information.
        /// </summary>
        /// <returns>ICommandInfo&lt;TFilter&gt;.</returns>
        /// <value>The command information.</value>
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


    }
}