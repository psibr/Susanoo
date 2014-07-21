using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace Susanoo
{
    /// <summary>
    /// Helpful Expression extension methods
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Given an expression, extract the listed property name; similar to reflection but with familiar LINQ+lambdas.
        /// </summary>
        /// <typeparam name="TModel">the model type to extract property names</typeparam>
        /// <typeparam name="TValue">the value type of the expected property</typeparam>
        /// <param name="propertySelector">expression that just selects a model property to be turned into a string</param>
        /// <returns>indicated property name</returns>
        public static string GetPropertyName<TModel, TValue>(this Expression<Func<TModel, TValue>> propertySelector)
        {
            string name = null;
            if (propertySelector.Body is UnaryExpression)
                name = ((MemberExpression)(propertySelector.Body as UnaryExpression).Operand).Member.Name;
            else
                name = ((MemberExpression)propertySelector.Body).Member.Name;

            return name;
        }

        public static IEnumerable<TResult> Execute<TFilter, TResult>(this IDatabaseManager databaseManager,
            ICommandProcessor<TFilter, TResult> command, TFilter filter = default(TFilter), params IDbDataParameter[] explicitParameters)
            where TResult : new()
        {
            return command.Execute(databaseManager, filter, explicitParameters);
        }
    }
}