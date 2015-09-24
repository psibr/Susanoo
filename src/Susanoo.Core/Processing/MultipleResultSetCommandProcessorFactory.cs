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
    /// Builds a command processor that returns multiple result sets.
    /// </summary>
    public class MultipleResultSetCommandProcessorFactory : IMultipleResultSetCommandProcessorFactory
    {
        /// <summary>
        /// Builds the command processor.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter.</typeparam>
        /// <param name="mappings">The mappings.</param>
        /// <param name="name">The name.</param>
        /// <param name="resultTypes">The result types.</param>
        /// <returns>INoResultCommandProcessor&lt;TFilter, TResult&gt;.</returns>
        public IMultipleResultSetCommandProcessor<TFilter> BuildCommandProcessor<TFilter>(ICommandResultInfo<TFilter> mappings,
            string name = null, params Type[] resultTypes) => new MultipleResultSetCommandProcessor<TFilter>(mappings, name, resultTypes);
    }
}
