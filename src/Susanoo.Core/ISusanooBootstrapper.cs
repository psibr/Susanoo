using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text.RegularExpressions;
using Susanoo.Command;
using Susanoo.Deserialization;
using Susanoo.Pipeline;

namespace Susanoo
{
    /// <summary>
    /// Exposure points for extending or overriding Susanoo's behavior.
    /// </summary>
    public interface ISusanooBootstrapper
    {
        /// <summary>
        /// Gets or sets the CommandBuilder builder.
        /// </summary>
        /// <value>The CommandBuilder builder.</value>
        /// <exception cref="System.ArgumentNullException">value</exception>
        ICommandExpressionBuilder RetrieveCommandBuilder();

        /// <summary>
        /// Retrieves the deserializer resolver.
        /// </summary>
        /// <returns>IDeserializerResolver.</returns>
        IDeserializerResolver RetrieveDeserializerResolver();

        /// <summary>
        /// Retrieves the property metadata extractor Default uses ComponentModel Attributes.
        /// </summary>
        /// <returns>IPropertyMetadataExtractor.</returns>
        IPropertyMetadataExtractor RetrievePropertyMetadataExtractor();

        /// <summary>
        /// Retrieves a set of attributes to use to determine when to ignore a property unless explicitly included.
        /// </summary>
        /// <returns>System.Collections.Generic.IEnumerable&lt;System.Attribute&gt;.</returns>
        IEnumerable<Type> RetrieveIgnoredPropertyAttributes();

        /// <summary>
        /// Called when an execution exception is encountered.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="parameters">The parameters.</param>
        void OnExecutionException(ICommandInfo info, Exception exception,
            DbParameter[] parameters);
    }
}