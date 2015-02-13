using System;
using System.Data.Common;
using Susanoo.Pipeline;
using Susanoo.Pipeline.Command;
using Susanoo.Pipeline.Command.ResultSets.Processing.Deserialization;

namespace Susanoo
{
    /// <summary>
    /// Provides all options for overriding Susanoo's behavior.
    /// </summary>
    public class SusanooBootstrapper : ISusanooBootstrapper
    {
        /// <summary>
        /// Gets or sets the command builder.
        /// </summary>
        /// <value>The command builder.</value>
        /// <exception cref="System.ArgumentNullException">value</exception>
        public virtual ICommandExpressionBuilder RetrieveCommandBuilder()
        {
            return new CommandBuilder();
        }

        /// <summary>
        /// Retrieves the deserializer resolver.
        /// </summary>
        /// <returns>IDeserializerResolver.</returns>
        public virtual IDeserializerResolver RetrieveDeserializerResolver()
        {
            return new DeserializerResolver();
        }

        /// <summary>
        /// Retrieves the property metadata extractor Default uses ComponentModel Attributes..
        /// </summary>
        /// <returns>IPropertyMetadataExtractor.</returns>
        public virtual IPropertyMetadataExtractor RetrievePropertyMetadataExtractor()
        {
            return new ComponentModelMetadataExtractor();
        }

        /// <summary>
        /// Called when an execution exception is encountered.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="parameters">The parameters.</param>
        public virtual void OnExecutionException(ICommandExpressionInfo info, Exception exception,
            DbParameter[] parameters)
        {
            throw exception;
        }
    }
}