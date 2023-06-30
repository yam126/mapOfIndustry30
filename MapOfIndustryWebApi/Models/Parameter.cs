/// <summary>
/// 接口参数
/// </summary>
namespace mapOfIndustryWebApi.Models.Parameter
{
    /// <summary>
    /// 地块儿和温室大棚接口参数
    /// </summary>
    public class MassifGreenHouseVPParameter
    {
        /// <summary>
        /// 数据库自增编号
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        ///地块编号_1条地块对应多条数据
        /// </summary>
        public String LandId { get; set; }

        /// <summary>
        /// 地块名称
        /// </summary>
        public String LandName { get; set; }

        /// <summary>
        ///农作物名称
        /// </summary>
        public String CropsName { get; set; }
        /// <summary>
        ///农作物的种植面积_单位亩
        /// </summary>
        public String PlantingArea { get; set; }

        /// <summary>
        ///务工人口数量
        /// </summary>
        public String JobPopulation { get; set; }

        /// <summary>
        ///总人口数量
        /// </summary>
        public String TotalPopulation { get; set; }

        /// <summary>
        ///总产量
        /// </summary>
        public String TotalOutput { get; set; }

        /// <summary>
        ///总产值_农作物的总产值
        /// </summary>
        public String TotaValue { get; set; }

        /// <summary>
        ///土壤类型
        /// </summary>
        public String SoilType { get; set; }

        /// <summary>
        ///灌溉类型
        /// </summary>
        public String WateringType { get; set; }

        /// <summary>
        ///土地属性_租赁或者购买
        /// </summary>
        public String LandProperty { get; set; }

        /// <summary>
        ///土地租赁年限
        /// </summary>
        public String LeaseYear { get; set; }

        /// <summary>
        ///同一作物种植年限
        /// </summary>
        public String CropOutput { get; set; }

        /// <summary>
        ///上一年度作物产量
        /// </summary>
        public String LastYearOutput { get; set; }

        /// <summary>
        ///本年度作物产量
        /// </summary>
        public String CurrentYearOutput { get; set; }

        /// <summary>
        ///农作物售价_单位公斤
        /// </summary>
        public String CropsSalesPrice { get; set; }

        /// <summary>
        ///录入时间_录入数据的时间_用户可编辑
        /// </summary>
        public String EnterTime { get; set; }

        /// <summary>
        ///添加人
        /// </summary>
        public System.String Creater { get; set; }
    }

    /// <summary>
    /// Action方法参数类
    /// </summary>
    [Serializable]
    public class ActionParameter
    {
        /// <summary>
        /// 参数名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 参数值
        /// </summary>
        public object Value { get; set; }
    }
}
