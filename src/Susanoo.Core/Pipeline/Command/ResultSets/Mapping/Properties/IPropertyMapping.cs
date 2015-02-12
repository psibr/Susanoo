using System;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace Susanoo.Pipeline.Command.ResultSets.Mapping.Properties
{
    /// <summary>
    /// Allows retrieval of configurations at the property level.
    /// </summary>
    public interface IPropertyMapping : IFluentPipelineFragment
    {
        /// <summary>
        /// Gets the property metadata.
        /// </summary>
        /// <value>The property metadata.</value>
        PropertyInfo PropertyMetadata { get; }

        /// <summary>
        /// Gets or sets the name of the return column.
        /// </summary>
        /// <value>The name of the return.</value>
        string ActiveAlias { get; }

        /// <summary>
        /// Assembles the mapping expression.
        /// </summary>
        /// <param name="propertyExpression">The property.</param>
        /// <returns>Expression&lt;Action&lt;IDataRecord, int&gt;&gt;.</returns>
        Expression<Action<IDataRecord, int>> AssembleMappingExpression(MemberExpression propertyExpression);
    }
}