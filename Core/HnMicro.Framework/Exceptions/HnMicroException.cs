namespace HnMicro.Framework.Exceptions
{
    public class HnMicroException : Exception
    {
        public HnMicroException() : base()
        {
        }

        public HnMicroException(string message) : base(message)
        {
        }

        public HnMicroException(string message, params object[] args) : base(string.Format(message, args))
        {
        }

        public HnMicroException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
