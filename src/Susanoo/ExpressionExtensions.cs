using System;
using System.Linq.Expressions;

namespace Susanoo
{
    /// <summary>
    /// Helpful Expression extension methods
    /// </summary>
    internal static class ExpressionExtensions
    {
        /// <summary>
        /// Given an expression, extract the listed property name; similar to reflection but with familiar LINQ+lambdas.
        /// </summary>
        /// <remarks>Cheats and uses the ToString output -- Should consult performance differences</remarks>
        /// <typeparam name="TModel">the model type to extract property names</typeparam>
        /// <typeparam name="TValue">the value type of the expected property</typeparam>
        /// <param name="propertySelector">expression that just selects a model property to be turned into a string</param>
        /// <returns>indicated property name</returns>
        public static string GetPropertyName<TModel, TValue>(this Expression<Func<TModel, TValue>> propertySelector)
        {
            return ((MemberExpression)propertySelector.Body).Member.Name;
        }
    }
}