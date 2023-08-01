using System.Text.Json.Serialization;

namespace ArispexBlacklistSystemBot.Models;

public class BlacklistEntry
{
    [JsonPropertyName("userId")]
    public int UserId { get; set; }
    
    [JsonPropertyName("qq")]
    public string QQ { get; set; }
    
    [JsonPropertyName("reason")]
    public string Reason { get; set; }
    
    [JsonPropertyName("note")]
    public string Note { get; set; }
    
    [JsonPropertyName("createTime")]
    public DateTime CreateTime { get; set; }
    
    [JsonPropertyName("updateTime")]
    public DateTime UpdateTime { get; set; }
}