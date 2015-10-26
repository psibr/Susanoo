using Susanoo.Command;
using Susanoo.Processing;
using System;

namespace Susanoo.ResultSets
{
    /// <summary>
    /// Allows building ICommandMultipleResultExpressions.
    /// </summary>
    public class CommandMultipleResultExpressionFactory : ICommandMultipleResultExpressionFactory
    {
        private readonly IPropertyMetadataExtractor _propertyMetadataExtractor;
        private readonly IMultipleResultSetCommandProcessorFactory _multipleResultSetCommandProcessorFactory;

        /// <summary>
        /// Provides dependencies for ICommandMultipleResultExpressions.
        /// </summary>
        /// <param name="propertyMetadataExtractor">The property metadata extractor.</param>
        /// <param name="multipleResultSetCommandProcessorFactory">The multiple result set command processor factory.</param>
        public CommandMultipleResultExpressionFactory(
            IPropertyMetadataExtractor propertyMetadataExtractor,
            IMultipleResultSetCommandProcessorFactory multipleResultSetCommandProcessorFactory)
        {
            _propertyMetadataExtractor = propertyMetadataExtractor;
            _multipleResultSetCommandProcessorFactory = multipleResultSetCommandProcessorFactory;
        }

        /// <summary>
        /// Builds the command multiple result expression.
        /// </summary>
        /// <typeparam name="TFilter">The type of the t filter.</typeparam>
        /// <param name="command">The command.</param>
        /// <param name="resultTypes">The result types.</param>
        /// <returns>ICommandMultipleResultExpression&lt;TFilter&gt;.</returns>
        public ICommandMultipleResultExpression<TFilter> BuildCommandMultipleResultExpression<TFilter>(
            ICommandBuilderInfo<TFilter> command, params Type[] resultTypes)
        {
            return new CommandMultipleResultExpression<TFilter>(
                _propertyMetadataExtractor, 
                _multipleResultSetCommandProcessorFactory,
                command, resultTypes);
        } 
    }
}
