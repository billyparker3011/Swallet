namespace HnMicro.Core.Helpers
{
    public static class ExceptionHelper
    {
        private static IEnumerable<Exception> GetInnerException(this Exception ex)
        {
            if (ex == null)
            {
                throw new ArgumentNullException("ex");
            }

            var innerException = ex;

            do
            {
                yield return innerException;

                innerException = innerException.InnerException;
            }
            while (innerException != null);
        }

        private static IEnumerable<string> GetMessageExceptions(this Exception ex)
        {
            return ex.GetInnerException().Select(f => f.Message);
        }

        private static IEnumerable<string> GetStackTraceExceptions(this Exception ex)
        {
            return ex.GetInnerException().Select(f => f.StackTrace);
        }

        public static string GetMessageException(this Exception ex)
        {
            var exceptions = ex.GetMessageExceptions().ToList();
            return string.Join(Environment.NewLine, exceptions);
        }

        public static string GetStackTraceException(this Exception ex)
        {
            var exceptions = ex.GetStackTraceExceptions().ToList();
            return string.Join(Environment.NewLine, exceptions);
        }
    }
}
