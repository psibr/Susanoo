using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Susanoo.Mapping.Properties
{
    /// <summary>
    /// Base for mapping to handle common functionality
    /// </summary>
    public abstract class ResultMappingBase
    {
        /// <summary>
        /// Map of actions for conversions
        /// </summary>
        protected IDictionary<string, IPropertyMapping> _mappingActions = new Dictionary<string, IPropertyMapping>();

        /// <summary>
        /// Attempts to add a configuration to the mapped actions
        /// </summary>
        protected void TryAddMapping(KeyValuePair<System.Reflection.PropertyInfo, PropertyMapping> item)
        {
            if (TryAddMapping<Guid>(item)) return;
            if (TryAddMapping<TimeSpan>(item)) return;
            if (TryAddMapping<DateTime?>(item)) return;
            if (TryAddMapping<DateTime>(item)) return;
            if (TryAddMapping<sbyte?>(item)) return;
            if (TryAddMapping<sbyte>(item)) return;
            if (TryAddMapping<char?>(item)) return;
            if (TryAddMapping<char>(item)) return;
            if (TryAddMapping<byte?>(item)) return;
            if (TryAddMapping<byte>(item)) return;
            if (TryAddMapping<ushort?>(item)) return;
            if (TryAddMapping<ushort>(item)) return;
            if (TryAddMapping<short?>(item)) return;
            if (TryAddMapping<short>(item)) return;
            if (TryAddMapping<uint?>(item)) return;
            if (TryAddMapping<uint>(item)) return;
            if (TryAddMapping<int?>(item)) return;
            if (TryAddMapping<int>(item)) return;
            if (TryAddMapping<ulong?>(item)) return;
            if (TryAddMapping<ulong>(item)) return;
            if (TryAddMapping<long?>(item)) return;
            if (TryAddMapping<long>(item)) return;
            if (TryAddMapping<double?>(item)) return;
            if (TryAddMapping<double>(item)) return;
            if (TryAddMapping<decimal?>(item)) return;
            if (TryAddMapping<decimal>(item)) return;
            if (TryAddMapping<float?>(item)) return;
            if (TryAddMapping<float>(item)) return;
            if (TryAddMapping<bool?>(item)) return;
            if (TryAddMapping<bool>(item)) return;
            if (TryAddMapping<string>(item)) return;
            if (TryAddMapping<object>(item)) return;
        }
        
        private bool TryAddMapping<T>(KeyValuePair<System.Reflection.PropertyInfo, PropertyMapping> item)
        {
            if (CanConvert(typeof(T), item.Key.PropertyType))
            {
                AddMappingActions<T>(item);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Determines if a type can be converted to another type.
        /// </summary>
        private static bool CanConvert(Type from, Type to)
        {
            if (from == to) return true;
            if (from.Namespace.Equals("system", StringComparison.OrdinalIgnoreCase) && to.Namespace.Equals("system", StringComparison.OrdinalIgnoreCase)) return false;
            Func<Expression, UnaryExpression> bodyFunction = body => Expression.Convert(body, to);
            ParameterExpression inp = Expression.Parameter(from, "inp");
            try
            {
                Expression.Lambda(bodyFunction(inp), inp).Compile();
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        /// <summary>
        /// After determining the generic type, adds a configuration of that type to the mapping actions
        /// </summary>
        private void AddMappingActions<T>(KeyValuePair<System.Reflection.PropertyInfo, PropertyMapping> item)
        {
            var configuration = new PropertyMappingConfiguration<T>(item.Key);
            configuration.UseAlias(item.Value.ActiveAlias);
            _mappingActions.Add(item.Key.Name, configuration);
        }

    }
}
