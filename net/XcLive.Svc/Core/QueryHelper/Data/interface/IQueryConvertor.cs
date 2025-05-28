namespace SchemaBuilder.Svc.Core.QueryHelper.Data.@interface;

/// <summary>
/// 查询转换接口
/// </summary>
public interface IQueryConvertor
{
    /// <summary>
    /// 将分页查询对象转换分页查询数据
    /// </summary>
    /// <typeparam name="T">查询对象</typeparam>
    /// <param name="queryObject">分页查询对象</param>
    /// <returns>分页查询数据</returns>
    PageQueryData Convertor<T>(PageQueryObject<T> queryObject);

    /// <summary>
    /// 将查询对象转换成查询项数组
    /// </summary>
    /// <param name="obj">需要转换的查询对象实例</param>
    /// <param name="names">需要进行转换的公共属性如果为null代表全部转换</param>
    /// <returns>转换完成的查询项</returns>
    QueryElementItem[] Convertor<T>(T obj, HashSet<string> names = null);
}