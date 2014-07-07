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
    public class ResultMappingImplementor<TFilter, TResult>
        : IResultMappingImplementor<TFilter, TResult>
        where TResult : new()
    {
        private readonly IDictionary<string, Action<IPropertyMappingConfiguration<IDataRecord>>> mappingActions =
            new Dictionary<string, Action<IPropertyMappingConfiguration<IDataRecord>>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultMappingImplementor{TFilter, TResult}"/> class.
        /// </summary>
        /// <param name="commandExpression">The command expression.</param>
        public ResultMappingImplementor()
        {
            MapDeclarativeProperties();
        }

        /// <summary>
        /// Clears the result mappings.
        /// </summary>
        public virtual void ClearMappings()
        {
            this.mappingActions.Clear();
        }

        /// <summary>
        /// Mapping options for a property in the result model.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="options">The options.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public virtual void ForProperty(
            Expression<Func<TResult, object>> propertyExpression,
            Action<IPropertyMappingConfiguration<IDataRecord>> options)
        {
            this.ForProperty(propertyExpression.GetPropertyName(), options);
        }

        /// <summary>
        /// Mapping options for a property in the result model.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="options">The options.</param>
        public virtual void ForProperty(
            string propertyName,
            Action<IPropertyMappingConfiguration<IDataRecord>> options)
        {
            if (!this.mappingActions.ContainsKey(propertyName))
                this.mappingActions.Add(propertyName, options);
            else
                this.mappingActions[propertyName] = options;
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
        public void MapDeclarativeProperties()
        {
            foreach (var item in this.PropertyMetadataExtractor
                .FindAllowedProperties(typeof(TResult), Susanoo.DescriptorActions.Read))
            {
                mappingActions.Add(item.Key.Name, o => o.AliasProperty(item.Value.Alias));
            }
        }

        private IPropertyMetadataExtractor _PropertyMetadataExtractor = new ComponentModelMetadataExtractor();

        /// <summary>
        /// Gets or sets the property metadata extractor.
        /// </summary>
        /// <value>The property metadata extractor.</value>
        protected virtual IPropertyMetadataExtractor PropertyMetadataExtractor
        {
            get { return this._PropertyMetadataExtractor; }
            set { if (value != null) this._PropertyMetadataExtractor = value; }
        }
    }
}