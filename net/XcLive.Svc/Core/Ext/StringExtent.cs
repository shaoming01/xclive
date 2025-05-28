using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using SchemaBuilder.Svc.Helpers;

namespace SchemaBuilder.Svc.Core.Ext;

public static class StringExtent
{
    public static string ToLowerFirstChar(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input; // 如果输入字符串为空或 null，直接返回

        // 将首字母转换为小写，并拼接剩余部分
        return char.ToLower(input[0]) + input.Substring(1);
    }

    private static char[] EmptyChars = new char[4]
    {
        '\r',
        '\n',
        ' ',
        '\t'
    };

    private static string _keys = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private static int _exponent = StringExtent._keys.Length;

    /// <summary>Trim所有非打印字符</summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string TrimNoPrintChar(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;
        return value.Trim(' ', '　', '\t', '\r', '\n');
    }

    /// <summary>此方法不管空格等空字符</summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsNullOrEmpty([NotNullWhen(true)] this string? value) => string.IsNullOrEmpty(value);

    /// <summary>此方法管到空格了</summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsNullOrWhiteSpace([NotNullWhen(true)] this string? value) => string.IsNullOrWhiteSpace(value);

    public static bool IsEmptyGuid(this string str) => str == "0";

    public static bool IsJson([NotNullWhen(true)] this string? data)
    {
        if (data.IsNullOrEmpty())
            return false;
        char realFirstChar = GetRealFirstChar(data);
        char realLastChar = GetRealLastChar(data);
        return realFirstChar == '{' && realLastChar == '}' || realFirstChar == '[' && realLastChar == ']';
    }

    private static char GetRealFirstChar(string data)
    {
        if (data == null)
            return ' ';
        for (int index = 0; index < data.Length; ++index)
        {
            if (!((IEnumerable<char>)StringExtent.EmptyChars).Contains<char>(data[index]))
                return data[index];
        }

        return ' ';
    }

    private static char GetRealLastChar(string data)
    {
        if (data == null)
            return ' ';
        for (int index = data.Length - 1; index > 0; --index)
        {
            if (!((IEnumerable<char>)StringExtent.EmptyChars).Contains<char>(data[index]))
                return data[index];
        }

        return ' ';
    }

    public static string[] SplitToArray(this string str, bool filterEmpty = false)
    {
        if (str.IsNullOrEmpty())
            return new string[0];
        str = str.TirmEx();
        string[] array = ((IEnumerable<string>)str.Split(new char[1]
            {
                ','
            }, StringSplitOptions.RemoveEmptyEntries)).Select<string, string>((Func<string, string>)(x => x.Trim()))
            .ToArray<string>();
        if (filterEmpty)
            array = ((IEnumerable<string>)array).Where<string>((Func<string, bool>)(x => !x.IsNullOrEmpty()))
                .ToArray<string>();
        return array;
    }

    public static string[] SplitByRow(this string str, bool filterEmpty = false)
    {
        if (str.IsNullOrEmpty())
            return new string[0];
        str = str.TirmEx();
        string[] array = ((IEnumerable<string>)str.Split(new char[2]
            {
                '\r',
                '\n'
            }, StringSplitOptions.RemoveEmptyEntries)).Select<string, string>((Func<string, string>)(x => x.Trim()))
            .ToArray<string>();
        if (filterEmpty)
            array = ((IEnumerable<string>)array).Where<string>((Func<string, bool>)(x => !x.IsNullOrEmpty()))
                .ToArray<string>();
        return array;
    }

    public static string[] SplitEx(this string str, char separator = ',', bool filterEmpty = false)
    {
        return str.SplitEx(new char[1] { separator }, (filterEmpty ? 1 : 0) != 0);
    }

    public static string[] SplitEx(this string str, char[] separatorList, bool filterEmpty = false)
    {
        if (str.IsNullOrEmpty())
            return new string[0];
        string[] array = ((IEnumerable<string>)str.Split(separatorList, StringSplitOptions.RemoveEmptyEntries))
            .Select<string, string>((Func<string, string>)(x => x.Trim())).ToArray<string>();
        if (filterEmpty)
            array = ((IEnumerable<string>)array).Where<string>((Func<string, bool>)(x => !x.IsNullOrEmpty()))
                .ToArray<string>();
        return array;
    }

    /// <summary>将字符串中的英文逗号、分号、空格替换为英文逗号</summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string TirmEx(this string str)
    {
        return str.Replace("，", ",").Replace("；", ",").Replace(";", ",").Replace(" ", ",").Replace(" ", ",").Trim(',');
    }

    /// <summary>字符串反转</summary>
    /// <param name="original"></param>
    /// <returns></returns>
    public static string ToReverse(this string original)
    {
        char[] charArray = original.ToCharArray();
        Array.Reverse((Array)charArray);
        return new string(charArray);
    }

    /// <summary>按字节数截断字条串</summary>
    /// <param name="str"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public static string Cut(this string str, int length)
    {
        if (string.IsNullOrEmpty(str) || str.Length < length / 2)
            return str;
        Encoding encoding = Encoding.GetEncoding("GB2312");
        char[] chArray = new char[length];
        int length1 = 0;
        int num = 0;
        string str1 = "";
        int index = 0;
        while (index < str.Length)
        {
            char ch = str[index];
            int byteCount = encoding.GetByteCount(new char[1]
            {
                ch
            });
            if (num + byteCount > length)
            {
                str1 = new string(chArray, 0, length1);
                break;
            }

            chArray[length1] = ch;
            num += byteCount;
            if (index + 1 == str.Length)
            {
                str1 = new string(chArray, 0, length1 + 1);
                break;
            }

            ++index;
            ++length1;
        }

        if (str1.Length > 3 && str1.Length < str.Length)
            str1 = str1.Substring(0, str1.Length - 3) + "...";
        return str1;
    }

    public static int GetByteLength(this string str)
    {
        return string.IsNullOrEmpty(str) ? 0 : Encoding.GetEncoding("GB2312").GetByteCount(str);
    }

    /// <summary>按字节数分隔字符串</summary>
    /// <param name="str"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public static string[] SplitByLength(this string str, int length)
    {
        List<string> stringList = new List<string>();
        Encoding encoding = Encoding.GetEncoding("GB2312");
        char[] chArray = new char[length];
        int length1 = 0;
        int num = 0;
        int index = 0;
        while (index < str.Length)
        {
            char ch = str[index];
            int byteCount = encoding.GetByteCount(new char[1]
            {
                ch
            });
            if (num + byteCount > length)
            {
                string str1 = new string(chArray, 0, length1);
                stringList.Add(str1);
                length1 = 0;
                num = 0;
            }

            chArray[length1] = ch;
            num += byteCount;
            if (index + 1 == str.Length)
            {
                string str2 = new string(chArray, 0, length1 + 1);
                stringList.Add(str2);
            }

            ++index;
            ++length1;
        }

        return stringList.ToArray();
    }

    /// <summary>用正则表达式获取以begin开头，以end结果的中间字符串</summary>
    /// <param name="input"></param>
    /// <param name="begin"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public static string GetPart(this string input, string begin, string end)
    {
        if (string.IsNullOrEmpty(input))
            return "";
        string pattern = string.Format("{0}(?<X>[\\s\\S].*?){1}", (object)begin, (object)end);
        Match match = Regex.Match(input, pattern, RegexOptions.IgnoreCase);
        return !match.Success || match.Groups.Count < 2 ? "" : match.Groups[1].Value;
    }

    /// <summary>从Url中拿到文件名</summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string GetFileNameFromUrl(this string input)
    {
        if (input.IsNullOrEmpty())
            return (string)null;
        string[] strArray = input.Split('/');
        return strArray[strArray.Length - 1];
    }

    /// <summary>使用正则表达式匹配内容，不包含头尾</summary>
    /// <param name="source"></param>
    /// <param name="wordsBegin"></param>
    /// <param name="wordsEnd"></param>
    /// <returns></returns>
    public static string RegexMatch(this string source, string wordsBegin, string wordsEnd)
    {
        string str = "";
        if (source != null)
        {
            for (Match match =
                     new Regex(wordsBegin + "(?<title>[\\s\\S]*?)" + wordsEnd, RegexOptions.IgnoreCase).Match(source);
                 match.Success;
                 match = match.NextMatch())
            {
                str = match.Groups["title"].ToString();
                if (str.Length > 0)
                    break;
            }
        }

        return str;
    }

    public static string ClearPath(this string? inputPath)
    {
        if (inputPath.IsNullOrWhiteSpace())
        {
            return "";
        }

        if (inputPath.IsUrl())
        {
            var uri = new Uri(inputPath);
            inputPath = uri.PathAndQuery + uri.Fragment;
        }

        inputPath = inputPath.Replace("\\", "/");
        inputPath = inputPath.Replace("//", "/").Replace("//", "/").Replace("//", "/");
        return inputPath.ToLower();
    }

    public static string RegexMatch(this string source, string pattern)
    {
        if (source.IsNullOrEmpty())
            return "";
        Match match = Regex.Match(source, pattern);
        return !match.Success ? "" : match.Value;
    }

    public static string RegexMatchReplace(
        this string source,
        string pattern,
        string replaceTo,
        int maxCount = 100)
    {
        if (source == null)
            return "";
        string input = source;
        for (int index = 0; index < maxCount; ++index)
        {
            string str = Regex.Replace(input, pattern, replaceTo);
            if (str == input)
                return str;
            input = str;
        }

        return input;
    }

    /// <summary>解析URL参数</summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static Dictionary<string, string> ExtractParameters(this string url)
    {
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        int num = url.IndexOf("?", StringComparison.Ordinal);
        if (num < 0)
            return parameters;
        string str1 = url.Substring(num + 1, url.Length - num - 1);
        string[] separator1 = new string[2] { "#", "&" };
        foreach (string str2 in str1.Split(separator1, StringSplitOptions.RemoveEmptyEntries))
        {
            string[] separator2 = new string[1] { "=" };
            string[] strArray = str2.Split(separator2, StringSplitOptions.RemoveEmptyEntries);
            if (strArray.Length == 2)
                parameters[strArray[0]] = strArray[1];
        }

        return parameters;
    }

    public static bool IsUrl(this string? url)
    {
        if (url == null)
            return false;
        string lower = url.ToLower();
        return lower.StartsWith("http://") || lower.StartsWith("https://");
    }

    /// <summary>过滤某开头的字条串</summary>
    /// <param name="input"></param>
    /// <param name="replacement"></param>
    /// <returns></returns>
    public static string TrimStartEx(this string input, string replacement)
    {
        if (input == null || replacement == null)
            return input;
        while (input.StartsWith(replacement, StringComparison.OrdinalIgnoreCase))
            input = input.Substring(replacement.Length, input.Length - replacement.Length);
        return input;
    }

    public static string ReadUrlPar(this string url, string parName)
    {
        if (url == null || parName == null)
            return url;
        int num1 = url.IndexOf("?" + parName + "=", StringComparison.OrdinalIgnoreCase);
        if (num1 < 0)
            num1 = url.IndexOf("&" + parName + "=", StringComparison.OrdinalIgnoreCase);
        if (num1 < 0)
            return (string)null;
        int num2 = url.IndexOf("&", num1 + 1, StringComparison.OrdinalIgnoreCase);
        if (num2 < 0)
            num2 = url.Length;
        int startIndex = num1 + parName.Length + 2;
        int length = num2 - num1 - parName.Length - 2;
        return length <= 0 ? "" : url.Substring(startIndex, length);
    }

    public static string AppendQueryPar(this string tarUrl, string queryStr)
    {
        if (queryStr.IsNullOrEmpty())
            return tarUrl;
        queryStr = queryStr.TrimStart('?').TrimStart('&');
        tarUrl = tarUrl.TrimEnd('?');
        return tarUrl.Contains("?") ? tarUrl + "&" + queryStr : tarUrl + "?" + queryStr;
    }

    public static string AppendQueryPar(
        this string tarUrl,
        Dictionary<string, string> pars,
        Encoding code = null)
    {
        if (!pars.Has<KeyValuePair<string, string>>())
            return tarUrl;
        if (code == null)
            code = Encoding.UTF8;
        StringBuilder postData = new StringBuilder();
        pars.ForEach<KeyValuePair<string, string>>((Action<KeyValuePair<string, string>>)(pair =>
        {
            if (string.IsNullOrEmpty(pair.Key) || string.IsNullOrEmpty(pair.Value))
                return;
            postData.Append("&");
            postData.Append(pair.Key);
            postData.Append("=");
            postData.Append(pair.Value.UrlEncode(code));
        }));
        tarUrl = tarUrl.TrimEnd('?');
        if (tarUrl.Contains("?"))
            return tarUrl + "&" + postData?.ToString();
        string str = postData.ToString();
        if (!string.IsNullOrEmpty(str) && str.StartsWith("&"))
            str = str.Substring(1, str.Length - 1);
        return tarUrl + "?" + str;
    }

    public static long To64Cvt10(string value)
    {
        long num = 0;
        for (int index = 0; index < value.Length; ++index)
        {
            int x = value.Length - index - 1;
            num += (long)StringExtent._keys.IndexOf(value[index]) *
                   StringExtent.Pow((long)StringExtent._exponent, (long)x);
        }

        return num;
    }

    private static long Pow(long baseNo, long x)
    {
        long num = 1;
        for (; x > 0L; --x)
            num *= baseNo;
        return num;
    }

    public static bool IsEquals(this string str, string value,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        return string.Equals(str, value, comparison);
    }

    public static string UrlEncode(this string str, Encoding encoding = null, bool toUpper = false)
    {
        if (encoding == null)
            encoding = Encoding.UTF8;
        string str1 = HttpUtility.UrlEncode(str, encoding);
        if (str1 == null)
            return "";
        string input = str1.Replace("+", "%20").Replace("*", "%2A");
        if (toUpper)
        {
            MatchCollection matchCollection = Regex.Matches(input, "%\\S{2}");
            for (int i = 0; i < matchCollection.Count; ++i)
            {
                Match match = matchCollection[i];
                input = input.Replace(match.Value, match.Value.ToUpper());
            }
        }

        return input;
    }
}