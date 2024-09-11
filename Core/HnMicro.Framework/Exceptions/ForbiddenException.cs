namespace HnMicro.Framework.Exceptions
{
    public class ForbiddenException : Exception
    {
        public ForbiddenException() : base()
        {
        }

        public ForbiddenException(string message) : base(message)
        {
        }

        public ForbiddenException(string message, params object[] args) : base(string.Format(message, args))
        {
        }

        public ForbiddenException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
