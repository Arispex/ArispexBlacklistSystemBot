using System.Text.Json.Serialization;

namespace ArispexBlacklistSystemBot.Responses;

public class DeleteBlacklistEntryResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }
    
    [JsonPropertyName("msg")]
    public string Msg { get; set; }
}