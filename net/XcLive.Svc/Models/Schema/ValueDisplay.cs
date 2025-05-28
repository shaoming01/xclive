namespace SchemaBuilder.Svc.Models.Schema;

public class ValueDisplayQuery
{
    public ValueDisplayType? Type { get; set; }
    public string? EnumTypeName { get; set; }
}

public class ValueDisplay
{
    public ValueDisplay()
    {
    }

    public ValueDisplay(string value, string label)
    {
        Value = value;
        Label = label;
    }

    public string Value { get; set; }
    public string Label { get; set; }

    public List<ValueDisplay>? Children { get; set; }
}

/// <summary>
/// 不经常变动的基础数据类型，业务数据不能用此方式显示
/// </summary>
public enum ValueDisplayType
{
    Undefined = 0,
    Type1 = 1,
    SysModule = 2,
    LiveScriptTemplate = 3,
    LiveRoom = 5,
    LiveObserver = 6,
    LiveHuangCheOperate = 7,
    TtsModel = 8,
    MainScriptTemplate = 9,
    ChatScriptTemplate = 10,
    AiVerticalAnchor = 11,
    InteractScriptTemplate = 12,
    ShelfTaskConfig = 13,
}