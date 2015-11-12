#region

using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

#endregion

namespace Susanoo.Mapping.Properties
{
    /// <summary>
    /// Allows configuration of the Susanoo mapper at the property level during CommandBuilder definition.
    /// </summary>
    public class PropertyMappingConfiguration<T> : IPropertyMappingConfiguration, IPropertyMapping
    {
        private Expression<Func<object, T>> _conversionProcessExpression = (value) => DatabaseManager.CastValue<T>(value);
        
        /// <param name="propertyInfo">The property information.</param>
        public PropertyMappingConfiguration(PropertyInfo propertyInfo)
        {
            PropertyMetadata = propertyInfo;
            ActiveAlias = propertyInfo.Name;
        }

        /// <summary>
        /// Gets the conversion process.
        /// </summary>
        /// <value>The conversion process.</value>
        public Func<object, T> ConversionProcess { get; private set; } = (value) => DatabaseManager.CastValue<T>(value);

        /// <summary>
        /// Gets the active alias of the property.
        /// </summary>
        /// <value>The active alias.</value>
        public virtual string ActiveAlias { get; private set; }

        /// <summary>
        /// Assembles the mapping expression.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>Expression&lt;Action&lt;IDataRecord&gt;&gt;.</returns>
        public virtual Expression<Action<IDataRecord, int>> AssembleMappingExpression(MemberExpression property)
        {
            var record = Expression.Parameter(typeof (IDataRecord), "record");
            var ordinal = Expression.Parameter(typeof (int), "ordinal");

            Expression body = AssembleAssignment(property, record, ordinal);

            var assignmentExpression =
                Expression.Lambda<Action<IDataRecord, int>>(body, record, ordinal);

            return assignmentExpression;
        }

        /// <summary>
        /// Gets the <c>PropertyInfo</c> that describes the property.
        /// </summary>
        /// <value>The property reflection meta data.</value>
        public virtual PropertyInfo PropertyMetadata { get; }

        /// <summary>
        /// Uses the specified alias when mapping from the data call.
        /// </summary>
        /// <param name="columnNameAlias">The alias.</param>
        /// <returns>Susanoo.IResultMappingExpression&lt;TFilter,TResult&gt;.</returns>
        public virtual IPropertyMappingConfiguration UseAlias(string columnNameAlias)
        {
            ActiveAlias = columnNameAlias;

            return this;
        }

        /// <summary>
        /// Processes the value in some form before assignment.
        /// </summary>
        /// <param name="process">The process.</param>
        /// <returns>IPropertyMappingConfiguration&lt;TRecord&gt;.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public virtual IPropertyMappingConfiguration ProcessValueUsing(Func<object, T> process)
        {
            ConversionProcess = process;
            _conversionProcessExpression = (value) => process(value);

            return this;
        }

        /// <summary>
        /// Assembles the assignment expression.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="record">The record parameter.</param>
        /// <param name="ordinal">The ordinal parameter.</param>
        /// <returns>BinaryExpression.</returns>
        protected virtual BinaryExpression AssembleAssignment(MemberExpression propertyExpression, ParameterExpression record,
            ParameterExpression ordinal)
        {
            var iType = typeof(IDataRecord).GetProperty("Item", new[] { typeof(int) });
            var arrOrd = new[] { ordinal };
            var eIndex = Expression.MakeIndex(record, iType, arrOrd);
            var eInvoke = Expression.Invoke(_conversionProcessExpression, eIndex);
            var convert = Expression.Convert(eInvoke, propertyExpression.Type);
            return Expression.Assign(propertyExpression, convert);
        }
    }
}