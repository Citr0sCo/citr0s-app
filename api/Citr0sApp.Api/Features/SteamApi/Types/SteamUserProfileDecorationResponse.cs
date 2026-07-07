using Citr0sApp.Api.Core.Types;

namespace Citr0sApp.Api.Features.SteamApi.Types;

public class SteamUserProfileDecorationResponse : CommunicationResponse
{
    public string AvatarBorder { get; set; }
    public string ProfileBackground { get; set; }
}