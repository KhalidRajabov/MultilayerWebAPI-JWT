using System.Text.Json.Serialization;

namespace SharedLibrary.DTOs
{
    public class Response<T> where T : class
    {
        public T? Data { get; private set; }
        public int StatusCode { get; private set; }
        [JsonIgnore] public bool isSuccessfull { get;private set; }

        public ErrorDTO? Error { get;private set; }

        public static Response<T> Success(T data, int statusCode)
        {
            return new Response<T> { Data = data, StatusCode = statusCode, isSuccessfull = true, };
        }

        public static Response<T> Success(int statusCode)
        {
            return new Response<T> { Data = default, StatusCode = statusCode, isSuccessfull = true };
        }

        public static Response<T> Fail(ErrorDTO error, int statusCode)
        {
            return new Response<T> { Error = error, StatusCode = statusCode, isSuccessfull = false };
        }

        public static Response<T> Fail(string errorMessage, int statusCode, bool isShow)
        {
            var errorDto = new ErrorDTO(errorMessage, isShow);
            return new Response<T> { Error = errorDto, StatusCode = statusCode, isSuccessfull = false };
        }
    }
}
