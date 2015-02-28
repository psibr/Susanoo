using System;
using System.Collections.Generic;
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
        /// Retrieves a set of attributes to use to determine when to ignore a property unless explicitly included.
        /// </summary>
        /// <returns>System.Collections.Generic.IEnumerable&lt;System.Attribute&gt;.</returns>
        public IEnumerable<Type> RetrieveIgnoredPropertyAttributes()
        {
            return new [] { typeof(System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute) };
        }

        /// <summary>
        /// Called when an execution exception is encountered.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="parameters">The parameters.</param>
        public virtual void OnExecutionException(ICommandInfo info, Exception exception,
            DbParameter[] parameters)
        {
            throw exception;
        }
    }
}