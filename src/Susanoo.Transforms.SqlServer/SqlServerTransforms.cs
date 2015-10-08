using System;
using System.Data;
using Susanoo.Command;
using Susanoo.Processing;
using Susanoo.Proxies.Transforms;

namespace Susanoo.Transforms
{
    public static class SqlServerTransforms
    {
        /// <summary>
        /// Makes the query a paged query using OFFSET/FETCH. REQUIRES Sql Server 2012.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="processor">The processor.</param>
        /// <param name="format">The format.</param>
        /// <param name="rowCountParameterName">Name of the row count parameter.</param>
        /// <param name="pageNumberParameterName">Name of the page number parameter.</param>
        /// <returns>ICommandExpression&lt;TFilter&gt;.</returns>
        /// <exception cref="System.ArgumentException">Only CommandType.Text CommandBuilder Expressions can be dynamically paged.
        /// or
        /// CommandText must contain an Order By clause to be paged.</exception>
        public static CommandTransform OffsetFetch<TFilter, TResult>(
            ISingleResultSetCommandProcessor<TFilter, TResult> processor, string format,
            string rowCountParameterName = "RowCount", string pageNumberParameterName = "PageNumber")
        {
            var commandInfo = processor.CommandBuilderInfo;

            if (commandInfo.DbCommandType != CommandType.Text)
                throw new ArgumentException("Only CommandType.Text CommandBuilder Expressions can be dynamically paged.");

            return new CommandTransform(
                "OFFSET/FETCH",
                info => new ExecutableCommandInfo
                {
                    CommandText = string.Concat(info.CommandText, 
                        string.Format(format, pageNumberParameterName, rowCountParameterName)),
                    DbCommandType = info.DbCommandType,
                    Parameters = info.Parameters
                });
        }

        /// <summary>
        /// Makes the query a paged query using OFFSET/FETCH. REQUIRES Sql Server 2012.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="processor">The processor.</param>
        /// <param name="rowCountParameterName">Name of the row count parameter.</param>
        /// <param name="pageNumberParameterName">Name of the page number parameter.</param>
        /// <param name="computePageNumber">if set to <c>true</c> computes the page number.</param>
        /// <returns>ICommandExpression&lt;TFilter&gt;.</returns>
        /// <exception cref="System.ArgumentException">Only CommandType.Text CommandBuilder Expressions can be dynamically paged.
        /// or
        /// CommandText must contain an Order By clause to be paged.</exception>
        public static CommandTransform OffsetFetch<TFilter, TResult>(
            ISingleResultSetCommandProcessor<TFilter, TResult> processor,
            string rowCountParameterName = "RowCount", string pageNumberParameterName = "PageNumber", bool computePageNumber = true)
        {
            var format = computePageNumber
                ? "\r\nOFFSET (@{0} - 1) * @{1} ROWS" +
                  "\r\nFETCH NEXT @{1} ROWS ONLY"
                : "\r\nOFFSET @{0} ROWS" +
                  "\r\nFETCH NEXT @{1} ROWS ONLY";

            return OffsetFetch(processor, format, rowCountParameterName, pageNumberParameterName);
        }

        /// <summary>
        /// Adds a total row count to the query wrapper.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="processor">The processor.</param>
        /// <param name="totalRowsColumnName">Total name of the total row count column.</param>
        /// <param name="format">The format.</param>
        /// <returns>ICommandExpression&lt;TFilter&gt;.</returns>
        /// <exception cref="System.ArgumentException">Only CommandType.Text CommandBuilder Expressions can be dynamically built.</exception>
        /// <exception cref="ArgumentException">Only CommandType.Text CommandBuilder Expressions can be dynamically built.</exception>
        public static CommandTransform QueryWrapperWithTotalRowCount<TFilter, TResult>(
            ISingleResultSetCommandProcessor<TFilter, TResult> processor, 
            string totalRowsColumnName = "TotalRows", string format = "{0} = COUNT(*) OVER ()")
        {
            var commandInfo = processor.CommandBuilderInfo;

            if (commandInfo.DbCommandType != CommandType.Text)
                throw new ArgumentException("Only CommandType.Text CommandBuilder Expressions can be dynamically built.");

            var totalsFormat = format;

            var totalText = string.Format(totalsFormat, totalRowsColumnName);

            var wrapper = Transforms.QueryWrapper(totalText);

            return wrapper;
        }
    }
}