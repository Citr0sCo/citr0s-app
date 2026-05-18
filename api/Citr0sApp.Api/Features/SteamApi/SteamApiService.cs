using Citr0sApp.Api.Core.Types;
using Citr0sApp.Api.Features.SteamApi.Types;
using Newtonsoft.Json;

namespace Citr0sApp.Api.Features.SteamApi;

public class SteamApiService
{
    private readonly string? _steamApiKey = Environment.GetEnvironmentVariable("ASPNETCORE_STEAM_API_KEY");
    private readonly string _steamApiBaseUrl = "https://api.steampowered.com";
    private readonly HttpClient _httpClient;

    public SteamApiService(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient();
    }
    
    public async Task<SteamUserProfileResponse> GetUserProfile(string steamId)
    {
        var apiRequest = await _httpClient.GetAsync($"{_steamApiBaseUrl}/ISteamUser/GetPlayerSummaries/v0002/?key={_steamApiKey}&steamids={steamId}").ConfigureAwait(false);
        
        var apiResponse = await apiRequest.Content.ReadAsStringAsync();
        
        var parsedResponse = JsonConvert.DeserializeObject<SteamApiUserProfileResponse>(apiResponse);

        var response = new SteamUserProfileResponse();
        
        if (parsedResponse?.Response.Players.Count == 0)
        {
            response.AddError(new Error
            {
                Code = ErrorCode.ThirdPartyApiError,
                UserMessage = "No players found for steamId " + steamId,
                TechnicalMessage = "No players found for steamId " + steamId
            });
            return response;
        }

        var user = parsedResponse!.Response.Players.First();

        return new SteamUserProfileResponse
        {
            Username =  user.PersonaName,
            AvatarUrl = user.Avatarfull,
            ProfileUrl = user.Profileurl,
            Status = (SteamUserStatus)user.PersonaState,
            LastOnline = DateTimeOffset.FromUnixTimeSeconds(user.Lastlogoff).DateTime
        };
    }

    public async Task<SteamUserProfileDecorationResponse> GetUserProfileDecoration(string steamId)
    {
        var apiRequest = await _httpClient.GetAsync($"{_steamApiBaseUrl}/IPlayerService/GetProfileItemsEquipped/v1/?key={_steamApiKey}&steamid={steamId}").ConfigureAwait(false);
        
        var apiResponse = await apiRequest.Content.ReadAsStringAsync();
        
        var parsedResponse = JsonConvert.DeserializeObject<SteamApiUserProfileDecorationResponse>(apiResponse);

        return new SteamUserProfileDecorationResponse
        {
            AvatarBorder = $"https://shared.akamai.steamstatic.com/community_assets/images/{parsedResponse!.Response.AvatarFrame.ImageSmall}",
            ProfileBackground = $"https://shared.akamai.steamstatic.com/community_assets/images/{parsedResponse!.Response.MiniProfileBackground.MovieMp4}"
        };
    }

    public async Task<SteamUserRecentlyPlayedResponse> GetRecentlyPlayed(string steamId)
    {
        var apiRequest = await _httpClient.GetAsync($"{_steamApiBaseUrl}/IPlayerService/GetRecentlyPlayedGames/v0001/?key={_steamApiKey}&steamid={steamId}&format=json").ConfigureAwait(false);
        
        var apiResponse = await apiRequest.Content.ReadAsStringAsync();
        
        var parsedResponse = JsonConvert.DeserializeObject<SteamApiUserRecentlyPlayedResponse>(apiResponse);

        var response = new SteamUserRecentlyPlayedResponse();
        
        if (parsedResponse?.Response.Games.Count == 0)
        {
            response.AddError(new Error
            {
                Code = ErrorCode.ThirdPartyApiError,
                UserMessage = "No gmaes found for steamId " + steamId,
                TechnicalMessage = "No games found for steamId " + steamId
            });
            return response;
        }

        return new SteamUserRecentlyPlayedResponse
        {
            Total = parsedResponse!.Response.TotalCount,
            Games = parsedResponse!.Response.Games.ConvertAll(game => new SteamGameSummary
            {
                AppId = game.Appid,
                Name = game.Name,
                IconUrl = $"https://media.steampowered.com/steamcommunity/public/images/apps/{game.Appid}/{game.ImgIconUrl}.jpg",
                PlaytimeLastTwoWeeksInMinutes = game.Playtime2weeks,
                PlaytimeForeverInMinutes = game.PlaytimeForever
            })
        };
    }
}