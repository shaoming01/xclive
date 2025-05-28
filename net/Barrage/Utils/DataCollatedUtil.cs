using Barrage.Utils.DataCollated;

namespace Barrage.Utils
{
    /// <summary>
    /// 数据整理类
    /// </summary>
    public static class DataCollatedUtil
    {

        #region Douyin
        private static IDataCollated? _douyin = null;
        public static IDataCollated? Douyin
        {
            get
            {
                if (_douyin == null)
                {
                    _douyin = new DouyinDataCollated();
                }

                return _douyin;
            }
        }
        #endregion




    }
}
