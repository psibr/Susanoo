using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Susanoo
{
    public class CommandResultImplementor<TFilter> : ICommandResultImplementor<TFilter>, IFluentPipelineFragment
    {
        private readonly IDictionary<Type, IFluentPipelineFragment> _MappingContainer;

        public CommandResultImplementor()
        {
            _MappingContainer = new Dictionary<Type, IFluentPipelineFragment>();
        }

        public System.Numerics.BigInteger CacheHash
        {
            get
            {
                return _MappingContainer.Aggregate(default(BigInteger), (p, c) => (p * 31) ^ c.Value.CacheHash);
            }
        }

        /// <summary>
        /// Retrieves the mapping.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <returns>IResultMappingExpression&lt;TFilter, TResult&gt;.</returns>
        public virtual IResultMappingExpression<TFilter, TResult> RetrieveMapping<TResult>() where TResult : new()
        {
            IResultMappingExpression<TFilter, TResult> result = null;

            if (_MappingContainer.ContainsKey(typeof(TResult)))
                result = _MappingContainer[typeof(TResult)] as IResultMappingExpression<TFilter, TResult>;

            return result ?? new ResultMappingExpression<TFilter, TResult>();
        }

        /// <summary>
        /// Stores the mapping.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="mapping">The mapping.</param>
        public virtual void StoreMapping<TResult>(Action<IResultMappingExpression<TFilter, TResult>> mapping) where TResult : new()
        {
            if (_MappingContainer.ContainsKey(typeof(TResult)))
            {
                mapping(_MappingContainer[typeof(TResult)] as IResultMappingExpression<TFilter, TResult>);
            }
            else
            {
                IResultMappingExpression<TFilter, TResult> mappingExpression = new ResultMappingExpression<TFilter, TResult>();

                mapping(mappingExpression);

                _MappingContainer.Add(typeof(TResult), mappingExpression);
            }
        }

        /// <summary>
        /// Exports this instance.
        /// </summary>
        /// <typeparam name="TResultType">The type of the t result type.</typeparam>
        /// <returns>IDictionary&lt;System.String, IPropertyMappingConfiguration&lt;System.Data.IDataRecord&gt;&gt;.</returns>
        public IDictionary<string, IPropertyMapping> Export<TResultType>() where TResultType : new()
        {
            return this.RetrieveMapping<TResultType>()
                .Export();
        }
    }
}