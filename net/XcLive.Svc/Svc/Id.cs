#nullable disable
namespace SchemaBuilder.Svc.Svc
{
    /// <summary>本地化的Id函数【也是按照时间走的】【非全局】</summary>
    public class Id
    {
        private static long _lastId;
        private static readonly ReaderWriterLockSlim LockSlim = new ReaderWriterLockSlim();

        /// <summary>切换的时间【以后的时间用这个时间计算差】【秒】</summary>
        private static readonly DateTime CutDateTime = new DateTime(2018, 10, 30);

        /// <summary>创建Id</summary>
        /// <returns></returns>
        public static long NewId()
        {
            LockSlim.EnterWriteLock();
            long num = NewId(DateTime.Now);
            LockSlim.ExitWriteLock();
            return num;
        }

        /// <summary>根据特定时间创建Id</summary>
        /// <param name="now"></param>
        /// <returns></returns>
        public static long NewId(DateTime now)
        {
            int index = (BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0) & int.MaxValue) >> 8;
            if (index < 6553600)
                index += 6553600;
            long num = CreateId(now, index);
            if (num >> 24 == _lastId >> 24)
                num = ++_lastId;
            else
                _lastId = num;
            return num;
        }

        /// <summary>创建Id</summary>
        /// <param name="dateTime">时间</param>
        /// <param name="index">计数索引【当前秒】</param>
        /// <returns>生成的Id</returns>
        private static long CreateId(DateTime dateTime, int index)
        {
            byte[] bytes1 = BitConverter.GetBytes((long)(dateTime - CutDateTime).TotalSeconds + 446683533120L);
            byte[] bytes2 = BitConverter.GetBytes(index);
            byte[] destinationArray = new byte[8];
            Array.Copy((Array)bytes2, 0, (Array)destinationArray, 0, 3);
            Array.Copy((Array)bytes1, 0, (Array)destinationArray, 3, 5);
            return BitConverter.ToInt64(destinationArray, 0);
        }

        /// <summary>将Id转换成时间</summary>
        public static DateTime GetDate(long id)
        {
            return GetDateEx(id);
        }

        /// <summary>得到分布式Id时间</summary>
        private static DateTime GetDateEx(long id)
        {
            byte[] bytes = BitConverter.GetBytes(id);
            byte[] destinationArray = new byte[8];
            Array.Copy((Array)bytes, 3, (Array)destinationArray, 0, 5);
            long num = BitConverter.ToInt64(destinationArray, 0) - 446683533120L;
            return CutDateTime.AddSeconds((double)num);
        }
    }
}