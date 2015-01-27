#region

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Susanoo.Pipeline.Command.ResultSets.Mapping.Properties;

#endregion

namespace Susanoo.Pipeline.Command.ResultSets.Mapping
{
    /// <summary>
    /// A step in the command definition Fluent API, in which properties are mapped to potential result data.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public interface IResultMappingExpression<TFilter, TResult> : IResultMappingExport, IFluentPipelineFragment
        where TResult : new()
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


    }

    /// <summary>
    /// Exposes property mapping export capabilities.
    /// </summary>
    public interface IResultMappingExport : IFluentPipelineFragment
    {
        /// <summary>
        /// Exports this instance.
        /// </summary>
        /// <returns>IDictionary&lt;System.String, Action&lt;IPropertyMappingConfiguration&lt;IDataRecord&gt;&gt;&gt;.</returns>
        IDictionary<string, IPropertyMapping> Export();
    }

    /// <summary>
    /// A step in the command definition Fluent API, in which properties are mapped to potential result data.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public interface IResultMappingImplementor<TResult> : IResultMappingExport, IFluentPipelineFragment
        where TResult : new()
    {
        /// <summary>
        /// Clears the result mappings.
        /// </summary>
        void ClearMappings();

        /// <summary>
        /// Mapping options for a property in the result model.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="options">The options.</param>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        void ForProperty(Expression<Func<TResult, object>> propertyExpression,
            Action<IPropertyMappingConfiguration> options);

        /// <summary>
        /// Mapping options for a property in the result model.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="options">The options.</param>
        void ForProperty(string propertyName, Action<IPropertyMappingConfiguration> options);



        /// <summary>
        /// Maps the declarative properties.
        /// </summary>
        void MapDeclarativeProperties();
    }
}