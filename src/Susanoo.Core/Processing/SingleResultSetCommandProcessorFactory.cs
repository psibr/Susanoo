using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Susanoo.ResultSets;

namespace Susanoo.Processing
{
    /// <summary>
    /// Builds a command processor that returns a single result set.
    /// </summary>
    public class SingleResultSetCommandProcessorFactory : ISingleResultSetCommandProcessorFactory
    {
        /// <summary>
        /// Builds the command processor.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <param name="name">The name.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult&gt;.</returns>
        public ICommandProcessor<TFilter, TResult> BuildCommandProcessor<TFilter, TResult>(ICommandResultInfo<TFilter> mappings,
            string name = null) => new SingleResultSetCommandProcessor<TFilter, TResult>(mappings, name);
    }
}
