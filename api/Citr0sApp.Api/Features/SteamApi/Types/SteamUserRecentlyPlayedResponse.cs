using Citr0sApp.Api.Core.Types;

namespace Citr0sApp.Api.Features.SteamApi.Types;

public class SteamUserRecentlyPlayedResponse : CommunicationResponse
{
    public int Total { get; set; }
    public List<SteamGameSummary> Games { get; set; }
}

public class SteamGameSummary
{
    public int AppId { get; set; }
    public string Name { get; set; }
    public string IconUrl { get; set; }
    public int PlaytimeLastTwoWeeksInMinutes { get; set; }
    public int PlaytimeForeverInMinutes { get; set; }
}