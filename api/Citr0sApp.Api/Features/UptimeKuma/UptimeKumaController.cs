using Citr0sApp.Api.Features.UptimeKuma.Types;
using Microsoft.AspNetCore.Mvc;

namespace Citr0sApp.Api.Features.UptimeKuma;

[ApiController]
[Route("api/uptime-kuma")]
public sealed class UptimeKumaController : ControllerBase
{
    private readonly UptimeKumaService _service;

    public UptimeKumaController(UptimeKumaService service)
    {
        _service = service;
    }

    [HttpGet("status")]
    public Task<UptimeKumaStatusResponse> GetStatus(CancellationToken cancellationToken)
    {
        return _service.GetStatus(cancellationToken);
    }
}
