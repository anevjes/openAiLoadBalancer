using DotNetLoadBalancer.Classes;
using System;
using Microsoft.AspNetCore.Mvc;
using Polly;
using System.Net.Http;
using System.Net;

namespace DotNetLoadBalancer.Controllers;

[ApiController]
[Route("[controller]")]
public class PostOpenAIProxyController : ControllerBase
{
    

    private readonly ILogger<PostOpenAIProxyController> _logger;
    private HttpClient _httpClient;

    public PostOpenAIProxyController(ILogger<PostOpenAIProxyController> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    [HttpPost(Name = "PostOpenAIProxyCalls")]
    public async Task<IActionResult> Post()
    {
        HttpResponseMessage response;
        int retryCount = 0;

        HostString hostString = new HostString(DotNetLoadBalancer.Classes.RoundRobin.GetRoundRobinEntry());
        HttpRequest request = HttpContext.Request;
        request.Host = hostString;
        request.Path = "";
        response = await _httpClient.SendAsync(RequestTranscriptHelpers.ToHttpRequestMessage(request));


        if ((int)response.StatusCode == 429)
        {
            while (retryCount <= 3)
            {
                request.Host = new HostString(DotNetLoadBalancer.Classes.RoundRobin.GetRoundRobinEntry());
                //remove the controller path as part of the response
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

