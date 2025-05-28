using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Frame.Data;
using Frame.Ext;
using Newtonsoft.Json;

namespace Frame.Utils;

public class CertValidate
{
    public static Dictionary<string, string> ServerCertSignCache = new();
    public static DateTime? LastCheckDomainCert;

    public static bool CheckCert(string? url, X509Certificate? cert = null)
    {
        if (AppHelper.IsDebug)
        {
            return true;
        }

        if (!url.Has())
        {
            return false;
        }

        var uri = new Uri(url);
        if (uri.Scheme != "https")
        {
            return true;
        }

        return CheckCert(uri.Host, uri.Port, cert);
    }

    /// <summary>
    /// 校验证书，2种方式，一种是已经有了本地证书就验这个证书合法性，每次都验；如果没有本地证书，就再请求握手一次，拿到证书，这个就是本地证书，再验，这种方式验证需要保持3秒内不需要验，否则会有性能问题
    /// </summary>
    /// <param name="domain"></param>
    /// <param name="port"></param>
    /// <param name="cert"></param>
    /// <returns></returns>
    public static bool CheckCert(string domain, int port, X509Certificate? cert = null)
    {
        var clientSign = "";
        if (cert == null)
        {
            //3秒内不重复检测，如果抓包工具在检测成功后的3秒内启动，这3秒内的其他请示是会被抓包的
            if (DateTime.Now - LastCheckDomainCert <= TimeSpan.FromSeconds(3))
            {
                return true;
            }

            clientSign = GetClientCertSign(domain, port);
        }
        else
        {
            clientSign = CalcCertSign(cert);
        }

        if (!clientSign.Has())
        {
            return false;
        }


        var key = domain + ':' + port;
        if (!ServerCertSignCache.TryGetValue(key, out var serverSign))
        {
            var re = GetServerCertSign(domain, port);
            if (!re.success || !re.data.Has())
            {
                return false;
            }

            ServerCertSignCache[key] = re.data;
            serverSign = re.data;
        }

        var succ = serverSign == clientSign;
        if (succ && cert == null) //远程验证，需要保持3秒不需要再验
        {
            LastCheckDomainCert = DateTime.Now;
        }

        return succ;
    }

    private static string CalcCertSign(X509Certificate cert)
    {
        var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(cert.GetCertHash());
        var sign = BitConverter.ToString(hash);
        return sign.ToMd5();
    }

    /// <summary>
    /// 正确的证书签名
    /// </summary>
    /// <param name="domain"></param>
    /// <param name="port"></param>
    /// <returns></returns>
    public static R<string> GetServerCertSign(string domain, int port)
    {
        var url = $"{AppHelper.ServerHost.TrimEnd('/')}/api/product/GetCertSign?domain={domain}&port={port}";

        var client = new HttpClient(new HttpClientHandler()
        {
            UseProxy = false,
        });
        var resp = client.GetAsync(url).Result;
        if (!resp.IsSuccessStatusCode)
        {
            return R.Faild<string>("获取网站证书出错" + resp.StatusCode);
        }

        var json = resp.Content.ReadAsStringAsync().Result;
        var re = JsonConvert.DeserializeObject<R<string>>(json);
        return re;
    }

    public static string GetClientCertSign(string domain, int port)
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
                return "";
            var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(cert.GetCertHash());
            var sign = BitConverter.ToString(hash);
            return sign.ToMd5();
        }
        catch (Exception e)
        {
            return "";
        }
    }
}