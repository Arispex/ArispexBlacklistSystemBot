using System.ComponentModel.DataAnnotations;

namespace ArispexBlacklistSystemBot.Models;

public class AuthorizedGroup
{
    [Key]
    public string GroupId { get; set; }
    public DateTime AuthorizedTime { get; set; } = DateTime.Now;
}