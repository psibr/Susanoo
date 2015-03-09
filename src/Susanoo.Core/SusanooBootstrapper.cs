using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text.RegularExpressions;
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
        public virtual IEnumerable<Type> RetrieveIgnoredPropertyAttributes()
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

        private Regex _orderByRegex;

        /// <summary>
        /// Retrieves the order by regex used for whitelisting allowed cahracters.
        /// </summary>
        /// <returns>Regex.</returns>
        public virtual Regex RetrieveOrderByRegex()
        {
            return _orderByRegex ?? (_orderByRegex = new Regex(
                @"\A
		            # 1. Match all of these conditions
		            (?:
		              # 2. Leading Whitespace
		              \ *
		              # 3. ColumnName: a-z, A-Z, 0-9, _
		              (?<ColumnName>[0-9_a-z]*)
		              # 4. Whitespace
		              \ *
		              # 5. SortDirection: ASC or DESC case-insensitive
		              (?<SortDirection>ASC|DESC)?
		              # 6. Optional Comma
		              ,?
		            )*
		            \z",
                RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace));
        }
    }
}