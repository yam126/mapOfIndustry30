using Npoi.Mapper.Attributes;

namespace MapOfIndustryWebApi.Models
{
    /// <summary>
    /// 城市列表信息
    /// </summary>
    public class OscRegionItem
    {
        /// <summary>
        /// 编号
        /// </summary>
        [Column("编号")]
        public string id { get; set; }

        /// <summary>
        /// 地块儿名字 
        ///</summary>
        [Column("地块儿名字")]
        public string name { get; set; }

        /// <summary>
        /// 上级ID
        /// </summary>
        [Column("上级地块儿ID")]
        public string parent_id { get; set; }

        /// <summary>
        /// 区号
        /// </summary>
        [Column("区号")]
        public string citycode { get; set; }

        /// <summary>
        /// 城市编码
        /// </summary>
        [Column("城市编码")]
        public string adcode { get; set; }

        /// <summary>
        /// 经纬度
        /// </summary>
        [Column("经纬度")]
        public string center { get; set; }

        /// <summary>
        /// 地区层级
        /// </summary>
        [Column("地区层级")]
        public string level { get; set; }
    }

    /// <summary>
    /// 城市列表信息
    /// </summary>
    public class OscRegion
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 城市名字 
        ///</summary>
        public string name { get; set; }

        /// <summary>
        /// 上级ID
        /// </summary>
        public string parent_id { get; set; }

        /// <summary>
        /// 区号
        /// </summary>
        public string citycode { get; set; }

        /// <summary>
        /// 城市编码
        /// </summary>
        public string adcode { get; set; }

        /// <summary>
        /// 经纬度
        /// </summary>
        public string center { get; set; }

        /// <summary>
        /// 地区显示
        /// </summary>
        public string level { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string created { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreatedTime { get; set; }


        /// <summary>
        ///修改人
        /// </summary>
        public string Modifier { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public string ModifiedTime { get; set; }
    }

    /// <summary>
    /// 地块儿视图
    /// </summary>
    public class OscRegionNew
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 城市名字 
        ///</summary>
        public string regionName { get; set; }

        /// <summary>
        /// 上级ID
        /// </summary>
        public string parent_id { get; set; }

        /// <summary>
        /// 区号
        /// </summary>
        public string citycode { get; set; }

        /// <summary>
        /// 城市编码
        /// </summary>
        public string adcode { get; set; }

        /// <summary>
        /// 经纬度
        /// </summary>
        public string center { get; set; }

        /// <summary>
        /// 地区显示
        /// </summary>
        public string level { get; set; }

        /// <summary>
        /// GPS位置范围数据
        /// </summary>
        public string GPSLocations { get; set; }

        /// <summary>
        /// 地块儿颜色
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// 统计信息
        /// </summary>
        public GroupResult StatisticalInfo { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class OscRegionRoot
    {
        /// <summary>
        /// 
        /// </summary>
        public List<OscRegion> Record { get; set; }
    }
}
