using System;
using System.Collections.Generic;
using System.Numerics;
using Susanoo.Mapping.Properties;
using Susanoo.Pipeline;

namespace Susanoo.Mapping
{
    /// <summary>
    /// Simple mapping when none were explicitly provided.
    /// </summary>
    public class DefaultResultMapping 
        : IMappingExport
    {
        private readonly IDictionary<string, IPropertyMapping> _mappingActions =
            new Dictionary<string, IPropertyMapping>();

        private readonly IPropertyMetadataExtractor _propertyMetadataExtractor;
        private readonly Type _resultType;
        private readonly DescriptorActions _actions;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultResultMapping" /> class.
        /// </summary>
        /// <param name="propertyMetadataExtractor">The property metadata extractor.</param>
        /// <param name="resultType">Type of the result.</param>
        /// <param name="actions">The actions.</param>
        public DefaultResultMapping(IPropertyMetadataExtractor propertyMetadataExtractor, Type resultType, DescriptorActions actions = DescriptorActions.Read)
        {
            _propertyMetadataExtractor = propertyMetadataExtractor;
            _resultType = resultType;
            _actions = actions;

            MapDeclarativeProperties();
        }

        /// <summary>
        /// Maps the declarative properties.
        /// </summary>
        public void MapDeclarativeProperties()
        {
            foreach (var item in _propertyMetadataExtractor
                .FindAllowedProperties(_resultType, _actions))
            {
                var configuration = new PropertyMappingConfiguration(item.Key);

                configuration.UseAlias(item.Value.ActiveAlias);

                _mappingActions.Add(item.Key.Name, configuration);
            }
        }

        /// <summary>
        /// Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public BigInteger CacheHash => -1;

        /// <summary>
        /// Exports this instance.
        /// </summary>
        /// <returns>IDictionary&lt;System.String, Action&lt;IPropertyMappingConfiguration&lt;IDataRecord&gt;&gt;&gt;.</returns>
        public IDictionary<string, IPropertyMapping> Export()
        {
            return _mappingActions;
        }


    }
}
