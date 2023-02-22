using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Prometheus;
using System;
using System.Threading.Tasks;

namespace Simple
{
  
    public class GaugeMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public GaugeMiddleware(
            RequestDelegate next
            , ILoggerFactory loggerFactory
            )
        {
            this._next = next;
            this._logger = loggerFactory.CreateLogger<RequestMiddleware>();
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var path = httpContext.Request.Path.Value;
            var method = httpContext.Request.Method;
          
            var gauge = Metrics.CreateGauge("mvcFrontGauge", "mvcFrontGaugeHelp");
            //.CreateCounter("mvcfront_demo_request_total", "HTTP Requests Total", new CounterConfiguration
            //{
            //    LabelNames = new[] { "path", "method", "status" }
            //});

            var statusCode = 200;

            try
            {
                await _next.Invoke(httpContext);
                
                gauge.IncTo(100);
            }
            catch (Exception)
            {
                statusCode = 500;
                //counter.Labels(path, method, statusCode.ToString()).Inc();
                gauge.DecTo(100);

                throw;
            }

            if (path != "/metrics")
            {
                //statusCode = httpContext.Response.StatusCode;
                //counter.Labels(path, method, statusCode.ToString()).Inc();
            }
        }
    }


    public static class GaugeMiddlewareExtensions
    {
        public static IApplicationBuilder UseGaugeMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GaugeMiddleware>();
        }
    }
}
