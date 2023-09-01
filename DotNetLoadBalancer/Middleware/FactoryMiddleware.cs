using System;
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

            HttpClient httpClient = new HttpClient();

            Microsoft.AspNetCore.Http.HostString hostString = new Microsoft.AspNetCore.Http.HostString(DotNetLoadBalancer.Classes.RoundRobin.GetRoundRobinEntry());
            context.Request.Host = hostString;
            var response = await httpClient.SendAsync(RequestTranscriptHelpers.ToHttpRequestMessage(context.Request));


            while ((int)response.StatusCode == 429)
            {
                
            }


            await next(context);

            _logger.LogInformation("Completed middleware handlers and returned response");
        }
    }
}

