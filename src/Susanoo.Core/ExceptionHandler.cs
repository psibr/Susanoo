using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Susanoo.Pipeline.Command;

namespace Susanoo
{
    /// <summary>
    /// Describes how to handle an exception based on conditions.
    /// </summary>
    [ImmutableObject(true)]
    public class ExceptionHandler
    {
        private readonly Func<ICommandExpressionInfo, Exception, DbParameter[], bool> _conditionFunc;
        private readonly Action<ICommandExpressionInfo, Exception, DbParameter[]> _handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionHandler"/> class.
        /// </summary>
        /// <param name="conditionFunc">The condition function.</param>
        /// <param name="handler">The handler.</param>
        public ExceptionHandler(
            Func<ICommandExpressionInfo, Exception, DbParameter[], bool> conditionFunc,
            Action<ICommandExpressionInfo, Exception, DbParameter[]> handler)
        {
            _conditionFunc = conditionFunc;
            _handler = handler;
        }

        /// <summary>
        /// Gets the exception handler.
        /// </summary>
        /// <value>The handler.</value>
        public Action<ICommandExpressionInfo, Exception, DbParameter[]> Handler
        {
            get { return _handler; }
        }

        /// <summary>
        /// Gets the condition function.
        /// </summary>
        /// <value>The condition function.</value>
        public Func<ICommandExpressionInfo, Exception, DbParameter[], bool> ConditionFunc
        {
            get { return _conditionFunc; }
        }
    }
}
