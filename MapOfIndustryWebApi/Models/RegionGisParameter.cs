namespace MapOfIndustryWebApi.Models
{
    /// <summary>
    /// 地块儿GIS范围坐标参数类
    /// </summary>
    public class RegionGisParameter
    {
        /// <summary>
        /// 地儿编号
        /// </summary>
        public string landId { get; set; }

        /// <summary>
        /// GPS范围坐标文本字符串
        /// </summary>
        public string GPSLocations { get; set; }

        /// <summary>
        /// 地块儿颜色
        /// </summary>
        public string Color { get; set; }
    }
}
