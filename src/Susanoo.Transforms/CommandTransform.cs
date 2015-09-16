
































///// <summary>
///// Makes the query a paged query using OFFSET/FETCH. REQUIRES Sql Server 2012.
///// </summary>
///// <typeparam name="TFilter">The type of the filter.</typeparam>
///// <typeparam name="TResult">The type of the result.</typeparam>
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
///// <typeparam name="TResult">The type of the result.</typeparam>
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










