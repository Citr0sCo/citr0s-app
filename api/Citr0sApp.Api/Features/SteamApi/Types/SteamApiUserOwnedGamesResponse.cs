using Newtonsoft.Json;

namespace Citr0sApp.Api.Features.SteamApi.Types;

public class SteamApiUserOwnedGamesResponse
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

        [JsonProperty("rtime_last_played")]
        public long RtimeLastPlayed { get; set; }
    }

    public class InnerResponse
    {
        [JsonProperty("game_count")]
        public int GameCount { get; set; }

        [JsonProperty("games")]
        public List<Game> Games { get; set; }
    }
}
