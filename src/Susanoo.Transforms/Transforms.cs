using Susanoo.Command;
using Susanoo.Processing;
using Susanoo.Proxies.Transforms;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Susanoo.Transforms
{
	public static class Transforms
	{
		private static readonly Regex OrderByRegex = new Regex(
							@"\A
										# 1. Match all of these conditions
										(?:
										  # 2. Leading Whitespace
										  \ *
										  # 3. ColumnName: a-z, A-Z, 0-9, _, []
										  (?<ColumnName>[0-9_a-z\[\]]*)
										  # 4. Whitespace
										  \ *
										  # 5. SortDirection: ASC or DESC case-insensitive
										  (?<SortDirection>ASC|DESC)?
										  # 6. Optional Comma
										  ,?
										)*
										\z",
							RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

		/// <summary>
		/// Builds a single query wrapper.
		/// </summary>
		/// <param name="additionalColumns">The additional columns.</param>
		/// <returns>CommandTransform.</returns>
		public static CommandTransform QueryWrapper(string additionalColumns = null)
		{
			if (additionalColumns != null)
			{
				additionalColumns = additionalColumns.Trim();

				if (!additionalColumns.StartsWith(","))
					additionalColumns = ", " + additionalColumns;
			}

			const string format = "SELECT *{1}" + "\r\n" +
                                  "FROM (" + "\r\n" +
                                  "{0} " + "\r\n" +
                                  ") susanoo_query_wrapper" + "\r\n" +
                                  "WHERE 1=1" + "\r\n";

			return new CommandTransform("Query Wrapper", info =>
				new ExecutableCommandInfo
				{
					CommandText = string.Format(format, info.CommandText, additionalColumns ?? string.Empty),
					DbCommandType = info.DbCommandType,
					Parameters = info.Parameters
				});
		}

	    /// <summary>
	    /// A transform that allows a parameter at execution time to control the ORDER BY statement.
	    /// </summary>
	    /// <param name = "parameterName" > Name of the parameter.</param>
	    /// <exception cref="ArgumentNullException">parameterName is null.</exception>
	    /// <returns>ICommandExpression&lt; TFilter&gt;.</returns>
	    /// <exception cref="FormatException">Order By parameter either contains unsafe characters or a bad format</exception>
	    public static CommandTransform OrderByExpression(string parameterName = "OrderBy")
		{
			if (parameterName == null)
				throw new ArgumentNullException(nameof(parameterName));

			var orderByModifier = new CommandTransform("OrderByExpression", info =>
				{
					var orderByParameter = info.Parameters.First(p => p.ParameterName == parameterName);

					if (orderByParameter.Value == null
						|| !(OrderByRegex.IsMatch(orderByParameter.Value.ToString())))
						throw new FormatException("Order By paramter either contains unsafe characters or a bad format");

					return new ExecutableCommandInfo
					{
						CommandText = info.CommandText + "\r\nORDER BY " + orderByParameter.Value,
						Parameters = info.Parameters.Where(p => p.ParameterName != parameterName).ToArray(),
						DbCommandType = info.DbCommandType
					};
				}
			);

			return orderByModifier;
		}

		/// <summary>
		/// Builds the where filter.
		/// </summary>
		/// <typeparam name="TFilter">The type of the t filter.</typeparam>
		/// <typeparam name="TResult">The type of the t result.</typeparam>
		/// <param name="processor">The processor.</param>
		/// <param name="optionsObject">The options object.</param>
		/// <returns>ICommandExpression&lt;TFilter&gt;.</returns>
		public static CommandTransform WhereFilter<TFilter>(ICommandProcessorWithResults<TFilter> processor, Type resultType, object optionsObject = null)
		{
			var whereFilterModifier = new CommandTransform("WhereFilter", 
				new WhereFilterTransformFactory<TFilter>(processor, resultType, optionsObject != null ? optionsObject.ToExpando() : new ExpandoObject())
					.BuildWhereFilterTransform);

			return whereFilterModifier;
		}

		/// <summary>
		/// Builds the where filter.
		/// </summary>
		/// <typeparam name="TFilter">The type of the t filter.</typeparam>
		/// <typeparam name="TResult">The type of the t result.</typeparam>
		/// <param name="processor">The processor.</param>
		/// <param name="whereFilterOptions">The where filter options.</param>
		/// <returns>ICommandExpression&lt;TFilter&gt;.</returns>
		public static CommandTransform WhereFilter<TFilter, TResult>(ICommandProcessorWithResults<TFilter> processor, Type resultType, IDictionary<string, object> whereFilterOptions)
		{
			var whereFilterModifier = new CommandTransform("WhereFilter",
				new WhereFilterTransformFactory<TFilter>(processor, resultType, whereFilterOptions)
					.BuildWhereFilterTransform);

			return whereFilterModifier;
		}

		/// <summary>
		/// Builds the where filter.
		/// </summary>
		/// <typeparam name="TFilter">The type of the t filter.</typeparam>
		/// <typeparam name="TResult">The type of the t result.</typeparam>
		/// <param name="processor">The processor.</param>
		/// <param name="options">The options.</param>
		/// <returns>ICommandExpression&lt;TFilter&gt;.</returns>
		public static CommandTransform WhereFilter<TFilter, TResult>(ICommandProcessorWithResults<TFilter> processor, Type resultType, ExpandoObject options)
		{
			var whereFilterModifier = new CommandTransform("WhereFilter",
				new WhereFilterTransformFactory<TFilter>(processor, resultType, options)
					.BuildWhereFilterTransform);

			return whereFilterModifier;
		}
	}
}