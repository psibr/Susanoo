using Susanoo.Pipeline.Command.ResultSets.Mapping.Properties;
using System.Collections.Generic;

namespace Susanoo.Pipeline.Command.ResultSets.Mapping
{
    /// <summary>
    /// Exposes property mapping export capabilities.
    /// </summary>
    public interface IResultMappingExport : IFluentPipelineFragment
    {
        /// <summary>
        /// Exports this instance.
        /// </summary>
        /// <returns>IDictionary&lt;System.String, Action&lt;IPropertyMappingConfiguration&lt;IDataRecord&gt;&gt;&gt;.</returns>
        IDictionary<string, IPropertyMapping> Export();
    }
}