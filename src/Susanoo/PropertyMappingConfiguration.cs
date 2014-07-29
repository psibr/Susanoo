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
    public class PropertyMappingConfiguration
        : IPropertyMappingConfiguration, IPropertyMapping, IFluentPipelineFragment
    {
        private Expression<Func<IDataRecord, string, bool>> MapOnCondition = null;

        private Expression<Func<PropertyInfo, object, object>> conversionProcess = (property, value) => DatabaseManager.CastValue(property.PropertyType, value, property.PropertyType);


        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMappingConfiguration"/> class.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
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

        /// <summary>
        /// Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public virtual BigInteger CacheHash
        {
            get
            {
                return FnvHash.GetHash(this.PropertyMetadata.Name + this.ActiveAlias + conversionProcess.ToString(), 64);
            }
        }

        public virtual IPropertyMappingConfiguration MakeNavigationalProperty<TFilter, TParent>(
            params Func<TFilter, TParent, KeyValuePair<string, object>>[] parameterBuilder)
        {
            return this;
        }

        public virtual IPropertyMappingConfiguration MakeNavigationalProperty<TFilter, TParent>(
            params Func<TFilter, IEnumerable<TParent>, KeyValuePair<string, object>>[] parameterBuilder)
        {
            return this;
        }

        public virtual IPropertyMappingConfiguration MakeNavigationalProperty<TFilter, TParent>(
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
        public virtual IPropertyMappingConfiguration UseAlias(string alias)
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
        public virtual IPropertyMappingConfiguration ProcessValueUsing(Expression<Func<PropertyInfo, object, object>> process)
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
            ParameterExpression recordParam = Expression.Parameter(typeof(IDataRecord), "record");

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
                            Expression.Constant(this.PropertyMetadata, typeof(PropertyInfo)),
                            Expression.MakeIndex(recordParam, typeof(IDataRecord).GetProperty("Item", new[] { typeof(string) }),
                                        new[]
                                        {
                                            Expression.Constant(this.ActiveAlias)
                                        }),
                            Expression.Constant(null)),
                        property.Type));
        }
    }
}