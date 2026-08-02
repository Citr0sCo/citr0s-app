using Citr0sApp.Api.Core.Types;

namespace Citr0sApp.Api.Features.SteamApi.Types;

public class SteamUserOwnedGameStatsResponse : CommunicationResponse
{
    public int TotalOwned { get; set; }
    public int Played { get; set; }
    public int NeverPlayed { get; set; }
    public int RecentlyActive { get; set; }
    public double PlayedPercentage { get; set; }
    public double NeverPlayedPercentage { get; set; }
}
