using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Susanoo.Command;

namespace Susanoo.Transforms
{
    public static class CommandManagerTransformExtensions
    {
        /// <summary>
        /// Retrieves the query wrapper format.
        /// </summary>
        /// <returns>System.String.</returns>
        public static string RetrieveQueryWrapperFormat()
        {
            return
@"SELECT *{1}
FROM (
    {0}
) susanoo_query_wrapper
WHERE 1=1";
        }

        /// <summary>
        /// Builds a query wrapper.
        /// </summary>
        /// <param name="commandManager">The command manager.</param>
        /// <param name="additionalColumns">The additional columns.</param>
        /// <returns>CommandTransform.</returns>
        public static CommandTransform BuildQueryWrapper(this CommandManager commandManager, string additionalColumns = null)
        {
            if (additionalColumns != null)
            {
                additionalColumns = additionalColumns.Trim();

                if (!additionalColumns.StartsWith(","))
                    additionalColumns = ", " + additionalColumns;
            }

            var format = RetrieveQueryWrapperFormat();

            return new CommandTransform
            {
                Description = "SusanooWrapper",
                Priority = 1000,
                Transform = info =>
                    new ExecutableCommandInfo
                    {

                        CommandText = string.Format(format, info.CommandText, additionalColumns ?? string.Empty),
                        DbCommandType = info.DbCommandType,
                        Parameters = info.Parameters
                    },
                CacheHash = HashBuilder.Compute(format)
            };
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
    }
}


/// <summary>
/// Builds the where filter.
/// </summary>
/// <param name="parameterName">Name of the parameter.</param>
/// <returns>ICommandExpression&lt;TFilter&gt;.</returns>
/// <exception cref="Exception">Conflicting priorities for CommandBuilder modifiers</exception>
//public ICommandResultExpression<TFilter, TResult> AddOrderByExpression(string parameterName = "OrderBy")
//{
//    if (parameterName == null)
//        throw new ArgumentNullException(nameof(parameterName));

//    var orderByModifier = new CommandTransform
//    {
//        Description = "OrderByExpression",
//        Priority = 800,
//        Transform = info =>
//        {
//            var orderByParameter = info.Parameters.First(p => p.ParameterName == parameterName);

//            if (orderByParameter.Value == null
//                || !CommandManager.Instance.Bootstrapper.RetrieveOrderByRegex()
//                    .IsMatch(orderByParameter.Value.ToString()))
//                throw new FormatException("Order By paramter either contains unsafe characters or a bad format");

//            return new ExecutableCommandInfo
//            {
//                CommandText = info.CommandText + "\r\nORDER BY " + orderByParameter.Value,
//                Parameters = info.Parameters.Where(p => p.ParameterName != parameterName).ToArray(),
//                DbCommandType = info.DbCommandType
//            };
//        },
//        CacheHash = HashBuilder.Compute("ORDER BY @" + parameterName)
//    };

//    if (!Command.TryAddCommandModifier(orderByModifier))
//        throw new Exception("Conflicting priorities for CommandBuilder modifiers");

//    return this;
//}

///// <summary>
///// Builds the where filter implementation.
///// </summary>
///// <param name="info">The information.</param>
///// <returns>ICommandResultExpression&lt;TFilter, TResult&gt;.</returns>
//protected virtual IExecutableCommandInfo BuildWhereFilterImplementation(
//    IExecutableCommandInfo info)
//{
//    var mappings = info.Parameters
//        .Join(Export(typeof(TFilter)), parameter => parameter.SourceColumn, pair => pair.Key,
//            (parameter, pair) =>
//                new Tuple<string, Type, string, string>(
//                    pair.Key,                                 //Property Name
//                    pair.Value.PropertyMetadata.PropertyType, //Property Type
//                    parameter.ParameterName,                  //Parameter Name
//                    pair.Value.ActiveAlias                    //Result Column Name
//                    ))
//        .GroupJoin(WhereFilterOptions, tuple => tuple.Item1, pair => pair.Key,
//            (tuple, pairs) => new { tuple, comparer = pairs.Select(kvp => kvp.Value).FirstOrDefault() })
//        .Select(o => new Tuple<string, Type, string, string, object>(
//            o.tuple.Item1,                                          //Property Name
//            o.tuple.Item2,                                          //Property Type
//            o.tuple.Item3,                                          //Parameter Name
//            o.tuple.Item4,                                          //Result Column Name
//            o.comparer ?? GetDefaultCompareMethod(o.tuple.Item2)    //Comparer
//            ));

//    return new ExecutableCommandInfo
//    {
//        CommandText = info.CommandText + string.Concat(mappings.Select(o =>
//        {
//            var parameter = info.Parameters.SingleOrDefault(param => param.ParameterName == o.Item3);

//            var compareFormat = string.Empty;
//            //If no matching parameter or value is null or DBNull, don't add a comaparison.
//            if (parameter != null && parameter.Value != null && parameter.Value != DBNull.Value)
//            {
//                if (o.Item5 is CompareMethod)
//                    compareFormat = Comparison.GetComparisonFormat((CompareMethod)o.Item5);
//            }

//            var value = o.Item5 as ComparisonOverride;
//            compareFormat = value != null ? value.OverrideText : compareFormat;

//            if (compareFormat.Contains('{'))
//                compareFormat = string.Format(compareFormat, o.Item3, "[" + o.Item4 + "]");

//            return compareFormat;
//        })),
//        DbCommandType = info.DbCommandType,
//        Parameters = info.Parameters
//    };
//}

///// <summary>
///// Gets the default compare method.
///// </summary>
///// <param name="type">The type.</param>
///// <returns>CompareMethod.</returns>
//private static CompareMethod GetDefaultCompareMethod(Type type)
//{
//    var result = Comparison.Equal;
//    if (type == typeof(string))
//        result = CompareMethod.Contains;
//    else if (type == typeof(DateTime) || CommandManager.GetDbType(type) == null)
//        result = CompareMethod.Ignore;

//    return result;
//}