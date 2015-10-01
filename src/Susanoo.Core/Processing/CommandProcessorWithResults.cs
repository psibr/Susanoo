#region

using System;
using System.Collections.Concurrent;
using System.Numerics;
using Susanoo.Command;
using Susanoo.ResultSets;

#endregion

namespace Susanoo.Processing
{
    /// <summary>
    ///     Common components between CommandProcessors with ResultSets
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    public abstract class CommandProcessorWithResults<TFilter> :
        ICommandProcessorWithResults<TFilter>,
        ICommandProcessor<TFilter>
    {

        /// <summary>
        ///     Gets the CommandBuilder expression.
        /// </summary>
        /// <value>The CommandBuilder expression.</value>
        public virtual ICommandBuilderInfo<TFilter> CommandBuilderInfo => CommandResultInfo.GetCommandInfo();

        /// <summary>
        /// Gets or sets the timeout of a command execution.
        /// </summary>
        /// <value>The timeout.</value>
        public virtual TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>
        ///     Gets the mapping expressions.
        /// </summary>
        /// <value>The mapping expressions.</value>
        public virtual ICommandResultInfo<TFilter> CommandResultInfo { get; protected set; }

        /// <summary>
        ///     Clears any column index information that may have been cached.
        /// </summary>
        public virtual void ClearColumnIndexInfo()
        {
        }

        /// <summary>
        ///     Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public virtual BigInteger CacheHash => 
            CommandResultInfo.CacheHash;

        /// <summary>
        ///     Retrieves a copy of the column index information.
        /// </summary>
        /// <returns>ColumnChecker.</returns>
        public virtual ColumnChecker RetrieveColumnIndexInfo()
        {
            return new ColumnChecker(0);
        }
    }
}