using System.Net;

namespace WebAPIWithJWTAndIdentity.Response;

public class Response<T>
{
    public int StatusCode { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }

    public Response() { }

    public Response(T data)
    {
        Data = data;
        StatusCode = (int)HttpStatusCode.OK;
    }

    public Response(HttpStatusCode statusCode, string message)
    {
        StatusCode = (int)statusCode;
        Message = message;
    }

    public Response(HttpStatusCode statusCode, List<string> messages)
    {
        StatusCode = (int)statusCode;
        Message = string.Join("; ", messages);
    }

    public Response(HttpStatusCode statusCode, string message, T? data)
    {
        StatusCode = (int)statusCode;
        Message = message;
        Data = data;
    }
}