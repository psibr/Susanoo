#region

using Susanoo.Command;
using System;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

#endregion

namespace Susanoo.Exceptions
{
    /// <summary>
    /// Exception that occurs during execution of a SQL command.
    /// </summary>
    [Serializable]
    public class SusanooExecutionException 
        : Exception
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
        /// <exception cref="SerializationException">The class name is null or <see cref="P:System.Exception.HResult" /> is zero (0). </exception>
        /// <exception cref="ArgumentNullException">The <paramref name="info" /> parameter is null. </exception>
        [SuppressMessage("ReSharper", "ExceptionNotDocumented")]
        protected SusanooExecutionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Info = (ICommandInfo)info.GetValue("Info", typeof(ICommandInfo));
            Timeout = (TimeSpan)info.GetValue("Timeout", typeof(TimeSpan));
            Parameters = (DbParameter[]) info.GetValue("Parameters", typeof (DbParameter[]));
        }

        /// <summary>
        /// When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown. </param><param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination. </param><exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is a null reference (Nothing in Visual Basic). </exception>
        [SuppressMessage("ReSharper", "ExceptionNotDocumented")]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("Info", Info);
            info.AddValue("Timeout", Timeout);
            info.AddValue("Parameters", Parameters);
        }
    }
}