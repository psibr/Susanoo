using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Susanoo.Pipeline.Command;
using Susanoo.Pipeline.Command.ResultSets;

namespace Susanoo
{
    /// <summary>
    /// Extends Result expressions to add SQL Server specific funtionality
    /// </summary>
    public static class CommandResultExpressionExtensions
    {

        /// <summary>
        /// Makes the query a paged query using OFFSET/FETCH. REQUIRES Sql Server 2012.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter.</typeparam>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="commandResultExpression">The command result expression.</param>
        /// <param name="rowCountParameterName">Name of the row count parameter.</param>
        /// <param name="pageNumberParameterName">Name of the page number parameter.</param>
        /// <returns>ICommandExpression&lt;TFilter&gt;.</returns>
        /// <exception cref="System.ArgumentException">Only CommandType.Text Command Expressions can be dynamically paged.
        /// or
        /// CommandText must contain an Order By clause to be paged.</exception>
        public static ICommandResultExpression<TFilter, TResult> OffsetFetch<TFilter, TResult>(
            this ICommandResultExpression<TFilter, TResult> commandResultExpression,
            string rowCountParameterName = "RowCount", string pageNumberParameterName = "PageNumber")
        {
            var commandInfo = commandResultExpression.Command;

            if (commandInfo.DbCommandType != CommandType.Text)
                throw new ArgumentException("Only CommandType.Text Command Expressions can be dynamically paged.");

            var offsetFetchStatement = string.Format(PagingFormat, pageNumberParameterName, rowCountParameterName);

            commandResultExpression.Command.TryAddCommandModifier(new CommandModifier
            {
                Description = "OFFSET/FETCH",
                Priority = 100,
                ModifierFunc = info => new ExecutableCommandInfo
                {
                    CommandText = string.Concat(info.CommandText, offsetFetchStatement),
                    DbCommandType = info.DbCommandType,
                    Parameters = info.Parameters
                },
                CacheHash = HashBuilder.Compute(offsetFetchStatement)
            });

            return commandResultExpression;
        }

        private const string PagingFormat =
            "\r\nOFFSET (@{0} - 1) * @{1} ROWS" +
            "\r\nFETCH NEXT @{1} ROWS ONLY";

        /// <summary>
        /// Adds a total row count to the query wrapper.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="commandResultExpression">The command result expression.</param>
        /// <param name="totalRowsColumnName">Total name of the total row count column.</param>
        /// <returns>ICommandExpression&lt;TFilter&gt;.</returns>
        /// <exception cref="ArgumentException">Only CommandType.Text Command Expressions can be dynamically built.</exception>
        public static ICommandResultExpression<TFilter, TResult> AddTotalRowCount<TFilter, TResult>(
            this ICommandResultExpression<TFilter, TResult> commandResultExpression,
            string totalRowsColumnName = "TotalRows")
        {
            var commandInfo = commandResultExpression.Command;

            if (commandInfo.DbCommandType != CommandType.Text)
                throw new ArgumentException("Only CommandType.Text Command Expressions can be dynamically built.");

            const string totalsFormat = "{0} = COUNT(*) OVER ()";

            var totalText = string.Format(totalsFormat, totalRowsColumnName);

            var wrapper =
                CommandManager.Bootstrapper.BuildQueryWrapper(totalText);

            wrapper.CacheHash = (wrapper.CacheHash * 31) ^ HashBuilder.Compute(totalText);

            commandResultExpression.Command
                .AddOrReplaceCommandModifier(wrapper);

            return commandResultExpression;
        }

    }
}
