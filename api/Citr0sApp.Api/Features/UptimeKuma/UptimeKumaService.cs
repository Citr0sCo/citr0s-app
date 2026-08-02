using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Fennel.CSharp;
using Citr0sApp.Api.Features.UptimeKuma.Types;

namespace Citr0sApp.Api.Features.UptimeKuma;

public sealed class UptimeKumaService
{
    private const string StatusPageSlug = "default";
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<UptimeKumaService> _logger;
    private readonly string? _baseUrl;
    private readonly string? _apiKey;

    public UptimeKumaService(IHttpClientFactory httpClientFactory, ILogger<UptimeKumaService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _baseUrl = Environment.GetEnvironmentVariable("ASPNETCORE_UPTIME_KUMA_BASE_URL")
            ?? Environment.GetEnvironmentVariable("ASPNETCORE_UPTIME_KUMA_URL");
        _apiKey = Environment.GetEnvironmentVariable("ASPNETCORE_UPTIME_KUMA_API_KEY");
    }

    public async Task<UptimeKumaStatusResponse> GetStatus(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_baseUrl))
        {
            _logger.LogWarning("Uptime Kuma is not configured. Set ASPNETCORE_UPTIME_KUMA_BASE_URL.");
            return new UptimeKumaStatusResponse { RetrievedAt = DateTimeOffset.UtcNow };
        }

        try
        {
            using var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(20);

            var baseUrl = _baseUrl.TrimEnd('/');
            var statusPageTask = GetJson(client, $"{baseUrl}/api/status-page/{StatusPageSlug}", cancellationToken);
            var heartbeatTask = GetJson(client, $"{baseUrl}/api/status-page/heartbeat/{StatusPageSlug}", cancellationToken);
            var metricsTask = GetMetrics(client, baseUrl, cancellationToken);

            await Task.WhenAll(statusPageTask, heartbeatTask, metricsTask).ConfigureAwait(false);

            return BuildResponse(statusPageTask.Result, heartbeatTask.Result, metricsTask.Result);
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException or JsonException)
        {
            _logger.LogWarning(exception, "Unable to read Uptime Kuma status.");
            return new UptimeKumaStatusResponse { RetrievedAt = DateTimeOffset.UtcNow };
        }
    }

    private async Task<JsonDocument> GetJson(HttpClient client, string url, CancellationToken cancellationToken)
    {
        using var response = await client.GetAsync(url, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        return await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    private async Task<Dictionary<string, bool>> GetMetrics(HttpClient client, string baseUrl, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
            return [];

        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($":{_apiKey}"));
        using var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/metrics");
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

        try
        {
            using var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            return ParseMetrics(body);
        }
        catch (HttpRequestException exception)
        {
            _logger.LogWarning(exception, "Unable to read authenticated Uptime Kuma metrics; falling back to heartbeat status.");
            return [];
        }
    }

    private static Dictionary<string, bool> ParseMetrics(string body)
    {
        var metrics = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

        foreach (var line in Prometheus.ParseText(body))
        {
            if (!line.IsMetric)
                continue;

            var metric = (Metric)line;
            if (metric.MetricName != "monitor_status" || !metric.Labels.TryGetValue("monitor_name", out var name))
                continue;

            metrics[name] = metric.MetricValue == 1;
        }

        return metrics;
    }

    private static UptimeKumaStatusResponse BuildResponse(
        JsonDocument statusPage,
        JsonDocument heartbeat,
        Dictionary<string, bool> metrics)
    {
        using (statusPage)
        using (heartbeat)
        {
            var heartbeatById = ParseHeartbeats(heartbeat.RootElement, out var uptimeById);
            var monitors = new List<UptimeKumaMonitor>();

            if (statusPage.RootElement.TryGetProperty("publicGroupList", out var groups))
            {
                foreach (var group in groups.EnumerateArray())
                {
                    var groupName = GetString(group, "name");
                    if (!group.TryGetProperty("monitorList", out var monitorList))
                        continue;

                    foreach (var monitor in monitorList.EnumerateArray())
                    {
                        var id = GetInt32(monitor, "id");
                        var name = GetString(monitor, "name");
                        var history = heartbeatById.GetValueOrDefault(id, []);
                        var latest = history.LastOrDefault();
                        bool? isUp = latest is null ? null : latest.Status == 1;
                        if (metrics.TryGetValue(name, out var metricStatus))
                            isUp = metricStatus;

                        monitors.Add(new UptimeKumaMonitor
                        {
                            Id = id,
                            Name = name,
                            GroupName = groupName,
                            IsUp = isUp,
                            Uptime24Hours = uptimeById.GetValueOrDefault($"{id}_24"),
                            History = history
                        });
                    }
                }
            }

            return new UptimeKumaStatusResponse
            {
                RetrievedAt = DateTimeOffset.UtcNow,
                Monitors = monitors
            };
        }
    }

    private static Dictionary<int, List<UptimeKumaHeartbeat>> ParseHeartbeats(
        JsonElement root,
        out Dictionary<string, double> uptimeById)
    {
        var heartbeatById = new Dictionary<int, List<UptimeKumaHeartbeat>>();
        uptimeById = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

        if (root.TryGetProperty("heartbeatList", out var heartbeatList))
        {
            foreach (var property in heartbeatList.EnumerateObject())
            {
                if (!int.TryParse(property.Name, out var id) || property.Value.ValueKind != JsonValueKind.Array)
                    continue;

                heartbeatById[id] = property.Value.EnumerateArray()
                    .Select(ParseHeartbeat)
                    .TakeLast(120)
                    .ToList();
            }
        }

        if (root.TryGetProperty("uptimeList", out var uptimeList))
        {
            foreach (var property in uptimeList.EnumerateObject())
            {
                if (property.Value.TryGetDouble(out var uptime))
                    uptimeById[property.Name] = uptime;
            }
        }

        return heartbeatById;
    }

    private static UptimeKumaHeartbeat ParseHeartbeat(JsonElement value)
    {
        DateTimeOffset? time = null;
        var timeText = GetString(value, "time");
        if (DateTimeOffset.TryParse(timeText, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var parsedTime))
            time = parsedTime;

        return new UptimeKumaHeartbeat
        {
            Status = GetInt32(value, "status"),
            Time = time,
            Ping = value.TryGetProperty("ping", out var ping) && ping.TryGetDouble(out var pingValue) ? pingValue : null,
            Message = GetString(value, "msg")
        };
    }

    private static string GetString(JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out var property) && property.ValueKind == JsonValueKind.String
            ? property.GetString() ?? string.Empty
            : string.Empty;
    }

    private static int GetInt32(JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out var property) && property.TryGetInt32(out var value)
            ? value
            : 0;
    }
}
