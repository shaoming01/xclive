using log4net;

public class Log4
{
    public static readonly ILog Log = (ILog)null;

    static Log4() => Log4.Log = LogManager.GetLogger(typeof(ILog));
}