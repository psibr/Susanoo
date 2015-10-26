using Susanoo.Mapping.Properties;
using System.Collections.Generic;

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