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
        /// Adds a query wrapper.
        /// </summary>
        protected void AddQueryWrapper()
        {
            const string format =
@"SELECT *
FROM (
    {0}
) susanoo_query_wrapper
WHERE 1=1";

            TryAddCommandModifier(new CommandModifier
            {
                Description = "SusanooWrapper",
                Priority = 100,
                ModifierFunc = info => new ExecutableCommandInfo
                {
                    CommandText = string.Format(format, info.CommandText),
                    DbCommandType = info.DbCommandType,
                    Parameters = info.Parameters
                },
                CacheHash = HashBuilder.Compute(format)
            });
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
                return (_implementor.CacheHash ^ (Command.CacheHash * 31)) ^
                       CommandModifiers.Aggregate(new BigInteger(0), (i, modifier) => (i * 31) ^ modifier.CacheHash);
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
        /// Tries to add command modifier.
        /// </summary>
        /// <param name="modifier">The modifier.</param>
        /// <returns><c>true</c> if no other modifier exists with the same priority, <c>false</c> otherwise.</returns>
        public bool TryAddCommandModifier(CommandModifier modifier)
        {

            var result = !_commandModifiers.ContainsKey(modifier.Priority);

            if (result)
                _commandModifiers.Add(modifier.Priority, modifier);

            return result;

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

        /// <summary>
        /// Gets the command modifiers.
        /// </summary>
        /// <value>The command modifiers.</value>
        public IEnumerable<CommandModifier> CommandModifiers
        {
            get { return _commandModifiers.Values; }
        }

        private readonly IDictionary<int, CommandModifier> _commandModifiers = new Dictionary<int, CommandModifier>();
    }
}