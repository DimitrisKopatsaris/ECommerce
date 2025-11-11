using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ECommerce.Api.Middlewares;

public sealed class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _log;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> log)
    {
        _next = next;
        _log = log;
    }

    public async Task Invoke(HttpContext ctx)
    {
        try
        {
            await _next(ctx);
        }
        catch (OperationCanceledException) when (ctx.RequestAborted.IsCancellationRequested)
        {
            // client disconnected; don't treat as an error
            _log.LogWarning("Request aborted by client: {Method} {Path}", ctx.Request.Method, ctx.Request.Path);
            ctx.Response.StatusCode = StatusCodes.Status499ClientClosedRequest; // non-standard but informative
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Unhandled exception for {Method} {Path}", ctx.Request.Method, ctx.Request.Path);

            ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            ctx.Response.ContentType = "application/problem+json";

            var problem = new
            {
                type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
                title = "Unhandled Server Error",
                status = ctx.Response.StatusCode,
                traceId = ctx.TraceIdentifier
            };

            await ctx.Response.WriteAsync(JsonSerializer.Serialize(problem));
        }
    }
}
