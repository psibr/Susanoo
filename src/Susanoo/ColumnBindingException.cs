using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Susanoo
{
    [Serializable]
    public class ColumnBindingException : InvalidCastException
    {
        public ColumnBindingException()
            : base() { }

        public ColumnBindingException(string message)
            : base(message) { }

        public ColumnBindingException(string message, Exception innerException)
            : base(message, innerException) { }

        public ColumnBindingException(string message, int errorCode)
            : base(message, errorCode) { }

        public ColumnBindingException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
