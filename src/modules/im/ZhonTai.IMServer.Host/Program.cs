using NLog.Web;
using System.Text;
using ZhonTai.IMServer.Host.Core.Configs;

var builder = WebApplication.CreateBuilder(args);

//�����־��Ӧ���򣬱���.net�Դ���־���������̨
builder.Logging.ClearProviders();
//ʹ��NLog��־
builder.Host.UseNLog();

var imServerConfig = builder.Configuration.GetSection("ImServerConfig").Get<ImServerConfig>();
var healthChecks = imServerConfig.HealthChecks;
//��ӽ������
if (healthChecks != null && healthChecks.Enable)
{
    builder.Services.AddHealthChecks();
}

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
Console.InputEncoding = Encoding.GetEncoding(imServerConfig.InputEncodingName);
Console.OutputEncoding = Encoding.GetEncoding(imServerConfig.OutputEncodingName);

app.UseFreeImServer(new ImServerOptions
{
    Redis = new FreeRedis.RedisClient(imServerConfig.RedisClientConnectionString),
    Servers = imServerConfig.Servers,
    Server = imServerConfig.Server,
});

app.Run();
