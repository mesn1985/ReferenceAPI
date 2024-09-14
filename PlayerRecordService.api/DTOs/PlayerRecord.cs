
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using SkycavePlayerService.api.Infrastructure.Attributes;
using SkycavePlayerService.Shared.Models.PlayerRecord;

namespace SkycavePlayerService.api.DTOs
{
   /// <summary>
   /// is the dto type, used for representing player record
   /// in inbound requests, our outbound response.
   /// Generally this class should only be used within the
   /// controller class, because is a dto, and no other layers
   /// of the api implementation should have dependencies towards  it.
   /// @author: team india
   /// </summary>
    public class PlayerRecord
    {
        /// <summary>Unique id of the player</summary>
        ///<example>string</example>
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
        [Position( ErrorMessage = "Position must be in the format of (d,d,d) E.g. (0,2,4)")]
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
