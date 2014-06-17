using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace Susanoo
{
    public interface ICommandResultMappingExpression<TFilter, TResult>
        where TResult : new()
    {
        /// <summary>
        /// Clears the result mappings.
        /// </summary>
        /// <returns>ICommandResultMappingExpression&lt;TFilter, TResult&gt;.</returns>
        ICommandResultMappingExpression<TFilter, TResult> ClearMappings();

        /// <summary>
        /// Mapping options for a property in the result model.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="options">The options.</param>
        /// <returns>ICommandResultMappingExpression&lt;TFilter, TResult&gt;.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        ICommandResultMappingExpression<TFilter, TResult> ForProperty(Expression<Func<TResult, object>> propertyExpression, Action<IPropertyMappingConfiguration<IDataRecord>> options);

        /// <summary>
        /// Mapping options for a property in the result model.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="options">The options.</param>
        /// <returns>ICommandResultMappingExpression&lt;TFilter, TResult&gt;.</returns>
        ICommandResultMappingExpression<TFilter, TResult> ForProperty(string propertyName, Action<IPropertyMappingConfiguration<IDataRecord>> options);

        /// <summary>
        /// Prepares the command for caching and executing.
        /// </summary>
        /// <returns>ICommandProcessor&lt;TFilter, TResult&gt;.</returns>
        ICommandProcessor<TFilter, TResult> PrepareCommand();

        /// <summary>
        /// Gets the command expression.
        /// </summary>
        /// <value>The command expression.</value>
        ICommandExpression<TFilter, TResult> CommandExpression { get; }

        /// <summary>
        /// Exports this instance.
        /// </summary>
        /// <returns>IDictionary&lt;System.String, Action&lt;IPropertyMappingConfiguration&lt;IDataRecord&gt;&gt;&gt;.</returns>
        IDictionary<string, IPropertyMappingConfiguration<IDataRecord>> Export();
    }

    public interface ICommandResultMappingExpression<TResult>
        where TResult : new()
    {
        /// <summary>
        /// Clears the result mappings.
        /// </summary>
        /// <returns>ICommandResultMappingExpression&lt;TResult&gt;.</returns>
        ICommandResultMappingExpression<TResult> ClearMappings();

        /// <summary>
        /// Mapping options for a property in the result model.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="options">The options.</param>
        /// <returns>ICommandResultMappingExpression&lt;TResult&gt;.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        ICommandResultMappingExpression<TResult> ForProperty(Expression<Func<TResult, object>> propertyExpression, Action<IPropertyMappingConfiguration<IDataRecord>> options);

        /// <summary>
        /// Prepares the command for caching and executing.
        /// </summary>
        /// <returns>ICommandProcessor&lt;TResult&gt;.</returns>
        ICommandProcessor<TResult> PrepareCommand();

        /// <summary>
        /// Gets the command expression.
        /// </summary>
        /// <value>The command expression.</value>
        ICommandExpression<TResult> CommandExpression { get; }

        /// <summary>
        /// Exports this instance.
        /// </summary>
        /// <returns>IDictionary&lt;System.String, Action&lt;IPropertyMappingConfiguration&lt;IDataRecord&gt;&gt;&gt;.</returns>
        IDictionary<string, IPropertyMappingConfiguration<IDataRecord>> Export();
    }
}