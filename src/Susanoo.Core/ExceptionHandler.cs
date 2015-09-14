using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Susanoo.Command;

namespace Susanoo
{
    /// <summary>
    /// Describes how to handle an exception based on conditions.
    /// </summary>
    [ImmutableObject(true)]
    public class ExceptionHandler
    {
        private readonly Func<ICommandInfo, Exception, DbParameter[], bool> _conditionFunc;
        private readonly Action<ICommandInfo, Exception, DbParameter[]> _handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionHandler"/> class.
        /// </summary>
        /// <param name="conditionFunc">The condition function.</param>
        /// <param name="handler">The handler.</param>
        public ExceptionHandler(
            Func<ICommandInfo, Exception, DbParameter[], bool> conditionFunc,
            Action<ICommandInfo, Exception, DbParameter[]> handler)
        {
            _conditionFunc = conditionFunc;
            _handler = handler;
        }

        /// <summary>
        /// Gets the exception handler.
        /// </summary>
        /// <value>The handler.</value>
        public Action<ICommandInfo, Exception, DbParameter[]> Handler
        {
            get { return _handler; }
        }

        /// <summary>
        /// Gets the condition function.
        /// </summary>
        /// <value>The condition function.</value>
        public Func<ICommandInfo, Exception, DbParameter[], bool> ConditionFunc
        {
            get { return _conditionFunc; }
        }
    }
}
