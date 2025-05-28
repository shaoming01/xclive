using System.Security.Cryptography;
using System.Text;

namespace SchemaBuilder.Svc.Core.Ext;

public static class StringUtil
{
    public static string ToMd5(this string encryptStr, Encoding encoding = null)
    {
        if (encoding == null)
            encoding = Encoding.UTF8;
        if (encryptStr.IsNullOrEmpty())
            return "";
        byte[] hash = MD5.Create().ComputeHash(encoding.GetBytes(encryptStr));
        StringBuilder stringBuilder = new StringBuilder();
        foreach (byte num in hash)
            stringBuilder.Append(num.ToString("X2"));
        return stringBuilder.ToString();
    }
}