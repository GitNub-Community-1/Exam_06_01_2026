using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace WebAPIWithJWTAndIdentity.MiddleWare;

public class CustomLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public CustomLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            var method = context.Request.Method;
            var path = context.Request.Path;
            var userId = context.User.Identity?.IsAuthenticated == true
                ? context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                : "Anonymous";
            var clientIp = context.Connection.RemoteIpAddress?.ToString();
            var time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            Console.WriteLine($"[{time}] Method: {method}, Path: {path}, UserId: {userId}, IP: {clientIp}");

            var stopwatch = Stopwatch.StartNew();
            await _next(context);
            stopwatch.Stop();

            Console.WriteLine($"Request duration: {stopwatch.ElapsedMilliseconds} ms");
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
        }
    }
}