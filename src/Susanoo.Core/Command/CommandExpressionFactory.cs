using Susanoo.Processing;
using Susanoo.ResultSets;
using System;
using System.Data;

namespace Susanoo.Command
{
    /// <summary>
    /// Allows construction of a command expression and provides dependencies.
    /// </summary>
    public class CommandExpressionFactory : ICommandExpressionFactory
    {
        private readonly IPropertyMetadataExtractor _propertyMetadataExtractor;
        private readonly INoResultSetCommandProcessorFactory _noResultSetCommandProcessorFactory;
        private readonly ICommandMultipleResultExpressionFactory _commandMultipleResultExpressionFactory;
        private readonly ICommandSingleResultExpressionFactory _commandSingleResultExpressionFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandExpressionFactory" /> class.
        /// </summary>
        /// <param name="propertyMetadataExtractor">The property metadata extractor.</param>
        /// <param name="noResultSetCommandProcessorFactory">The no result set command processor factory.</param>
        /// <param name="commandMultipleResultExpressionFactory">The command multiple result expression factory.</param>
        /// <param name="commandSingleResultExpressionFactory">The command single result expression factory.</param>
        /// <exception cref="System.ArgumentNullException">
        /// </exception>
        public CommandExpressionFactory(
            IPropertyMetadataExtractor propertyMetadataExtractor,
            INoResultSetCommandProcessorFactory noResultSetCommandProcessorFactory,
            ICommandMultipleResultExpressionFactory commandMultipleResultExpressionFactory,
            ICommandSingleResultExpressionFactory commandSingleResultExpressionFactory)
        {
            if (propertyMetadataExtractor == null)
                throw new ArgumentNullException(nameof(propertyMetadataExtractor));
            if (noResultSetCommandProcessorFactory == null)
                throw new ArgumentNullException(nameof(noResultSetCommandProcessorFactory));
            if (commandMultipleResultExpressionFactory == null)
                throw new ArgumentNullException(nameof(commandMultipleResultExpressionFactory));
            if (commandSingleResultExpressionFactory == null)
                throw new ArgumentNullException(nameof(commandSingleResultExpressionFactory));
            _propertyMetadataExtractor = propertyMetadataExtractor;
            _noResultSetCommandProcessorFactory = noResultSetCommandProcessorFactory;
            _commandMultipleResultExpressionFactory = commandMultipleResultExpressionFactory;
            _commandSingleResultExpressionFactory = commandSingleResultExpressionFactory;
        }

        /// <summary>
        /// Builds the command expression.
        /// </summary>
        /// <typeparam name="TFilter">The type of the t filter.</typeparam>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <returns>ICommandExpression&lt;TFilter&gt;.</returns>
        /// <exception cref="ArgumentNullException">commandText</exception>
        /// <exception cref="ArgumentException">No CommandBuilder text provided.;commandText
        /// or
        /// TableDirect is not supported.;commandType</exception>
        public virtual ICommandExpression<TFilter> BuildCommandExpression<TFilter>(string commandText, CommandType commandType)
        {
            return new CommandExpression<TFilter>(
                _propertyMetadataExtractor, 
                _noResultSetCommandProcessorFactory,
                _commandMultipleResultExpressionFactory,
                _commandSingleResultExpressionFactory, commandText, commandType);
        } 
    }
}
