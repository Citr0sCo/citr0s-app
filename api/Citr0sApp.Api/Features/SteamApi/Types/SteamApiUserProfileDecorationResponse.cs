using Newtonsoft.Json;

namespace Citr0sApp.Api.Features.SteamApi.Types;

public class SteamApiUserProfileDecorationResponse
{
    [JsonProperty("response")]
    public InnerResponse Response { get; set; }
    
    public class AnimatedAvatar
    {
    }

    public class AvatarFrame
    {
        [JsonProperty("communityitemid")]
        public string Communityitemid { get; set; }

        [JsonProperty("image_small")]
        public string ImageSmall { get; set; }

        [JsonProperty("image_large")]
        public string ImageLarge { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("item_title")]
        public string ItemTitle { get; set; }

        [JsonProperty("item_description")]
        public string ItemDescription { get; set; }

        [JsonProperty("appid")]
        public int Appid { get; set; }

        [JsonProperty("item_type")]
        public int ItemType { get; set; }

        [JsonProperty("item_class")]
        public int ItemClass { get; set; }
    }

    public class MiniProfileBackground
    {
        [JsonProperty("communityitemid")]
        public string Communityitemid { get; set; }

        [JsonProperty("image_large")]
        public string ImageLarge { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("item_title")]
        public string ItemTitle { get; set; }

        [JsonProperty("item_description")]
        public string ItemDescription { get; set; }

        [JsonProperty("appid")]
        public int Appid { get; set; }

        [JsonProperty("item_type")]
        public int ItemType { get; set; }

        [JsonProperty("item_class")]
        public int ItemClass { get; set; }

        [JsonProperty("movie_webm")]
        public string MovieWebm { get; set; }

        [JsonProperty("movie_mp4")]
        public string MovieMp4 { get; set; }
    }

    public class ProfileBackground
    {
        [JsonProperty("communityitemid")]
        public string Communityitemid { get; set; }

        [JsonProperty("image_large")]
        public string ImageLarge { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("item_title")]
        public string ItemTitle { get; set; }

        [JsonProperty("item_description")]
        public string ItemDescription { get; set; }

        [JsonProperty("appid")]
        public int Appid { get; set; }

        [JsonProperty("item_type")]
        public int ItemType { get; set; }

        [JsonProperty("item_class")]
        public int ItemClass { get; set; }

        [JsonProperty("equipped_flags")]
        public int EquippedFlags { get; set; }
    }

    public class ProfileModifier
    {
    }

    public class InnerResponse
    {
        [JsonProperty("profile_background")]
        public ProfileBackground ProfileBackground { get; set; }

        [JsonProperty("mini_profile_background")]
        public MiniProfileBackground MiniProfileBackground { get; set; }

        [JsonProperty("avatar_frame")]
        public AvatarFrame AvatarFrame { get; set; }

        [JsonProperty("animated_avatar")]
        public AnimatedAvatar AnimatedAvatar { get; set; }

        [JsonProperty("profile_modifier")]
        public ProfileModifier ProfileModifier { get; set; }

        [JsonProperty("steam_deck_keyboard_skin")]
        public SteamDeckKeyboardSkin SteamDeckKeyboardSkin { get; set; }
    }

    public class SteamDeckKeyboardSkin
    {
    }
}