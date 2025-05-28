using ServiceStack.Logging;

namespace SchemaBuilder.Svc.Svc
{
    public class OrmliteLogFactory : ILogFactory
    {
        #region ILogFactory Members

        public ILog GetLogger(Type type)
        {
            return new OrmliteLogger(type.Name);
        }

        public ILog GetLogger(string typeName)
        {
            return new OrmliteLogger(typeName);
        }

        #endregion
    }
}