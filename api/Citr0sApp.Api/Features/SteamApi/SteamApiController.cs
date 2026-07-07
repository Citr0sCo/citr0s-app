using Citr0sApp.Api.Features.SteamApi.Types;
using Microsoft.AspNetCore.Mvc;

namespace Citr0sApp.Api.Features.SteamApi;

[ApiController]
[Route("api/steam")]
public class SteamApiController : ControllerBase
{
    private readonly SteamApiCache _service;

    public SteamApiController(IHttpClientFactory factory)
    {
        _service = SteamApiCache.Instance(factory);
    }

    [HttpGet("{steamId}")]
    public async Task<ActionResult<SteamUserProfileResponse>> GetUserProfile(string steamId)
    {
        var response = await _service.GetUserProfile(steamId);
        return Ok(response);
    }

    [HttpGet("{steamId}/decoration")]
    public async Task<ActionResult<SteamUserProfileDecorationResponse>> GetUserProfileDecoration(string steamId)
    {
        var response = await _service.GetUserProfileDecoration(steamId);
        return Ok(response);
    }

    [HttpGet("{steamId}/activity")]
    public async Task<ActionResult<SteamUserRecentlyPlayedResponse>> GetUserActivity(string steamId)
    {
        var response = await _service.GetRecentlyPlayed(steamId);
        return Ok(response);
    }
}