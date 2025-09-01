﻿using Microsoft.AspNetCore.Mvc.Filters;

namespace ZhonTai.Admin.Core.Handlers;

/// <summary>
/// 自定义权限处理接口
/// </summary>
public interface ICustomPermissionHandler
{
    /// <summary>
    /// 权限验证
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    Task<bool> ValidateAsync(AuthorizationFilterContext context);
}