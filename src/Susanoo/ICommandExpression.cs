using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace Susanoo
{
    public interface ICommandExpression<TFilter, TResult>
        where TResult : new()
    {
        /// <summary>
        /// Gets the database manager.
        /// </summary>
        /// <value>The database manager.</value>
        IDatabaseManager DatabaseManager { get; }

        /// <summary>
        /// Gets the command text.
        /// </summary>
        /// <value>The command text.</value>
        string CommandText { get; }

        /// <summary>
        /// Gets the type of the database command.
        /// </summary>
        /// <value>The type of the database command.</value>
        CommandType DbCommandType { get; }

        /// <summary>
        /// Builds the parameters.
        /// </summary>
        /// <returns>IEnumerable&lt;IDbDataParameter&gt;.</returns>
        IEnumerable<IDbDataParameter> BuildParameters(TFilter filter, params IDbDataParameter[] explicitParameters);

        /// <summary>
        /// Includes the property.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <returns>ICommandExpression&lt;T&gt;.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        ICommandExpression<TFilter, TResult> IncludeProperty(Expression<Func<TFilter, object>> propertyExpression);

        /// <summary>
        /// Includes the property.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="parameterOptions">The parameter options.</param>
        /// <returns>ICommandExpression&lt;T&gt;.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        ICommandExpression<TFilter, TResult> IncludeProperty(Expression<Func<TFilter, object>> propertyExpression, Action<IDbDataParameter> parameterOptions);

        /// <summary>
        /// Includes the property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>ICommandExpression&lt;T&gt;.</returns>
        ICommandExpression<TFilter, TResult> IncludeProperty(string propertyName);

        /// <summary>
        /// Includes the property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="parameterOptions">The parameter options.</param>
        /// <returns>ICommandExpression&lt;T&gt;.</returns>
        ICommandExpression<TFilter, TResult> IncludeProperty(string propertyName, Action<IDbDataParameter> parameterOptions);

        /// <summary>
        /// Adds the parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>ICommandExpression&lt;T&gt;.</returns>
        ICommandExpression<TFilter, TResult> AddParameters(params IDbDataParameter[] parameters);

        /// <summary>
        /// Excludes the property.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <returns>ICommandExpression&lt;T&gt;.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        ICommandExpression<TFilter, TResult> ExcludeProperty(Expression<Func<TFilter, object>> propertyExpression);

        /// <summary>
        /// Excludes the property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>ICommandExpression&lt;TFilter, TResult&gt;.</returns>
        ICommandExpression<TFilter, TResult> ExcludeProperty(string propertyName);

        /// <summary>
        /// Defines the result mappings.
        /// </summary>
        /// <returns>ICommandResultMappingExpression&lt;TFilter, TResult&gt;.</returns>
        ICommandResultMappingExpression<TFilter, TResult> DefineResultMappings();
    }

    public interface ICommandExpression<TResult>
        where TResult : new()
    {
        /// <summary>
        /// Adds the parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>ICommandExpression.</returns>
        ICommandExpression<TResult> AddParameters(params IDbDataParameter[] parameters);

        /// <summary>
        /// Builds the parameters.
        /// </summary>
        /// <returns>IEnumerable&lt;IDbDataParameter&gt;.</returns>
        IEnumerable<IDbDataParameter> BuildParameters(params IDbDataParameter[] explicitParameters);

        /// <summary>
        /// Defines the result mappings.
        /// </summary>
        /// <returns>ICommandResultMappingExpression&lt;TResult&gt;.</returns>
        ICommandResultMappingExpression<TResult> DefineResultMappings();
    }
}