using System;

namespace HnMicro.Framework.Exceptions
{
    public class UnhandledException : Exception
    {
        public UnhandledException() : base()
        {
        }

        public UnhandledException(string message) : base(message)
        {
        }

        public UnhandledException(string message, params object[] args) : base(string.Format(message, args))
        {
        }

        public UnhandledException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
