using Susanoo.Mapping.Properties;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Susanoo.Mapping
{
    /// <summary>
    /// Simple mapping when none were explicitly provided.
    /// </summary>
    public class DefaultResultMapping : ResultMappingBase, IMappingExport
    {

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
        }

        /// <summary>
        /// Gets the mapping actions.
        /// </summary>
        /// <value>The mapping actions.</value>
        private IDictionary<string, IPropertyMapping> MappingActions
        {
            get
            {
                if (_mappingActions == null || _mappingActions.Count == 0)
                {
                    _mappingActions = new Dictionary<string, IPropertyMapping>();
                    MapDeclarativeProperties();
                }

                return _mappingActions;
            }
        }

        /// <summary>
        /// Maps the declarative properties.
        /// </summary>
        public void MapDeclarativeProperties()
        {
            foreach (var item in _propertyMetadataExtractor.FindAllowedProperties(_resultType, _actions))
            {
                TryAddMapping(item);
            }
        }
        
        /// <summary>
        /// Exports this instance.
        /// </summary>
        /// <returns>IDictionary&lt;System.String, Action&lt;IPropertyMappingConfiguration&lt;IDataRecord&gt;&gt;&gt;.</returns>
        public IDictionary<string, IPropertyMapping> Export()
        {
            return MappingActions;
        }
    }
}
