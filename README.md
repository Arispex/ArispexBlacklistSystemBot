# ArispexBlacklistSystemBot
专为 [Arispex云黑系统](https://arispex.qianyiovo.com/) 编写的 QQ 机器人

## 部署

### go-cqhttp 要求

+ 连接服务：正向
+ 监听地址（默认）：127.0.0.2:8080

### 修改配置文件

修改配置文件 `appsettings.json`

```json
{
  "OwnerId": "",  // 主人QQ
  "goCqHttp": {
    "BaseUrl": "ws://127.0.0.1:8080",
    "AccessToken": ""
  },
  "Key": "",  // Arispex云黑系统的 Key
  "ServerBaseUrl": "https://arispex.qianyiovo.com",
  "AutoCheck": true,  // 是否启动入群自动检测
  "AutoKick": true  // 是否入群自动检测后自动踢出
}
```

## 第一次使用

1. 使用 `授权群` 命令授权某个群，被授权的群就可以使用云黑相关功能。
2. 使用 `授权用户` 命令授权某个用户，被授权的用户即可在被授权的群内使用云黑相关功能。
3. 使用 `菜单` 命令可查看所有可用的功能。

~~聪明的你一定会用了吧~~