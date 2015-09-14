#region

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Numerics;
using Susanoo.Mapping.Properties;

#endregion

namespace Susanoo.Mapping
{
    /// <summary>
    /// A step in the CommandBuilder definition Fluent API, in which properties are mapped to potential result data.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public class ResultMappingExpression<TFilter, TResult>
        : IResultMappingExpression<TFilter, TResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResultMappingExpression{TFilter, TResult}" /> class.
        /// </summary>
        public ResultMappingExpression()
        {
            Implementor = new ResultMappingImplementor<TResult>();
        }

        /// <summary>
        /// Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public virtual BigInteger CacheHash =>
            Implementor.CacheHash;

        /// <summary>
        /// Gets the implementor this is the Bridge design pattern.
        /// </summary>
        /// <value>The implementor.</value>
        protected IResultMappingImplementor<TResult> Implementor { get; }

        /// <summary>
        /// Clears the result mappings.
        /// </summary>
        /// <returns>IResultMappingExpression&lt;TFilter, TResult&gt;.</returns>
        public virtual IResultMappingExpression<TFilter, TResult> ClearMappings()
        {
            Implementor.ClearMappings();

            return this;
        }

        /// <summary>
        /// Mapping options for a property in the result model.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="options">The options.</param>
        /// <returns>IResultMappingExpression&lt;TFilter, TResult&gt;.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public virtual IResultMappingExpression<TFilter, TResult> ForProperty(
            Expression<Func<TResult, object>> propertyExpression,
            Action<IPropertyMappingConfiguration> options)
        {
            return ForProperty(propertyExpression.GetPropertyName(), options);
        }

        /// <summary>
        /// Mapping options for a property in the result model.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="options">The options.</param>
        /// <returns>IResultMappingExpression&lt;TFilter, TResult&gt;.</returns>
        public virtual IResultMappingExpression<TFilter, TResult> ForProperty(
            string propertyName,
            Action<IPropertyMappingConfiguration> options)
        {
            Implementor.ForProperty(propertyName, options);

            return this;
        }

        /// <summary>
        /// Exports this instance.
        /// </summary>
        /// <returns>IDictionary&lt;System.String, Action&lt;IPropertyMappingConfiguration&lt;IDataRecord&gt;&gt;&gt;.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual IDictionary<string, IPropertyMapping> Export()
        {
            return Implementor.Export();
        }
    }
}