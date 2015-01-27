#region

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using Susanoo.Pipeline.Command.ResultSets.Mapping.Properties;

#endregion

namespace Susanoo.Pipeline.Command.ResultSets.Mapping
{
    /// <summary>
    /// A step in the command definition Fluent API, in which properties are mapped to potential result data.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public class ResultMappingImplementor<TResult>
        : IResultMappingImplementor<TResult>
        where TResult : new()
    {
        private IPropertyMetadataExtractor _propertyMetadataExtractor = new ComponentModelMetadataExtractor();

        private readonly IDictionary<string, IPropertyMapping> _mappingActions =
            new Dictionary<string, IPropertyMapping>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultMappingImplementor{TResult}" /> class.
        /// </summary>
        public ResultMappingImplementor()
        {
            MapDeclarativeProperties();
        }

        /// <summary>
        /// Gets or sets the property metadata extractor.
        /// </summary>
        /// <value>The property metadata extractor.</value>
        protected IPropertyMetadataExtractor PropertyMetadataExtractor
        {
            get { return _propertyMetadataExtractor; }
            set { if (value != null) _propertyMetadataExtractor = value; }
        }

        /// <summary>
        /// Clears the result mappings.
        /// </summary>
        public virtual void ClearMappings()
        {
            _mappingActions.Clear();
        }

        /// <summary>
        /// Mapping options for a property in the result model.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="options">The options.</param>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public virtual void ForProperty(
            Expression<Func<TResult, object>> propertyExpression,
            Action<IPropertyMappingConfiguration> options)
        {
            ForProperty(propertyExpression.GetPropertyName(), options);
        }

        /// <summary>
        /// Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public BigInteger CacheHash
        {
            get { return _mappingActions.Aggregate(HashBuilder.Seed, (i, pair) => (i*31) ^ pair.Value.CacheHash); }
        }

        /// <summary>
        /// Mapping options for a property in the result model.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="options">The options.</param>
        public virtual void ForProperty(
            string propertyName,
            Action<IPropertyMappingConfiguration> options)
        {
            var config = new PropertyMappingConfiguration(typeof (TResult).GetProperty(propertyName));
            options.Invoke(config);

            if (!_mappingActions.ContainsKey(propertyName))
                _mappingActions.Add(propertyName, config);
            else
                _mappingActions[propertyName] = config;
        }

        /// <summary>
        /// Exports this instance.
        /// </summary>
        /// <returns>IDictionary&lt;System.String, Action&lt;IPropertyMappingConfiguration&lt;IDataRecord&gt;&gt;&gt;.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual IDictionary<string, IPropertyMapping> Export()
        {
            return new Dictionary<string, IPropertyMapping>(_mappingActions);
        }

        /// <summary>
        /// Maps the declarative properties.
        /// </summary>
        public void MapDeclarativeProperties()
        {
            foreach (var item in PropertyMetadataExtractor
                .FindAllowedProperties(typeof (TResult), DescriptorActions.Read))
            {
                var item1 = item;

                var configuration = new PropertyMappingConfiguration(item1.Key);

                configuration.UseAlias(item1.Value.ActiveAlias);

                _mappingActions.Add(item.Key.Name, configuration);
            }
        }
    }
}