using System.Collections.Generic;
using Susanoo.Mapping.Properties;

namespace Susanoo.Mapping
{
    /// <summary>
    /// Exposes property mapping export capabilities.
    /// </summary>
    public interface IMappingExport
    {
        /// <summary>
        /// Exports this instance.
        /// </summary>
        /// <returns>IDictionary&lt;System.String, Action&lt;IPropertyMappingConfiguration&lt;IDataRecord&gt;&gt;&gt;.</returns>
        IDictionary<string, IPropertyMapping> Export();
    }
}