using Microsoft.AspNetCore.Mvc;

namespace DotNetLoadBalancer.Controllers;

[ApiController]
[Route("[controller]")]
public class PostOpenAIProxyController : ControllerBase
{
    

    private readonly ILogger<PostOpenAIProxyController> _logger;

    public PostOpenAIProxyController(ILogger<PostOpenAIProxyController> logger)
    {
        _logger = logger;
    }

    [HttpPost(Name = "PostOpenAIProxyCalls")]
    public string Post()
    {
        return "aaa";
    }
}

