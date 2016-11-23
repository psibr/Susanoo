#region

using Susanoo.Mapping.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

#endregion

namespace Susanoo.Mapping
{
    /// <summary>
    /// A step in the CommandBuilder definition Fluent API, in which properties are mapped to potential result data.
    /// </summary>
    /// <typeparam name="TFilter">The type of the t filter.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public class ResultMappingExpression<TFilter, TResult> : ResultMappingBase, IResultMappingExpression<TFilter, TResult>
    {
        private readonly IPropertyMetadataExtractor _propertyMetadataExtractor;

        /// <summary>
        /// Resolves dependencies for Result mapping expressions and instantiates.
        /// </summary>
        /// <param name="propertyMetadataExtractor">The property metadata extractor.</param>
        public ResultMappingExpression(IPropertyMetadataExtractor propertyMetadataExtractor)
        {
            _propertyMetadataExtractor = propertyMetadataExtractor;

            MapDeclarativeProperties();
        }

        /// <summary>
        /// Clears the result mappings.
        /// </summary>
        public virtual IResultMappingExpression<TFilter, TResult> ClearMappings()
        {
            _mappingActions.Clear();

            return this;
        }

        /// <summary>
        /// Mapping options for a property in the result model.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="options">The options.</param>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public virtual IResultMappingExpression<TFilter, TResult> ForProperty(
            Expression<Func<TResult, object>> propertyExpression,
            Action<IPropertyMappingConfiguration> options)
        {
            ForProperty(propertyExpression.GetPropertyName(), options);

            return this;
        }

        /// <summary>
        /// Mapping options for a property in the result model.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="options">The options.</param>
        public virtual IResultMappingExpression<TFilter, TResult> ForProperty(
            string propertyName,
            Action<IPropertyMappingConfiguration> options)
        {
            var propInfo = typeof (TResult).GetTypeInfo().GetProperty(propertyName);

            if (!_mappingActions.ContainsKey(propertyName))
                TryAddMapping(new KeyValuePair<PropertyInfo, PropertyMapping>(propInfo, new PropertyMapping(propInfo, propertyName)));

            var config = _mappingActions[propertyName] as IPropertyMappingConfiguration;

            options.Invoke(config);

            return this;
        }

        public virtual IResultMappingExpression<TFilter, TResult> MapPropertyToColumn(
            Expression<Func<TResult, object>> propertyExpression, string columnName)
        {
            return ForProperty(propertyExpression, configuration => configuration.UseAlias(columnName));
        }

        public virtual IResultMappingExpression<TFilter, TResult> MapPropertyToColumn(
            string propertyName, string columnName)
        {
            return ForProperty(propertyName, configuration => configuration.UseAlias(columnName));
        }

        /// <summary>
        /// Exports this instance.
        /// </summary>
        /// <returns>IDictionary&lt;System.String, Action&lt;IPropertyMappingConfiguration&lt;IDataRecord&gt;&gt;&gt;.</returns>
        public virtual IDictionary<string, IPropertyMapping> Export()
        {
            return new Dictionary<string, IPropertyMapping>(_mappingActions);
        }

        /// <summary>
        /// Maps the declarative properties.
        /// </summary>
        protected void MapDeclarativeProperties()
        {
            foreach (var item in _propertyMetadataExtractor.FindAllowedProperties(typeof(TResult).GetTypeInfo()))
            {
                TryAddMapping(item);
            }
        }
    }
}