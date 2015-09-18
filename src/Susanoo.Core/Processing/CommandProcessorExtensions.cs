using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Susanoo.Processing
{
    /// <summary>
    /// Class CommandProcessorExtensions.
    /// </summary>
    public static class CommandProcessorExtensions
    {
        /// <summary>
        /// Sets the timeout.
        /// </summary>
        /// <typeparam name="TCommandProcessor">The type of the t command processor.</typeparam>
        /// <param name="processor">The processor.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>TCommandProcessor.</returns>
        public static TCommandProcessor SetTimeout<TCommandProcessor>(this TCommandProcessor processor, TimeSpan timeout)
            where TCommandProcessor : ICommandProcessorInterop
        {
            processor.Timeout = timeout;
            return processor;
        }
    }
}
