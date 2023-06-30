namespace MapOfIndustryWebApi.Models
{
    /// <summary>
    /// 分组返回值
    /// </summary>
    public class GroupResult
    {
        /// <summary>
        /// 地块编号
        /// </summary>
        public int LandId { get; set; }

        /// <summary>
        /// 地块名称
        /// </summary>
        public string LandName { get; set; }

        /// <summary>
        ///务工人口数量
        /// </summary>
        public System.Int32? JobPopulation { get; set; }
        /// <summary>
        ///总人口数量
        /// </summary>
        public System.Int32? TotalPopulation { get; set; }

        /// <summary>
        ///总产量
        /// </summary>
        public System.Decimal? TotalOutput { get; set; }

        /// <summary>
        ///总产值_农作物的总产值
        /// </summary>
        public System.Decimal? TotaValue { get; set; }

        /// <summary>
        /// 农作物列表
        /// </summary>
        public List<CropsInfo> CropsList { get; set; }
    }

    /// <summary>
    /// 农作物信息
    /// </summary>
    public class CropsInfo
    {
        /// <summary>
        /// 农作物名称
        /// </summary>
        public string CropsName { get; set; }

        /// <summary>
        ///农作物的种植面积_单位亩
        /// </summary>
        public System.Decimal? PlantingArea { get; set; }

        /// <summary>
        ///土壤类型
        /// </summary>
        public System.String SoilType { get; set; }

        /// <summary>
        ///灌溉类型
        /// </summary>
        public System.String WateringType { get; set; }

        /// <summary>
        ///土地属性_租赁或者购买
        /// </summary>
        public System.String LandProperty { get; set; }

        /// <summary>
        ///土地租赁年限
        /// </summary>
        public System.Int32? LeaseYear { get; set; }

        /// <summary>
        ///同一作物种植年限
        /// </summary>
        public System.Double? CropOutput { get; set; }

        /// <summary>
        ///上一年度作物产量
        /// </summary>
        public System.String LastYearOutput { get; set; }

        /// <summary>
        ///本年度作物产量
        /// </summary>
        public System.Decimal? CurrentYearOutput { get; set; }

        /// <summary>
        ///农作物售价_单位公斤
        /// </summary>
        public System.Decimal? CropsSalesPrice { get; set; }
    }
}
