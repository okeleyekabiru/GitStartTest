namespace GitStartFramework.Shared.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException() : base("Invalid params")
        {
        }

        public BadRequestException(string message) : base(message)
        {
        }

        public BadRequestException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}