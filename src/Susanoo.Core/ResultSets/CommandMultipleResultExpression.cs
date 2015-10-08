#region

using System;
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
        private readonly IMultipleResultSetCommandProcessorFactory _multipleResultSetCommandProcessorFactory;
        private readonly Type[] _resultTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandMultipleResultExpression{TFilter}" /> class.
        /// </summary>
        /// <param name="propertyMetadataExtractor">The property metadata extractor.</param>
        /// <param name="multipleResultSetCommandProcessorFactory">The multiple result set command processor factory.</param>
        /// <param name="command">The CommandBuilder.</param>
        /// <param name="resultTypes">The result types.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public CommandMultipleResultExpression(
            IPropertyMetadataExtractor propertyMetadataExtractor,
            IMultipleResultSetCommandProcessorFactory multipleResultSetCommandProcessorFactory,
            ICommandBuilderInfo<TFilter> command, params Type[] resultTypes)
            : base(propertyMetadataExtractor, command)
        {
            if (multipleResultSetCommandProcessorFactory == null)
                throw new ArgumentNullException(nameof(multipleResultSetCommandProcessorFactory));

            _multipleResultSetCommandProcessorFactory = multipleResultSetCommandProcessorFactory;
            _resultTypes = resultTypes;
        }

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
            return _multipleResultSetCommandProcessorFactory
                       .BuildCommandProcessor(this, _resultTypes);
        }
    }
}