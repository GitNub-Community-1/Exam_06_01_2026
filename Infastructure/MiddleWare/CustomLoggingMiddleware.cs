using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace WebAPIWithJWTAndIdentity.MiddleWare;

public class CustomLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CustomLoggingMiddleware> _logger;

    public CustomLoggingMiddleware(RequestDelegate next, ILogger<CustomLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
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
            var time = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            System.Console.WriteLine("-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-");
            _logger.LogInformation("User Request Activity {@Activity}", new
            {
                Method = method,
                Path = path,
                UserId = userId,
                IP = clientIp,
                Time = time
            });
            System.Console.WriteLine("-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-");
            var stopwatch = Stopwatch.StartNew();
            await _next(context);
            stopwatch.Stop();
            System.Console.WriteLine("-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-");
            _logger.LogInformation("Request finished in {Duration} ms", stopwatch.ElapsedMilliseconds);
            System.Console.WriteLine("-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-");
        }
        catch (Exception e)
        {
            System.Console.WriteLine("-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-");
            _logger.LogError("Something went wrong, take error message: {Errors}" , e.Message);
            _logger.LogError("This is a global error list: {GlobalError}", e);
            System.Console.WriteLine("-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-");
        }
    }
}