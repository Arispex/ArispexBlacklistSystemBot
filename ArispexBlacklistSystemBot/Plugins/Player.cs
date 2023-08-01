using ArispexBlacklistSystemBot.Data;
using ArispexBlacklistSystemBot.Models;
using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Message;
using EleCho.GoCqHttpSdk.Post;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ArispexBlacklistSystemBot.Plugins;

public class Player : CqPostPlugin
{
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly ILogger<Player> _logger;

    public Player(IConfiguration configuration, ApplicationDbContext applicationDbContext, ILogger<Player> logger)
    {
        _configuration = configuration;
        _applicationDbContext = applicationDbContext;
        _logger = logger;
    }

    public override async Task OnPrivateMessageReceivedAsync(CqPrivateMessagePostContext context)
    {
        if (context.Session is not ICqActionSession actionSession) return;

        var text = context.Message.Text;
        var parameters = text.Split(' ');
        // 授权用户
        if (text.StartsWith("授权用户"))
        {
            if (parameters.Length != 2)
            {
                await actionSession.SendPrivateMessageAsync(context.UserId, new CqMessage("授权用户 <QQ号>"));
                return;
            }

            if (context.UserId == long.Parse(_configuration["OwnerId"]))
            {
                if (_applicationDbContext.AuthorizedUsers.Any(x => x.Id == parameters[1]))
                {
                    await actionSession.SendPrivateMessageAsync(context.UserId, new CqMessage("该用户已经授权过了。"));
                    return;
                }

                _applicationDbContext.AuthorizedUsers.Add(new AuthorizedUser()
                {
                    Id = parameters[1]
                });
                await _applicationDbContext.SaveChangesAsync();
                _logger.LogInformation("已授权用户 " + parameters[1]);
                await actionSession.SendPrivateMessageAsync(context.UserId, new CqMessage("授权成功！"));
                return;
            }

            await actionSession.SendPrivateMessageAsync(context.UserId, new CqMessage("该功能仅限主人可用。"));
            return;
        }

        // 取消授权用户
        if (text.StartsWith("取消授权用户"))
        {
            if (parameters.Length != 2)
            {
                await actionSession.SendPrivateMessageAsync(context.UserId, new CqMessage("取消授权用户 <QQ号>"));
                return;
            }

            if (context.UserId == long.Parse(_configuration["OwnerId"]))
            {
                if (!_applicationDbContext.AuthorizedUsers.Any(x => x.Id == parameters[1]))
                {
                    await actionSession.SendPrivateMessageAsync(context.UserId, new CqMessage("该用户还没有被授权。"));
                    return;
                }

                _applicationDbContext.AuthorizedUsers.Remove(
                    _applicationDbContext.AuthorizedUsers.FirstOrDefault(x => x.Id == parameters[1]));
                await _applicationDbContext.SaveChangesAsync();
                _logger.LogInformation("已取消授权用户 " + parameters[1]);
                await actionSession.SendPrivateMessageAsync(context.UserId, new CqMessage("取消成功！"));
                return;
            }

            await actionSession.SendPrivateMessageAsync(context.UserId, new CqMessage("该功能仅限主人可用。"));
            return;
        }
    }
}