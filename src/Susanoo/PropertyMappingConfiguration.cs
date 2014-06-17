using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Susanoo
{
    public class PropertyMappingConfiguration<TRecord>
        : IPropertyMappingConfiguration<TRecord>
        where TRecord : IDataRecord
    {
        public PropertyMappingConfiguration(PropertyInfo propertyInfo)
        {
            this.PropertyMetadata = propertyInfo;
            this.ReturnName = propertyInfo.Name;
        }

        public PropertyInfo PropertyMetadata { get; private set; }

        private Expression<Func<TRecord, string, bool>> MapOnCondition = (record, name) => HasColumn(record, name);

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

        private Expression<Func<Type, object, object, object>> conversionProcess = (type, value, defaultValue) => DatabaseManager.CastValue(type, value, defaultValue);
        public string ReturnName { get; private set; }

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
        public IPropertyMappingConfiguration<TRecord> AliasProperty(string propertyAlias)
        {
            this.ReturnName = propertyAlias;

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
                                    Expression.Constant(this.ReturnName))),
                            Expression.Assign(
                                property,
                                Expression.Convert(
                                    Expression.Invoke(this.conversionProcess,
                                        Expression.Constant(this.PropertyMetadata.PropertyType, typeof(Type)),
                                        Expression.MakeIndex(recordParam, typeof(IDataRecord).GetProperty("Item", new[] { typeof(string) }),
                                                    new[]
                                                    {
                                                        Expression.Constant(this.ReturnName)
                                                    }),
                                        Expression.Constant(null)),
                                    property.Type)))),
                recordParam);

            return assignmentExpression;
        }
    }
}
