using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Susanoo
{
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
