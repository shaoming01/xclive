namespace Frame.Ext;

public static class DateTimeExt
{
    public static long ToJsTimeStamp(this DateTime dt)
    {
        // 确保时间基准为UTC
        DateTime utcTime = dt.Kind == DateTimeKind.Utc
            ? dt
            : dt.ToUniversalTime(); // 处理Local/Unspecified

        DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return (long)(utcTime - unixEpoch).TotalMilliseconds;
    }
}