using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using PlayerRecordService.Shared.Models.PlayerRecord;

namespace PlayerRecordService.Implementations.Storage.Models
{
    internal class PlayerRecordRedisModel
    {
        [Required]
        [JsonPropertyName("playerId")]
        public string PlayerId { get; set; }
        /// <summary>Player name</summary>
        ///<example>string</example>
        [Required]
        [JsonPropertyName("playerName")]
        public string PlayerName { get; set; }
        /// <summary> Players group name</summary>
        /// <example>string</example>
        [Required]
        [JsonPropertyName("groupName")]
        public string GroupName { get; set; }
        /// <summary>Region of the player</summary>
        /// <example>AARHUS</example>
        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonPropertyName("region")]
        public Region Region { get; set; }
        /// <summary>Player position in cave</summary>
        ///<example>(0,4,1)</example>
        [Required]
        [JsonPropertyName("position")]
        [MinLength(7)]
        public string Position { get; set; }
        /// <summary>Player access token</summary>
        /// <example>string</example>
        [AllowNull]
        [JsonPropertyName("accessToken")]
        public string? AccessToken { get; set; }

    }
}
