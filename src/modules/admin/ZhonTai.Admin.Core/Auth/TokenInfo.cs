﻿namespace ZhonTai.Admin.Core.Auth;

/// <summary>
/// 令牌信息
/// </summary>
public class TokenInfo
{
    private string _accessToken;

    /// <summary>
    /// 访问令牌
    /// </summary>
    public string AccessToken
    {
        get => _accessToken;
        set
        {
            _accessToken = value;
        }
    }

    /// <summary>
    /// 访问令牌的过期时间
    /// </summary>
    public DateTime AccessTokenExpiresAt { get; set; }

    /// <summary>
    /// 访问令牌的生命周期（以秒为单位）
    /// </summary>
    public int AccessTokenLifeTime { get; set; }

    /// <summary>
    /// 刷新令牌
    /// </summary>
    public string RefreshToken { get; set; }

    /// <summary>
    /// 刷新令牌的过期时间
    /// </summary>
    public DateTime RefreshTokenExpiresAt { get; set; }

    /// <summary>
    /// 刷新令牌的生命周期（以秒为单位）
    /// </summary>
    public int RefreshTokenLifeTime { get; set; }

    /// <summary>
    /// 创建令牌信息时间戳
    /// </summary>
    public long Timestamp { get; set; }
}
