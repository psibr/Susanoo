﻿#region

using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;

#endregion

namespace Susanoo
{
    /// <summary>
    ///     Allows configuration of the Susanoo mapper at the property level during command definition.
    /// </summary>
    public class PropertyMappingConfiguration
        : IPropertyMappingConfiguration, IPropertyMapping
    {
        private readonly Expression<Func<IDataRecord, string, bool>> _mapOnCondition = null;

        private Func<Type, object, object> _conversionProcess =
            (type, value) => DatabaseManager.CastValue(type, value, type, null);

        private Expression<Func<Type, object, object>> _conversionProcessExpression =
            (type, value) => DatabaseManager.CastValue(type, value, type, null);

        /// <summary>
        ///     Initializes a new instance of the <see cref="PropertyMappingConfiguration" /> class.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        public PropertyMappingConfiguration(PropertyInfo propertyInfo)
        {
            PropertyMetadata = propertyInfo;
            ActiveAlias = propertyInfo.Name;
        }

        /// <summary>
        ///     Gets the conversion process.
        /// </summary>
        /// <value>The conversion process.</value>
        public Func<Type, object, object> ConversionProcess
        {
            get { return _conversionProcess; }
        }

        /// <summary>
        ///     Gets the active alias of the property.
        /// </summary>
        /// <value>The active alias.</value>
        public virtual string ActiveAlias { get; private set; }

        /// <summary>
        ///     Assembles the mapping expression.
        /// </summary>
        /// <returns>Expression&lt;Action&lt;IDataRecord&gt;&gt;.</returns>
        public virtual Expression<Action<IDataRecord>> AssembleMappingExpression(MemberExpression property)
        {
            ParameterExpression recordParam = Expression.Parameter(typeof(IDataRecord), "record");

            Expression body = (_mapOnCondition != null)
                ? HasMapCondition(property, recordParam)
                : AssembleAssignment(property, recordParam);

            var assignmentExpression =
                Expression.Lambda<Action<IDataRecord>>(body, recordParam);

            return assignmentExpression;
        }

        /// <summary>
        ///     Gets the <c>PropertyInfo</c> that describes the property.
        /// </summary>
        /// <value>The property reflection meta data.</value>
        public virtual PropertyInfo PropertyMetadata { get; private set; }

        /// <summary>
        ///     Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public virtual BigInteger CacheHash
        {
            get { return FnvHash.GetHash(PropertyMetadata.Name + ActiveAlias + _conversionProcess, 64); }
        }

        /// <summary>
        ///     Uses the specified alias when mapping from the data call.
        /// </summary>
        /// <param name="alias">The alias.</param>
        /// <returns>Susanoo.IResultMappingExpression&lt;TFilter,TResult&gt;.</returns>
        public virtual IPropertyMappingConfiguration UseAlias(string alias)
        {
            ActiveAlias = alias;

            return this;
        }

        /// <summary>
        ///     Processes the value in some form before assignment.
        /// </summary>
        /// <param name="process"></param>
        /// <returns>IPropertyMappingConfiguration&lt;TRecord&gt;.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public virtual IPropertyMappingConfiguration ProcessValueUsing(Func<Type, object, object> process)
        {
            _conversionProcess = process;
            _conversionProcessExpression = (type, value) => process(type, value);

            return this;
        }

        /// <summary>
        ///     Determines whether the specified property has a mapping condition.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="recordParam">The record parameter.</param>
        protected virtual Expression HasMapCondition(MemberExpression property, ParameterExpression recordParam)
        {
            return Expression.Block(
                Expression.IfThen(
                    Expression.IsTrue(
                        Expression.Invoke(
                            _mapOnCondition,
                            recordParam,
                            Expression.Constant(ActiveAlias))),
                    AssembleAssignment(property, recordParam)));
        }

        /// <summary>
        ///     Assembles the assignment expression.
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
                        Expression.Invoke(_conversionProcessExpression,
                            Expression.Constant(PropertyMetadata.PropertyType, typeof(Type)),
                            Expression.MakeIndex(recordParam,
                                typeof(IDataRecord).GetProperty("Item", new[] { typeof(string) }),
                                new[]
                                {
                                    Expression.Constant(ActiveAlias)
                                })),
                        property.Type));
        }
    }
}