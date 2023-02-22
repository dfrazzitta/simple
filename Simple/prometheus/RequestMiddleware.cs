using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Prometheus;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Simple
{
    public class RequestMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private Counter counter;
        private readonly Histogram _responseTimeHistogram;
        public RequestMiddleware(
            RequestDelegate next
            , ILoggerFactory loggerFactory
            )
        {
            this._next = next;
            this._logger = loggerFactory.CreateLogger<RequestMiddleware>();
             
        }

        public async Task Invoke(HttpContext httpContext, MetricCollector collector)
        {
            var path = httpContext.Request.Path.Value;
            var method = httpContext.Request.Method;

            counter = Metrics.CreateCounter("mvcfront_demo_request_total", "HTTP Requests Total", new CounterConfiguration
            {
                LabelNames = new[] { "path", "method", "status" }
            });

            var statusCode = 200;
            var sw = Stopwatch.StartNew();
            try
            {
                await _next.Invoke(httpContext);
                counter.Labels(path, method, statusCode.ToString()).Inc();
            }
            catch (Exception)
            {
                statusCode = 500;
                counter.Labels(path, method, statusCode.ToString()).Inc();

                throw;
            }
            finally
            {
                sw.Stop();

                collector.RegisterRequest();
                collector.RegisterResponseTime(httpContext.Response.StatusCode, httpContext.Request.Method, sw.Elapsed);



            }

            if (path != "/metrics")
            {
                statusCode = httpContext.Response.StatusCode;
                counter.Labels(path, method, statusCode.ToString()).Inc();
            }
        }
    }


    public static class RequestMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestMiddleware>();
        }
    }
}
