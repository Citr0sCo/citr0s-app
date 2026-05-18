using Newtonsoft.Json;

namespace Citr0sApp.Api.Features.SteamApi.Types;

public class SteamApiUserRecentlyPlayedResponse
{
    [JsonProperty("response")]
    public InnerResponse Response { get; set; }
    
    public class Game
    {
        [JsonProperty("appid")]
        public int Appid { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("playtime_2weeks")]
        public int Playtime2weeks { get; set; }

        [JsonProperty("playtime_forever")]
        public int PlaytimeForever { get; set; }

        [JsonProperty("img_icon_url")]
        public string ImgIconUrl { get; set; }

        [JsonProperty("playtime_windows_forever")]
        public int PlaytimeWindowsForever { get; set; }

        [JsonProperty("playtime_mac_forever")]
        public int PlaytimeMacForever { get; set; }

        [JsonProperty("playtime_linux_forever")]
        public int PlaytimeLinuxForever { get; set; }

        [JsonProperty("playtime_deck_forever")]
        public int PlaytimeDeckForever { get; set; }
    }

    public class InnerResponse
    {
        [JsonProperty("total_count")]
        public int TotalCount { get; set; }

        [JsonProperty("games")]
        public List<Game> Games { get; set; }
    }
}