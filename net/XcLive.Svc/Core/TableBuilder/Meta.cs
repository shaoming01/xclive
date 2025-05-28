using System.Reflection;
using ServiceStack.DataAnnotations;

namespace SchemaBuilder.Svc.Core.TableBuilder
{
    public class ModelTable
    {
        private readonly List<ModelColumn> _cols = new List<ModelColumn>();

        public ModelTable(Type modelType)
        {
            ModelType = modelType;
            Name = modelType.Name;
            InitialCols();
        }

        public string Name { get; set; }
        public Type ModelType { get; set; }

        public List<ModelColumn> Cols
        {
            get { return _cols; }
        }

        private void InitialCols()
        {
            ModelColumn idCol = null;
            List<PropertyInfo> pers = ModelType.GetProperties().ToList();
            pers.ForEach(p =>
            {
                if (p.GetCustomAttributes(typeof (IgnoreAttribute), true).Any())
                {
                    return;
                }
                int? length = null;
                var lenAtt =  p.GetCustomAttributes(typeof (StringLengthAttribute), true).FirstOrDefault() as
                        StringLengthAttribute;
                if (lenAtt != null)
                {
                    length = lenAtt.MaximumLength;
                }
                object[] incrementAtt = p.GetCustomAttributes(typeof (AutoIncrementAttribute), true);
                bool autoIncrement = incrementAtt.Any();
                var col = new ModelColumn
                {
                    Name = p.Name,
                    PerInfo = p,
                    Length = length,
                    AutoIncrement = autoIncrement,
                    IsRowVersion = p.Name == "RowVersion" && p.PropertyType == typeof (ulong)
                };
                Cols.Add(col);
                if (col.Name.ToLower() == "id")
                {
                    idCol = col;
                }
            });
            if (idCol != null)
            {
                Cols.Remove(idCol);
                Cols.Insert(0, idCol);
            }
        }
    }
}