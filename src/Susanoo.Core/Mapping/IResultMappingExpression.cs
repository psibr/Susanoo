#region

using Susanoo.Mapping.Properties;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

#endregion

namespace Susanoo.Mapping
{
    /// <summary>
    /// A step in the CommandBuilder definition Fluent API, in which properties are mapped to potential result data.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public interface IResultMappingExpression<TFilter, TResult> : IMappingExport
    {
        /// <summary>
        /// Clears the result mappings.
        /// </summary>
        /// <returns>IResultMappingExpression&lt;TFilter, TResult&gt;.</returns>
        IResultMappingExpression<TFilter, TResult> ClearMappings();

        /// <summary>
        /// Mapping options for a property in the result model.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="options">The options.</param>
        /// <returns>IResultMappingExpression&lt;TFilter, TResult&gt;.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        IResultMappingExpression<TFilter, TResult> ForProperty(Expression<Func<TResult, object>> propertyExpression,
            Action<IPropertyMappingConfiguration> options);

        /// <summary>
        /// Mapping options for a property in the result model.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="options">The options.</param>
        /// <returns>IResultMappingExpression&lt;TFilter, TResult&gt;.</returns>
        IResultMappingExpression<TFilter, TResult> ForProperty(string propertyName,
            Action<IPropertyMappingConfiguration> options);

        /// <summary>
        /// Associates a result object property to a column alias.
        /// </summary>
        /// <param name="propertyExpression">Result object property selector.</param>
        /// <param name="columnName">Column alias in the resultset.</param>
        /// <returns>IResultMappingExpression&lt;TFilter, TResult&gt;.</returns>
        IResultMappingExpression<TFilter, TResult> MapPropertyToColumn(
            Expression<Func<TResult, object>> propertyExpression, string columnName);

        /// <summary>
        /// Associates a result object property to a column alias.
        /// </summary>
        /// <param name="propertyName">Result object property name.</param>
        /// <param name="columnName">Column alias in the resultset.</param>
        /// <returns>IResultMappingExpression&lt;TFilter, TResult&gt;.</returns>
        IResultMappingExpression<TFilter, TResult> MapPropertyToColumn(
            string propertyName, string columnName);

    }
}