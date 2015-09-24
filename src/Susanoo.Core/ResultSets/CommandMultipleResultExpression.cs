#region

using System;
using System.Linq;
using System.Numerics;
using Susanoo.Command;
using Susanoo.Mapping;
using Susanoo.Processing;

#endregion

namespace Susanoo.ResultSets
{
    /// <summary>
    /// Provides methods for customizing how results are handled and compiling result mappings.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    public class CommandMultipleResultExpression<TFilter> :
        CommandResultExpression<TFilter>,
        ICommandMultipleResultExpression<TFilter>
    {
        private readonly Type[] _resultTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandMultipleResultExpression{TFilter}" /> class.
        /// </summary>
        /// <param name="command">The CommandBuilder.</param>
        /// <param name="resultTypes">The result types.</param>
        public CommandMultipleResultExpression(ICommandBuilderInfo<TFilter> command, params Type[] resultTypes)
            : base(command)
        {
            _resultTypes = resultTypes;
        }

        /// <summary>
        ///     Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public override BigInteger CacheHash =>
            (base.CacheHash * 31)
            ^ HashBuilder.Compute(_resultTypes.Aggregate(string.Empty, (s, t) => s + t.AssemblyQualifiedName));

        /// <summary>
        ///     Provide mapping actions and options for a result set
        /// </summary>
        /// <param name="mappings">The mappings.</param>
        /// <returns>ICommandSingleResultExpression&lt;TFilter, TResult&gt;.</returns>
        public ICommandMultipleResultExpression<TFilter> ForResults<TResult>(
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
        public IMultipleResultSetCommandProcessor<TFilter> Realize(string name = null)
        {
            ICommandProcessorWithResults instance;
            IMultipleResultSetCommandProcessor<TFilter> result = null;

            if (name == null)
            {
                var hash = (CacheHash * 31) ^
                           GetTypeArgumentHashCode(typeof(IMultipleResultSetCommandProcessor<TFilter>));

                if (CommandManager.Instance.TryGetCommandProcessor(hash, out instance))
                    result = (IMultipleResultSetCommandProcessor<TFilter>)instance;
            }
            else
            {
                if (CommandManager.Instance.TryGetCommandProcessor(name, out instance))
                    result = (IMultipleResultSetCommandProcessor<TFilter>)instance;
            }

            return result ??
                   CommandManager.Instance.Bootstrapper
                       .ResolveDependency<IMultipleResultSetCommandProcessorFactory>()
                       .BuildCommandProcessor(this, name, _resultTypes);
        }
    }
}