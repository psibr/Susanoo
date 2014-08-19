#region

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

#endregion

namespace Susanoo
{
    /// <summary>
    ///     A step in the command definition Fluent API, in which properties are mapped to potential result data.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public class ResultMappingImplementor<TResult>
        : IResultMappingImplementor<TResult>
        where TResult : new()
    {
        private readonly IDictionary<string, Action<IPropertyMappingConfiguration>> _mappingActions =
            new Dictionary<string, Action<IPropertyMappingConfiguration>>();

        private IPropertyMetadataExtractor _propertyMetadataExtractor = new ComponentModelMetadataExtractor();

        /// <summary>
        ///     Initializes a new instance of the <see cref="ResultMappingImplementor{TResult}" /> class.
        /// </summary>
        public ResultMappingImplementor()
        {
            MapDeclarativeProperties();
        }

        /// <summary>
        ///     Gets or sets the property metadata extractor.
        /// </summary>
        /// <value>The property metadata extractor.</value>
        protected IPropertyMetadataExtractor PropertyMetadataExtractor
        {
            get { return _propertyMetadataExtractor; }
            set { if (value != null) _propertyMetadataExtractor = value; }
        }

        /// <summary>
        ///     Clears the result mappings.
        /// </summary>
        public virtual void ClearMappings()
        {
            _mappingActions.Clear();
        }

        /// <summary>
        ///     Mapping options for a property in the result model.
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
        ///     Mapping options for a property in the result model.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="options">The options.</param>
        public virtual void ForProperty(
            string propertyName,
            Action<IPropertyMappingConfiguration> options)
        {
            if (!_mappingActions.ContainsKey(propertyName))
                _mappingActions.Add(propertyName, options);
            else
                _mappingActions[propertyName] = options;
        }

        /// <summary>
        ///     Exports this instance.
        /// </summary>
        /// <returns>IDictionary&lt;System.String, Action&lt;IPropertyMappingConfiguration&lt;IDataRecord&gt;&gt;&gt;.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual IDictionary<string, IPropertyMapping> Export()
        {
            var exportDictionary = new Dictionary<string, IPropertyMapping>();

            foreach (var item in _mappingActions)
            {
                var config = new PropertyMappingConfiguration(typeof (TResult).GetProperty(item.Key));
                item.Value.Invoke(config);

                exportDictionary.Add(item.Key, config);
            }

            return exportDictionary;
        }

        /// <summary>
        ///     Maps the declarative properties.
        /// </summary>
        public void MapDeclarativeProperties()
        {
            foreach (var item in PropertyMetadataExtractor
                .FindAllowedProperties(typeof (TResult), DescriptorActions.Read))
            {
                KeyValuePair<PropertyInfo, PropertyMap> item1 = item;
                _mappingActions.Add(item.Key.Name, o => o.UseAlias(item1.Value.ActiveAlias));
            }
        }
    }
}