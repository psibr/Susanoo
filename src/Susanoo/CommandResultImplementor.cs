#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#endregion

namespace Susanoo
{
    /// <summary>
    ///     Provides a common class for ICommandResultExpressions to store and retrieve mappings.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    public class CommandResultImplementor<TFilter> : ICommandResultImplementor<TFilter>
    {
        private readonly IDictionary<Type, IFluentPipelineFragment> _mappingContainer;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CommandResultImplementor{TFilter}" /> class.
        /// </summary>
        public CommandResultImplementor()
        {
            _mappingContainer = new Dictionary<Type, IFluentPipelineFragment>();
        }

        /// <summary>
        ///     Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public BigInteger CacheHash
        {
            get { return _mappingContainer.Aggregate(default(BigInteger), (p, c) => (p * 31) ^ c.Value.CacheHash); }
        }

        /// <summary>
        ///     Retrieves the mapping.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <returns>IResultMappingExpression&lt;TFilter, TResult&gt;.</returns>
        public virtual IResultMappingExpression<TFilter, TResult> RetrieveMapping<TResult>() where TResult : new()
        {
            IResultMappingExpression<TFilter, TResult> result = null;

            if (_mappingContainer.ContainsKey(typeof(TResult)))
                result = _mappingContainer[typeof(TResult)] as IResultMappingExpression<TFilter, TResult>;

            return result ?? new ResultMappingExpression<TFilter, TResult>();
        }

        /// <summary>
        ///     Stores the mapping.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="mapping">The mapping.</param>
        public virtual void StoreMapping<TResult>(Action<IResultMappingExpression<TFilter, TResult>> mapping)
            where TResult : new()
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
        ///     Exports this instance.
        /// </summary>
        /// <typeparam name="TResultType">The type of the t result type.</typeparam>
        /// <returns>IDictionary&lt;System.String, IPropertyMappingConfiguration&lt;System.Data.IDataRecord&gt;&gt;.</returns>
        public IDictionary<string, IPropertyMapping> Export<TResultType>() where TResultType : new()
        {
            return RetrieveMapping<TResultType>()
                .Export();
        }
    }
}