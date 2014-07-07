using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Susanoo
{
    public class CommandResultImplementor<TFilter> : ICommandResultImplementor<TFilter>
    {
        private readonly IDictionary<Type, object> _MappingContainer = new Dictionary<Type, object>();

        public virtual IResultMappingExpression<TFilter, TResult> RetrieveMapping<TResult>() where TResult : new()
        {
            IResultMappingExpression<TFilter, TResult> result = null;

            if (_MappingContainer.ContainsKey(typeof(TResult)))
                result = _MappingContainer[typeof(TResult)] as IResultMappingExpression<TFilter, TResult>;

            return result;
        }

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

        public IDictionary<string, IPropertyMappingConfiguration<System.Data.IDataRecord>> Export<TResultType>() where TResultType : new()
        {
            return this.RetrieveMapping<TResultType>()
                .Export();
        }
    }
}
