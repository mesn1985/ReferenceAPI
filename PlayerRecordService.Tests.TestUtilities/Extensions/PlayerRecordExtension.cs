﻿using System.Text.Json;
using SkycavePlayerService.api.DTOs;
using SkycavePlayerService.Shared.Models.PlayerRecord;

namespace SkycavePlayerService.Tests.TestUtilities.Extensions
{
    public static class PlayerRecordExtension
    {
        public static StringContent AsStringContent(this PlayerRecord playerRecord)
        {
            string json = JsonSerializer.Serialize(playerRecord);
            return new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        }

        public static PlayerRecordModel AsModel(this PlayerRecord playerRecord)
        {
            return new PlayerRecordModel(
                playerRecord.PlayerId,
                playerRecord.PlayerName,
                playerRecord.GroupName,
                playerRecord.Region,
                playerRecord.Position,
                playerRecord.AccessToken
                
                );
        }

    }
    
}