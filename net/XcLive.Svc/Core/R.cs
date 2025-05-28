namespace SchemaBuilder.Svc.Core;

public class R
{
    public bool Success
    {
        get { return Code == 0; }
        set { Code = value ? 0 : 1; }
    }

    public int Code { get; set; }
    public string Message { get; set; }

    public static R OK()
    {
        return new R()
        {
            Success = true,
        };
    }

    public static R<T> OK<T>(T data)
    {
        return new R<T>()
        {
            Success = true,
            Data = data
        };
    }

    public static R<T> Faild<T>(string message)
    {
        return new R<T>()
        {
            Message = message,
            Success = false,
        };
    }

    public static R Faild(string message)
    {
        return new R()
        {
            Success = false, Message = message
        };
    }
}

public class R<T> : R
{
    public T Data { get; set; }
}