using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchemaBuilder.Svc.Core;
using SchemaBuilder.Svc.Core.Ext;
using SchemaBuilder.Svc.Helpers;
using SchemaBuilder.Svc.Models.Table;
using SchemaBuilder.Svc.Models.ViewModel;
using SchemaBuilder.Svc.Svc;
using ServiceStack.OrmLite;

namespace SchemaBuilder.Web.ApiControllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class ProductController : ControllerBase
{
    [HttpGet, HttpPost, AllowAnonymous]
    public R<AppInfo> Info(string? tenantId)
    {
        using var db = Db.Open();
        var ten = db.SingleById<Tenant>(tenantId.ToLong() ?? 0);
        if (ten != null)
        {
            return R.OK(new AppInfo()
            {
                Version = new Version(ten.ProductVersion ?? "1.1"),
                ProductName = ten.ProductName ?? "",
            });
        }

        return R.OK(new AppInfo
        {
            Version = new Version(1, 1),
            ProductName = "火星",
        });
    }

    [HttpGet, HttpPost, AllowAnonymous]
    public R<string> GetCertSign(string domain, int port)
    {
        try
        {
            using var tcpClient = new TcpClient(domain, port);
            // 创建 SslStream，不验证证书
            using var sslStream = new SslStream(tcpClient.GetStream(), false);
            // 进行 SSL 握手
            sslStream.AuthenticateAsClient(domain);

            // 获取证书信息
            var cert = sslStream.RemoteCertificate;
            if (cert == null)
                return R.Faild<string>("证书为空");
            var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(cert.GetCertHash());
            var sign = BitConverter.ToString(hash);
            return R.OK(sign.ToMd5());
        }
        catch (Exception e)
        {
            return R.Faild<string>("获取证书异常" + e.Message);
        }
    }
}