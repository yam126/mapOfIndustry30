using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapOfIndustryDataAccess.Models
{
    /// <summary>
    /// 公司信息表 
    /// </summary>
    [Serializable]
    public partial class CompanyInfo
    {
        /// <summary>
        /// 雪花ID
        /// </summary>
        public System.Int64? RecordId { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        public System.String CompanyName { get; set; }

        /// <summary>
        /// 企业类型
        /// </summary>
        public System.String EnterpriseType { get; set; }

        /// <summary>
        /// 注册资本
        /// </summary>
        public System.Decimal? RegisteredCapital { get; set; }

        /// <summary>
        /// 成立时间
        /// </summary>
        public System.DateTime? FoundedTime { get; set; }

        /// <summary>
        ///所属乡镇
        /// </summary>
        public System.String TownShip { get; set; }

        /// <summary>
        /// 所属行业
        /// </summary>
        public System.String Industry { get; set; }

        /// <summary>
        /// 区域级别
        /// </summary>
        public System.String RegionalLevel { get; set; }

        /// <summary>
        /// 企业性质
        /// </summary>
        public System.String EnterpriseNature { get; set; }

        /// <summary>
        ///员工人数
        /// </summary>
        public System.Int32? EmployeesNum { get; set; }

        /// <summary>
        ///联系人
        /// </summary>
        public System.String Contacts { get; set; }

        /// <summary>
        ///联系电话
        /// </summary>
        public System.String ContactPhone { get; set; }

        /// <summary>
        ///企业地址
        /// </summary>
        public System.String EnterpriseAddress { get; set; }

        /// <summary>
        ///企业简介
        /// </summary>
        public System.String EnterpriseIntroduction { get; set; }

        /// <summary>
        ///焦点图地址Json
        /// </summary>
        public System.String FocusImages { get; set; }

        /// <summary>
        ///本年度销售额度
        /// </summary>
        public System.Decimal? CurrentYearSales { get; set; }

        /// <summary>
        ///公司类别[A类、B类、C类根据销售本年度销售额度判断A类销售额度大于800万、B类销售额度在500到800万之间、C类在200到500万之间]
        /// </summary>
        public System.String CompanyType { get; set; }

        /// <summary>
        ///地图经度
        /// </summary>
        public System.Double? Lng { get; set; }

        /// <summary>
        ///地图纬度
        /// </summary>
        public System.Double? Lat { get; set; }

        /// <summary>
        ///视频地址
        /// </summary>
        public System.String videoUrl { get; set; }

        /// <summary>
        ///备用字段01
        /// </summary>
        public System.String Backup01 { get; set; }

        /// <summary>
        ///备用字段02
        /// </summary>
        public System.String Backup02 { get; set; }

        /// <summary>
        ///备用字段03
        /// </summary>
        public System.String Backup03 { get; set; }

        /// <summary>
        ///备用字段04
        /// </summary>
        public System.String Backup04 { get; set; }

        /// <summary>
        ///备用字段05
        /// </summary>
        public System.String Backup05 { get; set; }

        /// <summary>
        ///备用字段06
        /// </summary>
        public System.String Backup06 { get; set; }

        /// <summary>
        ///备用字段07
        /// </summary>
        public System.String Backup07 { get; set; }

        /// <summary>
        ///备用字段08
        /// </summary>
        public System.String Backup08 { get; set; }

        /// <summary>
        ///备用字段09
        /// </summary>
        public System.String Backup09 { get; set; }

        /// <summary>
        ///添加人
        /// </summary>
        public System.String Created { get; set; }

        /// <summary>
        ///添加时间
        /// </summary>
        public System.DateTime? CreatedTime { get; set; }

        /// <summary>
        ///修改人
        /// </summary>
        public System.String Modifier { get; set; }

        /// <summary>
        ///修改时间
        /// </summary>
        public System.DateTime? ModifiedTime { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CountData
    {
        /// <summary>
        /// 年
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<SowData> SowDatas { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SowData
    {
        public Int32 Id { get; set; }

        /// <summary>
        /// 乡镇名称
        /// </summary>
        public String RegionName { get; set; }

        /// <summary>
        /// 种植面积
        /// </summary>
        public decimal SowArea { get; set; }

        /// <summary>
        /// 种植面积百分比
        /// </summary>
        public decimal SowAreaPercentage { get; set; }


        /// <summary>
        /// 产量
        /// </summary>
        public decimal Yield { get; set; }

        /// <summary>
        /// 产量百分比
        /// </summary>
        public decimal YieldPercentage { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class PlantArea
    {
        /// <summary>
        /// 总计
        /// </summary>
        public decimal CountArea { get; set; }

        public List<RegionArea> RegionAreas { get; set; }

    }

    /// <summary>
    /// 农作物种植面积统计
    /// </summary>
    public class CropsPlandArea
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

    /// <summary>
    /// 
    /// </summary>
    public class RegionArea
    {
        public Int32 Id { get; set; }
        
        /// <summary>
        /// 乡镇名称
        /// </summary>
        public String RegionName { get; set; }

        /// <summary>
        /// 种植面积
        /// </summary>
        public decimal SowArea { get; set; }

        /// <summary>
        /// 种植面积百分比
        /// </summary>
        public decimal SowAreaPercentage { get; set; }
    }

    /// <summary>
    /// 地块儿与温室大棚产值数据 
    /// </summary>
    [Serializable]
    public partial class MassifGreenHouseVP
    {
        /// <summary>
        ///自增编号
        /// </summary>
        public System.Int64? ID { get; set; }

        /// <summary>
        ///地块编号_1条地块对应多条数据
        /// </summary>
        public System.Int32? LandId { get; set; }

        /// <summary>
        ///地块名称
        /// </summary>
        public System.String LandName { get; set; }

        /// <summary>
        ///农作物名称
        /// </summary>
        public System.String CropsName { get; set; }

        /// <summary>
        ///农作物的种植面积_单位亩
        /// </summary>
        public System.Decimal? PlantingArea { get; set; }

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

        /// <summary>
        ///录入时间_录入数据的时间_用户可编辑
        /// </summary>
        public System.DateTime? EnterTime { get; set; }

        /// <summary>
        ///添加人
        /// </summary>
        public System.String Creater { get; set; }

        /// <summary>
        ///添加时间
        /// </summary>
        public System.DateTime? CreatedTime { get; set; }

        /// <summary>
        ///修改人
        /// </summary>
        public System.String Modifier { get; set; }

        /// <summary>
        ///修改时间
        /// </summary>
        public System.DateTime? ModifiedTime { get; set; }
    }

    /// <summary>
    /// 地块儿与温室大棚产值数据 
    /// </summary>
    [Serializable]
    public partial class MOI_massifGreenHouseVP
    {
        /// <summary>
        ///自增编号
        /// </summary>
        public System.Int64? ID { get; set; }

        /// <summary>
        ///地块编号_1条地块对应多条数据
        /// </summary>
        public System.Int32? LandId { get; set; }

        /// <summary>
        ///农作物名称
        /// </summary>
        public System.String CropsName { get; set; }

        /// <summary>
        ///农作物的种植面积_单位亩
        /// </summary>
        public System.Decimal? PlantingArea { get; set; }

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

        /// <summary>
        ///录入时间_录入数据的时间_用户可编辑
        /// </summary>
        public System.DateTime? EnterTime { get; set; }

        /// <summary>
        ///添加人
        /// </summary>
        public System.String Creater { get; set; }

        /// <summary>
        ///添加时间
        /// </summary>
        public System.DateTime? CreatedTime { get; set; }

        /// <summary>
        ///修改人
        /// </summary>
        public System.String Modifier { get; set; }

        /// <summary>
        ///修改时间
        /// </summary>
        public System.DateTime? ModifiedTime { get; set; }
    }

    /// <summary>
    /// 地块儿信息
    /// </summary>
    [Serializable]
    public partial class osc_region
    {

        /// <summary>
        ///编号数据
        /// </summary>
        public System.Int32? id { get; set; }

        /// <summary>
        ///地块儿名称
        /// </summary>
        public System.String? name { get; set; }

        /// <summary>
        ///上级地块儿编号
        /// </summary>
        public System.Int32? parent_id { get; set; }

        /// <summary>
        ///城市编号
        /// </summary>
        public System.String? citycode { get; set; }

        /// <summary>
        ///区域编码
        /// </summary>
        public System.String? adcode { get; set; }

        /// <summary>
        ///中心坐标
        /// </summary>
        public System.String? center { get; set; }

        /// <summary>
        ///级别
        /// </summary>
        public System.String? level { get; set; }

        /// <summary>
        ///创建人
        /// </summary>
        public System.String? created { get; set; }

        /// <summary>
        ///创建时间
        /// </summary>
        public System.DateTime? CreatedTime { get; set; }

        /// <summary>
        ///修改人
        /// </summary>
        public System.String? Modifier { get; set; }

        /// <summary>
        ///修改时间
        /// </summary>
        public System.DateTime? ModifiedTime { get; set; }

        /// <summary>
        ///是否根地块儿_0否_1是
        /// </summary>
        public System.Int32? IsRoot { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class vw_MassifGreenHouseVP
    {


        /// <summary>
        /// 
        /// </summary>
        public long ID { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public int LandId { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string LandName { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string CropsName { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public decimal PlantingArea { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public int JobPopulation { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public int TotalPopulation { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public decimal TotalOutput { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public decimal TotaValue { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string SoilType { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string WateringType { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string LandProperty { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public double CropOutput { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public int LeaseYear { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string LastYearOutput { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentYearOutput { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public decimal CropsSalesPrice { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public DateTime EnterTime { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string Creater { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public DateTime CreatedTime { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string Modifier { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public DateTime ModifiedTime { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public int parent_id { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string citycode { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string adcode { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string level { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string center { get; set; }

    }

    /// <summary>
    /// 地图边界版本号 
    /// </summary>
    [Serializable]
    public partial class MapLocationVerison
    {

        /// <summary>
        ///自增编号
        /// </summary>
        public System.Int32? Id { get; set; }

        /// <summary>
        ///地图边界编号
        /// </summary>
        public System.Int32? MapLocalVerison { get; set; }

        /// <summary>
        ///创建人
        /// </summary>
        public System.String? Created { get; set; }

        /// <summary>
        ///创建时间
        /// </summary>
        public System.DateTime? CreatedTime { get; set; }

        /// <summary>
        ///地块儿编号
        /// </summary>
        public System.Int32? landId { get; set; }
    }

    /// <summary>
    /// 地块数据
    /// </summary>
    [Serializable]
    public class vw_osc_region
    {


        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public int Parent_id { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string Citycode { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string Adcode { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string Center { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string Level { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string GPSLocations { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public int LandId { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string LandName { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string CropsName { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public decimal PlantingArea { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public int JobPopulation { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public int TotalPopulation { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public decimal TotalOutput { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public decimal TotaValue { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string SoilType { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string WateringType { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string LandProperty { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public int LeaseYear { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public double CropOutput { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string LastYearOutput { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentYearOutput { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public decimal CropsSalesPrice { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public DateTime EnterTime { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string Creater { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public DateTime CreatedTime { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string Modifier { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public DateTime ModifiedTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Color { get; set; }

    }

    /// <summary>
    /// 地块儿分组视图
    /// </summary>
    [Serializable]
    public class vw_osc_region_group
    {


        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public int Parent_id { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string Citycode { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string Adcode { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string Center { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string Level { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string GPSLocations { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string CropsName { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string SoilType { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string WateringType { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string LandProperty { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public decimal PlantingArea { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public decimal LeaseYear { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public decimal CropOutput { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public decimal LastYearOutput { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public decimal CurrentYearOutput { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public decimal CropsSalesPrice { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public decimal JobPopulation { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public decimal TotalPopulation { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public decimal TotalOutput { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public decimal TotaValue { get; set; }

    }

    /// <summary>
    /// 地块儿坐标范围数据 
    /// </summary>
    [Serializable]
    public partial class region_gis
    {

        /// <summary>
        ///编号
        /// </summary>
        public System.Int32? Id { get; set; }

        /// <summary>
        ///地块儿编号
        /// </summary>
        public System.Int32? landId { get; set; }

        /// <summary>
        ///GPS坐标范围数据
        /// </summary>
        public System.String? GPSLocations { get; set; }

        /// <summary>
        /// Color颜色
        /// </summary>
        public System.String? Color { get; set; }
    }
}
