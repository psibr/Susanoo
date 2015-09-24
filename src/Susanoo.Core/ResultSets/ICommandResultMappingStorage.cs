#region

using System;
using System.Collections.Generic;
using Susanoo.Mapping;
using Susanoo.Mapping.Properties;
using Susanoo.Pipeline;

#endregion

namespace Susanoo.ResultSets
{
    /// <summary>
    /// Provides a common form of storage and retrieval for mapping details of results.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    public interface ICommandResultMappingStorage<TFilter> : IFluentPipelineFragment
    {
        /// <summary>
        /// Retrieves a mapping exporter.
        /// </summary>
        /// <param name="resultType">Type of the result.</param>
        /// <returns>IResultMappingExpression&lt;TFilter, TResult&gt;.</returns>
        IMappingExport RetrieveExporter(Type resultType);

        /// <summary>
        /// Stores a mapping.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="mapping">The mapping.</param>
        void StoreMapping<TResult>(Action<IResultMappingExpression<TFilter, TResult>> mapping);

        /// <summary>
        /// Exports a results mappings for processing.
        /// </summary>
        /// <param name="resultType">Type of the result.</param>
        /// <returns>IDictionary&lt;System.String, IPropertyMapping&gt;.</returns>
        IDictionary<string, IPropertyMapping> Export(Type resultType);
    }
}