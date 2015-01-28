using System;
using System.Collections.Generic;
using System.Numerics;
using Susanoo.Pipeline.Command.ResultSets.Mapping.Properties;

namespace Susanoo.Pipeline.Command.ResultSets.Mapping
{
    /// <summary>
    /// Simple mapping when none were explicitly provided.
    /// </summary>
    public class DefaultResultMapping : IResultMappingExport
    {
        private readonly IDictionary<string, IPropertyMapping> _mappingActions =
            new Dictionary<string, IPropertyMapping>();

        private readonly Type _resultType;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultResultMapping"/> class.
        /// </summary>
        /// <param name="resultType">Type of the result.</param>
        public DefaultResultMapping(Type resultType)
        {
            _resultType = resultType;
        }

        /// <summary>
        /// Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public BigInteger CacheHash
        {
            get { return 0; }
        }

        /// <summary>
        /// Exports this instance.
        /// </summary>
        /// <returns>IDictionary&lt;System.String, Action&lt;IPropertyMappingConfiguration&lt;IDataRecord&gt;&gt;&gt;.</returns>
        public IDictionary<string, IPropertyMapping> Export()
        {
            foreach (var item in new ComponentModelMetadataExtractor()
                .FindAllowedProperties(_resultType, DescriptorActions.Read))
            {
                var item1 = item;

                var configuration = new PropertyMappingConfiguration(item1.Key);

                configuration.UseAlias(item1.Value.ActiveAlias);

                _mappingActions.Add(item.Key.Name, configuration);
            }

            return _mappingActions;
        }


    }
}
