using Susanoo.Pipeline.Command.ResultSets.Mapping.Properties;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Susanoo.Pipeline.Command.ResultSets.Mapping
{
    /// <summary>
    /// A step in the command definition Fluent API, in which properties are mapped to potential result data.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public interface IResultMappingImplementor<TResult> : IResultMappingExport, IFluentPipelineFragment
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