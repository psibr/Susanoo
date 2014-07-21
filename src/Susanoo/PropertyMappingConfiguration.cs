using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;

namespace Susanoo
{
    /// <summary>
    /// Allows configuration of the Susanoo mapper at the property level during command definition.
    /// </summary>
    /// <typeparam name="TRecord">The type of the record.</typeparam>
    public class PropertyMappingConfiguration<TRecord>
        : IPropertyMappingConfiguration<TRecord>, IFluentPipelineFragment
        where TRecord : IDataRecord
    {
        private Expression<Func<TRecord, string, bool>> MapOnCondition = null;

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
        public virtual PropertyInfo PropertyMetadata { get; private set; }

        /// <summary>
        /// Gets the active alias of the property.
        /// </summary>
        /// <value>The active alias.</value>
        public virtual string ActiveAlias { get; private set; }

        public virtual IPropertyMappingConfiguration<TRecord> MakeNavigationalProperty<TFilter, TParent>(
            params Func<TFilter, TParent, KeyValuePair<string, object>>[] parameterBuilder)
        {
            return this;
        }

        public virtual IPropertyMappingConfiguration<TRecord> MakeNavigationalProperty<TFilter, TParent>(
            params Func<TFilter, IEnumerable<TParent>, KeyValuePair<string, object>>[] parameterBuilder)
        {
            return this;
        }

        public virtual IPropertyMappingConfiguration<TRecord> MakeNavigationalProperty<TFilter, TParent>(
            string KeyName,
            Action<TParent> foreignKeySelector)
        {
            return this;
        }

        /// <summary>
        /// Uses the specified alias when mapping from the data call.
        /// </summary>
        /// <param name="alias">The alias.</param>
        /// <returns>Susanoo.IResultMappingExpression&lt;TFilter,TResult&gt;.</returns>
        public virtual IPropertyMappingConfiguration<TRecord> AliasProperty(string alias)
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
        public virtual IPropertyMappingConfiguration<TRecord> ProcessValue(Expression<Func<Type, object, object, object>> process)
        {
            this.conversionProcess = process;

            return this;
        }

        /// <summary>
        /// Assembles the mapping expression.
        /// </summary>
        /// <returns>Expression&lt;Action&lt;IDataRecord&gt;&gt;.</returns>
        public virtual Expression<Action<IDataRecord>> AssembleMappingExpression(MemberExpression property)
        {
            ParameterExpression recordParam = Expression.Parameter(typeof(TRecord), "record");

            Expression body = (this.MapOnCondition != null)
                ? HasMapCondition(property, recordParam)
                : AssembleAssignment(property, recordParam);

            var assignmentExpression =
                Expression.Lambda<Action<IDataRecord>>(body, recordParam);

            return assignmentExpression;
        }

        /// <summary>
        /// Determines whether the specified property has a mapping condition.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="recordParam">The record parameter.</param>
        protected virtual Expression HasMapCondition(MemberExpression property, ParameterExpression recordParam)
        {
            return Expression.Block(
                Expression.IfThen(
                    Expression.IsTrue(
                        Expression.Invoke(
                            this.MapOnCondition,
                            recordParam,
                            Expression.Constant(this.ActiveAlias))),
                            AssembleAssignment(property, recordParam)));
        }

        /// <summary>
        /// Assembles the assignment expression.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="recordParam">The record parameter.</param>
        /// <returns>BinaryExpression.</returns>
        protected virtual BinaryExpression AssembleAssignment(MemberExpression property, ParameterExpression recordParam)
        {
            return
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
                        property.Type));
        }

        public virtual BigInteger CacheHash
        {
            get
            {
                return FnvHash.GetHash(this.PropertyMetadata.Name + this.ActiveAlias + conversionProcess.ToString(), 64);
            }
        }
    }
}