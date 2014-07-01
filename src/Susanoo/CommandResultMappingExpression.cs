using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace Susanoo
{
    /// <summary>
    /// A step in the command definition Fluent API, in which properties are mapped to potential result data.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public class CommandResultMappingExpression<TFilter, TResult>
        : ICommandResultMappingExpression<TFilter, TResult>
        where TResult : new()
    {
        private readonly IDictionary<string, Action<IPropertyMappingConfiguration<IDataRecord>>> mappingActions = 
            new Dictionary<string, Action<IPropertyMappingConfiguration<IDataRecord>>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResultMappingExpression{TFilter, TResult}"/> class.
        /// </summary>
        /// <param name="commandExpression">The command expression.</param>
        public CommandResultMappingExpression(ICommandExpression<TFilter, TResult> commandExpression)
        {
            this.CommandExpression = commandExpression;

            MapDeclarativeProperties();
        }

        /// <summary>
        /// Gets the command expression.
        /// </summary>
        /// <value>The command expression.</value>
        public virtual ICommandExpression<TFilter, TResult> CommandExpression { get; private set; }

        /// <summary>
        /// Clears the result mappings.
        /// </summary>
        /// <returns>ICommandResultMappingExpression&lt;TFilter, TResult&gt;.</returns>
        public virtual ICommandResultMappingExpression<TFilter, TResult> ClearMappings()
        {
            this.mappingActions.Clear();

            return this;
        }

        /// <summary>
        /// Mapping options for a property in the result model.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="options">The options.</param>
        /// <returns>ICommandResultMappingExpression&lt;TFilter, TResult&gt;.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public virtual ICommandResultMappingExpression<TFilter, TResult> ForProperty(
            Expression<Func<TResult, object>> propertyExpression, 
            Action<IPropertyMappingConfiguration<IDataRecord>> options)
        {
            return ForProperty(propertyExpression.GetPropertyName(), options);
        }

        /// <summary>
        /// Mapping options for a property in the result model.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="options">The options.</param>
        /// <returns>ICommandResultMappingExpression&lt;TFilter, TResult&gt;.</returns>
        public virtual ICommandResultMappingExpression<TFilter, TResult> ForProperty(
            string propertyName,
            Action<IPropertyMappingConfiguration<IDataRecord>> options)
        {
            this.mappingActions.Add(propertyName, options);

            return this;
        }

        /// <summary>
        /// Prepares the command for caching and executing.
        /// </summary>
        /// <returns>ICommandProcessor&lt;TFilter, TResult&gt;.</returns>
        public virtual ICommandProcessor<TFilter, TResult> PrepareCommand()
        {
            return new CommandProcessor<TFilter, TResult>(this);
        }

        /// <summary>
        /// Exports this instance.
        /// </summary>
        /// <returns>IDictionary&lt;System.String, Action&lt;IPropertyMappingConfiguration&lt;IDataRecord&gt;&gt;&gt;.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual IDictionary<string, IPropertyMappingConfiguration<IDataRecord>> Export()
        {
            var exportDictionary = new Dictionary<string, IPropertyMappingConfiguration<IDataRecord>>();

            foreach (var item in this.mappingActions)
            {
                var config = new PropertyMappingConfiguration<IDataRecord>(typeof(TResult).GetProperty(item.Key));
                item.Value.Invoke(config);

                exportDictionary.Add(item.Key, config);
            }

            return exportDictionary;
        }

        /// <summary>
        /// Maps the declarative properties.
        /// </summary>
        protected virtual void MapDeclarativeProperties()
        {
            foreach (var item in new ComponentModelMetadataExtractor()
                .FindAllowedProperties(typeof(TResult), Susanoo.DescriptorActions.Read))
            {
                mappingActions.Add(item.Key.Name, o => o.AliasProperty(item.Value.Alias));
            }
        }
    }
}