using System;
using Polly;
using Polly.Retry;
using Microsoft.AspNetCore.Http;
using DotNetLoadBalancer.Classes;



namespace DotNetLoadBalancer.Middleware
{
    public class FactoryMiddleware : IMiddleware
    {
        private readonly ILogger _logger;

        public FactoryMiddleware(ILogger logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(
            HttpContext context,
            RequestDelegate next)
        {
            _logger.LogInformation("Entering pre request middleware handlers");

            await next(context);

            _logger.LogInformation("Completed middleware handlers and returned response");
        }
    }
}

