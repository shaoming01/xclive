using System.IO.Compression;
using System.Text;

public class CompressionUtils
{
    public static string CompressToBase64(string input)
    {
        // 将字符串转换为字节数组
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);

        // 使用 MemoryStream 来存储压缩后的数据
        using (var outputStream = new MemoryStream())
        {
            // 使用 GZipStream 对字节数组进行压缩
            using (var gzipStream = new GZipStream(outputStream, CompressionLevel.Optimal))
            {
                gzipStream.Write(inputBytes, 0, inputBytes.Length);
            }

            // 获取压缩后的字节数据
            byte[] compressedBytes = outputStream.ToArray();

            // 将压缩后的字节数组转换为 Base64 编码的字符串
            return Convert.ToBase64String(compressedBytes);
        }
    }

    public static string DecompressFromBase64(string base64Input)
    {
        // 将 Base64 字符串转换为字节数组
        byte[] compressedBytes = Convert.FromBase64String(base64Input);

        // 使用 MemoryStream 来读取压缩后的数据
        using (var inputStream = new MemoryStream(compressedBytes))
        using (var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress))
        using (var reader = new StreamReader(gzipStream, Encoding.UTF8))
        {
            // 解压并返回字符串
            return reader.ReadToEnd();
        }
    }
}