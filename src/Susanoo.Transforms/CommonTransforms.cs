using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;
using Susanoo.Command;

namespace Susanoo.Transforms
{
    public static class CommonTransforms
    {
        /// <summary>
        /// Builds a query wrapper.
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

            const string format = @"SELECT *{1}
FROM (
    {0}
) susanoo_query_wrapper
WHERE 1=1"; ;

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
        /// <returns>ICommandExpression&lt; TFilter&gt;.</returns>
        /// <exception cref = "Exception" > Conflicting priorities for CommandBuilder modifiers</exception>
        public static CommandTransform OrderByExpression(string parameterName = "OrderBy")
        {
            if (parameterName == null)
                throw new ArgumentNullException(nameof(parameterName));

            var orderByModifier = new CommandTransform("OrderByExpression", info =>
                {
                    var orderByParameter = info.Parameters.First(p => p.ParameterName == parameterName);

                    if (orderByParameter.Value == null
                        || !(new Regex(
                            @"\A
		                                # 1. Match all of these conditions
		                                (?:
		                                  # 2. Leading Whitespace
		                                  \ *
		                                  # 3. ColumnName: a-z, A-Z, 0-9, _
		                                  (?<ColumnName>[0-9_a-z]*)
		                                  # 4. Whitespace
		                                  \ *
		                                  # 5. SortDirection: ASC or DESC case-insensitive
		                                  (?<SortDirection>ASC|DESC)?
		                                  # 6. Optional Comma
		                                  ,?
		                                )*
		                                \z",
                            RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace)
                            .IsMatch(orderByParameter.Value.ToString())))
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
        /// <param name="optionsObject">The options object.</param>
        /// <returns>ICommandExpression&lt;TFilter&gt;.</returns>
        public static CommandTransform WhereFilter<TFilter>(object optionsObject = null)
        {
            var whereFilterModifier = new CommandTransform("WhereFilter", 
                new WhereFilterTransformFactory(optionsObject != null ? optionsObject.ToExpando() : new ExpandoObject())
                    .BuildWhereFilterTransform<TFilter>);

            return whereFilterModifier;
        }

        /// <summary>
        /// Builds the where filter.
        /// </summary>
        /// <typeparam name="TFilter">The type of the t filter.</typeparam>
        /// <param name="whereFilterOptions">The where filter options.</param>
        /// <returns>ICommandExpression&lt;TFilter&gt;.</returns>
        public static CommandTransform WhereFilter<TFilter>(IDictionary<string, object> whereFilterOptions)
        {
            var whereFilterModifier = new CommandTransform("WhereFilter",
                new WhereFilterTransformFactory(whereFilterOptions)
                    .BuildWhereFilterTransform<TFilter>);

            return whereFilterModifier;
        }

        /// <summary>
        /// Builds the where filter.
        /// </summary>
        /// <typeparam name="TFilter">The type of the t filter.</typeparam>
        /// <param name="options">The options.</param>
        /// <returns>ICommandExpression&lt;TFilter&gt;.</returns>
        public static CommandTransform WhereFilter<TFilter>(ExpandoObject options)
        {
            var whereFilterModifier = new CommandTransform("WhereFilter",
                new WhereFilterTransformFactory(options)
                    .BuildWhereFilterTransform<TFilter>);

            return whereFilterModifier;
        }
    }
}

///// <summary>
///// Builds a computed insert statement.
///// </summary>
///// <typeparam name="TFilter">The type of the filter.</typeparam>
///// <returns>ICommandExpression&lt;TFilter, TResult&gt;.</returns>
//public ICommandProcessor<TFilter> DefineInsert<TFilter>(string tableName, Func<ICommandExpression<TFilter>, ICommandExpression<TFilter>> commandFunc = null)
//{
//    var command =
//        Bootstrapper.RetrieveCommandBuilder()
//            .DefineCommand<TFilter>(string.Empty, CommandType.Text);

//    if (commandFunc != null)
//        command = commandFunc(command);

//    var commandInfo = (ICommandBuilderInfo<TFilter>)command;

//    var columnNames = Bootstrapper.RetrievePropertyMetadataExtractor()
//        .FindAllowedProperties(
//            typeof(TFilter),
//            DescriptorActions.Insert,
//            commandInfo.PropertyWhitelist.ToArray(),
//            commandInfo.PropertyBlacklist.ToArray())
//        .Select(p => p.Value.ActiveAlias)
//        .Aggregate(string.Empty, (p, c) =>
//            p.Length == 0 ? c : p + ", " + c);

//    var insertReadyFormat = $"INSERT INTO {tableName} ( {columnNames} ) VALUES {{0}}";

//    commandInfo.TryAddCommandModifier(new CommandTransform
//    {
//        Priority = 1,
//        Description = "Insert Builder",
//        Transform = info => new ExecutableCommandInfo
//        {
//            CommandText = string.Format(insertReadyFormat, null), //
//            DbCommandType = CommandType.Text,
//            Parameters = info.Parameters
//        },
//        CacheHash = HashBuilder.Compute(typeof(TFilter).AssemblyQualifiedName + "_Susanoo_Insert")
//    });

//    return command.Realize();
//}