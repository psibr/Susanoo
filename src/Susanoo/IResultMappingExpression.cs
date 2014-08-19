﻿#region

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

#endregion

namespace Susanoo
{
    /// <summary>
    ///     A step in the command definition Fluent API, in which properties are mapped to potential result data.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public interface IResultMappingExpression<TFilter, TResult> : IFluentPipelineFragment
        where TResult : new()
    {
        /// <summary>
        ///     Clears the result mappings.
        /// </summary>
        /// <returns>IResultMappingExpression&lt;TFilter, TResult&gt;.</returns>
        IResultMappingExpression<TFilter, TResult> ClearMappings();

        /// <summary>
        ///     Mapping options for a property in the result model.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="options">The options.</param>
        /// <returns>IResultMappingExpression&lt;TFilter, TResult&gt;.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        IResultMappingExpression<TFilter, TResult> ForProperty(Expression<Func<TResult, object>> propertyExpression,
            Action<IPropertyMappingConfiguration> options);

        /// <summary>
        ///     Mapping options for a property in the result model.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="options">The options.</param>
        /// <returns>IResultMappingExpression&lt;TFilter, TResult&gt;.</returns>
        IResultMappingExpression<TFilter, TResult> ForProperty(string propertyName,
            Action<IPropertyMappingConfiguration> options);

        /// <summary>
        ///     Exports this instance.
        /// </summary>
        /// <returns>IDictionary&lt;System.String, Action&lt;IPropertyMappingConfiguration&lt;IDataRecord&gt;&gt;&gt;.</returns>
        IDictionary<string, IPropertyMapping> Export();
    }

    /// <summary>
    ///     A step in the command definition Fluent API, in which properties are mapped to potential result data.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public interface IResultMappingImplementor<TResult>
        where TResult : new()
    {
        /// <summary>
        ///     Clears the result mappings.
        /// </summary>
        void ClearMappings();

        /// <summary>
        ///     Mapping options for a property in the result model.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="options">The options.</param>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        void ForProperty(Expression<Func<TResult, object>> propertyExpression,
            Action<IPropertyMappingConfiguration> options);

        /// <summary>
        ///     Mapping options for a property in the result model.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="options">The options.</param>
        void ForProperty(string propertyName, Action<IPropertyMappingConfiguration> options);

        /// <summary>
        ///     Exports this instance.
        /// </summary>
        /// <returns>IDictionary&lt;System.String, Action&lt;IPropertyMappingConfiguration&lt;IDataRecord&gt;&gt;&gt;.</returns>
        IDictionary<string, IPropertyMapping> Export();

        /// <summary>
        ///     Maps the declarative properties.
        /// </summary>
        void MapDeclarativeProperties();
    }
}