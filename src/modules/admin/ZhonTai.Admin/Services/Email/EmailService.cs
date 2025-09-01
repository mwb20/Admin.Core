﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using DotNetCore.CAP;
using MimeKit;
using MailKit.Net.Smtp;
using ZhonTai.Admin.Core.Configs;
using ZhonTai.Admin.Core.Consts;
using ZhonTai.Admin.Services.Email.Events;
using ZhonTai.Admin.Resources;

namespace ZhonTai.Admin.Services.Email;

public class EmailService: ICapSubscribe
{
    private readonly EmailConfig _emailConfig;
    private readonly AdminLocalizer _adminLocalizer;

    public EmailService(IOptions<EmailConfig> emailConfig, AdminLocalizer adminLocalizer)
    {
        _emailConfig = emailConfig.Value;
        _adminLocalizer = adminLocalizer;
    }

    /// <summary>
    /// 邮件单发
    /// </summary>
    /// <param name="event"></param>
    /// <returns></returns>
    [NonAction]
    [CapSubscribe(SubscribeNames.EmailSingleSend)]
    public async Task SingleSendAsync(EmailSingleSendEvent @event)
    {
        var emailConfig = _emailConfig;

        var builder = new BodyBuilder()
        {
            HtmlBody = @event.Body
        };
        var message = new MimeMessage()
        {
            Subject = @event.Subject,
            Body = builder.ToMessageBody()
        };

        var fromEmailName = @event.FromEmail!=null && @event.FromEmail.Name.NotNull() ? @event.FromEmail.Name : emailConfig.FromEmail.Name;
        var fromEmailAddress = @event.FromEmail != null && @event.FromEmail.Address.NotNull() ? @event.FromEmail.Address : emailConfig.FromEmail.Address;
        message.From.Add(new MailboxAddress(fromEmailName, fromEmailAddress));
        message.To.Add(new MailboxAddress(@event.ToEmail.Name, @event.ToEmail.Address));

        using var client = new SmtpClient();
        await client.ConnectAsync(emailConfig.Host, emailConfig.Port, emailConfig.UseSsl);
        // 检查是否需要身份验证
        var hasAuthentication = client.Capabilities.HasFlag(SmtpCapabilities.Authentication);
        if (hasAuthentication)
        {
            await client.AuthenticateAsync(emailConfig.UserName, emailConfig.Password);
        }
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    /// <summary>
    /// 发送邮箱验证码
    /// </summary>
    /// <param name="event"></param>
    /// <returns></returns>
    [NonAction]
    [CapSubscribe(SubscribeNames.EmailSendCode)]
    public async Task SendCodeAsync(EmailSendCodeEvent @event)
    {
        await SingleSendAsync(new EmailSingleSendEvent
        {
            ToEmail = new EmailSingleSendEvent.Models.EmailModel
            {
                Address = @event.ToEmail.Address,
            },
            Subject = _adminLocalizer["邮箱验证码"],
            Body = _adminLocalizer["<p>你正在进行邮箱登录操作</p><p>邮箱验证码: {0}，有效期5分钟</p><p>如非本人操作，请忽略。</p>", @event.Code]
        });
    }
}
