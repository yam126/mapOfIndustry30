/// <summary>
/// 接口返回值
/// </summary>
namespace MapOfIndustryWebApi.Models.Result
{
    /// <summary>
    /// 普通添加修改返回值
    /// </summary>
    public class Result
    {
        /// <summary>
        /// 状态（0正确，非0错误）
        /// </summary>
        public Int32 Status { get; set; }

        /// <summary>
        /// 错误提示信息
        /// </summary>
        public String Msg { get; set; }
    }

    /// <summary>
    /// 带数据返回
    /// </summary>
    /// <typeparam name="T">返回数据类型</typeparam>
    public class EntityResult<T>
    {
        /// <summary>
        /// 状态（0正确，非0错误）
        /// </summary>
        public Int32 Status { get; set; }

        /// <summary>
        /// 错误提示信息
        /// </summary>
        public String Msg { get; set; }

        /// <summary>
        /// 返回数据
        /// </summary>
        public T Result { get; set; }
    }

    /// <summary>
    /// 返回数组
    /// </summary>
    /// <typeparam name="T">要返回的数组类型</typeparam>
    public class ArrayResult<T>
    {
        /// <summary>
        /// 状态（0正确，非0错误）
        /// </summary>
        public Int32 Status { get; set; }

        /// <summary>
        /// 错误提示信息
        /// </summary>
        public String Msg { get; set; }

        /// <summary>
        /// 返回的数组
        /// </summary>
        public T[] Result { get; set; }
    }

    /// <summary>
    /// List返回值
    /// </summary>
    /// <typeparam name="T">返回的数据类型</typeparam>
    public class ListResult<T>
    {
        /// <summary>
        /// 状态（0正确，非0错误）
        /// </summary>
        public Int32 Status { get; set; }

        /// <summary>
        /// 错误提示信息
        /// </summary>
        public String Msg { get; set; }


        /// <summary>
        /// 返回的List
        /// </summary>
        public IList<T> Result { get; set; } = new List<T>();
    }

    /// <summary>
    /// 分页返回值
    /// </summary>
    /// <typeparam name="T">返回的数据类型</typeparam>
    public class PageResult<T>
    {
        /// <summary>
        /// 状态（0正确，非0错误）
        /// </summary>
        public Int32 Status { get; set; }

        /// <summary>
        /// 错误提示信息
        /// </summary>
        public String Msg { get; set; }

        /// <summary>
        /// 页面记录数
        /// </summary>
        public Int32 PageCount { get; set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        public Int32 RecordCount { get; set; }

        /// <summary>
        /// 分页的每页数据
        /// </summary>
        public T[] Result { get; set; }
    }

    /// <summary>
    /// 平均价格
    /// </summary>
    public class AveragePrice
    {
        /// <summary>
        /// 农作物名称
        /// </summary>
        public string CropsName { get; set; }

        /// <summary>
        /// 年份数据
        /// </summary>
        public List<int> YearsData { get; set; }

        /// <summary>
        /// 价格数据
        /// </summary>
        public List<decimal> PriceData { get; set; }
    }
}
