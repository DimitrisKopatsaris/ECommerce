using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ECommerce.Api.Middlewares;

public sealed class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _log;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> log)
    {
        _next = next;
        _log = log;
    }

    public async Task Invoke(HttpContext ctx)
    {
        var sw = Stopwatch.StartNew();

        var method = ctx.Request.Method;
        var path   = ctx.Request.Path.ToString();
        var query  = ctx.Request.QueryString.HasValue ? ctx.Request.QueryString.Value : string.Empty;

        _log.LogInformation("HTTP {Method} {Path}{Query} started", method, path, query);

        await _next(ctx);

        sw.Stop();
        _log.LogInformation("HTTP {Method} {Path} responded {StatusCode} in {ElapsedMs}ms",
            method, path, ctx.Response.StatusCode, sw.ElapsedMilliseconds);
    }
}
