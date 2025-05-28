using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Frame.Utils;

public static class WgetHelper
{
    public static async Task<DownloadMessage> DownloadFileAsync(string url, string filePath,
        EventHandler<DownloadMessage>? onMessage = null,
        int maxRetry = 3)
    {
#if DEBUG
        if (File.Exists(filePath))
        {
            return new DownloadMessage()
            {
                Message = "下载成功",
                PercentComplete = 1,
                Status = DownloadStatus.Completed,
            };
        }
#endif
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
                (HttpRequestMessage msg, X509Certificate2 cert, X509Chain chain, SslPolicyErrors errors) => true,
            UseProxy = false,
        };
        using var httpClient = new HttpClient(handler);
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/135.0.0.0 Safari/537.36 Edg/135.0.0.0"); // 模拟 NuGet 客户端请求头
        string error = "";
        for (int attempt = 1; attempt <= maxRetry; attempt++)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                var dirExt = new FileInfo(filePath).Directory?.Exists;

                if (dirExt.HasValue && !dirExt.Value)
                {
                    new FileInfo(filePath).Directory?.Create();
                }

                using var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                var totalBytes = response.Content.Headers.ContentLength ?? -1L;
                var canReportProgress = totalBytes > 0;

                await using var contentStream = await response.Content.ReadAsStreamAsync();
                await using var fileStream =
                    new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);

                var buffer = new byte[8192];
                long totalRead = 0;
                int read;
                var lastReport = DateTime.Now;

                while ((read = await contentStream.ReadAsync(buffer)) > 0)
                {
                    await fileStream.WriteAsync(buffer.AsMemory(0, read));
                    totalRead += read;

                    // 控制台输出简单进度
                    if (canReportProgress && (DateTime.Now - lastReport).TotalMilliseconds > 200)
                    {
                        onMessage?.Invoke(null, new DownloadMessage()
                        {
                            Message = "下载中",
                            Status = DownloadStatus.Processing,
                            PercentComplete = (decimal)totalRead / totalBytes,
                        });
                        lastReport = DateTime.Now;
                    }
                }

                var msg = new DownloadMessage()
                {
                    Message = "下载完成",
                    Status = DownloadStatus.Completed,
                    PercentComplete = 1,
                };

                onMessage?.Invoke(null, msg);
                return msg;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                if (attempt == maxRetry) break;
                await Task.Delay(1000);
            }
        }

        var abmsg = new DownloadMessage()
        {
            Message = "下载失败" + error,
            Status = DownloadStatus.Failed,
            PercentComplete = 0,
        };
        onMessage?.Invoke(null, abmsg);
        return abmsg;
    }

    public class DownloadMessage
    {
        public DownloadStatus Status { get; set; }
        public string? Message { get; set; }
        public decimal PercentComplete { get; set; }
    }

    public enum DownloadStatus
    {
        Processing,
        Completed,
        Failed,
    }
}