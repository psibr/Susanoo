#region

using System;
using System.Data.Common;
using System.Runtime.Serialization;
using Susanoo.Command;

#endregion

namespace Susanoo.Exceptions
{
    /// <summary>
    /// Exception that occurs during execution of a SQL command.
    /// </summary>
    [Serializable]
    public class SusanooExecutionException : Exception
    {
        /// <summary>
        /// Gets the command information.
        /// </summary>
        /// <value>The information.</value>
        public ICommandInfo Info { get; }

        /// <summary>
        /// Gets the timeout period.
        /// </summary>
        /// <value>The timeout.</value>
        public TimeSpan Timeout { get; }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        public DbParameter[] Parameters { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SusanooExecutionException" /> class with a specified error message and a
        /// reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the
        /// <paramref name="innerException" /> parameter is not a null reference (Nothing in Visual Basic), the current
        /// exception is raised in a catch block that handles the inner exception.</param>
        /// <param name="info">The information.</param>
        /// <param name="timeout">The timeout.</param>
        /// <param name="parameters">The parameters.</param>
        public SusanooExecutionException(string message, Exception innerException, ICommandInfo info, TimeSpan timeout,
            DbParameter[] parameters)
            : base(message, innerException)
        {
            Info = info;
            Timeout = timeout;
            Parameters = parameters;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SusanooExecutionException" /> class with a specified error message and a
        /// reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception. If the
        /// <paramref name="innerException" /> parameter is not a null reference (Nothing in Visual Basic), the current
        /// exception is raised in a catch block that handles the inner exception.</param>
        /// <param name="info">The information.</param>
        /// <param name="timeout">The timeout.</param>
        /// <param name="parameters">The parameters.</param>
        public SusanooExecutionException(Exception innerException, ICommandInfo info, TimeSpan timeout,
            DbParameter[] parameters)
            : base("Susanoo encountered an error during command excution.", innerException)
        {
            Info = info;
            Timeout = timeout;
            Parameters = parameters;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SusanooExecutionException" /> class with serialized data.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        protected SusanooExecutionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}