namespace MapOfIndustryWebApi.Models
{
    /// <summary>
    /// 产值统计数据
    /// </summary>
    public class OutputValueStatisticsResult
    {
        /// <summary>
        /// 总产值
        /// </summary>
        public string TotalOutputValue { get; set; }

        /// <summary>
        /// 统计年份
        /// </summary>
        public int StatisticYear { get; set; }
    }

    /// <summary>
    /// 统计信息
    /// </summary>
    public class TotalStatisticsInfo
    {
        /// <summary>
        /// 总产量
        /// </summary>
        public decimal TotalOutputValue { get; set; }

        /// <summary>
        /// 总产值
        /// </summary>
        public decimal TotaValue { get; set; }
    }

    /// <summary>
    /// 种植面积统计
    /// </summary>
    public class PlantingAreaStatisticsResult
    {
        /// <summary>
        /// 地块儿名称
        /// </summary>
        public string LandName { get; set; }

        /// <summary>
        /// 种植面积统计
        /// </summary>
        public string PlantingAreaStatistics { get; set; }
    }

    /// <summary>
    /// 地块儿统计
    /// </summary>
    public class landStatistics
    {
        /// <summary>
        /// 地块儿编号
        /// </summary>
        public int landId { get; set; }

        /// <summary>
        /// 地块儿名称
        /// </summary>
        public string landName { get; set; }

        /// <summary>
        /// 务工人口
        /// </summary>
        public decimal jobPopulation { get; set; }

        /// <summary>
        /// 总人口
        /// </summary>
        public decimal totalPopulation { get; set; }

        /// <summary>
        /// 总产量
        /// </summary>
        public decimal totalOutput { get; set; }

        /// <summary>
        /// 总产值
        /// </summary>
        public decimal totaValue { get; set; }

        /// <summary>
        /// 农作物列表
        /// </summary>
        public List<landCropsStatistics> cropsList { get; set; }
    }

    /// <summary>
    /// 地块儿统计值
    /// </summary>
    public class landCropsStatistics
    {
        /// <summary>
        /// 农作物名称
        /// </summary>
        public string cropsName { get; set; }

        /// <summary>
        /// 种植面积
        /// </summary>
        public decimal plantingArea { get; set; }

        /// <summary>
        /// 土地属性
        /// </summary>
        public string soilType { get; set; }

        /// <summary>
        /// 灌溉类型
        /// </summary>
        public string wateringType { get; set; }

        /// <summary>
        /// 土地属性
        /// </summary>
        public string landProperty { get; set; }

        public string leaseYear { get; set; }

        public string cropOutput { get; set; }

        public string lastYearOutput { get; set; }

        public decimal currentYearOutput { get; set; }

        public decimal cropsSalesPrice { get; set; }
    }
}
