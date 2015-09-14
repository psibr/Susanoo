using System;
using System.Numerics;
using Susanoo.Command;
using Susanoo.Pipeline;

namespace Susanoo.Transforms
{
    /// <summary>
    /// Describes and places a priority ranking on a modification of a CommandBuilder.
    /// </summary>
    public class CommandTransform 
        : IFluentPipelineFragment
    {
        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        /// <value>The priority.</value>
        public int Priority { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the modifier function.
        /// </summary>
        /// <value>The modifier function.</value>
        public Func<IExecutableCommandInfo, IExecutableCommandInfo> Transform { get; set; }

        /// <summary>
        /// Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public BigInteger CacheHash { get; set; }
    }
}


///// <summary>
///// Gets the where filter options. Null if no where filter.
///// </summary>
///// <value>The where filter options.</value>
//public IDictionary<string, object> WhereFilterOptions { get; private set; }

///// <summary>
///// Builds the where filter.
///// </summary>
///// <param name="optionsObject">The options object.</param>
///// <returns>ICommandExpression&lt;TFilter&gt;.</returns>
//public ICommandResultExpression<TFilter, TResult> BuildWhereFilter(object optionsObject = null)
//{
//    WhereFilterOptions = optionsObject != null ? optionsObject.ToExpando() : new ExpandoObject();

//    //Make sure the CommandBuilder is wrapped in a new SELECT for simplicity.
//    Command.AddQueryWrapper();

//    var whereFilterModifier = new CommandTransform
//    {
//        Description = "WhereFilter",
//        Priority = 900,
//        Transform = BuildWhereFilterImplementation
//    };

//    whereFilterModifier.CacheHash =
//        HashBuilder.Compute(whereFilterModifier.Description + WhereFilterOptions.Aggregate(string.Empty,
//            (s, pair) => s + pair.Key + pair.Value));

//    if (!Command.TryAddCommandModifier(whereFilterModifier))
//        throw new Exception("Conflicting priorities for CommandBuilder modifiers");

//    return this;
//}





























///// <summary>
///// Makes the query a paged query using OFFSET/FETCH. REQUIRES Sql Server 2012.
///// </summary>
///// <typeparam name="TFilter">The type of the filter.</typeparam>
///// <typeparam name="TResult">The type of the t result.</typeparam>
///// <param name="commandResultExpression">The CommandBuilder result expression.</param>
///// <param name="rowCountParameterName">Name of the row count parameter.</param>
///// <param name="pageNumberParameterName">Name of the page number parameter.</param>
///// <param name="format">The format.</param>
///// <returns>ICommandExpression&lt;TFilter&gt;.</returns>
///// <exception cref="System.ArgumentException">Only CommandType.Text CommandBuilder Expressions can be dynamically paged.
///// or
///// CommandText must contain an Order By clause to be paged.</exception>
//public static ICommandResultExpression<TFilter, TResult> OffsetFetch<TFilter, TResult>(
//    this ICommandResultExpression<TFilter, TResult> commandResultExpression, string format,
//    string rowCountParameterName = "RowCount", string pageNumberParameterName = "PageNumber")
//{
//    var commandInfo = commandResultExpression.Command;

//    if (commandInfo.DbCommandType != CommandType.Text)
//        throw new ArgumentException("Only CommandType.Text CommandBuilder Expressions can be dynamically paged.");

//    var offsetFetchStatement = string.Format(format, pageNumberParameterName, rowCountParameterName);

//    commandResultExpression.Command.TryAddCommandModifier(new CommandTransform
//    {
//        Description = "OFFSET/FETCH",
//        Priority = 100,
//        Transform = info => new ExecutableCommandInfo
//        {
//            CommandText = string.Concat(info.CommandText, offsetFetchStatement),
//            DbCommandType = info.DbCommandType,
//            Parameters = info.Parameters
//        },
//        CacheHash = HashBuilder.Compute(offsetFetchStatement)
//    });

//    return commandResultExpression;
//}

///// <summary>
///// Makes the query a paged query using OFFSET/FETCH. REQUIRES Sql Server 2012.
///// </summary>
///// <typeparam name="TFilter">The type of the filter.</typeparam>
///// <typeparam name="TResult">The type of the t result.</typeparam>
///// <param name="commandResultExpression">The CommandBuilder result expression.</param>
///// <param name="rowCountParameterName">Name of the row count parameter.</param>
///// <param name="pageNumberParameterName">Name of the page number parameter.</param>
///// <param name="computePageNumber">if set to <c>true</c> computes the page number.</param>
///// <returns>ICommandExpression&lt;TFilter&gt;.</returns>
///// <exception cref="System.ArgumentException">Only CommandType.Text CommandBuilder Expressions can be dynamically paged.
///// or
///// CommandText must contain an Order By clause to be paged.</exception>
//public static ICommandResultExpression<TFilter, TResult> OffsetFetch<TFilter, TResult>(
//    this ICommandResultExpression<TFilter, TResult> commandResultExpression,
//    string rowCountParameterName = "RowCount", string pageNumberParameterName = "PageNumber", bool computePageNumber = true)
//{
//    var format = computePageNumber
//        ? "\r\nOFFSET (@{0} - 1) * @{1} ROWS" +
//          "\r\nFETCH NEXT @{1} ROWS ONLY"
//        : "\r\nOFFSET @{0} ROWS" +
//          "\r\nFETCH NEXT @{1} ROWS ONLY";

//    OffsetFetch(commandResultExpression, format, rowCountParameterName, pageNumberParameterName);

//    return commandResultExpression;
//}

///// <summary>
///// Adds a total row count to the query wrapper.
///// </summary>
///// <typeparam name="TFilter">The type of the filter.</typeparam>
///// <typeparam name="TResult">The type of the result.</typeparam>
///// <param name="commandResultExpression">The CommandBuilder result expression.</param>
///// <param name="totalRowsColumnName">Total name of the total row count column.</param>
///// <param name="format">The format.</param>
///// <returns>ICommandExpression&lt;TFilter&gt;.</returns>
///// <exception cref="System.ArgumentException">Only CommandType.Text CommandBuilder Expressions can be dynamically built.</exception>
///// <exception cref="ArgumentException">Only CommandType.Text CommandBuilder Expressions can be dynamically built.</exception>
//public static ICommandResultExpression<TFilter, TResult> AddTotalRowCount<TFilter, TResult>(
//    this ICommandResultExpression<TFilter, TResult> commandResultExpression,
//    string totalRowsColumnName = "TotalRows", string format = "{0} = COUNT(*) OVER ()")
//{
//    var commandInfo = commandResultExpression.Command;

//    if (commandInfo.DbCommandType != CommandType.Text)
//        throw new ArgumentException("Only CommandType.Text CommandBuilder Expressions can be dynamically built.");

//    var totalsFormat = format;

//    var totalText = string.Format(totalsFormat, totalRowsColumnName);

//    var wrapper =
//        CommandManager.Instance.Bootstrapper.BuildQueryWrapper(totalText);

//    wrapper.CacheHash = (wrapper.CacheHash * 31) ^ HashBuilder.Compute(totalText);

//    commandResultExpression.Command
//        .AddOrReplaceCommandModifier(wrapper);

//    return commandResultExpression;
//}









//private Regex _orderByRegex;

///// <summary>
///// Retrieves the order by regex used for whitelisting allowed cahracters.
///// </summary>
///// <returns>Regex.</returns>
//public virtual Regex RetrieveOrderByRegex()
//{
//    return _orderByRegex ?? (_orderByRegex = new Regex(
//        @"\A
//		            # 1. Match all of these conditions
//		            (?:
//		              # 2. Leading Whitespace
//		              \ *
//		              # 3. ColumnName: a-z, A-Z, 0-9, _
//		              (?<ColumnName>[0-9_a-z]*)
//		              # 4. Whitespace
//		              \ *
//		              # 5. SortDirection: ASC or DESC case-insensitive
//		              (?<SortDirection>ASC|DESC)?
//		              # 6. Optional Comma
//		              ,?
//		            )*
//		            \z",
//        RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace));
//}