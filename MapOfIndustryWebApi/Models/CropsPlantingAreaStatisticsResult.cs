namespace MapOfIndustryWebApi.Models
{
    /// <summary>
    /// 农作物种植面积统计结果
    /// </summary>
    public class CropsPlantingAreaStatisticsResult
    {
        /// <summary>
        /// 农作物名称
        /// </summary>
        public string CropsName { get; set; }

        /// <summary>
        /// 种植面积统计
        /// </summary>
        public decimal PlantingAreaCount { get; set; }
    }
}
