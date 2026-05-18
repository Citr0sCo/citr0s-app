using Citr0sApp.Api.Core.Types;

namespace Citr0sApp.Api.Features.SteamApi.Types;

public class SteamUserProfileResponse : CommunicationResponse
{
    public string Username { get; set; }
    public string AvatarUrl { get; set; }
    public string ProfileUrl { get; set; }
    public SteamUserStatus Status { get; set; }
    public DateTime LastOnline { get; set; }
}