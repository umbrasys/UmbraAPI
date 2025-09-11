using MareSynchronosAuthService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MareSynchronosAuthService.Controllers;

[AllowAnonymous]
[ApiController]
public class WellKnownController : Controller
{
    private readonly DiscoveryWellKnownProvider _provider;

    public WellKnownController(DiscoveryWellKnownProvider provider)
    {
        _provider = provider;
    }

    [HttpGet("/.well-known/Umbra/client")]
    public IActionResult Get()
    {
        var json = _provider.GetWellKnownJson(Request.Scheme, Request.Host.Value);
        return Content(json, "application/json");
    }
}

