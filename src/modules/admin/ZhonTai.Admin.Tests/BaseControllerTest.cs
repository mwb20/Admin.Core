﻿using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using ZhonTai.Admin.Tools.Cache;
using ZhonTai.Admin.Core;
using ZhonTai.Admin.Core.Configs;
using ZhonTai.Admin.Core.Dto;
using ZhonTai.Admin.Core.Enums;
using ZhonTai.Admin.Services.Auth;
using ZhonTai.Admin.Services.Auth.Dto;
using ZhonTai.Common.Helpers;

namespace ZhonTai.Admin.Tests;

public class BaseControllerTest : BaseTest
{
    private readonly ICacheTool _cache;
    private readonly AppConfig _appConfig;

    protected BaseControllerTest()
    {
        _cache = GetService<ICacheTool>();
        _appConfig = GetService<AppConfig>();
    }

    public static HttpContent GetHttpContent(object input, string contentType = "application/json;charset=UTF-8", ContentTypeEnum contentTypeEnum = ContentTypeEnum.Json)
    {
        // HttpContent httpContent = new StringContent(JsonHelper.Serialize(input));
        var content = Encoding.UTF8.GetBytes(JsonHelper.Serialize(input));
        HttpContent httpContent;
        if (contentTypeEnum == ContentTypeEnum.FormData)
        {
            httpContent = new FormUrlEncodedContent(JsonHelper.Deserialize<Dictionary<string, string>>(JsonHelper.Serialize(input)));
        }
        else
        {
            httpContent = new ByteArrayContent(content);
        }
        httpContent.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
        return httpContent;
    }

    public static HttpContent GetHttpContent(object input, ContentTypeEnum contentTypeEnum)
    {
        var contentType = contentTypeEnum switch
        {
            ContentTypeEnum.Json => "application/json;charset=UTF-8",
            ContentTypeEnum.FormData => "application/x-www-form-urlencoded;charset=UTF-8",
            _ => string.Empty
        };
        return GetHttpContent(input, contentType, contentTypeEnum);
    }

    public async Task<T> GetResult<T>(string apiPath, object input = null, bool checkStatus = true)
    {
        await Login();
        if (input != null)
        {
            var queryParams = ToParams(input);
            apiPath = apiPath.IndexOf('?') > -1 ? $"{apiPath}&{queryParams}" : $"{apiPath}?{queryParams}";
        }
        var res = await Client.GetAsync(apiPath);
        if (checkStatus)
        {
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        }
        var content = await res.Content.ReadAsStringAsync();
        return content.NotNull() ? JsonHelper.Deserialize<T>(content) : default;
    }

    public async Task<T> PostResult<T>(string apiPath, object input = null, bool checkStatus = true, string contentType = "application/json;charset=UTF-8")
    {
        await Login();
        var res = await Client.PostAsync(apiPath, GetHttpContent(input, contentType));
        if (checkStatus)
        {
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        }
        var content = await res.Content.ReadAsStringAsync();
        return content.NotNull() ? JsonHelper.Deserialize<T>(content) : default;
    }

    public async Task<string> PostResultAndGetContent(string apiPath, object input = null, bool checkStatus = true, string contentType = "application/json;charset=UTF-8")
    {
        //application/json;charset=UTF-8
        //application/x-www-form-urlencoded;charset=UTF-8
        await Login();
        var res = await Client.PostAsync(apiPath, GetHttpContent(input, contentType));
        if (checkStatus)
        {
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        }
        var content = await res.Content.ReadAsStringAsync();
        return content;
    }

    public async Task<T> PutResult<T>(string apiPath, object input = null, bool checkStatus = true, string contentType = "application/json;charset=UTF-8")
    {
        await Login();
        var res = await Client.PutAsync(apiPath, GetHttpContent(input, contentType));
        if (checkStatus)
        {
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        }
        var content = await res.Content.ReadAsStringAsync();
        return content.NotNull() ? JsonHelper.Deserialize<T>(content) : default;
    }

    public async Task<T> DeleteResult<T>(string apiPath, object input = null, bool checkStatus = true)
    {
        await Login();
        if (input != null)
        {
            var queryParams = ToParams(input);
            apiPath = apiPath.IndexOf('?') > -1 ? $"{apiPath}&{queryParams}" : $"{apiPath}?{queryParams}";
        }
        var res = await Client.DeleteAsync(apiPath);
        if (checkStatus)
        {
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        }
        var content = await res.Content.ReadAsStringAsync();
        return content.NotNull() ? JsonHelper.Deserialize<T>(content) : default;
    }

    public async Task<ResultOutput<dynamic>> GetResult(string apiPath, object input = null, bool checkStatus = true)
    {
        return await GetResult<ResultOutput<dynamic>>(apiPath, input, checkStatus);
    }

    public async Task<ResultOutput<dynamic>> PostResult(string apiPath, object input = null, bool checkStatus = true, string contentType = "application/json;charset=UTF-8")
    {
        return await PostResult<ResultOutput<dynamic>>(apiPath, input, checkStatus, contentType);
    }

    public async Task<ResultOutput<dynamic>> PutResult(string apiPath, object input = null, bool checkStatus = true, string contentType = "application/json;charset=UTF-8")
    {
        return await PutResult<ResultOutput<dynamic>>(apiPath, input, checkStatus, contentType);
    }

    public async Task<ResultOutput<dynamic>> DeleteResult(string apiPath, object input = null, bool checkStatus = true)
    {
        return await DeleteResult<ResultOutput<dynamic>>(apiPath, input, checkStatus);
    }

    public async Task Login(AuthLoginInput input = null)
    {
        var authorization = Client.DefaultRequestHeaders.FirstOrDefault(a => a.Key == "Authorization");
        if (authorization.Key != null)
        {
            return;
        }

        if (input == null)
        {
            input = new AuthLoginInput()
            {
                UserName = "admin",
                Password = "123asd"
            };
        }

        //Client.DefaultRequestHeaders.Connection.Add("keep-alive");
        Client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36");

        var result = await AppInfo.GetRequiredService<IAuthClientService>().LoginAsync(input);

        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {result.AccessToken}");
    }

    public static string ToParams(object source)
    {
        var stringBuilder = new StringBuilder(string.Empty);
        if (source == null)
        {
            return "";
        }

        var entries = from PropertyDescriptor property in TypeDescriptor.GetProperties(source)
                      let value = property.GetValue(source)
                      where value != null
                      select (property.Name, value);

        foreach (var (name, value) in entries)
        {
            stringBuilder.Append(WebUtility.UrlEncode(name) + "=" + WebUtility.UrlEncode(value + "") + "&");
        }

        return stringBuilder.ToString().Trim('&');
    }
}