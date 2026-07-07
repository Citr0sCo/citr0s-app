using Citr0sApp.Api.Features.SteamApi.Types;

namespace Citr0sApp.Api.Features.SteamApi;

public class SteamApiCache
{
    private readonly int _cacheIntervalInMinutes = 1;
    
    private SteamUserProfileResponse? _cachedSteamUserProfileResponse = null;
    private SteamUserProfileDecorationResponse? _cachedSteamUserProfileDecorationResponse = null;
    private SteamUserRecentlyPlayedResponse? _cachedSteamUserRecentlyPlayedResponse = null;
    private DateTime _cachedSteamUserProfileResponseLastLiveDataHit = DateTime.MinValue;
    private DateTime _cachedSteamUserProfileDecorationResponseLastLiveDataHit = DateTime.MinValue;
    private DateTime _cachedSteamUserRecentlyPlayedResponseLastLiveDataHit = DateTime.MinValue;
    
    private readonly SteamApiService _service;
    private static SteamApiCache? _instance = null;

    private SteamApiCache(SteamApiService service)
    {
        _service = service;
    }

    public static SteamApiCache Instance(IHttpClientFactory factory)
    {
        if (_instance == null)
            _instance = new SteamApiCache(new SteamApiService(factory));

        return _instance;
    }

    public async Task<SteamUserProfileResponse> GetUserProfile(string steamId)
    {
        if (_cachedSteamUserProfileResponse != null && _cachedSteamUserProfileResponseLastLiveDataHit.AddMinutes(_cacheIntervalInMinutes) > DateTime.Now)
            return await Task.FromResult(_cachedSteamUserProfileResponse);
        
        var response = await _service.GetUserProfile(steamId);

        _cachedSteamUserProfileResponseLastLiveDataHit = DateTime.Now;
        _cachedSteamUserProfileResponse = response;

        return await Task.FromResult(_cachedSteamUserProfileResponse);
    }

    public async Task<SteamUserProfileDecorationResponse> GetUserProfileDecoration(string steamId)
    {
        if (_cachedSteamUserProfileDecorationResponse != null && _cachedSteamUserProfileDecorationResponseLastLiveDataHit.AddMinutes(_cacheIntervalInMinutes) > DateTime.Now)
            return await Task.FromResult(_cachedSteamUserProfileDecorationResponse);
        
        var response = await _service.GetUserProfileDecoration(steamId);

        _cachedSteamUserProfileDecorationResponseLastLiveDataHit = DateTime.Now;
        _cachedSteamUserProfileDecorationResponse = response;

        return await Task.FromResult(_cachedSteamUserProfileDecorationResponse);
    }

    public async Task<SteamUserRecentlyPlayedResponse> GetRecentlyPlayed(string steamId)
    {
        if (_cachedSteamUserRecentlyPlayedResponse != null && _cachedSteamUserRecentlyPlayedResponseLastLiveDataHit.AddMinutes(_cacheIntervalInMinutes) > DateTime.Now)
            return await Task.FromResult(_cachedSteamUserRecentlyPlayedResponse);
        
        var response = await _service.GetRecentlyPlayed(steamId);
       
        _cachedSteamUserRecentlyPlayedResponseLastLiveDataHit = DateTime.Now;
        _cachedSteamUserRecentlyPlayedResponse = response;

        return await Task.FromResult(_cachedSteamUserRecentlyPlayedResponse);
    }
}