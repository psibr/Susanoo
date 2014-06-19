using System;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace Susanoo
{
    /// <summary>
    /// Allows configuration of the Susanoo mapper at the property level during command definition.
    /// </summary>
    /// <typeparam name="TRecord">The type of the record.</typeparam>
    public class PropertyMappingConfiguration<TRecord>
        : IPropertyMappingConfiguration<TRecord>
        where TRecord : IDataRecord
    {
        private Expression<Func<TRecord, string, bool>> MapOnCondition = (record, name) => HasColumn(record, name);

        private Expression<Func<Type, object, object, object>> conversionProcess = (type, value, defaultValue) => DatabaseManager.CastValue(type, value, defaultValue);

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMappingConfiguration{TRecord}"/> class.
        /// </summary>
        /// <param name="propertyInfo">The property reflection meta data.</param>
        public PropertyMappingConfiguration(PropertyInfo propertyInfo)
        {
            this.PropertyMetadata = propertyInfo;
            this.ActiveAlias = propertyInfo.Name;
        }

        /// <summary>
        /// Gets the <c>PropertyInfo</c> that describes the property.
        /// </summary>
        /// <value>The property reflection meta data.</value>
        public PropertyInfo PropertyMetadata { get; private set; }

        /// <summary>
        /// Gets the active alias of the property.
        /// </summary>
        /// <value>The active alias.</value>
        public string ActiveAlias { get; private set; }

        /// <summary>
        /// Determines whether the specified record has a matching column.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="name">The name of the column.</param>
        /// <returns><c>true</c> if the specified record has a matching column; otherwise, <c>false</c>.</returns>
        public static bool HasColumn(TRecord record, string name)
        {
            bool map = false;

            for (int i = 0; i < record.FieldCount; i++)
            {
                if (record.GetName(i) == name)
                {
                    map = true;
                    break;
                }
            }

            return map;
        }

        /// <summary>
        /// Maps the property conditionally.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <returns>IPropertyMappingConfiguration&lt;TRecord&gt;.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public IPropertyMappingConfiguration<TRecord> MapIf(Expression<Func<TRecord, string, bool>> condition)
        {
            this.MapOnCondition = condition;

            return this;
        }

        /// <summary>
        /// Uses the specified alias when mapping from the data call.
        /// </summary>
        /// <param name="alias">The alias.</param>
        /// <returns>Susanoo.ICommandResultMappingExpression&lt;TFilter,TResult&gt;.</returns>
        public IPropertyMappingConfiguration<TRecord> AliasProperty(string alias)
        {
            this.ActiveAlias = alias;

            return this;
        }

        /// <summary>
        /// Processes the value in some form before assignment.
        /// </summary>
        /// <param name="process"></param>
        /// <returns>IPropertyMappingConfiguration&lt;TRecord&gt;.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public IPropertyMappingConfiguration<TRecord> ProcessValue(Expression<Func<Type, object, object, object>> process)
        {
            this.conversionProcess = process;

            return this;
        }

        /// <summary>
        /// Assembles the mapping expression.
        /// </summary>
        /// <returns>Expression&lt;Action&lt;IDataRecord&gt;&gt;.</returns>
        public Expression<Action<IDataRecord>> AssembleMappingExpression(MemberExpression property)
        {
            ParameterExpression recordParam = Expression.Parameter(typeof(TRecord), "record");

            var assignmentExpression =
                Expression.Lambda<Action<IDataRecord>>(
                    Expression.Block(
                        Expression.IfThen(
                            Expression.IsTrue(
                                Expression.Invoke(
                                    this.MapOnCondition,
                                    recordParam,
                                    Expression.Constant(this.ActiveAlias))),
                            Expression.Assign(
                                property,
                                Expression.Convert(
                                    Expression.Invoke(this.conversionProcess,
                                        Expression.Constant(this.PropertyMetadata.PropertyType, typeof(Type)),
                                        Expression.MakeIndex(recordParam, typeof(IDataRecord).GetProperty("Item", new[] { typeof(string) }),
                                                    new[]
                                                    {
                                                        Expression.Constant(this.ActiveAlias)
                                                    }),
                                        Expression.Constant(null)),
                                    property.Type)))),
                recordParam);

            return assignmentExpression;
        }
    }
}