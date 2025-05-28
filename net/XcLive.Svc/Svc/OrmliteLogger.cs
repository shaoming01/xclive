using SchemaBuilder.Svc.Core.Cache;
using ServiceStack.Logging;

namespace SchemaBuilder.Svc.Svc
{
    public class OrmliteLogger : ILog
    {
        private readonly string _typeName;

        public OrmliteLogger(string typeName)
        {
            _typeName = typeName;
        }

        #region ILog Members

        public void Debug(object message)
        {
            Log4.Log.Debug(message);
        }

        public void Debug(object message, Exception exception)
        {
            Log4.Log.Debug(_typeName + message, exception);
        }

        public void DebugFormat(string format, params object[] args)
        {
            Log4.Log.DebugFormat(_typeName + format, args);
        }

        public void Error(object message)
        {
            Log4.Log.Error(_typeName + message);
            throw new Exception(message.ToString());
        }

        public void Error(object message, Exception exception)
        {
            Log4.Log.Error(_typeName + message, exception);
            throw exception;
        }

        public void ErrorFormat(string format, params object[] args)
        {
            Log4.Log.ErrorFormat(_typeName + format, args);
            throw new Exception(string.Format(format, args));
        }

        public void Fatal(object message)
        {
            Log4.Log.Fatal(_typeName + message);
            throw new Exception(message.ToString());
        }

        public void Fatal(object message, Exception exception)
        {
            Log4.Log.Fatal(_typeName + message, exception);
            throw exception;
        }

        public void FatalFormat(string format, params object[] args)
        {
            Log4.Log.FatalFormat(_typeName + format, args);
            throw new Exception(string.Format(format, args));
        }

        public void Info(object message)
        {
            Log4.Log.Info(_typeName + message);
            throw new Exception(message.ToString());
        }

        public void Info(object message, Exception exception)
        {
            Log4.Log.Info(_typeName + message, exception);
            throw exception;
        }

        public void InfoFormat(string format, params object[] args)
        {
            Log4.Log.InfoFormat(_typeName + format, args);
            throw new Exception(string.Format(format, args));
        }

        public void Warn(object message)
        {
            Log4.Log.Warn(_typeName + message);
            throw new Exception(message.ToString());
        }

        public void Warn(object message, Exception exception)
        {
            Log4.Log.Warn(_typeName + message, exception);
            throw exception;
        }

        public void WarnFormat(string format, params object[] args)
        {
            Log4.Log.WarnFormat(_typeName + format, args);
            throw new Exception(string.Format(format, args));
        }

        public bool IsDebugEnabled
        {
            get { return true; }
        }

        #endregion
    }
}