using AutoMapper;
using Common;
using MapOfIndustryDataAccess.Models;
using mapOfIndustryWebApi.Models.Parameter;
using MapOfIndustryWebApi.Models;
using System.Data.SqlTypes;

namespace MapOfIndustryWebApi
{
    /// <summary>
    /// 实现参数到数据库实体类的自动映射
    /// </summary>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// 构造函数(用于添加需要映射的类对应关系)
        /// </summary>
        public MappingProfile()
        {
            #region 地块儿和温室大棚参数和实体类映射关系
            CreateMap<MassifGreenHouseVPParameter, MassifGreenHouseVP>()
                .ForMember(target => target.ID, map => map.MapFrom(source => string.IsNullOrEmpty(source.ID) ? 0 : Convert.ToInt32(source.ID)))
                .ForMember(target => target.LandId, map => map.MapFrom(source => string.IsNullOrEmpty(source.LandId) ? 0 : Convert.ToInt32(source.LandId)))
                .ForMember(target => target.LandName, map => map.MapFrom(source => source.LandName))
                .ForMember(target => target.CropsName, map => map.MapFrom(source => source.CropsName))
                .ForMember(target => target.PlantingArea, map => map.MapFrom(source => string.IsNullOrEmpty(source.PlantingArea) ? 0 : Convert.ToDecimal(source.PlantingArea)))
                .ForMember(target => target.JobPopulation, map => map.MapFrom(source => string.IsNullOrEmpty(source.JobPopulation) ? 0 : Convert.ToInt32(source.JobPopulation)))
                .ForMember(target => target.TotalPopulation, map => map.MapFrom(source => string.IsNullOrEmpty(source.JobPopulation) ? 0 : Convert.ToInt32(source.TotalPopulation)))
                .ForMember(target => target.TotalOutput, map => map.MapFrom(source => string.IsNullOrEmpty(source.TotalOutput) ? 0 : Convert.ToDecimal(source.TotalOutput)))
                .ForMember(target => target.TotaValue, map => map.MapFrom(source => string.IsNullOrEmpty(source.TotaValue) ? 0 : Convert.ToDecimal(source.TotaValue)))
                .ForMember(target => target.SoilType, map => map.MapFrom(source => source.SoilType))
                .ForMember(target => target.WateringType, map => map.MapFrom(source => source.WateringType))
                .ForMember(target => target.LandProperty, map => map.MapFrom(source => source.LandProperty))
                .ForMember(target => target.LeaseYear, map => map.MapFrom(source => string.IsNullOrEmpty(source.LeaseYear) ? 0 : Convert.ToInt32(source.LeaseYear)))
                .ForMember(target => target.CropOutput, map => map.MapFrom(source => string.IsNullOrEmpty(source.CropOutput) ? 0 : Convert.ToDouble(source.CropOutput)))
                .ForMember(target => target.LastYearOutput, map => map.MapFrom(source => source.LastYearOutput))
                .ForMember(target => target.CurrentYearOutput, map => map.MapFrom(source => string.IsNullOrEmpty(source.CurrentYearOutput) ? 0 : Convert.ToDecimal(source.CurrentYearOutput)))
                .ForMember(target => target.CropsSalesPrice, map => map.MapFrom(source => string.IsNullOrEmpty(source.CropsSalesPrice) ? 0 : Convert.ToDecimal(source.CropsSalesPrice)))
                .ForMember(target => target.EnterTime, map => map.MapFrom(source => string.IsNullOrEmpty(source.EnterTime) ? (DateTime)SqlDateTime.MinValue : Convert.ToDateTime(source.EnterTime)))
                .ForMember(target => target.Creater, map => map.MapFrom(source => source.Creater))
                .ReverseMap();
            #endregion

            #region 地块儿映射
            CreateMap<OscRegionItem, osc_region>()
                .ForMember(target => target.id, map => map.MapFrom(source => string.IsNullOrEmpty(source.id) ? 0 : Convert.ToInt32(source.id)))
                .ForMember(target => target.name, map => map.MapFrom(source => source.name))
                .ForMember(target => target.parent_id, map => map.MapFrom(source => string.IsNullOrEmpty(source.parent_id) ? 0 : Convert.ToInt32(source.parent_id)))
                .ForMember(target => target.citycode, map => map.MapFrom(source => source.citycode))
                .ForMember(target => target.adcode, map => map.MapFrom(source => source.adcode))
                .ForMember(target => target.center, map => map.MapFrom(source => source.center))
                .ForMember(target => target.level, map => map.MapFrom(source => source.level))
                .ReverseMap();
            #endregion

            #region 公司信息和实体类映射关系
            CreateMap<CompanyInfoParameter, CompanyInfo>()
            .ForMember(target => target.RecordId, map => map.MapFrom(source => string.IsNullOrEmpty(source.RecordId) ? 0 : Convert.ToInt64(source.RecordId)))
            .ForMember(target => target.CompanyName, map => map.MapFrom(source => string.IsNullOrEmpty(source.CompanyName) ? string.Empty : Convert.ToString(source.CompanyName).Trim()))
            .ForMember(target => target.EnterpriseType, map => map.MapFrom(source => string.IsNullOrEmpty(source.EnterpriseType) ? string.Empty : Convert.ToString(source.EnterpriseType).Trim()))
            .ForMember(target => target.RegisteredCapital, map => map.MapFrom(source => Utils.StrToDecimal(source.RegisteredCapital, 0.00M)))
            .ForMember(target => target.FoundedTime, map => map.MapFrom(source => Utils.StrToDateTime(source.FoundedTime).ToString("yyyy-MM-dd")))
            .ForMember(target => target.TownShip, map => map.MapFrom(source => string.IsNullOrEmpty(source.TownShip) ? string.Empty : Convert.ToString(source.TownShip).Trim()))
            .ForMember(target => target.Industry, map => map.MapFrom(source => string.IsNullOrEmpty(source.Industry) ? string.Empty : Convert.ToString(source.Industry).Trim()))
            .ForMember(target => target.RegionalLevel, map => map.MapFrom(source => string.IsNullOrEmpty(source.RegionalLevel) ? string.Empty : Convert.ToString(source.RegionalLevel).Trim()))
            .ForMember(target => target.EnterpriseNature, map => map.MapFrom(source => string.IsNullOrEmpty(source.EnterpriseNature) ? string.Empty : Convert.ToString(source.EnterpriseNature).Trim()))
            .ForMember(target => target.EmployeesNum, map => map.MapFrom(source => string.IsNullOrEmpty(source.EmployeesNum) ? 0 : Convert.ToInt32(source.EmployeesNum)))
            .ForMember(target => target.Contacts, map => map.MapFrom(source => string.IsNullOrEmpty(source.Contacts) ? string.Empty : Convert.ToString(source.Contacts).Trim()))
            .ForMember(target => target.ContactPhone, map => map.MapFrom(source => string.IsNullOrEmpty(source.ContactPhone) ? string.Empty : Convert.ToString(source.ContactPhone).Trim()))
            .ForMember(target => target.EnterpriseAddress, map => map.MapFrom(source => string.IsNullOrEmpty(source.EnterpriseAddress) ? string.Empty : Convert.ToString(source.EnterpriseAddress).Trim()))
            .ForMember(target => target.EnterpriseIntroduction, map => map.MapFrom(source => string.IsNullOrEmpty(source.EnterpriseIntroduction) ? string.Empty : Convert.ToString(source.EnterpriseIntroduction).Trim()))
            .ForMember(target => target.FocusImages, map => map.MapFrom(source => string.IsNullOrEmpty(source.FocusImages) ? string.Empty : Convert.ToString(source.FocusImages).Trim()))
            .ForMember(target => target.CurrentYearSales, map => map.MapFrom(source => Utils.StrToDecimal(source.CurrentYearSales, 0.00M).ToString("0.00")))
            .ForMember(target => target.CompanyType, map => map.MapFrom(source => string.IsNullOrEmpty(source.CompanyType) ? string.Empty : Convert.ToString(source.CompanyType).Trim()))
            .ForMember(target => target.Lng, map => map.MapFrom(source => Utils.StrToDouble(source.Lng).ToString("0.00")))
            .ForMember(target => target.Lat, map => map.MapFrom(source => Utils.StrToDouble(source.Lat).ToString("0.00")))
            .ForMember(target => target.videoUrl, map => map.MapFrom(source => string.IsNullOrEmpty(source.videoUrl) ? string.Empty : Convert.ToString(source.videoUrl).Trim()))
            .ForMember(target => target.Backup01, map => map.MapFrom(source => string.IsNullOrEmpty(source.Backup01) ? string.Empty : Convert.ToString(source.Backup01).Trim()))
            .ForMember(target => target.Backup02, map => map.MapFrom(source => string.IsNullOrEmpty(source.Backup02) ? string.Empty : Convert.ToString(source.Backup02).Trim()))
            .ForMember(target => target.Backup03, map => map.MapFrom(source => string.IsNullOrEmpty(source.Backup03) ? string.Empty : Convert.ToString(source.Backup03).Trim()))
            .ForMember(target => target.Backup04, map => map.MapFrom(source => string.IsNullOrEmpty(source.Backup04) ? string.Empty : Convert.ToString(source.Backup04).Trim()))
            .ForMember(target => target.Backup05, map => map.MapFrom(source => string.IsNullOrEmpty(source.Backup05) ? string.Empty : Convert.ToString(source.Backup05).Trim()))
            .ForMember(target => target.Backup06, map => map.MapFrom(source => string.IsNullOrEmpty(source.Backup06) ? string.Empty : Convert.ToString(source.Backup06).Trim()))
            .ForMember(target => target.Backup07, map => map.MapFrom(source => string.IsNullOrEmpty(source.Backup07) ? string.Empty : Convert.ToString(source.Backup07).Trim()))
            .ForMember(target => target.Backup08, map => map.MapFrom(source => string.IsNullOrEmpty(source.Backup08) ? string.Empty : Convert.ToString(source.Backup08).Trim()))
            .ForMember(target => target.Backup09, map => map.MapFrom(source => string.IsNullOrEmpty(source.Backup09) ? string.Empty : Convert.ToString(source.Backup09).Trim()))
            .ForMember(target => target.Created, map => map.MapFrom(source => string.IsNullOrEmpty(source.Created) ? string.Empty : Convert.ToString(source.Created).Trim()))
            .ForMember(target => target.CreatedTime, map => map.MapFrom(source => Utils.StrToDateTime(source.CreatedTime).ToString("yyyy-MM-dd")))
            .ForMember(target => target.Modifier, map => map.MapFrom(source => string.IsNullOrEmpty(source.Modifier) ? string.Empty : Convert.ToString(source.Modifier).Trim()))
            .ForMember(target => target.ModifiedTime, map => map.MapFrom(source => Utils.StrToDateTime(source.ModifiedTime).ToString("yyyy-MM-dd")))
            .ReverseMap();
            #endregion

            #region 地图边界版本号
            CreateMap<MapLocationVerisonParameter, MapLocationVerison>()
                .ForMember(target => target.Id, map => map.MapFrom(source => Utils.StrToInt(source.Id, -1)))
                .ForMember(target => target.MapLocalVerison, map => map.MapFrom(source => Utils.StrToInt(source.MapLocalVerison, 0)))
                .ForMember(target => target.Created, map => map.MapFrom(source => string.IsNullOrEmpty(source.Created) ? string.Empty : Convert.ToString(source.Created).Trim()))
                .ForMember(target => target.CreatedTime, map => map.MapFrom(source => Utils.StrToDateTime(source.CreatedTime).ToString("yyyy-MM-dd")))
                .ReverseMap();
            #endregion
        }
    }
}
