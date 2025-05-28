using System.Reflection;

namespace SchemaBuilder.Svc.Core.TableBuilder
{
    public class ModelColumn
    {
        public string Name { get; set; }
        public PropertyInfo PerInfo { get; set; }
        public int? Length { get; set; }
        public bool? IsNull { get; set; }
        public bool AutoIncrement { get; set; }
        public bool IsRowVersion { get; set; }
    }
}