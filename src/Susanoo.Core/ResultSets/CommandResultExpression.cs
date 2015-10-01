using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Susanoo.Command;
using Susanoo.Mapping.Properties;

namespace Susanoo.ResultSets
{
    /// <summary>
    /// Base implementation for CommandBuilder Results.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    public abstract class CommandResultExpression<TFilter> 
        : ICommandResultExpression<TFilter>, ICommandResultInfo<TFilter>, ICommandResultMappingExporter
    {
        private readonly ICommandResultMappingStorage<TFilter> _mappingStorage;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResultExpression{TFilter}" /> class.
        /// </summary>
        /// <param name="command">The CommandBuilder.</param>
        /// <param name="mappingStorage">The mappingStorage.</param>
        internal CommandResultExpression(ICommandBuilderInfo<TFilter> command, ICommandResultMappingStorage<TFilter> mappingStorage)
        {
            _mappingStorage = mappingStorage;

            Command = command;
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
        /// Gets the CommandBuilder expression.
        /// </summary>
        /// <value>The CommandBuilder expression.</value>
        public virtual ICommandBuilderInfo<TFilter> Command { get; }

        /// <summary>
        /// Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public virtual BigInteger CacheHash => 
            (_mappingStorage.CacheHash ^ (Command.CacheHash * 31));

        /// <summary>
        /// Gets the mappingStorage of the Commandresult functionality.
        /// </summary>
        /// <value>The mappingStorage.</value>
        protected virtual ICommandResultMappingStorage<TFilter> MappingStorage => 
            _mappingStorage;

        /// <summary>
        /// Exports a results mappings for processing.
        /// </summary>
        /// <param name="resultType">Type of the result.</param>
        /// <returns>IDictionary&lt;System.String, IPropertyMapping&gt;.</returns>
        public IDictionary<string, IPropertyMapping> Export(Type resultType)
        {
            return MappingStorage.Export(resultType);
        }

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
        ///     Gets the mappings exporter.
        /// </summary>
        /// <returns>ICommandResultMappingExporter.</returns>
        public ICommandResultMappingExporter GetExporter()
        {
            return this;
        }


    }
}