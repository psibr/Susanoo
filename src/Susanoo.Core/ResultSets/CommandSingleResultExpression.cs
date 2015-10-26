#region

using Susanoo.Command;
using Susanoo.Mapping;
using Susanoo.Processing;
using System;

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
        private readonly ISingleResultSetCommandProcessorFactory _singleResultSetCommandProcessorFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandSingleResultExpression{TFilter,TResult}" /> class.
        /// </summary>
        /// <param name="propertyMetadataExtractor">The property metadata extractor.</param>
        /// <param name="singleResultSetCommandProcessorFactory">The single result set command processor factory.</param>
        /// <param name="command">The CommandBuilder.</param>
        public CommandSingleResultExpression(
            IPropertyMetadataExtractor propertyMetadataExtractor,
            ISingleResultSetCommandProcessorFactory singleResultSetCommandProcessorFactory,
            ICommandBuilderInfo<TFilter> command)
            : base(propertyMetadataExtractor, command)
        {
            _singleResultSetCommandProcessorFactory = singleResultSetCommandProcessorFactory;
        }

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
        /// <returns>INoResultCommandProcessor&lt;TFilter, TResult&gt;.</returns>
        public ISingleResultSetCommandProcessor<TFilter, TResult> Realize()
        {
            return _singleResultSetCommandProcessorFactory
                       .BuildCommandProcessor<TFilter, TResult>(this);
        }
    }
}