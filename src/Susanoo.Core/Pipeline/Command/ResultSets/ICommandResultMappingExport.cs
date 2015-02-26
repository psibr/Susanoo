using System;
using System.Collections.Generic;
using Susanoo.Pipeline.Command.ResultSets.Mapping.Properties;

namespace Susanoo.Pipeline.Command.ResultSets
{
    /// <summary>
    /// Exposes property mapping export capabilities.
    /// </summary>
    public interface ICommandResultMappingExport : 
        IFluentPipelineFragment
    {
        /// <summary>
        /// Exports a results mappings for processing.
        /// </summary>
        /// <param name="resultType">Type of the result.</param>
        /// <returns>IDictionary&lt;System.String, IPropertyMapping&gt;.</returns>
        IDictionary<string, IPropertyMapping> Export(Type resultType);
    }
}