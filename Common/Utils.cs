using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Data.OracleClient;
using Devart.Data.Oracle;
using System.Web;
using System.Globalization;

namespace Common
{
    /// <summary>
    /// 工具类合集
    /// </summary>
    public class Utils
    {
        #region Public

        #region 对象转换处理

        /// <summary>
        /// 判断对象是否为Int32类型的数字
        /// </summary>
        /// <param name="Expression">表达式</param>
        /// <returns>是为数字否非数字</returns>
        public static bool IsNumeric(object expression)
        {
            if (expression != null)
                return IsNumeric(expression.ToString());

            return false;

        }

        /// <summary>
        /// 判断对象是否为Int32类型的数字
        /// </summary>
        /// <param name="Expression">表达式</param>
        /// <returns>是为数字否非数字</returns>
        public static bool IsNumeric(string expression)
        {
            if (expression != null)
            {
                string str = expression;
                if (str.Length > 0 && str.Length <= 11 && Regex.IsMatch(str, @"^[-]?[0-9]*[.]?[0-9]*$"))
                {
                    if ((str.Length < 10) || (str.Length == 10 && str[0] == '1') || (str.Length == 11 && str[0] == '-' && str[1] == '1'))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 是否为Double类型
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static bool IsDouble(object expression)
        {
            if (expression != null)
                return Regex.IsMatch(expression.ToString(), @"^([0-9])[0-9]*(\.\w*)?$");

            return false;
        }


        /// <summary>
        /// 正则验证是否合法Email地址
        /// </summary>
        /// <param name="expression">待验证字符串</param>
        /// <returns>是否Email地址</returns>
        public static bool IsEmail(string expression)
        {
            if (!string.IsNullOrEmpty(expression))
                return Regex.IsMatch(expression, @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");

            return false;
        }

        /// <summary>
        /// 正则验证是否合法IP地址
        /// </summary>
        /// <param name="expression">待验证字符串</param>
        /// <returns>是否IP地址</returns>
        public static bool IsIPAddress(string expression)
        {
            if (!string.IsNullOrEmpty(expression))
                return Regex.IsMatch(expression, @"\d+\.\d+\.\d+\.\d+");

            return false;
        }

        /// <summary>
        /// 正则验证是否合法域名(不带http://)
        /// </summary>
        /// <param name="expression">待验证字符串</param>
        /// <returns>是否合法域名</returns>
        public static bool IsHostDomain(string expression)
        {
            if (!string.IsNullOrEmpty(expression))
                return Regex.IsMatch(expression, @"^([a-zA-Z\d][a-zA-Z\d-_]+\.)+[a-zA-Z\d-_][^ ]*$");

            return false;
        }

        /// <summary>
        /// 正则匹配URL地址
        /// </summary>
        /// <param name="expression">URL地址</param>
        /// <returns>是否URL地址</returns>
        public static bool IsUrlAddress(string expression)
        {
            string Pattern = @"^(http|https|ftp|ftps)\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&$%\$#\=~])*$";
            Regex r = new Regex(Pattern);
            Match m = r.Match(expression);
            if (m.Success)
                return true;
            return false;
        }

        /// <summary>
        /// 将字符串转换为数组
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>字符串数组</returns>
        public static string[] GetStrArray(string str)
        {
            return str.Split(new char[',']);
        }

        /// <summary>
        /// 将数组转换为字符串
        /// </summary>
        /// <param name="list">List</param>
        /// <param name="speater">分隔符</param>
        /// <returns>转换后端字符串</returns>
        public static string GetArrayStr(List<string> list, string speater)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                if (i == list.Count - 1)
                {
                    sb.Append(list[i]);
                }
                else
                {
                    sb.Append(list[i]);
                    sb.Append(speater);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// object型转换为bool型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的bool类型结果</returns>
        public static bool StrToBool(object expression, bool defValue)
        {
            if (expression != null)
                return StrToBool(expression, defValue);

            return defValue;
        }

        /// <summary>
        /// string型转换为bool型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的bool类型结果</returns>
        public static bool StrToBool(string expression, bool defValue)
        {
            if (expression != null)
            {
                if (string.Compare(expression, "true", true) == 0)
                    return true;
                else if (string.Compare(expression, "false", true) == 0)
                    return false;
                if (string.Compare(expression, "0", true) == 0)
                    return false;
                if (string.Compare(expression, "1", true) == 0)
                    return true;
            }
            return defValue;
        }

        /// <summary>
        /// string型转换为bool型
        /// </summary>
        /// <param name="expression">要转换的字符串</param>
        /// <returns>转换后的bool类型结果</returns>
        public static bool StrToBool(string expression)
        {
            try
            {
                bool temp = true;
                if (expression != null)
                {
                    if (string.Compare(expression, "true", true) == 0)
                        temp = true;
                    else if (string.Compare(expression, "false", true) == 0)
                    {
                        temp = false;
                    }
                    if (string.Compare(expression, "0", true) == 0)
                        temp = false;
                    if (string.Compare(expression, "1", true) == 0)
                        temp = true;
                }
                return temp;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }

        /// <summary>
        /// 将对象转换为Int32类型
        /// </summary>
        /// <param name="expression">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static int ObjToInt(object expression, int defValue)
        {
            if (expression != null)
                return StrToInt(expression.ToString(), defValue);

            return defValue;
        }

        /// <summary>
        /// 去除前导0
        /// </summary>
        /// <param name="value">要去前导0的字符串</param>
        /// <returns>去除后的字符串</returns>
        public static string RemovePreambleZero(string value)
        {
            string result = string.Empty;
            double t = 0;
            if (double.TryParse(value, out t))
                result = Convert.ToDouble(value).ToString();
            else
                result = value;
            return result;
        }

        /// <summary>
        /// 将字符串转换为Int32类型
        /// </summary>
        /// <param name="expression">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static int StrToInt(string expression, int defValue)
        {
            if (string.IsNullOrEmpty(expression) || expression.Trim().Length >= 11 || !Regex.IsMatch(expression.Trim(), @"^([-]|[0-9])[0-9]*(\.\w*)?$"))
                return defValue;

            int rv;
            if (Int32.TryParse(expression, out rv))
                return rv;

            return Convert.ToInt32(StrToFloat(expression, defValue));
        }


        /// <summary>
        /// Object型转换为decimal型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的decimal类型结果</returns>
        public static decimal ObjToDecimal(object expression, decimal defValue)
        {
            if (expression != null)
                return StrToDecimal(expression.ToString(), defValue);

            return defValue;
        }

        /// <summary>
        /// string型转换为decimal型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的decimal类型结果</returns>
        public static decimal StrToDecimal(string expression, decimal defValue)
        {
            if ((expression == null) || (expression.Length > 18))
                return defValue;

            decimal intValue = defValue;
            if (expression != null)
            {
                bool IsDecimal = Regex.IsMatch(expression, @"^([-]|[0-9])[0-9]*(\.\w*)?$");
                if (IsDecimal)
                    decimal.TryParse(expression, out intValue);
            }
            return intValue;
        }

        /// <summary>
        /// Object型转换为float型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static float ObjToFloat(object expression, float defValue)
        {
            if (expression != null)
                return StrToFloat(expression.ToString(), defValue);

            return defValue;
        }

        /// <summary>
        /// string型转换为float型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static float StrToFloat(string expression, float defValue)
        {
            if ((expression == null) || (expression.Length > 10))
                return defValue;

            float intValue = defValue;
            if (expression != null)
            {
                bool IsFloat = Regex.IsMatch(expression, @"^([-]|[0-9])[0-9]*(\.\w*)?$");
                if (IsFloat)
                    float.TryParse(expression, out intValue);
            }
            return intValue;
        }

        /// <summary>
        /// 将对象转换为日期时间类型
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static DateTime StrToDateTime(string str, DateTime defValue)
        {
            if (!string.IsNullOrEmpty(str))
            {
                DateTime dateTime;
                if (DateTime.TryParse(str, out dateTime))
                    return dateTime;
            }
            return defValue;
        }
        /// <summary>
        /// 将对象转换为日期时间类型
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <returns>转换后的DateTime?类型结果</returns>
        public static DateTime? StrToDateTime2(object str)
        {
            DateTime? dt = null;
            try
            {
                DateTime tempdt = DateTime.MinValue;
                if (DateTime.TryParse(str.ToString(), out tempdt))
                {
                    dt = tempdt;
                }
            }
            catch
            {
            }
            return dt;
        }

        /// <summary>
        /// 将对象转换为日期时间类型
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <returns>转换后的int类型结果</returns>
        public static DateTime StrToDateTime(object str)
        {
            DateTime dt = (DateTime)SqlDateTime.MinValue;
            if (str == null)
                return dt;
            try
            {
                dt = StrToDateTime(str.ToString(), dt);
            }
            catch
            {
            }
            return dt;
        }

        /// <summary>
        /// 将日期时间转换为MM/dd/yyyy格式的字符串
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <returns>转换后的int类型结果</returns>
        public static string StrToDateTimeENStr(object str)
        {
            string result = string.Empty;
            DateTime dt = (DateTime)SqlDateTime.MinValue;
            try
            {
                dt = StrToDateTime(str.ToString(), dt);
                result = dt.Month.ToString("00") + "/" + dt.Day.ToString("00") + "/" + dt.Year.ToString("0000");
            }
            catch
            {
                result = dt.Month.ToString("00") + "/" + dt.Day.ToString("00") + "/" + dt.Year.ToString("0000");
            }
            return result;
        }

        /// <summary>
        /// 转换时间字符串
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>返回字符串</returns>
        public static string StrToDateTimeCNStr(object str)
        {
            string result = string.Empty;
            string tempStr = str.ToString();
            List<string> chineseWeeks = new List<string>() { "周一", "周二", "周三", "周四", "周五", "周六", "周天", "周日" };
            DateTime dt = (DateTime)SqlDateTime.MinValue;
            try
            {
                chineseWeeks.ForEach(weekItem => {
                    tempStr = tempStr.Replace("/" + weekItem, "");
                });
                tempStr = tempStr.Replace("/", "-");
                result = dt.Year.ToString("0000") + "-" + dt.Month.ToString("00") + "-" + dt.Day.ToString("00");
            }
            catch
            {
                result = dt.Year.ToString("0000") + "/" + dt.Month.ToString("00") + "/" + dt.Day.ToString("00");
            }
            return result;
        }

        /// <summary>
        /// 将对象转换为日期时间类型
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <returns>转换后的int类型结果</returns>
        public static DateTime StrToDateTime(DataRow dr, string FieldName, string DateFormat)
        {
            DateTime dt = (DateTime)SqlDateTime.MinValue;
            if (Convert.IsDBNull(dr[FieldName]))
                return dt;
            bool iscomplate = DateTime.TryParseExact(dr[FieldName].ToString(), DateFormat, Thread.CurrentThread.CurrentCulture, DateTimeStyles.AssumeUniversal, out dt);
            if (!iscomplate)
            {
                iscomplate = DateTime.TryParseExact(dr[FieldName].ToString(), "yyyy-MM-dd", Thread.CurrentThread.CurrentCulture, DateTimeStyles.AssumeUniversal, out dt);
                if (!iscomplate)
                {
                    iscomplate = DateTime.TryParse(dr[FieldName].ToString(), out dt);
                    if (!iscomplate)
                        dt = (DateTime)SqlDateTime.MinValue;
                }
            }
            return dt;
        }

        /// <summary>
        /// 将对象转换为日期时间类型
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <returns>转换后的int类型结果</returns>
        public static DateTime StrToDateTime(string str, string DateFormat)
        {
            DateTime dt = (DateTime)SqlDateTime.MinValue;
            if (string.IsNullOrEmpty(str))
                return dt;
            bool iscomplate = DateTime.TryParseExact(str, DateFormat, Thread.CurrentThread.CurrentCulture, DateTimeStyles.AssumeUniversal, out dt);
            if (!iscomplate)
            {
                iscomplate = DateTime.TryParseExact(str, "yyyy-MM-dd", Thread.CurrentThread.CurrentCulture, DateTimeStyles.AssumeUniversal, out dt);
                if (!iscomplate)
                {
                    iscomplate = DateTime.TryParse(str, out dt);
                    if (!iscomplate)
                        dt = (DateTime)SqlDateTime.MinValue;
                }
            }
            return dt;
        }

        /// <summary>
        /// 将对象转换为日期时间类型
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <returns>转换后的int类型结果</returns>
        public static DateTime StrToDateTime1(string str, string DateFormat)
        {
            DateTime dt = (DateTime)SqlDateTime.MinValue;
            if (string.IsNullOrEmpty(str))
                return dt;
            bool iscomplate = DateTime.TryParseExact(str, DateFormat, Thread.CurrentThread.CurrentCulture, DateTimeStyles.AssumeUniversal, out dt);
            if (!iscomplate)
            {
                iscomplate = DateTime.TryParseExact(str, "yyyy-MM-dd", Thread.CurrentThread.CurrentCulture, DateTimeStyles.AssumeUniversal, out dt);
                if (!iscomplate)
                {
                    iscomplate = DateTime.TryParse(str, out dt);
                    if (!iscomplate)
                        dt = (DateTime)SqlDateTime.MinValue;
                }
            }
            return dt;
        }

        /// <summary>
        /// 将对象转换为日期时间类型
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <returns>转换后的int类型结果</returns>
        public static DateTime StrToDateTime2(string str, string DateFormat)
        {
            DateTime dt = (DateTime)SqlDateTime.MinValue;
            if (string.IsNullOrEmpty(str))
                return dt;
            bool iscomplate = DateTime.TryParseExact(str, DateFormat, System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);
            if (!iscomplate)
            {
                iscomplate = DateTime.TryParseExact(str, DateFormat, System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);
                if (!iscomplate)
                {
                    iscomplate = DateTime.TryParse(str, out dt);
                    if (!iscomplate)
                        dt = (DateTime)SqlDateTime.MinValue;
                }
            }
            return dt;
        }

        /// <summary>
        /// 将对象转换为日期时间类型
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <returns>转换后的int类型结果</returns>
        public static DateTime StrToDateTimeByOracle(object str)
        {
            DateTime dt = (DateTime)OracleDateTime.MinValue;
            try
            {
                dt = StrToDateTime(str.ToString(), dt);
            }
            catch
            {
            }
            return dt;
        }

        /// <summary>
        /// 正则取出字符串中的数字
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>数字</returns>
        public static double GetStringNumberByRegular(string str)
        {
            double result = 0;
            Match m = Regex.Match(str, "\\d+(\\.\\d+){0,1}");
            double.TryParse(m.Groups[0].ToString(), out result);
            return result;
        }
        /// <summary>
        /// 将对象转换为日期时间类型
        /// </summary>
        /// <param name="obj">要转换的对象</param>
        /// <returns>转换后的int类型结果</returns>
        public static DateTime ObjectToDateTime(object obj)
        {
            return StrToDateTime(obj.ToString());
        }

        /// <summary>
        /// 将对象转换为日期时间类型
        /// </summary>
        /// <param name="obj">要转换的对象</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static DateTime ObjectToDateTime(object obj, DateTime defValue)
        {
            return StrToDateTime(obj.ToString(), defValue);
        }

        /// <summary>
        /// 将对象转换为字符串
        /// </summary>
        /// <param name="obj">要转换的对象</param>
        /// <returns>转换后的string类型结果</returns>
        public static string ObjectToStr(object obj)
        {
            if (obj == null)
                return "";
            return obj.ToString().Trim();
        }

        /// <summary>
        /// 计算文件大小函数(保留两位小数),Size为字节大小
        /// </summary>
        /// <param name="size">初始文件大小</param>
        /// <returns>转换后的字符串</returns>
        public static string GetFileSize(long size)
        {
            var num = 1024.00; //byte

            if (size < num)
                return size + "B";
            if (size < Math.Pow(num, 2))
                return (size / num).ToString("f2") + "K"; //kb
            if (size < Math.Pow(num, 3))
                return (size / Math.Pow(num, 2)).ToString("f2") + "M"; //M
            if (size < Math.Pow(num, 4))
                return (size / Math.Pow(num, 3)).ToString("f2") + "G"; //G

            return (size / Math.Pow(num, 4)).ToString("f2") + "T"; //T
        }

        /// <summary>
        /// 将对象转换为字符串
        /// </summary>
        /// <param name="obj">要转换的对象</param>
        /// <returns>转换后的string类型结果</returns>
        public static Guid ObjectToGuid(object obj)
        {
            if (obj == null || string.IsNullOrEmpty(obj.ToString()))
                return new Guid();
            return new Guid(obj.ToString());
        }
        #endregion

        #region 分割字符串

        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <param name="strContent">字符串内容</param>
        /// <param name="strSplit">分割符</param>
        /// <returns>分割后端字符串</returns>
        public static string[] SplitString(string strContent, string strSplit)
        {
            if (!string.IsNullOrEmpty(strContent))
            {
                if (strContent.IndexOf(strSplit) < 0)
                    return new string[] { strContent };

                return Regex.Split(strContent, Regex.Escape(strSplit), RegexOptions.IgnoreCase);
            }
            else
                return new string[0] { };
        }

        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <param name="strContent">字符串内容</param>
        /// <param name="strSplit">分割符</param>
        /// <returns>分割后端字符串</returns>
        public static string[] SplitString(string strContent, string strSplit, int count)
        {
            string[] result = new string[count];
            string[] splited = SplitString(strContent, strSplit);

            for (int i = 0; i < count; i++)
            {
                if (i < splited.Length)
                    result[i] = splited[i];
                else
                    result[i] = string.Empty;
            }

            return result;
        }
        #endregion

        #region 删除最后结尾的一个逗号

        /// <summary>
        /// 删除最后结尾的一个逗号
        /// </summary>
        /// <param name="str">要处理的字符串</param>
        /// <returns>处理后的字符串</returns>
        public static string DelLastComma(string str)
        {
            return str.Substring(0, str.LastIndexOf(","));
        }
        #endregion

        #region 删除最后结尾的指定字符后的字符
        /// <summary>
        /// 删除最后结尾的指定字符后的字符
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <param name="strchar">指定字符后的字符串</param>
        /// <returns>返回的字符串</returns>
        public static string DelLastChar(string str, string strchar)
        {
            if (string.IsNullOrEmpty(str))
                return "";
            if (str.LastIndexOf(strchar) >= 0 && str.LastIndexOf(strchar) == str.Length - 1)
            {
                return str.Substring(0, str.LastIndexOf(strchar));
            }
            return str;
        }
        #endregion

        #region 生成指定长度的字符串
        /// <summary>
        /// 生成指定长度的字符串,即生成strLong个str字符串
        /// </summary>
        /// <param name="strLong">生成的长度</param>
        /// <param name="str">以str生成字符串</param>
        /// <returns>生成后的字符串</returns>
        public static string StringOfChar(int strLong, string str)
        {
            string ReturnStr = "";
            for (int i = 0; i < strLong; i++)
            {
                ReturnStr += str;
            }

            return ReturnStr;
        }
        #endregion

        #region 生成日期随机码

        /// <summary>
        /// 生成日期随机码
        /// </summary>
        /// <returns>生成后的日期随机码</returns>
        public static string GetRamCode()
        {
            #region
            return DateTime.Now.ToString("yyyyMMddHHmmssffff");
            #endregion
        }
        #endregion

        #region 生成随机字母或数字
        /// <summary>
        /// 生成随机数字
        /// </summary>
        /// <param name="length">生成长度</param>
        /// <returns>生成后的随机数</returns>
        public static string Number(int Length)
        {
            return Number(Length, false);
        }

        /// <summary>
        /// 生成随机数字
        /// </summary>
        /// <param name="Length">生成长度</param>
        /// <param name="Sleep">是否要在生成前将当前线程阻止以避免重复</param>
        /// <returns>生成后的随机数</returns>
        public static string Number(int Length, bool Sleep)
        {
            if (Sleep)
                System.Threading.Thread.Sleep(3);
            string result = "";
            System.Random random = new Random();
            for (int i = 0; i < Length; i++)
            {
                result += random.Next(10).ToString();
            }
            return result;
        }
        /// <summary>
        /// 生成随机字母字符串(数字字母混和)
        /// </summary>
        /// <param name="codeCount">待生成的位数</param>
        /// <returns>生成后的验证码</returns>
        public static string GetCheckCode(int codeCount)
        {
            string str = string.Empty;
            int rep = 0;
            long num2 = DateTime.Now.Ticks + rep;
            rep++;
            Random random = new Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> rep)));
            for (int i = 0; i < codeCount; i++)
            {
                char ch;
                int num = random.Next();
                if ((num % 2) == 0)
                {
                    ch = (char)(0x30 + ((ushort)(num % 10)));
                }
                else
                {
                    ch = (char)(0x41 + ((ushort)(num % 0x1a)));
                }
                str = str + ch.ToString();
            }
            return str;
        }

        /// <summary>
        /// 根据日期和随机码生成订单号
        /// </summary>
        /// <returns>生成后的验证码</returns>
        public static string GetOrderNumber()
        {
            string num = DateTime.Now.ToString("yyMMddHHmmss");//yyyyMMddHHmmssms
            return num + Number(2).ToString();
        }

        #endregion

        #region 截取字符长度
        /// <summary>
        /// 截取字符长度并清除HTML标记
        /// </summary>
        /// <param name="inputString">字符</param>
        /// <param name="len">长度</param>
        /// <returns>返回数据</returns>
        public static string CutString(string inputString, int len, HttpContent httpContent)
        {
            if (string.IsNullOrEmpty(inputString))
                return "";
            inputString = DropHTML(inputString, httpContent);
            ASCIIEncoding ascii = new ASCIIEncoding();
            int tempLen = 0;
            string tempString = "";
            byte[] s = ascii.GetBytes(inputString);
            for (int i = 0; i < s.Length; i++)
            {
                if ((int)s[i] == 63)
                {
                    tempLen += 2;
                }
                else
                {
                    tempLen += 1;
                }

                try
                {
                    tempString += inputString.Substring(i, 1);
                }
                catch
                {
                    break;
                }

                if (tempLen > len)
                    break;
            }
            //如果截过则加上半个省略号 
            byte[] mybyte = System.Text.Encoding.Default.GetBytes(inputString);
            if (mybyte.Length > len)
                tempString += "…";
            return tempString;
        }
        #endregion

        #region 清除HTML标记
        /// <summary>
        /// 正则表达式清除HTML标记
        /// </summary>
        /// <param name="Htmlstring">Html字符串</param>
        /// <param name="httpContent">http上下文</param>
        /// <returns>返回清除HTML标记后的字符串</returns>
        public static string DropHTML(string Htmlstring, HttpContent httpContent)
        {
            if (string.IsNullOrEmpty(Htmlstring)) return "";
            //删除脚本  
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML  
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            Htmlstring.Replace("<", "");
            Htmlstring.Replace(">", "");
            Htmlstring.Replace("\r\n", "");
            Htmlstring = WebUtility.HtmlEncode(Htmlstring).Trim();
            return Htmlstring;
        }
        #endregion

        #region 清除HTML标记且返回相应的长度
        /// <summary>
        /// 清除HTML标记且返回相应的长度
        /// </summary>
        /// <param name="Htmlstring">Html字符串</param>
        /// <param name="strLen">字符串长度</param>
        /// <param name="httpContent">http上下文</param>
        /// <returns>清除HTML标记后的字符串</returns>
        public static string DropHTML(string Htmlstring, int strLen, HttpContent httpContent)
        {
            return CutString(DropHTML(Htmlstring, httpContent), strLen, httpContent);
        }
        #endregion

        #region TXT代码转换成HTML格式
        /// <summary>
        /// 字符串字符处理
        /// </summary>
        /// <param name="chr">等待处理的字符串</param>
        /// <returns>处理后的字符串</returns>
        /// //把TXT代码转换成HTML格式
        public static String ToHtml(string Input)
        {
            StringBuilder sb = new StringBuilder(Input);
            sb.Replace("&", "&amp;");
            sb.Replace("<", "&lt;");
            sb.Replace(">", "&gt;");
            sb.Replace("\r\n", "<br />");
            sb.Replace("\n", "<br />");
            sb.Replace("\t", " ");
            //sb.Replace(" ", "&nbsp;");
            return sb.ToString();
        }
        #endregion

        #region HTML代码转换成TXT格式
        /// <summary>
        /// 字符串字符处理
        /// </summary>
        /// <param name="chr">等待处理的字符串</param>
        /// <returns>处理后的字符串</returns>
        /// //把HTML代码转换成TXT格式
        public static String ToTxt(String Input)
        {
            StringBuilder sb = new StringBuilder(Input);
            sb.Replace("&nbsp;", " ");
            sb.Replace("<br>", "\r\n");
            sb.Replace("<br>", "\n");
            sb.Replace("<br />", "\n");
            sb.Replace("<br />", "\r\n");
            sb.Replace("&lt;", "<");
            sb.Replace("&gt;", ">");
            sb.Replace("&amp;", "&");
            return sb.ToString();
        }
        #endregion

        #region 检测是否有Sql危险字符
        /// <summary>
        /// 检测是否有Sql危险字符
        /// </summary>
        /// <param name="str">要判断字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsSafeSqlString(string str)
        {
            return !Regex.IsMatch(str, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
        }

        /// <summary>
        /// 检查并过滤危险字符
        /// (含有危险SQL字符串则报错)
        /// </summary>
        /// <param name="Input">要检查的字符串</param>
        /// <returns>过滤后的字符串</returns>
        public static string Filter(string sInput)
        {
            if (sInput == null || sInput == "")
                return null;
            string sInput1 = sInput.ToLower();
            string output = sInput;
            string pattern = @"*|and|exec|insert|select|delete|update|count|master|truncate|declare|char(|mid(|chr(|'";
            if (Regex.Match(sInput1, Regex.Escape(pattern), RegexOptions.Compiled | RegexOptions.IgnoreCase).Success)
            {
                throw new Exception("字符串中含有非法字符!");
            }
            else
            {
                output = output.Replace("'", "''");
            }
            return output;
        }

        /// <summary> 
        /// 检查过滤设定的危险字符
        /// </summary> 
        /// <param name="InText">要过滤的字符串 </param> 
        /// <param name="word">指定的危险字符串</param>
        /// <returns>如果参数存在不安全字符，则返回true </returns> 
        public static bool SqlFilter(string word, string InText)
        {
            if (InText == null)
                return false;
            foreach (string i in word.Split('|'))
            {
                if ((InText.ToLower().IndexOf(i + " ") > -1) || (InText.ToLower().IndexOf(" " + i) > -1))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region 过滤特殊字符
        /// <summary>
        /// 过滤特殊字符
        /// </summary>
        /// <param name="Input">要过滤的字符串</param>
        /// <returns>过滤后的字符串</returns>
        public static string Htmls(string Input)
        {
            if (Input != string.Empty && Input != null)
            {
                string ihtml = Input.ToLower();
                ihtml = ihtml.Replace("<script", "&lt;script");
                ihtml = ihtml.Replace("script>", "script&gt;");
                ihtml = ihtml.Replace("<%", "&lt;%");
                ihtml = ihtml.Replace("%>", "%&gt;");
                ihtml = ihtml.Replace("<$", "&lt;$");
                ihtml = ihtml.Replace("$>", "$&gt;");
                return ihtml;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 过滤特殊字符
        /// </summary>
        /// <param name="SourceString">包括HTML，脚本，数据库关键字，特殊字符的源码 </param>
        /// <returns>已经过滤后的字符串</returns>
        public static string FilterSpecialString(string SourceString)
        {
            if (string.IsNullOrEmpty(SourceString))
            {
                return "";
            }
            else
            {
                //删除脚本
                SourceString = Regex.Replace(SourceString, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);

                #region 删除HTML
                SourceString = Regex.Replace(SourceString, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
                SourceString = Regex.Replace(SourceString, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
                SourceString = Regex.Replace(SourceString, @"-->", "", RegexOptions.IgnoreCase);
                SourceString = Regex.Replace(SourceString, @"<!--.*", "", RegexOptions.IgnoreCase);
                SourceString = Regex.Replace(SourceString, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
                SourceString = Regex.Replace(SourceString, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
                SourceString = Regex.Replace(SourceString, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
                SourceString = Regex.Replace(SourceString, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
                SourceString = Regex.Replace(SourceString, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
                SourceString = Regex.Replace(SourceString, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
                SourceString = Regex.Replace(SourceString, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
                SourceString = Regex.Replace(SourceString, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
                SourceString = Regex.Replace(SourceString, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
                SourceString = Regex.Replace(SourceString, @"&#(\d+);", "", RegexOptions.IgnoreCase);
                SourceString = Regex.Replace(SourceString, "xp_cmdshell", "", RegexOptions.IgnoreCase);
                #endregion

                #region 删除与数据库相关的词
                SourceString = Regex.Replace(SourceString, "select", "", RegexOptions.IgnoreCase);
                SourceString = Regex.Replace(SourceString, "insert", "", RegexOptions.IgnoreCase);
                SourceString = Regex.Replace(SourceString, "delete from", "", RegexOptions.IgnoreCase);
                SourceString = Regex.Replace(SourceString, "count''", "", RegexOptions.IgnoreCase);
                SourceString = Regex.Replace(SourceString, "drop table", "", RegexOptions.IgnoreCase);
                SourceString = Regex.Replace(SourceString, "truncate", "", RegexOptions.IgnoreCase);
                SourceString = Regex.Replace(SourceString, "asc", "", RegexOptions.IgnoreCase);
                SourceString = Regex.Replace(SourceString, "mid", "", RegexOptions.IgnoreCase);
                SourceString = Regex.Replace(SourceString, "char", "", RegexOptions.IgnoreCase);
                SourceString = Regex.Replace(SourceString, "xp_cmdshell", "", RegexOptions.IgnoreCase);
                SourceString = Regex.Replace(SourceString, "exec master", "", RegexOptions.IgnoreCase);
                SourceString = Regex.Replace(SourceString, "net localgroup administrators", "", RegexOptions.IgnoreCase);
                SourceString = Regex.Replace(SourceString, "and", "", RegexOptions.IgnoreCase);
                SourceString = Regex.Replace(SourceString, "net user", "", RegexOptions.IgnoreCase);
                SourceString = Regex.Replace(SourceString, "or", "", RegexOptions.IgnoreCase);
                SourceString = Regex.Replace(SourceString, "net", "", RegexOptions.IgnoreCase);
                //SourceString = Regex.Replace(SourceString,"*", "", RegexOptions.IgnoreCase);
                //SourceString = Regex.Replace(SourceString,"-", "", RegexOptions.IgnoreCase);
                SourceString = Regex.Replace(SourceString, "delete", "", RegexOptions.IgnoreCase);
                SourceString = Regex.Replace(SourceString, "drop", "", RegexOptions.IgnoreCase);
                SourceString = Regex.Replace(SourceString, "script", "", RegexOptions.IgnoreCase);
                #endregion

                #region 特殊的字符
                SourceString = SourceString.Replace("<", "");
                SourceString = SourceString.Replace(">", "");
                SourceString = SourceString.Replace("*", "");
                SourceString = SourceString.Replace("-", "");
                SourceString = SourceString.Replace("?", "");
                SourceString = SourceString.Replace(",", "");
                SourceString = SourceString.Replace("/", "");
                SourceString = SourceString.Replace(";", "");
                SourceString = SourceString.Replace("*/", "");
                SourceString = SourceString.Replace("\r\n", "");
                //SourceString = HttpContext.Current.Server.HtmlEncode(SourceString).Trim();
                #endregion

                return SourceString;
            }

        }
        #endregion

        #region 检查是否为IP地址
        /// <summary>
        /// 是否为ip
        /// </summary>
        /// <param name="ip">要检查的字符串</param>
        /// <returns>是否为IP地址</returns>
        public static bool IsIP(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }
        #endregion

        #region 文件操作
        /// <summary>
        /// 返回文件扩展名，不含“.”
        /// </summary>
        /// <param name="_filepath">文件全名称</param>
        /// <returns>文件扩展名</returns>
        public static string GetFileExt(string _filepath)
        {
            if (string.IsNullOrEmpty(_filepath))
            {
                return "";
            }
            if (_filepath.LastIndexOf(".") > 0)
            {
                return _filepath.Substring(_filepath.LastIndexOf(".") + 1); //文件扩展名，不含“.”
            }
            return "";
        }

        /// <summary>
        /// 返回文件名，不含路径
        /// </summary>
        /// <param name="_filepath">文件相对路径</param>
        /// <returns>文件名</returns>
        public static string GetFileName(string _filepath)
        {
            return _filepath.Substring(_filepath.LastIndexOf(@"/") + 1);
        }



        /// <summary>
        /// 返回指定文本文件的内容
        /// </summary>
        /// <param name="FPath">文件路径</param>
        /// <param name="eCode">字符编码对象</param>
        /// <returns>文本文件的内容</returns>
        public static string getTxtFileBody(string FPath, Encoding eCode)
        {
            StreamReader sr = new StreamReader(FPath, eCode);
            string TxtBody = "";
            string TempLine = "";
            while (TempLine != null)
            {
                TempLine = sr.ReadLine();
                if (TempLine != null)
                {
                    TxtBody += TempLine + "\n";
                }
            }
            sr.Close();
            return TxtBody;
        }

        /// <summary>
        /// 返回指定文本文件的内容(不带换行符)
        /// </summary>
        /// <param name="FPath">文件路径</param>
        /// <param name="eCode">字符编码对象</param>
        /// <returns>文本文件的内容</returns>
        public static string getTxtFileBody1(string FPath, Encoding eCode)
        {
            StreamReader sr = new StreamReader(FPath, eCode);
            string TxtBody = "";
            string TempLine = "";
            while (TempLine != null)
            {
                TempLine = sr.ReadLine();
                if (TempLine != null)
                {
                    TxtBody += TempLine;
                }
            }
            sr.Close();
            return TxtBody;
        }

        /// <summary>
        /// 写入制定的文本到文件里
        /// </summary>
        /// <param name="append">是否追加</param>
        /// <param name="Body">文件内容</param>
        /// <param name="FPath">文件路径</param>
        /// <param name="FileEncoding">文件字符串编码</param>
        /// <returns>文本文件内容</returns>
        public static string WriteTextToFile(string FPath, bool append, string Body, System.Text.Encoding FileEncoding)
        {
            string Message = String.Empty;
            try
            {
                StreamWriter sr = new StreamWriter(FPath, append, FileEncoding);
                sr.Write(Body);
                sr.Close();
            }
            catch (Exception exp)
            {
                Message = exp.Message;
            }
            return Message;
        }

        /// <summary>
        /// 写入制定的文本到文件里
        /// </summary>
        /// <param name="append">是否追加</param>
        /// <param name="Body">文件内容</param>
        /// <param name="FPath">文件路径</param>
        /// <returns>文本文件内容</returns>
        public static string WriteTextToFile(string FPath, bool append, string Body)
        {
            string Message = String.Empty;
            try
            {
                StreamWriter sr = new StreamWriter(FPath, append, Encoding.GetEncoding("gb2312"));
                sr.Write(Body);
                sr.Close();
            }
            catch (Exception exp)
            {
                Message = exp.Message;
            }
            return Message;
        }
        #endregion

        #region 读取或写入cookie
        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="strValue">值</param>
        /// <param name="httpContext">http上下文</param>
        /// <param name="expires">过期时间</param>
        public static void WriteCookie(string strName, string strValue, HttpContext httpContext, int? expires = null)
        {
            var cookie = httpContext.Request.Cookies[strName];
            CookieOptions cookieOptions = null;
            if (expires != null)
            {
                cookieOptions = new CookieOptions()
                {
                    Expires = DateTime.Now.AddMinutes(expires.GetValueOrDefault())
                };
            }
            if (cookie == null)
            {
                strValue = UrlEncode(strValue);
                httpContext.Response.Cookies.Append(strName, strValue);
            }
            else
            {
                httpContext.Response.Cookies.Delete(strName);
                httpContext.Response.Cookies.Append(strName, strValue);
            }
        }

        /// <summary>
        /// 读cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="httpContext">http上下文</param>
        /// <returns>cookie值</returns>
        public static string GetCookie(string strName, HttpContext httpContext)
        {
            if (httpContext.Request.Cookies != null && httpContext.Request.Cookies[strName] != null)
                return UrlDecode(httpContext.Request.Cookies[strName].ToString());
            return "";
        }

        /// <summary>
        /// 读cookie值
        /// </summary>
        /// <param name="httpContext">http上下文</param>
        /// <param name="key">键</param>
        /// <param name="strName">名称</param>
        /// <returns>cookie值</returns>
        public static string GetCookie(string strName, string key, HttpContext httpContext)
        {
            if (httpContext.Request.Cookies != null && httpContext.Request.Cookies[strName] != null && httpContext.Request.Cookies[strName].Any(item => item.ToString() == key))
                return UrlDecode(httpContext.Request.Cookies[strName].First(item => item.ToString() == key).ToString());
            return "";
        }
        #endregion

        #region 替换指定的字符串
        /// <summary>
        /// 替换指定的字符串
        /// </summary>
        /// <param name="originalStr">原字符串</param>
        /// <param name="oldStr">旧字符串</param>
        /// <param name="newStr">新字符串</param>
        /// <returns>替换后的字符串</returns>
        public static string ReplaceStr(string originalStr, string oldStr, string newStr)
        {
            if (string.IsNullOrEmpty(oldStr))
            {
                return "";
            }
            return originalStr.Replace(oldStr, newStr);
        }
        #endregion

        #region URL处理
        /// <summary>
        /// URL字符编码
        /// </summary>
        /// <param name="str">未编码的URL字符串</param>
        /// <returns>编码后的字符串</returns>
        public static string UrlEncode(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }
            str = str.Replace("'", "");
            return HttpUtility.UrlEncode(str);
        }

        /// <summary>
        /// URL字符解码
        /// </summary>
        /// <param name="str">未解码的URL字符串</param>
        /// <returns>编码后的字符串</returns>
        public static string UrlDecode(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }
            return HttpUtility.UrlDecode(str);
        }

        /// <summary>
        /// 组合URL参数
        /// </summary>
        /// <param name="_url">页面地址</param>
        /// <param name="_keys">参数名称</param>
        /// <param name="_values">参数值</param>
        /// <returns>组合后的url地址字符串</returns>
        public static string CombUrlTxt(string _url, string _keys, params string[] _values)
        {
            StringBuilder urlParams = new StringBuilder();
            try
            {
                string[] keyArr = _keys.Split(new char[] { '&' });
                for (int i = 0; i < keyArr.Length; i++)
                {
                    if (!string.IsNullOrEmpty(_values[i]) && _values[i] != "0")
                    {
                        _values[i] = UrlEncode(_values[i]);
                        urlParams.Append(string.Format(keyArr[i], _values) + "&");
                    }
                }
                if (!string.IsNullOrEmpty(urlParams.ToString()) && _url.IndexOf("?") == -1)
                    urlParams.Insert(0, "?");
            }
            catch
            {
                return _url;
            }
            return _url + DelLastChar(urlParams.ToString(), "&");
        }
        #endregion

        #region URL请求数据
        /// <summary>
        /// HTTP POST方式请求数据
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="param">POST的数据</param>
        /// <returns>返回的字符串数据</returns>
        public static string HttpPost(string url, string param)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Timeout = 15000;
            request.AllowAutoRedirect = false;

            StreamWriter requestStream = null;
            WebResponse response = null;
            string responseStr = null;

            try
            {
                requestStream = new StreamWriter(request.GetRequestStream());
                requestStream.Write(param);
                requestStream.Close();

                response = request.GetResponse();
                if (response != null)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    responseStr = reader.ReadToEnd();
                    reader.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                request = null;
                requestStream = null;
                response = null;
            }

            return responseStr;
        }

        /// <summary>
        /// HTTP GET方式请求数据.
        /// </summary>
        /// <param name="url">请求的URL地址</param>
        /// <returns>返回值</returns>
        public static string HttpGet(string url)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "GET";
            //request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Timeout = 15000;
            request.AllowAutoRedirect = false;

            WebResponse response = null;
            string responseStr = null;

            try
            {
                response = request.GetResponse();

                if (response != null)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    responseStr = reader.ReadToEnd();
                    reader.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                request = null;
                response = null;
            }

            return responseStr;
        }
        #endregion

        #region 时间日期
        /// <summary>
        /// 时间比较函数(VB.NET二次封装)
        /// </summary>
        /// <param name="Interval">时间间隔</param>
        /// <param name="Time1">时间1</param>
        /// <param name="Time2">时间2</param>
        /// <returns>相差时间</returns>
        public static long DateDiff(string vInterval, DateTime Time1, DateTime Time2)
        {
            long result = -1;
            switch (vInterval)
            {
                case "dd":
                    result = DateAndTime.DateDiff(DateInterval.Day, Time1.Date, Time2.Date);
                    break;
                case "yyyy":
                    result = DateAndTime.DateDiff(DateInterval.Year, Time1, Time2);
                    break;
                case "mm":
                    result = DateAndTime.DateDiff(DateInterval.Minute, Time1, Time2);
                    break;
                case "MM":
                    result = DateAndTime.DateDiff(DateInterval.Month, Time1, Time2);
                    break;
                case "hh":
                    result = DateAndTime.DateDiff(DateInterval.Hour, Time1, Time2);
                    break;
                case "Weekday":
                    result = DateAndTime.DateDiff(DateInterval.Weekday, Time1, Time2);
                    break;
                case "WeekOfYear":
                    result = DateAndTime.DateDiff(DateInterval.WeekOfYear, Time1, Time2);
                    break;
                case "DayOfYear":
                    result = DateAndTime.DateDiff(DateInterval.DayOfYear, Time1, Time2);
                    break;
                case "Quarter":
                    result = DateAndTime.DateDiff(DateInterval.Quarter, Time1, Time2);
                    break;
                case "ss":
                    result = DateAndTime.DateDiff(DateInterval.Second, Time1, Time2);
                    break;
            }

            return result;
        }

        /// <summary>
        /// 时间比较函数
        /// </summary>
        /// <param name="interval">比较单位</param>
        /// <param name="date1">时间1</param>
        /// <param name="date2">时间2</param>
        /// <returns>时间间隔</returns>
        public static long DateDiff(DateInterval interval, DateTime date1, DateTime date2)
        {

            TimeSpan ts = date2 - date1;

            switch (interval)
            {
                case DateInterval.Year:
                    return date2.Year - date1.Year;
                case DateInterval.Month:
                    return (date2.Month - date1.Month) + (12 * (date2.Year - date1.Year));
                case DateInterval.Weekday:
                    return Fix(ts.TotalDays) / 7;
                case DateInterval.Day:
                    return Fix(ts.TotalDays);
                case DateInterval.Hour:
                    return Fix(ts.TotalHours);
                case DateInterval.Minute:
                    return Fix(ts.TotalMinutes);
                default:
                    return Fix(ts.TotalSeconds);
            }
        }

        /// <summary>
        /// 以日期为单位返回时间范围
        /// </summary>
        /// <param name="time1">时间1</param>
        /// <param name="time2">时间2</param>
        /// <param name="FormatDateString">格式化字符串</param>
        /// <param name="message">错误消息</param>
        /// <returns>格式后的字符串</returns>
        public static string getTimeRangeByFormat(DateTime time1, DateTime time2, string FormatDateString, out string message)
        {
            string result = string.Empty;
            long timeRange = -1;
            message = string.Empty;
            DateTime StartTime = DateTime.MinValue;
            DateTime EndTime = DateTime.MinValue;
            if (string.IsNullOrEmpty(FormatDateString))
            {
                message = "参数:FormatDateString不能为空";
                return string.Empty;
            }
            try
            {
                if (DateDiff("dd", time1, time2) == 0)
                {
                    return time1.ToString(FormatDateString);
                }
                else if (DateDiff("dd", time1, time2) < 0)
                {
                    StartTime = time2;
                    EndTime = time1;
                    timeRange = DateDiff("dd", time2, time1);
                }
                else if (DateDiff("dd", time1, time2) > 0)
                {
                    StartTime = time1;
                    EndTime = time2;
                    timeRange = DateDiff("dd", time1, time2);
                }
                while (DateDiff("dd", StartTime, EndTime) > 0)
                {
                    StartTime = StartTime.AddDays(1);
                    result += "'" + StartTime.ToString(FormatDateString) + "',";
                }
                if (!string.IsNullOrEmpty(result))
                    result = result.Substring(0, result.Length - 1);
            }
            catch (Exception exp)
            {
                message = exp.Message;
                return string.Empty;
            }
            return result;
        }

        /// <summary>
        /// 判断是否是日期格式
        /// </summary>
        /// <param name="strDate">要判断的字符串</param>
        /// <returns>是否是日期</returns>
        public static bool IsDate(string strDate)
        {
            try
            {
                DateTime.Parse(strDate);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 正则判断

        /// <summary>
        /// 判断是否是整数
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>是否整数</returns>
        public static bool IsIntNum(string str)
        {
            System.Text.RegularExpressions.Regex reg1
            = new System.Text.RegularExpressions.
            Regex(@"^[-]?[1-9]{1}\d*$|^[0]{1}$");
            bool ismatch = reg1.IsMatch(str);
            return ismatch;
        }
        #endregion

        #region 数学计算函数
        /// <summary>
        /// 实现数据的四舍五入法
        /// </summary>
        /// <param name="v">要进行处理的数据</param>
        /// <param name="x">保留的小数位数</param>
        /// <returns>四舍五入后的结果</returns>
        public static double Round(double v, int x)
        {
            bool isNegative = false;
            //如果是负数
            if (v < 0)
            {
                isNegative = true;
                v = -v;
            }
            int IValue = 1;
            for (int i = 1; i <= x; i++)
            {
                IValue = IValue * 10;
            }
            double Int = Math.Round(v * IValue + 0.5, 0);
            v = Int / IValue;
            if (isNegative)
            {
                v = -v;
            }
            return v;
        }
        #endregion

        #region 转换数据类型

        /// <summary>
        /// 转换为decimal
        /// </summary>
        /// <param name="obj">要转换的object</param>
        /// <returns>转换后的decimal类型结果</returns>
        public decimal ConvertToDecimal(object obj)
        {
            try
            {
                return Convert.ToDecimal(obj);
            }
            catch (Exception)
            {
                return Convert.ToDecimal("0.00");
            }
        }

        /// <summary>
        /// 将string转换为decimal
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <returns>转换后的int类型结果</returns>
        public static decimal StrToDecimal(string str)
        {
            decimal dt = 0;
            if (string.IsNullOrEmpty(str))
                return dt;
            if (!decimal.TryParse(str, out dt))
                return 0;
            return dt;
        }

        /// <summary>
        /// 将string转换为long
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <returns>转换后的long类型结果</returns>
        public static long StrToLong(string str)
        {
            long dt = 0;
            if (string.IsNullOrEmpty(str))
                return dt;
            if (!long.TryParse(str, out dt))
                return 0;
            return dt;
        }

        /// <summary>
        /// 将string转换为short
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <returns>转换后的short类型结果</returns>
        public static short StrToShort(string str)
        {
            short dt = 0;
            if (string.IsNullOrEmpty(str))
                return dt;
            if (!short.TryParse(str, out dt))
                return 0;
            return dt;
        }

        /// <summary>
        /// 将string转换为Single
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <returns>转换后的short类型结果</returns>
        public static Single StrToSingle(string str)
        {
            Single dt = 0;
            if (string.IsNullOrEmpty(str))
                return dt;
            if (!Single.TryParse(str, out dt))
                return 0;
            return dt;
        }

        /// <summary>
        /// 将string转换为UInt16
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <returns>转换后的short类型结果</returns>
        public static UInt16 StrToUInt16(string str)
        {
            UInt16 dt = 0;
            if (string.IsNullOrEmpty(str))
                return dt;
            if (!UInt16.TryParse(str, out dt))
                return 0;
            return dt;
        }

        /// <summary>
        /// 将string转换为UInt32
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <returns>转换后的short类型结果</returns>
        public static UInt32 StrToUInt32(string str)
        {
            UInt32 dt = 0;
            if (string.IsNullOrEmpty(str))
                return dt;
            if (!UInt32.TryParse(str, out dt))
                return 0;
            return dt;
        }

        /// <summary>
        /// 将string转换为UInt32
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <returns>转换后的short类型结果</returns>
        public static UInt64 StrToUInt64(string str)
        {
            UInt64 dt = 0;
            if (string.IsNullOrEmpty(str))
                return dt;
            if (!UInt64.TryParse(str, out dt))
                return 0;
            return dt;
        }

        /// <summary>
        /// 将string转换为Double
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <returns>转换后的short类型结果</returns>
        public static Double StrToDouble(string str)
        {
            Double dt = 0;
            if (string.IsNullOrEmpty(str))
                return dt;
            if (!Double.TryParse(str, out dt))
                return 0;
            return dt;
        }

        /// <summary>
        /// 将string转换为Guid
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <returns>转换后的short类型结果</returns>
        public static Guid StrToGuid(string str)
        {
            Guid dt = Guid.Empty;
            if (string.IsNullOrEmpty(str))
                return dt;
            if (!Guid.TryParse(str, out dt))
                return dt;
            return dt;
        }

        /// <summary>
        /// 将string转换为Double
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <returns>转换后的short类型结果</returns>
        public static Byte StrToByte(string str)
        {
            Byte dt = 0;
            if (string.IsNullOrEmpty(str))
                return dt;
            if (!Byte.TryParse(str, out dt))
                return 0;
            return dt;
        }
        #endregion

        #region 数据库方法

        /// <summary>
        /// 获得数据行
        /// </summary>
        /// <param name="dr">数据行</param>
        /// <param name="columnName">列名</param>
        /// <returns>返回值</returns>
        public static object GetDataRow(DataRow dr, string columnName)
        {
            return Convert.IsDBNull(dr[columnName]) ? null : dr[columnName];
        }
        #endregion

        #endregion

        #region Private

        #region 随机数
        /// <summary>
        /// 微软系统方法生成随机数
        /// </summary>
        /// <param name="numSeeds">随机因子</param>
        /// <param name="length">随机数长度</param>
        /// <returns>生成后的随机数</returns>
        private static int Next(int numSeeds, int length)
        {
            byte[] buffer = new byte[length];
            System.Security.Cryptography.RNGCryptoServiceProvider Gen = new System.Security.Cryptography.RNGCryptoServiceProvider();
            Gen.GetBytes(buffer);
            uint randomResult = 0x0;//这里用uint作为生成的随机数  
            for (int i = 0; i < length; i++)
            {
                randomResult |= ((uint)buffer[i] << ((length - 1 - i) * 8));
            }
            return (int)(randomResult % numSeeds);
        }
        #endregion

        #region 数学函数
        /// <summary>
        /// 取整函数
        /// </summary>
        /// <param name="Number">要取整的数字</param>
        /// <returns>取整后的结果</returns>
        private static long Fix(double Number)
        {
            if (Number >= 0)
            {
                return (long)Math.Floor(Number);
            }
            return (long)Math.Ceiling(Number);
        }
        #endregion

        #endregion
    }

}
