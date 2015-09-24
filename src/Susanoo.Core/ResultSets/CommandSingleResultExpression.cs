#region

using System;
using System.Numerics;
using Susanoo.Command;
using Susanoo.Mapping;
using Susanoo.Processing;

#endregion

namespace Susanoo.ResultSets
{
    /// <summary>
    ///     Provides methods for customizing how results are handled and compiling result mappings.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public class CommandSingleResultExpression<TFilter, TResult> :
        CommandResultExpression<TFilter>,
        ICommandSingleResultExpression<TFilter, TResult>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CommandSingleResultExpression{TFilter,TResult}" /> class.
        /// </summary>
        /// <param name="command">The CommandBuilder.</param>
        public CommandSingleResultExpression(ICommandBuilderInfo<TFilter> command)
            : base(command)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CommandSingleResultExpression{TFilter,TResult}" /> class.
        /// </summary>
        /// <param name="command">The CommandBuilder.</param>
        /// <param name="mappingStorage">The mappingStorage.</param>
        internal CommandSingleResultExpression(ICommandBuilderInfo<TFilter> command,
            ICommandResultMappingStorage<TFilter> mappingStorage)
            : base(command, mappingStorage)
        {
        }

        /// <summary>
        ///     Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public override BigInteger CacheHash =>
            (base.CacheHash*31)
            ^ (GetTypeArgumentHashCode(this.GetType())*31)
            ^ GetType().AssemblyQualifiedName.GetHashCode();

        /// <summary>
        ///     Provide mapping actions and options for a result set
        /// </summary>
        /// <param name="mappings">The mappings.</param>
        /// <returns>ICommandSingleResultExpression&lt;TFilter, TResult&gt;.</returns>
        public ICommandSingleResultExpression<TFilter, TResult> ForResults(
            Action<IResultMappingExpression<TFilter, TResult>> mappings)
        {
            MappingStorage.StoreMapping(mappings);

            return this;
        }

        /// <summary>
        ///     Realizes the pipeline and compiles result mappings.
        /// </summary>
        /// <param name="name">The name of the processor.</param>
        /// <returns>INoResultCommandProcessor&lt;TFilter, TResult&gt;.</returns>
        public ISingleResultSetCommandProcessor<TFilter, TResult> Realize(string name = null)
        {
            ICommandProcessorWithResults instance;
            ISingleResultSetCommandProcessor<TFilter, TResult> result = null;

            if (name == null)
            {
                var hash = (CacheHash * 31) ^
                           GetTypeArgumentHashCode(typeof(ISingleResultSetCommandProcessor<TFilter, TResult>));

                if (CommandManager.Instance.TryGetCommandProcessor(hash, out instance))
                    result = (ISingleResultSetCommandProcessor<TFilter, TResult>)instance;
            }
            else
            {
                if (CommandManager.Instance.TryGetCommandProcessor(name, out instance))
                    result = (ISingleResultSetCommandProcessor<TFilter, TResult>)instance;
            }

            return result ??
                   CommandManager.Instance.Bootstrapper
                       .ResolveDependency<ISingleResultSetCommandProcessorFactory>()
                       .BuildCommandProcessor<TFilter, TResult>(this, name);
        }
    }
}