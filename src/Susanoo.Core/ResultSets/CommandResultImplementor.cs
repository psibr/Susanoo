#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Susanoo.Mapping;
using Susanoo.Mapping.Properties;

#endregion

namespace Susanoo.ResultSets
{
    /// <summary>
    /// Provides a common class for ICommandResultExpressions to store and retrieve mappings.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    public class CommandResultImplementor<TFilter> : ICommandResultImplementor<TFilter>
    {
        private readonly IDictionary<Type, IMappingExport> _mappingContainer;
        private readonly IDictionary<Type, IMappingExport> _mappingContainerRuntime;
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResultImplementor{TFilter}" /> class.
        /// </summary>
        public CommandResultImplementor()
        {
            _mappingContainer = new Dictionary<Type, IMappingExport>();
            _mappingContainerRuntime = new Dictionary<Type, IMappingExport>();
        }

        /// <summary>
        /// Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public BigInteger CacheHash => 
            _mappingContainer.Aggregate(default(BigInteger), (p, c) =>
                ((p * 31) ^ c.Value.CacheHash));

        /// <summary>
        /// Retrieves a mapping exporter.
        /// </summary>
        /// <param name="resultType">Type of the result.</param>
        /// <returns>IResultMappingExpression&lt;TFilter, TResult&gt;.</returns>
        public IMappingExport RetrieveExporter(Type resultType)
        {
            IMappingExport result = null;

            IMappingExport value;
            if (!_mappingContainer.TryGetValue(resultType, out value))
            {
                if (!_mappingContainerRuntime.TryGetValue(resultType, out value))
                {
                    result = new DefaultResultMapping(resultType);
                    _mappingContainerRuntime.Add(resultType, result);
                }
            }

            return result ?? value;
        }

        /// <summary>
        /// Stores the mapping.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="mapping">The mapping.</param>
        public virtual void StoreMapping<TResult>(Action<IResultMappingExpression<TFilter, TResult>> mapping)
        {
            if (_mappingContainer.ContainsKey(typeof(TResult)))
            {
                mapping(_mappingContainer[typeof(TResult)] as IResultMappingExpression<TFilter, TResult>);
            }
            else
            {
                IResultMappingExpression<TFilter, TResult> mappingExpression =
                    new ResultMappingExpression<TFilter, TResult>();

                mapping(mappingExpression);

                _mappingContainer.Add(typeof(TResult), mappingExpression);
            }
        }

        /// <summary>
        /// Exports a results mappings for processing.
        /// </summary>
        /// <param name="resultType">Type of the result.</param>
        /// <returns>IDictionary&lt;System.String, IPropertyMapping&gt;.</returns>
        public IDictionary<string, IPropertyMapping> Export(Type resultType)
        {
            return RetrieveExporter(resultType)
                .Export();
        }
    }
}