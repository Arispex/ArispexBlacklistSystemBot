using ArispexBlacklistSystemBot.Data;
using ArispexBlacklistSystemBot.Plugins;
using ArispexBlacklistSystemBot.Services;
using EleCho.GoCqHttpSdk;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var serviceCollection = new ServiceCollection();

serviceCollection.AddDbContext<ApplicationDbContext>();
serviceCollection.AddLogging(builder => builder.AddConsole());
serviceCollection.AddScoped<BlacklistService>();
serviceCollection.AddScoped<Group>();
serviceCollection.AddScoped<Player>();
serviceCollection.AddSingleton<Blacklist>();
serviceCollection.AddSingleton<Misc>();

var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
serviceCollection.AddSingleton<IConfiguration>(builder);

var serviceProvider = serviceCollection.BuildServiceProvider();

var group = serviceProvider.GetService<Group>();
var player = serviceProvider.GetService<Player>();
var blacklist = serviceProvider.GetService<Blacklist>();
var blacklistService = serviceProvider.GetService<BlacklistService>();
var misc = serviceProvider.GetService<Misc>();

var logger = serviceProvider.GetService<ILogger<Program>>();
var context = serviceProvider.GetService<ApplicationDbContext>();
var configuration = serviceProvider.GetService<IConfiguration>();

if (context.Database.EnsureCreated())
{
    logger.LogWarning("检测到数据库不存在，已自动创建");
}

logger.LogInformation("正在验证 Key");
try
{
    if (await blacklistService.TestKeyAsync())
    {
        logger.LogInformation("验证成功");
    }
    else
    {
        logger.LogWarning($"验证失败，无效的 Key。请在浏览器打开官网 {configuration["ServerBaseUrl"]} 后注册登入在个人资料中找到 Key，然后填写至 appsettings.json 中的 Key 值后重新启动程序。");
        logger.LogWarning("如有疑问，请加入QQ群 887996277");
        logger.LogWarning("按回车退出...");
        Console.ReadLine();
        Environment.Exit(1);
    }
}
catch (HttpRequestException)
{
    logger.LogWarning("无法连接至服务器，请检查网络。如有疑问，请加入QQ群 887996277");
    logger.LogWarning("按回车退出...");
    Console.ReadLine();
    Environment.Exit(1);
}

logger.LogInformation("正在初始化...");

var session = new CqWsSession(new CqWsSessionOptions(){
    BaseUri = new Uri(configuration["goCqHttp:BaseUrl"]),
    AccessToken = configuration["goCqHttp:AccessToken"],
});

logger.LogInformation("初始化完成");

logger.LogInformation("正在加载插件");

session.UsePlugin(group);
session.UsePlugin(player);
session.UsePlugin(blacklist);
session.UsePlugin(misc);

logger.LogInformation("插件加载完成");

logger.LogInformation($"监听地址：{configuration["goCqHttp:BaseUrl"]}");

logger.LogWarning("等待连接...");

while (!session.IsConnected)
{
    logger.LogWarning("等待连接...");
    await Task.Delay(1000);
    try
    {
        await session.StartAsync();
    }
    catch (Exception e)
    {
        logger.LogWarning("未检测到连接");
    }
}

logger.LogInformation("服务已启动");

logger.LogInformation("go-cqhttp 已连接");
await session.WaitForShutdownAsync();