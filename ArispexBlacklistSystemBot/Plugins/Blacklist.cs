using ArispexBlacklistSystemBot.Data;
using ArispexBlacklistSystemBot.Models;
using ArispexBlacklistSystemBot.Services;
using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Message;
using EleCho.GoCqHttpSdk.Post;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ArispexBlacklistSystemBot.Plugins;

public class Blacklist: CqPostPlugin
{
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly ILogger<Blacklist> _logger;
    private readonly BlacklistService _blacklistService;

    public Blacklist(IConfiguration configuration, ApplicationDbContext applicationDbContext, ILogger<Blacklist> logger, BlacklistService blacklistService)
    {
        _configuration = configuration;
        _applicationDbContext = applicationDbContext;
        _logger = logger;
        _blacklistService = blacklistService;
    }

    public override async Task OnGroupMessageReceivedAsync(CqGroupMessagePostContext context)
    {
        if (context.Session is not ICqActionSession actionSession) return;
        
        var text = context.Message.Text;
        var parameters = text.Split(' ');
        
        // 添加云黑
        if (text.StartsWith("添加云黑"))
        {
            if (!(_applicationDbContext.AuthorizedGroups.Select(x => x.GroupId)).ToList().Contains(context.GroupId.ToString()))
            {
                return;
            }

            if (!(_applicationDbContext.AuthorizedUsers.Select(x => x.Id)).ToList().Contains(context.UserId.ToString()))
            {
                await actionSession.SendGroupMessageAsync(context.GroupId, new CqMessage("该功能仅限授权用户使用。"));
                return;
            }
            if (parameters.Length != 4)
            {
                await actionSession.SendGroupMessageAsync(context.GroupId, new CqMessage("添加云黑 <QQ号> <原因> <备注（没有就写无）>"));
                return;
            }

            try
            {
                var result =
                    await _blacklistService.AddBlacklistEntryAsync(parameters[1], parameters[2], parameters[3]);
                if (result.Success)
                {
                    await actionSession.SendGroupMessageAsync(context.GroupId, new CqMessage("添加成功！"));
                    _logger.LogInformation($"{parameters[1]} 成功被添加至云黑\n原因：{parameters[2]}\n备注：{parameters[3]}");
                    return;
                }
                await actionSession.SendGroupMessageAsync(context.GroupId, new CqMessage(result.Msg));
                _logger.LogInformation($"{parameters[1]} 添加云黑失败\n失败原因：{result.Msg}");
                return;
            }
            catch (HttpRequestException)
            {
                await actionSession.SendGroupMessageAsync(context.GroupId, new CqMessage("无法连接至服务器"));
                return;
            }
        }
        
        // 删除云黑
        if (text.StartsWith("删除云黑"))
        {
            if (!(_applicationDbContext.AuthorizedGroups.Select(x => x.GroupId)).ToList().Contains(context.GroupId.ToString()))
            {
                return;
            }

            if (!(_applicationDbContext.AuthorizedUsers.Select(x => x.Id)).ToList().Contains(context.UserId.ToString()))
            {
                await actionSession.SendGroupMessageAsync(context.GroupId, new CqMessage("该功能仅限授权用户使用。"));
                return;
            }
            if (parameters.Length != 2)
            {
                await actionSession.SendGroupMessageAsync(context.GroupId, new CqMessage("删除云黑 <QQ号>"));
                return;
            }
            
            try
            {
                var result =
                    await _blacklistService.DeleteBlacklistEntryAsync(parameters[1]);
                if (result.Success)
                {
                    await actionSession.SendGroupMessageAsync(context.GroupId, new CqMessage("删除成功！"));
                    _logger.LogInformation($"{parameters[1]} 成功被删除云黑");
                    return;
                }
                await actionSession.SendGroupMessageAsync(context.GroupId, new CqMessage(result.Msg));
                _logger.LogInformation($"{parameters[1]} 删除云黑失败\n失败原因：{result.Msg}");
                return;
            }
            catch (HttpRequestException)
            {
                await actionSession.SendGroupMessageAsync(context.GroupId, new CqMessage("无法连接至服务器"));
                return;
            }
        }
        
        //云黑信息
        if (text.StartsWith("云黑信息"))
        {
            if (!(_applicationDbContext.AuthorizedGroups.Select(x => x.GroupId)).ToList().Contains(context.GroupId.ToString()))
            {
                return;
            }
            
            if (parameters.Length != 2)
            {
                await actionSession.SendGroupMessageAsync(context.GroupId, new CqMessage("删除云黑 <QQ号>"));
                return;
            }
            
            try
            {
                var result =
                    await _blacklistService.GetBlacklistEntryInfoAsync(parameters[1]);
                if (result.Success)
                {
                    await actionSession.SendGroupMessageAsync(context.GroupId, new CqMessage($"QQ：{result.Data.QQ}\n原因：{result.Data.Reason}\n备注：{result.Data.Note}" +
                        $"\n创建时间：{result.Data.CreateTime}\n更新时间：{result.Data.UpdateTime}"));
                    return;
                }
                await actionSession.SendGroupMessageAsync(context.GroupId, new CqMessage(result.Msg));
                return;
            }
            catch (HttpRequestException)
            {
                await actionSession.SendGroupMessageAsync(context.GroupId, new CqMessage("无法连接至服务器"));
                return;
            }
        }
    }

    public override async void OnGroupMemberIncreased(CqGroupMemberIncreasedPostContext context)
    {
        if (context.Session is not ICqActionSession actionSession) return;
        if (!_applicationDbContext.AuthorizedGroups.Select(x => x.GroupId).ToList().Contains(context.GroupId.ToString())) return;

        if (!_configuration.GetValue<bool>("AutoCheck"))
        {
            return;
        }
        try
        {
            var result = await _blacklistService.GetBlacklistEntryInfoAsync(context.UserId.ToString());
            if (result.Success)
            {
                await actionSession.SendGroupMessageAsync(context.GroupId,
                    new CqMessage("检测成功，", new CqAtMsg(context.UserId), $"{context.UserId} 在云黑列表中"));
                if (!_configuration.GetValue<bool>("AutoKick")) return;
                await actionSession.KickGroupMemberAsync(context.GroupId, context.UserId, true);
                await actionSession.SendGroupMessageAsync(context.GroupId, new CqMessage("已自动踢出，如未自动踢出请检查是否设置为管理员"));
                return;
            }
            else
            {
                await actionSession.SendGroupMessageAsync(context.GroupId,
                    new CqMessage("检测成功，", new CqAtMsg(context.UserId), $"{context.UserId} 不在云黑列表中"));
                return;
            }
        }
        catch (HttpRequestException)
        {
            await actionSession.SendGroupMessageAsync(context.GroupId, new CqMessage("无法连接至服务器"));
            return;
        } 
    }
}