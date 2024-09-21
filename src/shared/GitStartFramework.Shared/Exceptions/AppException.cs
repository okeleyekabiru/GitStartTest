namespace GitStartFramework.Shared.Exceptions
{
    public class AppException : Exception
    {
        public AppException() : base("An error occurred while processing ")
        {
        }

        public AppException(string message) : base(message)
        {
        }

        public AppException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}