#region

using System;

#endregion

namespace Susanoo.Exceptions
{
    /// <summary>
    /// Exception that describes a failure to resolve a dependency.
    /// </summary>
    public class SusanooDependencyResolutionException : InvalidCastException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SusanooDependencyResolutionException" /> class.
        /// </summary>
        public SusanooDependencyResolutionException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SusanooDependencyResolutionException" /> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public SusanooDependencyResolutionException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SusanooDependencyResolutionException" /> class with a specified error message and a
        /// reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the
        /// <paramref name="innerException" /> parameter is not a null reference (Nothing in Visual Basic), the current
        /// exception is raised in a catch block that handles the inner exception.</param>
        public SusanooDependencyResolutionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SusanooDependencyResolutionException" /> class with a specified message and error
        /// code.
        /// </summary>
        /// <param name="message">The message that indicates the reason the exception occurred.</param>
        /// <param name="errorCode">The error code (HRESULT) value associated with the exception.</param>
        public SusanooDependencyResolutionException(string message, int errorCode)
            : base(message, errorCode)
        {
        }
    }
}