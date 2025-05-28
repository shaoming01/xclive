namespace SchemaBuilder.Svc.Models.Dto;

public class VueComInfo
{
    public Dictionary<string, VueDoc> VueComponents { get; set; }
    public Dictionary<string, List<TypeProperty>> Types { get; set; }
}

public class TypeProperty
{
    public string Name { get; set; }
    public string Type { get; set; }
}

public class VueDoc
{
    public string Description { get; set; }
    public string DisplayName { get; set; }
    public Dictionary<string, VueProp> Props { get; set; }
}

public class VueProp
{
    public string Type { get; set; }
    public string Description { get; set; }
    public bool Required { get; set; }
    public object DefaultValue { get; set; }
}