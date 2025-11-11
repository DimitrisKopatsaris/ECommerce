using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ECommerce.Api.Middlewares;

public sealed class MetricsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<MetricsMiddleware> _log;

    private static readonly ConcurrentDictionary<string, EndpointMetrics> _metrics = new();

    public MetricsMiddleware(RequestDelegate next, ILogger<MetricsMiddleware> log)
    {
        _next = next;
        _log = log;
    }

    public async Task Invoke(HttpContext ctx)
    {
        var route = ctx.Request.Path.HasValue ? ctx.Request.Path.Value! : "/";
        var key = $"{ctx.Request.Method} {route}".ToLowerInvariant();

        var sw = Stopwatch.StartNew();
        var success = true;

        try
        {
            await _next(ctx);
            success = ctx.Response.StatusCode < 500;
        }
        catch
        {
            success = false;
            throw;
        }
        finally
        {
            sw.Stop();
            var m = _metrics.GetOrAdd(key, _ => new EndpointMetrics());
            m.Record(sw.Elapsed.TotalMilliseconds, success);

            // Optional: occasionally log aggregates for quick visibility
            if (m.Count % 20 == 0)
            {
                _log.LogInformation("METRICS {Key}: count={Count}, errors={Errors}, p50≈{P50}ms, last={LastMs}ms",
                    key, m.Count, m.Errors, m.ApproxP50Ms, sw.Elapsed.TotalMilliseconds);
            }
        }
    }

    private sealed class EndpointMetrics
    {
        private long _count;
        private long _errors;
        private readonly int _window = 32;
        private readonly double[] _recent;
        private int _idx;

        public EndpointMetrics()
        {
            _recent = new double[_window];
        }

        public long Count => _count;
        public long Errors => _errors;
        public double ApproxP50Ms
        {
            get
            {
                // simple rolling buffer median-ish (not precise; good enough for Day 7)
                var copy = _recent.ToArray();
                Array.Sort(copy);
                return copy[_window / 2];
            }
        }

        public void Record(double elapsedMs, bool success)
        {
            Interlocked.Increment(ref _count);
            if (!success) Interlocked.Increment(ref _errors);

            var i = Interlocked.Increment(ref _idx);
            _recent[i % _window] = elapsedMs;
        }
    }
}
