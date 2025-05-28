namespace Frame.Data;

public class R
{
    public bool success
    {
        get { return code == 0; }
        set { code = value ? 0 : 1; }
    }

    public int code { get; set; }
    public string message { get; set; }

    public static R OK()
    {
        return new R()
        {
            success = true,
        };
    }

    public static R<T> OK<T>(T data)
    {
        return new R<T>()
        {
            success = true,
            data = data
        };
    }

    public static R<T> Faild<T>(string message)
    {
        return new R<T>()
        {
            message = message,
            success = false,
        };
    }

    public static R Faild(string message)
    {
        return new R()
        {
            success = false, message = message
        };
    }
}

public class R<T> : R
{
    public T data { get; set; }
}