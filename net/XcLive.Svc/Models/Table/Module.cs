using ServiceStack.DataAnnotations;

namespace SchemaBuilder.Svc.Models.Table;

public class Module : ITable
{
    public long Id { get; set; }

    /// <summary>
    /// 由系统Module直接生成的
    /// </summary>
    public string? SysModuleId { get; set; }

    public string? Name { get; set; }
    public string? ComPath { get; set; }
    public string? SysModuleName { get; set; }

    /// <summary>
    /// 分类格式：一级分类:二级分类，用冒号分隔
    /// </summary>
    public string? CategoryPath { get; set; }

    [StringLength(80000)] public string? PropsJson { get; set; }
}