using System.Linq.Expressions;
using System.Reflection;

namespace SchemaBuilder.Svc.Core.Ext;

/// <summary>强类型获取字段名称</summary>
public class MemberVisit<T>
{
    /// <summary>
    ///     获取多个字段名称
    ///     x=&gt;new []{x.A,x.B,x.C}
    /// </summary>
    /// <param name="funn"></param>
    /// <returns></returns>
    public static string[] GetMembers(Expression<Func<T, object>> funn)
    {
        return MemberVisit<T>.GetMemberStrings(funn.Body);
    }

    /// <summary>
    ///     获取单个字段名称
    ///     x=&gt;x.A
    /// </summary>
    /// <param name="funn"></param>
    /// <returns></returns>
    public static string GetMember(Expression<Func<T, object>> funn)
    {
        return ((IEnumerable<string>)MemberVisit<T>.GetMemberStrings(funn.Body)).FirstOrDefault<string>();
    }

    public static string[] GetMemberStrings(Expression fun)
    {
        List<string> list = new List<string>();
        MemberVisit<T>.GetMemberStrings(fun, ref list);
        return list.ToArray();
    }

    public static void GetMemberStrings(Expression fun, ref List<string> list)
    {
        switch (fun.NodeType)
        {
            case ExpressionType.Convert:
                MemberVisit<T>.GetConvertMemberName(fun as UnaryExpression, ref list);
                break;
            case ExpressionType.Lambda:
                MemberVisit<T>.GetMemberStrings(((LambdaExpression)fun).Body, ref list);
                break;
            case ExpressionType.MemberAccess:
                MemberVisit<T>.GetMemberAccessMemberName(fun as MemberExpression, ref list);
                break;
            case ExpressionType.New:
                MemberVisit<T>.GetNewMemberName(fun as NewExpression, ref list);
                break;
            case ExpressionType.NewArrayInit:
                MemberVisit<T>.GetNewArrayMemberName(fun as NewArrayExpression, ref list);
                break;
            default:
                throw new Exception("表达式太复杂，无法识别");
        }
    }

    private static void GetConvertMemberName(UnaryExpression fun, ref List<string> list)
    {
        MemberVisit<T>.GetMemberStrings(fun.Operand, ref list);
    }

    private static void GetMemberAccessMemberName(
        MemberExpression memberExpression,
        ref List<string> list)
    {
        list.Add(memberExpression.Member.Name);
    }

    private static void GetNewArrayMemberName(NewArrayExpression expression, ref List<string> list)
    {
        foreach (Expression expression1 in expression.Expressions)
            MemberVisit<T>.GetMemberStrings(expression1, ref list);
    }

    private static void GetNewMemberName(NewExpression lambdaExpression, ref List<string> list)
    {
        list.AddRange(lambdaExpression.Members.Select<MemberInfo, string>((Func<MemberInfo, string>)(mem => mem.Name)));
    }
}