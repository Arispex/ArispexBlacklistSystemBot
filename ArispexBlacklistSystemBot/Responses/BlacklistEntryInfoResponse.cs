using System.Text.Json.Serialization;
using ArispexBlacklistSystemBot.Models;

namespace ArispexBlacklistSystemBot.Responses;

public class BlacklistEntryInfoResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }
    
    [JsonPropertyName("msg")]
    public string Msg { get; set; }
    
    [JsonPropertyName("data")]
    public BlacklistEntry? Data { get; set; }
}