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

    public async Task<SteamUserOwnedGameStatsResponse> GetOwnedGameStats(string steamId)
    {
        var apiRequest = await _httpClient.GetAsync($"{_steamApiBaseUrl}/IPlayerService/GetOwnedGames/v0001/?key={_steamApiKey}&steamid={steamId}&include_played_free_games=1&format=json").ConfigureAwait(false);
        var apiResponse = await apiRequest.Content.ReadAsStringAsync();
        var parsedResponse = JsonConvert.DeserializeObject<SteamApiUserOwnedGamesResponse>(apiResponse);
        if (parsedResponse?.Response?.Games == null)
        {
            var response = new SteamUserOwnedGameStatsResponse();
            response.AddError(new Error
            {
                Code = ErrorCode.ThirdPartyApiError,
                UserMessage = "Steam library stats are unavailable for steamId " + steamId,
                TechnicalMessage = "Steam owned games were not returned for steamId " + steamId
            });
            return response;
        }

        var games = parsedResponse.Response.Games;
        var totalOwned = games.Count;
        var played = games.Count(game => game.PlaytimeForever > 0);
        var recentlyActive = games.Count(game => game.Playtime2weeks > 0);
        var neverPlayed = totalOwned - played;

        return new SteamUserOwnedGameStatsResponse
        {
            TotalOwned = totalOwned,
            Played = played,
            NeverPlayed = neverPlayed,
            RecentlyActive = recentlyActive,
            PlayedPercentage = GetPercentage(played, totalOwned),
            NeverPlayedPercentage = GetPercentage(neverPlayed, totalOwned)
        };
    }

    private static double GetPercentage(int count, int total)
    {
        return total == 0 ? 0 : Math.Round(count * 100d / total, 1);
    }

    public async Task<SteamUserRecentlyPlayedResponse> GetRecentlyPlayed(string steamId)
    {
        var apiRequest = await _httpClient.GetAsync($"{_steamApiBaseUrl}/IPlayerService/GetOwnedGames/v0001/?key={_steamApiKey}&steamid={steamId}&include_appinfo=1&include_played_free_games=1&format=json").ConfigureAwait(false);

        var apiResponse = await apiRequest.Content.ReadAsStringAsync();
        var parsedResponse = JsonConvert.DeserializeObject<SteamApiUserOwnedGamesResponse>(apiResponse);
        var recentlyPlayedGames = parsedResponse?.Response?.Games?
            .Where(game => game.Playtime2weeks > 0 && game.RtimeLastPlayed > 0)
            .OrderByDescending(game => game.RtimeLastPlayed)
            .Take(5)
            .ToList() ?? new List<SteamApiUserOwnedGamesResponse.Game>();

        if (recentlyPlayedGames.Count == 0)
        {
            var response = new SteamUserRecentlyPlayedResponse();
            response.AddError(new Error
            {
                Code = ErrorCode.ThirdPartyApiError,
                UserMessage = "No games found for steamId " + steamId,
                TechnicalMessage = "No recently played games found for steamId " + steamId
            });
            return response;
        }

        return new SteamUserRecentlyPlayedResponse
        {
            Total = recentlyPlayedGames.Count,
            Games = recentlyPlayedGames.ConvertAll(game => new SteamGameSummary
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