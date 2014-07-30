using System;
using System.Collections.Generic;
using System.Data.Common;
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
        internal static string GetPropertyName<TModel, TValue>(this Expression<Func<TModel, TValue>> propertySelector)
        {
            string name = null;
            if (propertySelector.Body is UnaryExpression)
                name = ((MemberExpression)(propertySelector.Body as UnaryExpression).Operand).Member.Name;
            else
                name = ((MemberExpression)propertySelector.Body).Member.Name;

            return name;
        }

        /// <summary>
        /// Gets the default value of a type.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns>System.Object.</returns>
        internal static object GetDefaultValue(this Type t)
        {
            if (t.IsValueType && Nullable.GetUnderlyingType(t) == null)
                return Activator.CreateInstance(t);
            else
                return null;
        }

        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter.</typeparam>
        /// <typeparam name="TScalarResult">The type of the scalar result.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="command">The command.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>TScalarResult.</returns>
        public static TScalarResult ExecuteScalar<TFilter, TScalarResult>(this IDatabaseManager databaseManager,
            ICommandProcessor<TFilter> command, TFilter filter = default(TFilter), params DbParameter[] explicitParameters)
        {
            return command.ExecuteScalar<TScalarResult>(databaseManager, filter, explicitParameters);
        }

        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="command">The command.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>System.Int32.</returns>
        public static int ExecuteNonQuery<TFilter>(this IDatabaseManager databaseManager,
            ICommandProcessor<TFilter> command, TFilter filter = default(TFilter), params DbParameter[] explicitParameters)
        {
            return command.ExecuteNonQuery(databaseManager, filter, explicitParameters);
        }

        /// <summary>
        /// Executes the specified command.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="command">The command.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        public static IEnumerable<TResult> Execute<TFilter, TResult>(this IDatabaseManager databaseManager,
            ICommandProcessor<TFilter, TResult> command, TFilter filter = default(TFilter), params DbParameter[] explicitParameters)
            where TResult : new()
        {
            return command.Execute(databaseManager, filter, explicitParameters);
        }

        /// <summary>
        /// Executes the specified command.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter.</typeparam>
        /// <typeparam name="TResult1">The type of the 1st result.</typeparam>
        /// <typeparam name="TResult2">The type of the 2nd result.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="command">The command.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;IEnumerable&lt;TResult1&gt;, IEnumerable&lt;TResult2&gt;&gt;.</returns>
        public static Tuple<
            IEnumerable<TResult1>,
            IEnumerable<TResult2>> Execute<TFilter, TResult1, TResult2>(
                this IDatabaseManager databaseManager,
                ICommandProcessor<TFilter, TResult1, TResult2> command, TFilter filter = default(TFilter), params DbParameter[] explicitParameters)
            where TResult1 : new()
            where TResult2 : new()
        {
            return command.Execute(databaseManager, filter, explicitParameters);
        }

        /// <summary>
        /// Executes the specified command.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter.</typeparam>
        /// <typeparam name="TResult1">The type of the 1st result.</typeparam>
        /// <typeparam name="TResult2">The type of the 2nd result.</typeparam>
        /// <typeparam name="TResult3">The type of the 3rd result.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="command">The command.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;IEnumerable&lt;TResult1&gt;, IEnumerable&lt;TResult2&gt;, IEnumerable&lt;TResult3&gt;&gt;.</returns>
        public static Tuple<IEnumerable<TResult1>,
            IEnumerable<TResult2>,
            IEnumerable<TResult3>> Execute<TFilter, TResult1, TResult2, TResult3>(this IDatabaseManager databaseManager,
            ICommandProcessor<TFilter, TResult1, TResult2, TResult3> command,
            TFilter filter = default(TFilter),
            params DbParameter[] explicitParameters)
            where TResult1 : new()
            where TResult2 : new()
            where TResult3 : new()
        {
            return command.Execute(databaseManager, filter, explicitParameters);
        }

        /// <summary>
        /// Executes the specified command.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter.</typeparam>
        /// <typeparam name="TResult1">The type of the 1st result.</typeparam>
        /// <typeparam name="TResult2">The type of the 2nd result.</typeparam>
        /// <typeparam name="TResult3">The type of the 3rd result.</typeparam>
        /// <typeparam name="TResult4">The type of the 4th result.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="command">The command.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;IEnumerable&lt;TResult1&gt;, IEnumerable&lt;TResult2&gt;, IEnumerable&lt;TResult3&gt;, IEnumerable&lt;TResult4&gt;&gt;.</returns>
        public static Tuple<IEnumerable<TResult1>,
            IEnumerable<TResult2>,
            IEnumerable<TResult3>,
            IEnumerable<TResult4>> Execute<TFilter, TResult1, TResult2, TResult3, TResult4>(this IDatabaseManager databaseManager,
            ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4> command,
            TFilter filter = default(TFilter),
            params DbParameter[] explicitParameters)
            where TResult1 : new()
            where TResult2 : new()
            where TResult3 : new()
            where TResult4 : new()
        {
            return command.Execute(databaseManager, filter, explicitParameters);
        }

        /// <summary>
        /// Executes the specified command.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter.</typeparam>
        /// <typeparam name="TResult1">The type of the 1st result.</typeparam>
        /// <typeparam name="TResult2">The type of the 2nd result.</typeparam>
        /// <typeparam name="TResult3">The type of the 3rd result.</typeparam>
        /// <typeparam name="TResult4">The type of the 4th result.</typeparam>
        /// <typeparam name="TResult5">The type of the 5th result.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="command">The command.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;IEnumerable&lt;TResult1&gt;, IEnumerable&lt;TResult2&gt;, IEnumerable&lt;TResult3&gt;, IEnumerable&lt;TResult4&gt;, IEnumerable&lt;TResult5&gt;&gt;.</returns>
        public static Tuple<IEnumerable<TResult1>,
            IEnumerable<TResult2>,
            IEnumerable<TResult3>,
            IEnumerable<TResult4>,
            IEnumerable<TResult5>> Execute<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5>(this IDatabaseManager databaseManager,
            ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5> command,
            TFilter filter = default(TFilter),
            params DbParameter[] explicitParameters)
            where TResult1 : new()
            where TResult2 : new()
            where TResult3 : new()
            where TResult4 : new()
            where TResult5 : new()
        {
            return command.Execute(databaseManager, filter, explicitParameters);
        }

        /// <summary>
        /// Executes the specified command.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter.</typeparam>
        /// <typeparam name="TResult1">The type of the 1st result.</typeparam>
        /// <typeparam name="TResult2">The type of the 2nd result.</typeparam>
        /// <typeparam name="TResult3">The type of the 3rd result.</typeparam>
        /// <typeparam name="TResult4">The type of the 4th result.</typeparam>
        /// <typeparam name="TResult5">The type of the 5th result.</typeparam>
        /// <typeparam name="TResult6">The type of the 6th result.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="command">The command.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;IEnumerable&lt;TResult1&gt;, IEnumerable&lt;TResult2&gt;, IEnumerable&lt;TResult3&gt;, IEnumerable&lt;TResult4&gt;, IEnumerable&lt;TResult5&gt;, IEnumerable&lt;TResult6&gt;&gt;.</returns>
        public static Tuple<IEnumerable<TResult1>,
            IEnumerable<TResult2>,
            IEnumerable<TResult3>,
            IEnumerable<TResult4>,
            IEnumerable<TResult5>,
            IEnumerable<TResult6>> Execute<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>(this IDatabaseManager databaseManager,
            ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6> command,
            TFilter filter = default(TFilter),
            params DbParameter[] explicitParameters)
            where TResult1 : new()
            where TResult2 : new()
            where TResult3 : new()
            where TResult4 : new()
            where TResult5 : new()
            where TResult6 : new()
        {
            return command.Execute(databaseManager, filter, explicitParameters);
        }

        /// <summary>
        /// Executes the specified command.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter.</typeparam>
        /// <typeparam name="TResult1">The type of the 1st result.</typeparam>
        /// <typeparam name="TResult2">The type of the 2nd result.</typeparam>
        /// <typeparam name="TResult3">The type of the 3rd result.</typeparam>
        /// <typeparam name="TResult4">The type of the 4th result.</typeparam>
        /// <typeparam name="TResult5">The type of the 5th result.</typeparam>
        /// <typeparam name="TResult6">The type of the 6th result.</typeparam>
        /// <typeparam name="TResult7">The type of the 7th result.</typeparam>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="command">The command.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Tuple&lt;IEnumerable&lt;TResult1&gt;, IEnumerable&lt;TResult2&gt;, IEnumerable&lt;TResult3&gt;, IEnumerable&lt;TResult4&gt;, IEnumerable&lt;TResult5&gt;, IEnumerable&lt;TResult6&gt;, IEnumerable&lt;TResult7&gt;&gt;.</returns>
        public static Tuple<IEnumerable<TResult1>, IEnumerable<TResult2>, IEnumerable<TResult3>, IEnumerable<TResult4>, IEnumerable<TResult5>, IEnumerable<TResult6>, IEnumerable<TResult7>>
            Execute<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>(this IDatabaseManager databaseManager,
                ICommandProcessor<TFilter, TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7> command,
                TFilter filter = default(TFilter),
                params DbParameter[] explicitParameters)
            where TResult1 : new()
            where TResult2 : new()
            where TResult3 : new()
            where TResult4 : new()
            where TResult5 : new()
            where TResult6 : new()
            where TResult7 : new()
        {
            return command.Execute(databaseManager, filter, explicitParameters);
        }
    }
}