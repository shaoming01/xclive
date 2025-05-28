using SchemaBuilder.Svc.Core.QueryHelper.Data.@interface;

namespace SchemaBuilder.Svc.Core.QueryHelper;

/// <summary>
/// 查询转换工具类
/// </summary>
public static class QueryConvertorUtil
{
    /// <summary>
    /// 查询转换对象
    /// </summary>
    public static readonly IQueryConvertor QueryConvertor = new ReflectQueryConvertor();
}