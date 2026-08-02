using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Citr0sApp.Api.Features.UptimeKuma;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace HomeBoxLanding.Api.Tests;

[TestFixture]
public sealed class UptimeKumaServiceTests
{
    [Test]
    public async Task GetStatusUsesFennelMetricStatusWhenAvailable()
    {
        var originalBaseUrl = Environment.GetEnvironmentVariable("ASPNETCORE_UPTIME_KUMA_BASE_URL");
        var originalApiKey = Environment.GetEnvironmentVariable("ASPNETCORE_UPTIME_KUMA_API_KEY");
        Environment.SetEnvironmentVariable("ASPNETCORE_UPTIME_KUMA_BASE_URL", "https://uptime.test");
        Environment.SetEnvironmentVariable("ASPNETCORE_UPTIME_KUMA_API_KEY", "test-key");

        try
        {
            var handler = new StubHandler("monitor_status{monitor_name=\"Test server\",monitor_type=\"http\"} 1\n", 120, 0);
            using var httpClient = new HttpClient(handler);
            var service = new UptimeKumaService(new StubHttpClientFactory(httpClient), NullLogger<UptimeKumaService>.Instance);

            var response = await service.GetStatus();

            Assert.That(response.Monitors, Has.Count.EqualTo(1));
            Assert.That(response.Monitors[0].IsUp, Is.True);
            Assert.That(response.Monitors[0].Uptime24Hours, Is.EqualTo(1));
            Assert.That(response.Monitors[0].History[0].Ping, Is.EqualTo(120));
            Assert.That(handler.MetricsAuthorization?.Scheme, Is.EqualTo("Basic"));
        }
        finally
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_UPTIME_KUMA_BASE_URL", originalBaseUrl);
            Environment.SetEnvironmentVariable("ASPNETCORE_UPTIME_KUMA_API_KEY", originalApiKey);
        }
    }

    [Test]
    public async Task GetStatusIgnoresMalformedMetricsAndUsesHeartbeatStatus()
    {
        var originalBaseUrl = Environment.GetEnvironmentVariable("ASPNETCORE_UPTIME_KUMA_BASE_URL");
        var originalApiKey = Environment.GetEnvironmentVariable("ASPNETCORE_UPTIME_KUMA_API_KEY");
        Environment.SetEnvironmentVariable("ASPNETCORE_UPTIME_KUMA_BASE_URL", "https://uptime.test");
        Environment.SetEnvironmentVariable("ASPNETCORE_UPTIME_KUMA_API_KEY", "test-key");

        try
        {
            using var httpClient = new HttpClient(new StubHandler("# HELP monitor_status Uptime Kuma status\nmonitor_status{monitor_name=\"Test server\" 1\n"));
            var service = new UptimeKumaService(new StubHttpClientFactory(httpClient), NullLogger<UptimeKumaService>.Instance);

            var response = await service.GetStatus();

            Assert.That(response.Monitors, Has.Count.EqualTo(1));
            Assert.That(response.Monitors[0].Name, Is.EqualTo("Test server"));
            Assert.That(response.Monitors[0].IsUp, Is.True);
            Assert.That(response.Monitors[0].History, Has.Count.EqualTo(1));
            Assert.That(response.Monitors[0].History[0].Ping, Is.Null);
        }
        finally
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_UPTIME_KUMA_BASE_URL", originalBaseUrl);
            Environment.SetEnvironmentVariable("ASPNETCORE_UPTIME_KUMA_API_KEY", originalApiKey);
        }
    }

    private sealed class StubHttpClientFactory(HttpClient client) : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => client;
    }

    private sealed class StubHandler : HttpMessageHandler
    {
        private readonly string _metrics;
        private readonly string _ping;
        private readonly int _status;

        public StubHandler(string metrics, double? ping = null, int status = 1)
        {
            _metrics = metrics;
            _ping = ping?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? "null";
            _status = status;
        }

        public AuthenticationHeaderValue? MetricsAuthorization { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.RequestUri?.AbsolutePath == "/metrics")
                MetricsAuthorization = request.Headers.Authorization;

            var content = request.RequestUri?.AbsolutePath switch
            {
                "/api/status-page/default" => "{\"publicGroupList\":[{\"name\":\"Game Servers\",\"monitorList\":[{\"id\":1,\"name\":\"Test server\"}]}]}",
                "/api/status-page/heartbeat/default" => $"{{\"heartbeatList\":{{\"1\":[{{\"status\":{_status},\"time\":\"2026-08-02 18:00:00.000\",\"msg\":\"OK\",\"ping\":{_ping}}}]}},\"uptimeList\":{{\"1_24\":1}}}}",
                "/metrics" => _metrics,
                _ => string.Empty
            };

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(content)
            });
        }
    }
}
