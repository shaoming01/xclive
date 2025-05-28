using log4net.Util;


// ReSharper disable once CheckNamespace
namespace log4net.Appender
{
    /// <summary>
    /// 保留固定日志数量
    /// 通过maxSizeRollBackups来设置
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public class RollingFileAppenderEx : RollingFileAppender
    {
        private const string Pattern = "_LOG4_";
        static DateTime _lastClearDate = DateTime.Now.AddDays(-1);

        override protected void OpenFile(string fileName, bool append)
        {
            //强制增加日志文件识别字符，用于删除文件时作判断
            if (!fileName.EndsWith(Pattern))
            {
                fileName += Pattern;
            }

            base.OpenFile(fileName, append);
            if (MaxSizeRollBackups <= 1)
            {
                return;
            }

            var date = DateTime.Now.AddDays(-MaxSizeRollBackups);
            CleanUp(date);
        }

        public void CleanUp(DateTime date)
        {
            #region 一天执行1次

            if (_lastClearDate.Date == DateTime.Now.Date)
            {
                return;
            }

            _lastClearDate = DateTime.Now;

            #endregion

            try

            {
                var directory = Path.GetDirectoryName(File);
                CleanUp(directory, date);
            }
            catch (Exception e)
            {
                LogLog.Error(GetType(), "清除日志文件失败", e);
            }
        }

        /// <summary>
        /// Cleans up.
        /// </summary>
        /// <param name="logDirectory">The log directory.</param>
        /// <param name="date">Anything prior will not be kept.</param>
        public void CleanUp(string logDirectory, DateTime date)
        {
            if (string.IsNullOrEmpty(logDirectory))
                throw new ArgumentException("logDirectory is missing");


            var dirInfo = new DirectoryInfo(logDirectory);
            if (!dirInfo.Exists)
                return;

            var fileInfos = dirInfo.GetFiles(string.Format("*{0}*.*", Pattern));
            if (fileInfos.Length == 0)
                return;

            foreach (var info in fileInfos)
            {
                if (info.CreationTime < date)
                {
                    try
                    {
                        info.Delete();
                    }
                    catch (Exception e)
                    {
                        LogLog.Error(GetType(), "清除日志文件失败" + info.Name, e);
                    }
                }
            }
        }
    }
}