namespace SchemaBuilder.Svc.Core.Aq;

public interface IQueryField
{
    public string GetSqlExpression(string colName, string provider = "");
}