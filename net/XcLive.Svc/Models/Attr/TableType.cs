using System.ComponentModel;

namespace SchemaBuilder.Svc.Models.Attr;

[Flags]
public enum TableType
{
    [Description("主表")] MainTable = 1,

    [Description("子表")] DetailTable = 2,
}