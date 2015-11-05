using Susanoo.Command;
using Susanoo.Mapping;
using System;


namespace Susanoo.ResultSets
{
    /// <summary>
    /// Base implementation for CommandBuilder Results.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    public abstract class CommandResultExpression<TFilter> 
        : ICommandResultExpression<TFilter>, ICommandResultInfo<TFilter>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResultExpression{TFilter}" /> class.
        /// </summary>
        /// <param name="command">The CommandBuilder.</param>
        /// <param name="mappingStorage">The mappingStorage.</param>
        internal CommandResultExpression(ICommandBuilderInfo<TFilter> command,
            ICommandResultMappingStorage<TFilter> mappingStorage)
        {
            MappingStorage = mappingStorage;

            // ReSharper disable ArrangeThisQualifier
            //Mono required

            this.Command = command;
            // ReSharper restore ArrangeThisQualifier
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResultExpression{TFilter}" /> class.
        /// </summary>
        /// <param name="propertyMetadataExtractor">The property metadata extractor.</param>
        /// <param name="command">The CommandBuilder.</param>
        protected CommandResultExpression(IPropertyMetadataExtractor propertyMetadataExtractor, ICommandBuilderInfo<TFilter> command)
            : this(command, new CommandResultMappingStorage<TFilter>(propertyMetadataExtractor))
        {
        }

        /// <summary>
        /// Gets the CommandBuilder expression.
        /// </summary>
        /// <value>The CommandBuilder expression.</value>
        public virtual ICommandBuilderInfo<TFilter> Command { get; }

        /// <summary>
        /// Gets the mappingStorage of the Commandresult functionality.
        /// </summary>
        /// <value>The mappingStorage.</value>
        protected virtual ICommandResultMappingStorage<TFilter> MappingStorage { get; }


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
        /// Retrieves the result set mappings.
        /// </summary>
        /// <param name="resultType">Type of the result.</param>
        /// <returns>IMappingExport.</returns>
        public IMappingExport RetrieveResultSetMappings(Type resultType)
        {
            return MappingStorage.RetrieveExporter(resultType);
        }
    }
}