using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Susanoo.Command;
using Susanoo.Processing;

namespace Susanoo.ResultSets
{
    /// <summary>
    /// Allows building ICommandSingleResultExpressions.
    /// </summary>
    public class CommandSingleResultExpressionFactory 
        : ICommandSingleResultExpressionFactory
    {
        private readonly IPropertyMetadataExtractor _propertyMetadataExtractor;
        private readonly ISingleResultSetCommandProcessorFactory _singleResultSetCommandProcessorFactory;

        /// <summary>
        /// Provides dependencies for ICommandMultipleResultExpressions.
        /// </summary>
        /// <param name="propertyMetadataExtractor">The property metadata extractor.</param>
        /// <param name="singleResultSetCommandProcessorFactory">The single result set command processor factory.</param>
        public CommandSingleResultExpressionFactory(
            IPropertyMetadataExtractor propertyMetadataExtractor,
            ISingleResultSetCommandProcessorFactory singleResultSetCommandProcessorFactory)
        {
            _propertyMetadataExtractor = propertyMetadataExtractor;
            _singleResultSetCommandProcessorFactory = singleResultSetCommandProcessorFactory;
        }

        /// <summary>
        /// Builds the command single result expression.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="command">The command.</param>
        /// <returns>ICommandMultipleResultExpression&lt;TFilter&gt;.</returns>
        public ICommandSingleResultExpression<TFilter, TResult> BuildCommandSingleResultExpression<TFilter, TResult>(
            ICommandBuilderInfo<TFilter> command)
        {
            return new CommandSingleResultExpression<TFilter, TResult>(
                _propertyMetadataExtractor, _singleResultSetCommandProcessorFactory, command);
        } 
    }
}
