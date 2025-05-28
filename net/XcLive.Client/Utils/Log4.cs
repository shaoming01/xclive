using log4net;
using log4net.Config;

[assembly: XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]
public class Log4
{
    public static readonly ILog Log = (ILog)null;

    static Log4() => Log4.Log = LogManager.GetLogger(typeof(ILog));
}