namespace ArispexBlacklistSystemBot.Models;

public class AuthorizedUser
{
    public string Id { get; set; }
    public DateTime AuthorizationTime { get; set; } = DateTime.Now;
}