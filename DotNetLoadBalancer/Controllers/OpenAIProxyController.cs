using System;
using RoundRobin;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net;
using Microsoft.Extensions.Configuration;
using DotNetLoadBalancer.Classes;

namespace DotNetLoadBalancer.Controllers;

[ApiController]
[Route("[controller]")]
public class PostOpenAIProxyController : ControllerBase
{

    private readonly IConfiguration Configuration;
    private readonly List<string> _openAiEndpoints;
    private readonly ILogger<PostOpenAIProxyController> _logger;
    private HttpClient _httpClient;

    public PostOpenAIProxyController(IConfiguration configuration, ILogger<PostOpenAIProxyController> logger, HttpClient httpClient)
    {
        Configuration = configuration;
        _logger = logger;
        _httpClient = httpClient;
        _openAiEndpoints= Configuration.GetSection("openAiEndpoints").Get<List<string>>();
    }

    [HttpPost(Name = "PostOpenAIProxyCalls")]
    public async Task<IActionResult> Post()
    {
        HttpResponseMessage response;
        int retryCount = 0;


        HostString hostString = new HostString(DotNetLoadBalancer.Classes.RoundRobin.GetRoundRobinEntry(_openAiEndpoints));

        HttpRequest request = HttpContext.Request;
        request.Host = hostString;
        request.Path = "";

        _logger.LogInformation($"Calling endpoint:{hostString}");

        response = await _httpClient.SendAsync(RequestTranscriptHelpers.ToHttpRequestMessage(request));


        if ((int)response.StatusCode == 429)
        {
            while (retryCount <= 3)
            {
                request.Host = new HostString(DotNetLoadBalancer.Classes.RoundRobin.GetRoundRobinEntry(_openAiEndpoints));
                request.Path = "";
                response = await _httpClient.SendAsync(RequestTranscriptHelpers.ToHttpRequestMessage(request));
                retryCount++;
            }

            if (retryCount == 3)
            {
                // Create HttpResponseMessage with status code 429
                response.StatusCode = HttpStatusCode.TooManyRequests;
                
                // Set the response content
                response.Content = new StringContent("Rate limit exceeded - unsucessfully retried 3 times.");
            }
        }

        return new ResponseMessageResult(response);
    }
}

