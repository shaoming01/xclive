namespace SchemaBuilder.Svc.Core.Ext;

public static class DicExtent
{
    /// <summary>尝试获取值</summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="dic"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static TValue? TryGetOrDefault<TValue, TKey>(this Dictionary<TKey, TValue> dic, TKey key)
    {
        TValue orDefault = default(TValue);
        if (key != null)
            dic.TryGetValue(key, out orDefault);
        return orDefault;
    }

}