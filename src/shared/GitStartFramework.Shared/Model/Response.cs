namespace GitStartFramework.Shared.Model
{
    public class Response<TData>
    {
        public Response(TData? data, string message, string code)
        {
            Data = data;
            Message = message;
            Code = code;
        }

        public Response()
        {
        }

        public string? Message { get; set; }
        public string? Code { get; set; }
        public TData? Data { get; set; }

        public static Response<TData> Success(TData data, string message = "Api Successful", string code = "00")
        {
            return new Response<TData>(data, message, code);
        }

        public static Response<TData> Failed(TData data, string message, string code)
        {
            return new Response<TData>(data, message, code);
        }
    }
}