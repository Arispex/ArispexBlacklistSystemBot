using ArispexBlacklistSystemBot.Data;
using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Message;
using EleCho.GoCqHttpSdk.Post;
using Microsoft.Extensions.Configuration;

namespace ArispexBlacklistSystemBot.Plugins;

public class Misc: CqPostPlugin
{
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _applicationDbContext;

    public Misc(IConfiguration configuration, ApplicationDbContext applicationDbContext)
    {
        _configuration = configuration;
        _applicationDbContext = applicationDbContext;
    }

    public override async Task OnPrivateMessageReceivedAsync(CqPrivateMessagePostContext context)
    {
        if (context.Session is not ICqActionSession actionSession) return;

        if (context.Message.Text.StartsWith("菜单"))
        {
            if (context.UserId == long.Parse(_configuration["OwnerId"]))
            {
                await actionSession.SendPrivateMessageAsync(context.UserId, new CqMessage("授权群\n取消授权群\n授权用户\n取消授权用户"));
                return;
            }
            await actionSession.SendPrivateMessageAsync(context.UserId, new CqMessage("该功能仅限主人可用。"));
            return;
        }
    }

    public override async Task OnGroupMessageReceivedAsync(CqGroupMessagePostContext context)
    {
        if (context.Session is not ICqActionSession actionSession) return;
        if (!_applicationDbContext.AuthorizedGroups.Select(x => x.GroupId).ToList().Contains(context.GroupId.ToString())) return;
        if (context.Message.Text.StartsWith("菜单"))
        {
            await actionSession.SendGroupMessageAsync(context.GroupId,
                new CqMessage("添加云黑\n删除云黑\n云黑信息"));
            return;
        }
    }
}