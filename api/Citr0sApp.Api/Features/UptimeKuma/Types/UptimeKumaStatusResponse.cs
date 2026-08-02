namespace Citr0sApp.Api.Features.UptimeKuma.Types;

public sealed class UptimeKumaStatusResponse
{
    public DateTimeOffset RetrievedAt { get; init; }
    public List<UptimeKumaMonitor> Monitors { get; init; } = [];
}

public sealed class UptimeKumaMonitor
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string GroupName { get; init; } = string.Empty;
    public bool? IsUp { get; init; }
    public double? Uptime24Hours { get; init; }
    public List<UptimeKumaHeartbeat> History { get; init; } = [];
}

public sealed class UptimeKumaHeartbeat
{
    public int Status { get; init; }
    public DateTimeOffset? Time { get; init; }
    public double? Ping { get; init; }
    public string Message { get; init; } = string.Empty;
}
