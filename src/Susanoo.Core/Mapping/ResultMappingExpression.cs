#region

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using Susanoo.Mapping.Properties;
using Susanoo.Pipeline;

#endregion

namespace Susanoo.Mapping
{
    /// <summary>
    /// A step in the CommandBuilder definition Fluent API, in which properties are mapped to potential result data.
    /// </summary>
    /// <typeparam name="TFilter">The type of the t filter.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public class ResultMappingExpression<TFilter, TResult>
        : IResultMappingExpression<TFilter, TResult>
    {
        private readonly IDictionary<string, IPropertyMapping> _mappingActions =
            new Dictionary<string, IPropertyMapping>();

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
        /// Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public BigInteger CacheHash =>
            _mappingActions.Aggregate(HashBuilder.Seed, (i, pair) =>
                (i * 31) ^ pair.Value.CacheHash);

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
            var config = new PropertyMappingConfiguration(typeof(TResult).GetProperty(propertyName));
            options.Invoke(config);

            if (!_mappingActions.ContainsKey(propertyName))
                _mappingActions.Add(propertyName, config);
            else
                _mappingActions[propertyName] = config;

            return this;
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
            foreach (var item in _propertyMetadataExtractor
                .FindAllowedProperties(typeof(TResult)))
            {
                var configuration = new PropertyMappingConfiguration(item.Key);

                configuration.UseAlias(item.Value.ActiveAlias);

                _mappingActions.Add(item.Key.Name, configuration);
            }
        }
    }
}