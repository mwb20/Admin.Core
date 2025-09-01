﻿using Microsoft.Extensions.DependencyInjection;
using ZhonTai.Admin.Core.Configs;

namespace ZhonTai.Admin.Core.Extensions;

/// <summary>
/// Im扩展
/// </summary>
public static class ImExtensions
{
    /// <summary>
    /// 添加Im
    /// </summary>
    /// <param name="services"></param>
    public static void AddIm(this IServiceCollection services)
    {
        var imConfig = AppInfo.GetOptions<ImConfig>();

        ImHelper.Initialization(new ImClientOptions
        {
            Redis = new FreeRedis.RedisClient(imConfig.RedisConnectionString),
            Servers = imConfig.Servers,
        });

        ImHelper.Instance.OnSend += (s, e) =>
        {
            //Console.WriteLine($"ImClient.SendMessage(server={e.Server},data={JsonHelper.Serialize(e.Message)})");
        };

        ImHelper.EventBus(
            t =>
            {
                //Console.WriteLine(t.clientId + "上线了");
            },
            t =>
            {
                //Console.WriteLine(t.clientId + "下线了");
            }
        );
    }
}
