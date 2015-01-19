#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Susanoo.Pipeline.Command.ResultSets.Mapping;
using Susanoo.Pipeline.Command.ResultSets.Mapping.Properties;

#endregion

namespace Susanoo.Pipeline.Command.ResultSets
{
    /// <summary>
    /// Provides a common class for ICommandResultExpressions to store and retrieve mappings.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    public class CommandResultImplementor<TFilter> : ICommandResultImplementor<TFilter>
    {
        private readonly IDictionary<Type, IFluentPipelineFragment> _mappingContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResultImplementor{TFilter}" /> class.
        /// </summary>
        public CommandResultImplementor()
        {
            _mappingContainer = new Dictionary<Type, IFluentPipelineFragment>();
        }

        /// <summary>
        /// Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public BigInteger CacheHash
        {
            get { return _mappingContainer.Aggregate(default(BigInteger), (p, c) => (p*31) ^ c.Value.CacheHash); }
        }

        /// <summary>
        /// Retrieves the mapping.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <returns>IResultMappingExpression&lt;TFilter, TResult&gt;.</returns>
        public virtual IResultMappingExpression<TFilter, TResult> RetrieveMapping<TResult>() where TResult : new()
        {
            IResultMappingExpression<TFilter, TResult> result = null;

            IFluentPipelineFragment value;
            if (_mappingContainer.TryGetValue(typeof(TResult), out value))
                result = value as IResultMappingExpression<TFilter, TResult>;

            return result ?? new ResultMappingExpression<TFilter, TResult>();
        }

        public IResultMappingExport RetrieveExporter(Type resultType)
        {
            IResultMappingExport result = null;

            IFluentPipelineFragment value;
            if (_mappingContainer.TryGetValue(resultType, out value))
                result = value as IResultMappingExport;

            return result ?? new DefaultResultMapping(resultType);
        }

        /// <summary>
        /// Stores the mapping.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="mapping">The mapping.</param>
        public virtual void StoreMapping<TResult>(Action<IResultMappingExpression<TFilter, TResult>> mapping)
            where TResult : new()
        {
            if (_mappingContainer.ContainsKey(typeof (TResult)))
            {
                mapping(_mappingContainer[typeof (TResult)] as IResultMappingExpression<TFilter, TResult>);
            }
            else
            {
                IResultMappingExpression<TFilter, TResult> mappingExpression =
                    new ResultMappingExpression<TFilter, TResult>();

                mapping(mappingExpression);

                _mappingContainer.Add(typeof (TResult), mappingExpression);
            }
        }

        /// <summary>
        /// Exports a results mappings for processing.
        /// </summary>
        /// <typeparam name="TResultType">The type of the result type.</typeparam>
        /// <returns>IDictionary&lt;System.String, IPropertyMapping&gt;.</returns>
        public IDictionary<string, IPropertyMapping> Export<TResultType>() where TResultType : new()
        {
            return RetrieveMapping<TResultType>()
                .Export();
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