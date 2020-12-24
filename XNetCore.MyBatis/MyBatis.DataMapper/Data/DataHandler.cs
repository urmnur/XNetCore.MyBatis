using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyBatis.DataMapper.Data
{
    public class DataHandler
    {
        #region 单例模式
        private static DataHandler _this;
        public static DataHandler Instance
        {
            get
            {
                if (null == _this)
                    _this = new DataHandler();
                return _this;
            }
        }
        private DataHandler()
        {
        }
        #endregion

        #region 数据转换
        private string delete0(string str)
        {
            if (!str.Contains("."))
            {
                return str;
            }
            if (!str.EndsWith("0"))
            {
                return str;
            }
            string result = str;
            int len = result.Length;
            for (var i = len; i > 0; i--)
            {
                char c = str[i - 1];
                if (c == '0' || c == '.')
                {
                    result = str.Substring(0, i - 1);
                }
                else
                {
                    break;
                }
                if (!result.Contains("."))
                {
                    break;
                }
            }
            if (string.IsNullOrEmpty(result))
            {
                result = "0";
            }
            return result;
        }
        /// <summary>
        /// Oracle.DataAccess.dll 中当为Double类型时会出现进度问题
        /// 2.30 会变成  2.3000000000000003
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public double getDbDouble(double d)
        {
            var str = d.ToString("0.00000");
            str = delete0(str);
            return Convert.ToDouble(str);
        }
        /// <summary>
        /// Oracle.DataAccess.dll 中当为Double类型时会出现进度问题
        /// 2.30 会变成  2.3000000000000003
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public decimal getDbDecimal(decimal d)
        {
            var str = d.ToString("0.00000");
            str = delete0(str);
            return Convert.ToDecimal(str);
        }
        /// <summary>
        /// Oracle.DataAccess.dll 中当为Double类型时会出现进度问题
        /// 2.30 会变成  2.3000000000000003
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public float getDbFloat(float d)
        {
            var str = d.ToString("0.00000");
            str = delete0(str);
            return (float)Convert.ToDouble(str);
        }
        #endregion
    }
}
