using AutoMapper;
using Common;
using ePioneer.Data.Kernel;
using MapOfIndustryDataAccess.Data;
using MapOfIndustryDataAccess.Models;
using mapOfIndustryWebApi.Models.Parameter;
using MapOfIndustryWebApi.Filter;
using MapOfIndustryWebApi.Models;
using MapOfIndustryWebApi.Models.Result;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Text;

namespace MapOfIndustryWebApi.Controllers
{
    /// <summary>
    /// 产业一张图接口
    /// </summary>
    [Route("api/map-of-industrys")]
    [ApiController]
    public class MapOfIndustrysController : Controller
    {
        #region Fields

        /// <summary>
        /// 数据库操作类
        /// </summary>
        private IMOIRepository m_repository;

        /// <summary>
        /// AutoMapper参数映射类
        /// </summary>
        private IMapper m_mapper;
        #endregion

        #region Constructors

        /// <summary>
        /// 重载构造函数
        /// </summary>
        /// <param name="repository">数据库操作类</param>
        public MapOfIndustrysController(IMOIRepository repository, IMapper _mapper)
        {
            m_repository = repository;
            m_mapper = _mapper == null ? throw new ArgumentNullException(nameof(_mapper)) : _mapper;
        }

        #endregion

        #region Method

        #region Post
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="parameter">添加的参数</param>
        /// <returns></returns>
        [EnableCors("Cors")]
        [HttpPost]
        [LogFilter]
        public async Task<ActionResult<Result>> Add([FromBody] MassifGreenHouseVPParameter parameter)
        {
            return await SaveData("Add", parameter);
        }
        #endregion

        #region Put
        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="ID">编号</param>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        [EnableCors("Cors")]
        [HttpPut("{ID}")]
        public async Task<ActionResult<Result>> Modify(int ID, [FromBody] MassifGreenHouseVPParameter parameter)
        {
            parameter.ID = ID.ToString();
            return await SaveData("Edit", parameter);
        }
        #endregion

        #region Get

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pageIndex">当前页(默认从1开始)</param>
        /// <param name="pageSize">每页记录数(默认每页10条)</param>
        /// <param name="where">查询条件(SQL查询条件,可以为空,为空返回所有数据)</param>
        /// <param name="SortField">排序字段(默认创建时间)</param>
        /// <param name="SortMethod">排序方法[DESC|ASC(默认DESC)]</param>
        /// <returns>返回查询结果</returns>
        [HttpGet("page")]
        public async Task<ActionResult<PageResult<MassifGreenHouseVPParameter>>> GetPage(
            string? where = "",
            int? pageIndex = 1,
            int? pageSize = 10,
            string sortField = "CreatedTime",
            string sortMethod = "DESC"
            )
        {
            #region 声明变量

            //总页数
            int pageCount = 0;

            //总记录数
            int totalRecordCount = 0;

            //方法返回错误消息
            string message = string.Empty;

            //错误消息
            string checkMessage = string.Empty;

            //页面返回值
            List<MassifGreenHouseVP> pageData = null;

            //接口返回值
            List<MassifGreenHouseVPParameter> pageResultData = null;

            //返回值
            var result = new PageResult<MassifGreenHouseVPParameter>();
            #endregion

            #region 非空验证
            if (pageIndex == null)
                checkMessage += "当前页、";
            if (pageSize == null)
                checkMessage += "每页记录数、";
            if (!string.IsNullOrEmpty(checkMessage))
            {
                checkMessage = checkMessage.Substring(0, checkMessage.Length - 1);
                result = new PageResult<MassifGreenHouseVPParameter>()
                {
                    Status = -1,
                    PageCount = 0,
                    RecordCount = 0,
                    Msg = $"非空验证出错，原因[{checkMessage}]",
                    Result = null
                };
                return result;
            }
            #endregion

            #region 有效验证
            if (pageIndex <= 0)
                checkMessage += "当前页不能小于或等于0、";
            if (pageSize <= 0)
                checkMessage += "每页记录数不能小于或等于0、";
            if (!string.IsNullOrEmpty(checkMessage))
            {
                checkMessage = checkMessage.Substring(0, checkMessage.Length - 1);
                result = new PageResult<MassifGreenHouseVPParameter>()
                {
                    Status = -1,
                    PageCount = 0,
                    RecordCount = 0,
                    Msg = $"有效验证出错，原因[{checkMessage}]",
                    Result = null
                };
                return result;
            }
            #endregion

            #region 查询数据
            if (!string.IsNullOrEmpty(where) && where.IndexOf("LandId") == -1)
                where = $" LandName like '%{where}%' Or CropsName like '%{where}%' ";
            else if (!string.IsNullOrEmpty(where) && where.IndexOf("LandId") != -1)
            {
                string LandId = where.Split('_')[1];
                where = $" LandId in (select LandId from vw_MassifGreenHouseVP where LandId='{LandId}' Or parent_id='{LandId}' ) ";
            }
            pageData = m_repository.QueryMassifGreenHouseVP(
                where == null ? string.Empty : where,
                sortField,
                sortMethod,
                pageSize.GetValueOrDefault(),
                pageIndex.GetValueOrDefault(),
                out totalRecordCount,
                out message
                );
            if (pageData == null || pageData.Count <= 0)
            {
                if (!string.IsNullOrEmpty(message))
                {
                    result = new PageResult<MassifGreenHouseVPParameter>()
                    {
                        Status = -1,
                        PageCount = 0,
                        RecordCount = 0,
                        Msg = $"查询数据出错，原因[{message}]",
                        Result = null
                    };
                    return result;
                }
            }
            #endregion

            #region 计算总页数
            if (totalRecordCount % pageSize == 0)
                pageCount = Convert.ToInt32(totalRecordCount / pageSize);
            else
                pageCount = Convert.ToInt32(totalRecordCount / pageSize) + 1;
            #endregion

            #region 赋值返回数据
            pageResultData = new List<MassifGreenHouseVPParameter>();
            pageData.ForEach(item =>
            {
                pageResultData.Add(m_mapper.Map<MassifGreenHouseVP, MassifGreenHouseVPParameter>(item));
            });
            result = new PageResult<MassifGreenHouseVPParameter>()
            {
                Status = 0,
                PageCount = pageCount,
                RecordCount = totalRecordCount,
                Msg = String.Empty,
                Result = pageResultData.ToArray()
            };
            #endregion

            return result;
        }

        /// <summary>
        /// 获得地块儿农作物统计信息
        /// </summary>
        /// <param name="landId">地块儿编号</param>
        /// <returns>返回值</returns>
        [HttpGet("land/crops/statistics")]
        public async Task<ActionResult<EntityResult<landStatistics>>> GetLandCropsStatisticsAsync(int landId)
        {
            #region 声明变量

            //错误消息
            string message = string.Empty;

            //查询语句
            string SqlWhere = string.Empty;

            //数据库返回值
            List<vw_osc_region_group> databaseResult = null;

            //返回值
            EntityResult<landStatistics> result = null;
            #endregion

            #region 非空验证
            if (landId <= 0)
            {
                result = new EntityResult<landStatistics>()
                {
                    Status = -1,
                    Msg = "地块儿编号不能小于等于0",
                    Result = null
                };
                return result;
            }
            #endregion

            #region 数据库查询
            SqlWhere = $" Id='{landId}' ";
            databaseResult = m_repository.QueryViewOscRegionGroup(SqlWhere, out message);
            if (databaseResult == null || databaseResult.Count <= 0)
            {
                if (string.IsNullOrEmpty(message))
                    message = $"查询不到地块儿编号为[{landId}]的数据";
                else
                    message = $"查询地块儿编号为[{landId}]的数据出错，原因[{message}]";
                result = new EntityResult<landStatistics>()
                {
                    Status = -1,
                    Msg = message,
                    Result = null
                };
                return result;
            }
            #endregion

            #region 分组数据
            var groupData = databaseResult.GroupBy(oscRegionGroup => oscRegionGroup.Id)
                .Select(oscRegionItem => new
                {
                    id = oscRegionItem.First().Id,
                    name = oscRegionItem.First().Name,
                    parent_id = oscRegionItem.First().Parent_id,
                    citycode = oscRegionItem.First().Citycode,
                    adcode = oscRegionItem.First().Adcode,
                    center = oscRegionItem.First().Center,
                    level = oscRegionItem.First().Level,
                    GPSLocations = oscRegionItem.First().GPSLocations,
                    StatisticalInfo = oscRegionItem.Select(statisticalInfoItem => new
                    {
                        LandId = oscRegionItem.First().Id,
                        LandName = oscRegionItem.First().Name,
                        JobPopulation = oscRegionItem.Sum(sum => sum.JobPopulation),
                        TotalPopulation = oscRegionItem.Sum(sum => sum.TotalPopulation),
                        TotalOutput = oscRegionItem.Sum(sum => sum.TotalOutput),
                        TotaValue = oscRegionItem.Sum(sum => sum.TotaValue),
                        CropsList = oscRegionItem.GroupBy(cropsGroup => cropsGroup.CropsName).Select(cropsInfo => new
                        {
                            CropsName = cropsInfo.First().CropsName,
                            PlantingArea = cropsInfo.Sum(sum => sum.PlantingArea),
                            SoilType = cropsInfo.First().SoilType,
                            WateringType = cropsInfo.First().WateringType,
                            LandProperty = cropsInfo.First().LandProperty,
                            LeaseYear = cropsInfo.Sum(sum => sum.LeaseYear),
                            CropOutput = cropsInfo.Sum(sum => sum.CropOutput),
                            LastYearOutput = cropsInfo.Sum(sum => sum.LastYearOutput),
                            CurrentYearOutput = cropsInfo.Sum(sum => sum.CurrentYearOutput),
                            CropsSalesPrice = cropsInfo.Sum(sum => sum.CropsSalesPrice)
                        })
                    })
                });
            #endregion

            #region 赋值返回值
            landStatistics oscRegionItem = new landStatistics();
            oscRegionItem.landId = groupData.First().id;
            oscRegionItem.landName = groupData.First().name;
            oscRegionItem.totalPopulation = groupData.First().StatisticalInfo.First().TotalPopulation;
            oscRegionItem.totalOutput = groupData.First().StatisticalInfo.First().TotalOutput;
            oscRegionItem.totaValue = groupData.First().StatisticalInfo.First().TotaValue;
            oscRegionItem.cropsList = new List<landCropsStatistics>();
            foreach (var cropsInfoItem in groupData.First().StatisticalInfo.First().CropsList)
            {
                landCropsStatistics cropsInfo = new landCropsStatistics()
                {
                    cropsName = cropsInfoItem.CropsName,
                    cropOutput = Math.Round(cropsInfoItem.CropOutput, 2).ToString(),
                    wateringType = cropsInfoItem.WateringType,
                    landProperty = cropsInfoItem.LandProperty,
                    currentYearOutput = cropsInfoItem.CurrentYearOutput,
                    lastYearOutput = Math.Round(cropsInfoItem.LastYearOutput, 2).ToString(),
                    leaseYear = cropsInfoItem.LeaseYear.ToString(),
                    cropsSalesPrice = cropsInfoItem.CropsSalesPrice,
                    plantingArea = cropsInfoItem.PlantingArea,
                    soilType = cropsInfoItem.SoilType
                };
                oscRegionItem.cropsList.Add(cropsInfo);
            }
            #endregion

            result = new EntityResult<landStatistics>()
            {
                Status = 0,
                Msg = string.Empty,
                Result = oscRegionItem
            };
            return result;
        }

        /// <summary>
        /// 获得地块儿信息
        /// </summary>
        /// <param name="landId">地块儿编号</param>
        [HttpGet("vw-osc-region-redis")]
        public async Task<ActionResult<ListResult<OscRegionNew>>> GetOSCRegionByLandIdRedis(int landId) 
        {
            #region 声明变量

            //是否有新版本
            bool isHaveNewVerison = false;

            //错误消息
            string message = string.Empty;

            //查询语句
            string SqlWhere = string.Empty;

            //缓存键值
            string RedisKey = string.Empty;

            //json字符串
            string jsonString = string.Empty;

            //数据库返回值
            List<vw_osc_region> databaseResult = null;

            //返回数据列表
            List<OscRegionNew> resultList = new List<OscRegionNew>();

            //返回值
            ListResult<OscRegionNew> result = new ListResult<OscRegionNew>();

            //地图版本
            List<MapLocationVerison> locationVerisons= new List<MapLocationVerison>();

            //Redis缓存对象
            RedisHelper redisHelper = null;
            #endregion

            RedisKey = $"RDKEY_landId_{landId}";

            #region 非空验证
            if (landId <= 0)
            {
                result = new ListResult<OscRegionNew>()
                {
                    Status = -1,
                    Msg = "地块儿编号不能小于等于0",
                    Result = null
                };
                return result;
            }
            #endregion

            #region 读取地图版本
            SqlWhere = $" landId='{landId}' ";
            locationVerisons = m_repository.QueryMapLocationVerison(SqlWhere, out message);
            if (locationVerisons == null || locationVerisons.Count <= 0) 
            {
                if (!string.IsNullOrEmpty(message)) 
                {
                    result = new ListResult<OscRegionNew>()
                    {
                        Status = -1,
                        Msg = $"查询地块儿编号为[{landId}]的地图数据出错,原因[{message}]",
                        Result = null
                    };
                    return result;
                }
            }
            #endregion

            #region 同步地图版本
            SyncRedisMapLocationVerison(
                landId, 
                locationVerisons, 
                out isHaveNewVerison, 
                out message);
            #endregion

            if (isHaveNewVerison)
            {

                #region 数据库查询
                SqlWhere = $" Id='{landId}' Or Parent_id='{landId}' ";
                databaseResult = m_repository.QueryViewOscRegion(SqlWhere, out message);
                if (databaseResult == null || databaseResult.Count <= 0)
                {
                    if (string.IsNullOrEmpty(message))
                        message = $"查询不到地块儿编号为[{landId}]的数据";
                    else
                        message = $"查询地块儿编号为[{landId}]的数据出错，原因[{message}]";
                    result = new ListResult<OscRegionNew>()
                    {
                        Status = -1,
                        Msg = message,
                        Result = null
                    };
                    return result;
                }
                #endregion

                #region 分组数据
                var groupData = databaseResult
                    .GroupBy(oscRegionGroup => oscRegionGroup.LandId)
                    .Select(oscRegionItem => new
                    {
                        id = oscRegionItem.First().Id,
                        name = oscRegionItem.First().LandName,
                        parent_id = oscRegionItem.First().Parent_id,
                        citycode = oscRegionItem.First().Citycode,
                        adcode = oscRegionItem.First().Adcode,
                        center = oscRegionItem.First().Center,
                        level = oscRegionItem.First().Level,
                        GPSLocations = oscRegionItem.First().GPSLocations,
                        StatisticalInfo = oscRegionItem.Select(statisticalInfoItem => new
                        {
                            LandId = oscRegionItem.First().LandId,
                            LandName = oscRegionItem.First().LandName,
                            JobPopulation = oscRegionItem.Sum(sum => sum.JobPopulation),
                            TotalPopulation = oscRegionItem.Sum(sum => sum.TotalPopulation),
                            TotalOutput = oscRegionItem.Sum(sum => sum.TotalOutput),
                            TotaValue = oscRegionItem.Sum(sum => sum.TotaValue),
                            CropsList = oscRegionItem.GroupBy(cropsGroup => cropsGroup.CropsName).Select(cropsInfo => new
                            {
                                CropsName = cropsInfo.First().CropsName,
                                PlantingArea = cropsInfo.Sum(sum => sum.PlantingArea),
                                SoilType = cropsInfo.First().SoilType,
                                WateringType = cropsInfo.First().WateringType,
                                LandProperty = cropsInfo.First().LandProperty,
                                LeaseYear = cropsInfo.Sum(sum => sum.LeaseYear),
                                CropOutput = cropsInfo.Sum(sum => sum.CropOutput),
                                LastYearOutput = cropsInfo.Sum(sum => Utils.StrToDecimal(sum.LastYearOutput)),
                                CurrentYearOutput = cropsInfo.Sum(sum => sum.CurrentYearOutput),
                                CropsSalesPrice = cropsInfo.Sum(sum => sum.CropsSalesPrice)
                            })
                        })
                    });
                #endregion

                #region 循环赋值返回值
                foreach (var groupItem in groupData)
                {
                    OscRegionNew oscRegionItem = new OscRegionNew();
                    oscRegionItem.id = Convert.ToString(groupItem.id);
                    oscRegionItem.regionName = groupItem.name;
                    oscRegionItem.level = groupItem.level;
                    oscRegionItem.parent_id = Convert.ToString(groupItem.parent_id);
                    oscRegionItem.GPSLocations = groupItem.GPSLocations;
                    oscRegionItem.StatisticalInfo = new GroupResult();
                    oscRegionItem.StatisticalInfo.LandId = groupItem.StatisticalInfo.First().LandId;
                    oscRegionItem.StatisticalInfo.LandName = groupItem.StatisticalInfo.First().LandName;
                    oscRegionItem.StatisticalInfo.JobPopulation = groupItem.StatisticalInfo.First().JobPopulation;
                    oscRegionItem.StatisticalInfo.TotalPopulation = groupItem.StatisticalInfo.First().TotalPopulation;
                    oscRegionItem.StatisticalInfo.TotalOutput = Math.Round(groupItem.StatisticalInfo.First().TotalOutput, 2);
                    oscRegionItem.StatisticalInfo.TotaValue = Math.Round(groupItem.StatisticalInfo.First().TotaValue, 2);
                    oscRegionItem.StatisticalInfo.CropsList = new List<CropsInfo>();
                    foreach (var cropsInfoItem in groupItem.StatisticalInfo.First().CropsList)
                    {
                        CropsInfo cropsInfo = new CropsInfo();
                        cropsInfo.CropsName = cropsInfoItem.CropsName;
                        cropsInfo.SoilType = cropsInfoItem.SoilType;
                        cropsInfo.WateringType = cropsInfoItem.WateringType;
                        cropsInfo.LandProperty = cropsInfoItem.LandProperty;
                        cropsInfo.CropOutput = Math.Round(cropsInfoItem.CropOutput, 2);
                        cropsInfo.CurrentYearOutput = Math.Round(cropsInfoItem.CurrentYearOutput, 2);
                        cropsInfo.LastYearOutput = Math.Round(cropsInfoItem.LastYearOutput, 2).ToString();
                        cropsInfo.CropOutput = Math.Round(cropsInfoItem.CropOutput, 2);
                        cropsInfo.CropsSalesPrice = Math.Round(cropsInfoItem.CropsSalesPrice, 2);
                        cropsInfo.PlantingArea = Math.Round(cropsInfoItem.PlantingArea, 2);
                        oscRegionItem.StatisticalInfo.CropsList.Add(cropsInfo);
                    }
                    resultList.Add(oscRegionItem);
                }
                #endregion

                #region 保存缓存
                jsonString = JSONHelper.ListToJSON<OscRegionNew>(resultList, "", out message);
                if (string.IsNullOrEmpty(jsonString) || !string.IsNullOrEmpty(message))
                {
                    result = new ListResult<OscRegionNew>()
                    {
                        Status = -1,
                        Msg = $"转换JSON数据出错,原因[{message}]",
                        Result = null
                    };
                    return result;
                }
                try
                {
                    redisHelper = m_repository.CreateRedisHelper();
                    if (redisHelper.KeyExists(RedisKey))
                        redisHelper.KeyDelete(RedisKey);
                    if (!redisHelper.SetStringValue(RedisKey, jsonString))
                    {
                        result = new ListResult<OscRegionNew>()
                        {
                            Status = -1,
                            Msg = $"保存缓存数据出错",
                            Result = null
                        };
                        return result;
                    }
                }
                catch (Exception exp)
                {
                    result = new ListResult<OscRegionNew>()
                    {
                        Status = -1,
                        Msg = $"保存缓存数据出错,原因[{exp.Message}]",
                        Result = null
                    };
                    return result;
                }
                #endregion

            }
            else 
            {
                #region 读取缓存数据
                try
                {
                    redisHelper = m_repository.CreateRedisHelper();
                    jsonString=redisHelper.GetStringValue(RedisKey);
                    //resultList = JSONHelper.JSONToList<OscRegionNew>(jsonString, out message);
                    resultList = JsonConvert.DeserializeObject<List<OscRegionNew>>(jsonString);
                }
                catch(Exception exp) 
                {
                    result = new ListResult<OscRegionNew>()
                    {
                        Status = -1,
                        Msg = $"读取缓存数据出错,原因[{exp.Message}]",
                        Result = null
                    };
                    return result;
                }
                #endregion
            }



            result = new ListResult<OscRegionNew>()
            {
                Status = 0,
                Msg = string.Empty,
                Result = resultList
            };
            return result;
        }

        /// <summary>
        /// 获得地块儿信息
        /// </summary>
        /// <param name="landId">地块儿编号</param>
        [HttpGet("vw-osc-region")]
        public async Task<ActionResult<ListResult<OscRegionNew>>> GetOSCRegionByLandId(int landId)
        {
            #region 声明变量

            //错误消息
            string message = string.Empty;

            //查询语句
            string SqlWhere = string.Empty;

            //数据库返回值
            List<vw_osc_region> databaseResult = null;

            //返回数据列表
            List<OscRegionNew> resultList = new List<OscRegionNew>();

            //返回值
            ListResult<OscRegionNew> result = new ListResult<OscRegionNew>();
            #endregion

            #region 非空验证
            if (landId <= 0)
            {
                result = new ListResult<OscRegionNew>()
                {
                    Status = -1,
                    Msg = "地块儿编号不能小于等于0",
                    Result = null
                };
                return result;
            }
            #endregion

            DateTime runStartTime=DateTime.Now;
            Console.WriteLine($"开始运行时间{runStartTime.ToString("yyyy-MM-dd HH:mm:ss")}");
            #region 数据库查询
            SqlWhere = $" Id='{landId}' Or Parent_id='{landId}' ";
            databaseResult = m_repository.QueryViewOscRegion(SqlWhere, out message);
            if (databaseResult == null || databaseResult.Count <= 0)
            {
                if (string.IsNullOrEmpty(message))
                    message = $"查询不到地块儿编号为[{landId}]的数据";
                else
                    message = $"查询地块儿编号为[{landId}]的数据出错，原因[{message}]";
                result = new ListResult<OscRegionNew>()
                {
                    Status = -1,
                    Msg = message,
                    Result = null
                };
                return result;
            }
            #endregion

            #region 分组数据
            var groupData = databaseResult.GroupBy(oscRegionGroup => oscRegionGroup.LandId)
                .Select(oscRegionItem => new
                {
                    id = oscRegionItem.First().Id,
                    name = oscRegionItem.First().LandName,
                    parent_id = oscRegionItem.First().Parent_id,
                    citycode = oscRegionItem.First().Citycode,
                    adcode = oscRegionItem.First().Adcode,
                    center = oscRegionItem.First().Center,
                    level = oscRegionItem.First().Level,
                    GPSLocations = oscRegionItem.First().GPSLocations,
                    Color=oscRegionItem.First().Color,
                    StatisticalInfo = oscRegionItem.Select(statisticalInfoItem => new
                    {
                        LandId = oscRegionItem.First().LandId,
                        LandName = oscRegionItem.First().LandName,
                        JobPopulation = oscRegionItem.Sum(sum => sum.JobPopulation),
                        TotalPopulation = oscRegionItem.Sum(sum => sum.TotalPopulation),
                        TotalOutput = oscRegionItem.Sum(sum => sum.TotalOutput),
                        TotaValue = oscRegionItem.Sum(sum => sum.TotaValue),
                        CropsList = oscRegionItem.GroupBy(cropsGroup => cropsGroup.CropsName).Select(cropsInfo => new
                        {
                            CropsName = cropsInfo.First().CropsName,
                            PlantingArea = cropsInfo.Sum(sum => sum.PlantingArea),
                            SoilType = cropsInfo.First().SoilType,
                            WateringType = cropsInfo.First().WateringType,
                            LandProperty = cropsInfo.First().LandProperty,
                            LeaseYear = cropsInfo.Sum(sum => sum.LeaseYear),
                            CropOutput = cropsInfo.Sum(sum => sum.CropOutput),
                            LastYearOutput = cropsInfo.Sum(sum => Utils.StrToDecimal(sum.LastYearOutput)),
                            CurrentYearOutput = cropsInfo.Sum(sum => sum.CurrentYearOutput),
                            CropsSalesPrice = cropsInfo.Sum(sum => sum.CropsSalesPrice)
                        })
                    })
                });
            #endregion

            #region 循环赋值返回值
            foreach (var groupItem in groupData)
            {
                OscRegionNew oscRegionItem = new OscRegionNew();
                oscRegionItem.id = Convert.ToString(groupItem.id);
                oscRegionItem.regionName = groupItem.name;
                oscRegionItem.level = groupItem.level;
                oscRegionItem.parent_id = Convert.ToString(groupItem.parent_id);
                oscRegionItem.GPSLocations = groupItem.GPSLocations;
                oscRegionItem.Color = groupItem.Color;
                oscRegionItem.StatisticalInfo = new GroupResult();
                oscRegionItem.StatisticalInfo.LandId = groupItem.StatisticalInfo.First().LandId;
                oscRegionItem.StatisticalInfo.LandName = groupItem.StatisticalInfo.First().LandName;
                oscRegionItem.StatisticalInfo.JobPopulation = groupItem.StatisticalInfo.First().JobPopulation;
                oscRegionItem.StatisticalInfo.TotalPopulation = groupItem.StatisticalInfo.First().TotalPopulation;
                oscRegionItem.StatisticalInfo.TotalOutput = Math.Round(groupItem.StatisticalInfo.First().TotalOutput, 2);
                oscRegionItem.StatisticalInfo.TotaValue = Math.Round(groupItem.StatisticalInfo.First().TotaValue, 2);
                oscRegionItem.StatisticalInfo.CropsList = new List<CropsInfo>();
                foreach (var cropsInfoItem in groupItem.StatisticalInfo.First().CropsList)
                {
                    CropsInfo cropsInfo = new CropsInfo();
                    cropsInfo.CropsName = cropsInfoItem.CropsName;
                    cropsInfo.SoilType = cropsInfoItem.SoilType;
                    cropsInfo.WateringType = cropsInfoItem.WateringType;
                    cropsInfo.LandProperty = cropsInfoItem.LandProperty;
                    cropsInfo.CropOutput = Math.Round(cropsInfoItem.CropOutput, 2);
                    cropsInfo.CurrentYearOutput = Math.Round(cropsInfoItem.CurrentYearOutput, 2);
                    cropsInfo.LastYearOutput = Math.Round(cropsInfoItem.LastYearOutput, 2).ToString();
                    cropsInfo.CropOutput = Math.Round(cropsInfoItem.CropOutput, 2);
                    cropsInfo.CropsSalesPrice = Math.Round(cropsInfoItem.CropsSalesPrice, 2);
                    cropsInfo.PlantingArea = Math.Round(cropsInfoItem.PlantingArea, 2);
                    oscRegionItem.StatisticalInfo.CropsList.Add(cropsInfo);
                }
                resultList.Add(oscRegionItem);
            }
            #endregion
            DateTime runEndTime= DateTime.Now;
            Console.WriteLine($"结束运行时间{runEndTime.ToString("yyyy-MM-dd HH:mm:ss")}");
            long interval = Utils.DateDiff(Microsoft.VisualBasic.DateInterval.Second, runStartTime, runEndTime);
            Console.WriteLine($"运行时间:{interval}");
            result = new ListResult<OscRegionNew>()
            {
                Status = 0,
                Msg = string.Empty,
                Result = resultList
            };
            return result;
        }

        /// <summary>
        /// 查询地块信息
        /// </summary>
        /// <param name="landId">地块编号</param>
        /// <returns>返回值</returns>
        [HttpGet("osc-region")]
        public async Task<ListResult<osc_region>> QueryOscRegion(int landId)
        {
            #region 声明变量

            //错误消息
            string message = string.Empty;

            //返回值
            ListResult<osc_region> result = new ListResult<osc_region>();

            //地块信息返回值
            List<osc_region> resultList = new List<osc_region>();
            #endregion

            #region 读取数据
            resultList = m_repository.QueryOscRegion($" id='{landId}' or parent_id='{landId}'", out message);
            if (resultList == null || resultList.Count <= 0)
            {
                if (string.IsNullOrEmpty(message))
                    message = "没有查到任何数据";
                else
                    message = $"查询数据出错，原因[{message}]";
                result = new ListResult<osc_region>()
                {
                    Status = -1,
                    Msg = string.Empty,
                    Result = null
                };
                return result;
            }
            #endregion

            result = new ListResult<osc_region>()
            {
                Status = 0,
                Msg = string.Empty,
                Result = resultList
            };
            return result;
        }

        /// <summary>
        /// 返回统计信息
        /// </summary>
        /// <param name="LandId">地块编号</param>
        /// <returns>统计信息</returns>
        [Route("statistics")]
        [HttpGet]
        public async Task<ActionResult<EntityResult<TotalStatisticsInfo>>> GetStatisticsInfo(int LandId = 0)
        {
            EntityResult<TotalStatisticsInfo> result = new EntityResult<TotalStatisticsInfo>();
            TotalStatisticsInfo statisticsInfo = new TotalStatisticsInfo();
            DataTable statisticsInfoDb = null;
            statisticsInfoDb = m_repository.StatisticsInfo(LandId);
            if (statisticsInfoDb != null)
            {
                statisticsInfo.TotalOutputValue = Utils.StrToDecimal(statisticsInfoDb.Rows[0]["TotalOutput"].ToString());
                statisticsInfo.TotaValue = Utils.StrToDecimal(statisticsInfoDb.Rows[0]["TotaValue"].ToString());
            }
            else
            {
                statisticsInfo.TotalOutputValue = 0;
                statisticsInfo.TotaValue = 0;
            }
            result.Status = 0;
            result.Msg = String.Empty;
            result.Result = statisticsInfo;
            return result;
        }

        /// <summary>
        /// 获得产值统计
        /// </summary>
        /// <param name="YearRange">年份范围</param>
        /// <param name="LandId">地块儿编号</param>
        /// <returns>返回结果集</returns>
        [Route("statistics/outputvalue")]
        [HttpGet]
        public async Task<ActionResult<ListResult<OutputValueStatisticsResult>>> GetOutputValueStatistics(int YearRange, int LandId = 0)
        {
            #region 声明变量

            //查询条件
            string SqlWhere = "";

            //错误消息
            string message = string.Empty;

            //数据库返回值
            List<MassifGreenHouseVP> dbResultData = null;

            //开始年份
            int StartYear = DateTime.Now.Year - YearRange;

            //结束年份
            int EndYear = DateTime.Now.Year;

            //进制单位
            int hex = 10000000;

            //返回值
            var result = new ListResult<OutputValueStatisticsResult>();
            #endregion

            #region 查询数据
            SqlWhere = $" Year(EnterTime) between {StartYear} and {EndYear} ";
            if (LandId > 0)
                SqlWhere += $" and LandId='{LandId}' ";
            SqlWhere += " order by EnterTime desc";
            dbResultData = m_repository.QueryMassifGreenHouseVP(SqlWhere, out message);
            if (dbResultData == null || dbResultData.Count <= 0)
            {
                if (!string.IsNullOrEmpty(message))
                {
                    result = new ListResult<OutputValueStatisticsResult>()
                    {
                        Status = -1,
                        Msg = $" 查询数据出错，原因[{message}]",
                        Result = null
                    };
                    return result;
                }
            }
            #endregion

            #region 分组统计
            result.Result = new List<OutputValueStatisticsResult>();
            var resultData = dbResultData.GroupBy(g => new
            {
                g.EnterTime.GetValueOrDefault().Year
            }).Select(group => new
            {
                StatisticYear = group.Key.Year,
                TotalOutputValue = group.Sum(s => s.TotaValue.GetValueOrDefault())
            });
            #endregion

            #region 赋值返回数据
            foreach (var groupItem in resultData)
            {
                OutputValueStatisticsResult item = new OutputValueStatisticsResult();
                item.StatisticYear = groupItem.StatisticYear;
                item.TotalOutputValue = groupItem.TotalOutputValue.ToString();
                result.Result.Add(item);
            }
            #endregion

            result.Status = 0;
            result.Msg = string.Empty;
            return result;
        }


        /// <summary>
        /// 获取种植面积统计
        /// </summary>
        /// <param name="RootLandId"></param>
        /// <returns></returns>
        [Route("statistics/plantingarea")]
        [HttpGet]
        public async Task<ActionResult<ListResult<PlantingAreaStatisticsResult>>> GetPlantingAreaStatistics(int RootLandId = 0)
        {
            #region 声明变量

            //查询条件
            string SqlWhere = "";

            //错误消息
            string message = string.Empty;

            //地块编号字符串
            List<string> landIdString = new List<string>();

            //数据库返回值
            List<vw_MassifGreenHouseVP> dbResultData = null;

            //返回值
            var result = new ListResult<PlantingAreaStatisticsResult>();
            #endregion

            #region 查询数据
            SqlWhere = $" parent_id='{RootLandId}' Or LandId='{RootLandId}' ";
            dbResultData = m_repository.QueryViewMassifGreenHouseVP(SqlWhere, out message);
            if (dbResultData == null || dbResultData.Count <= 0)
            {
                if (!string.IsNullOrEmpty(message))
                {
                    result = new ListResult<PlantingAreaStatisticsResult>()
                    {
                        Status = -1,
                        Msg = $" 查询数据出错，原因[{message}]",
                        Result = null
                    };
                    return result;
                }
            }
            #endregion

            #region 分组统计
            result.Result = new List<PlantingAreaStatisticsResult>();
            var resultData = dbResultData.GroupBy(g => new
            {
                g.LandId,
                g.LandName
            }).Select(group => new
            {
                LandId = group.Key.LandId,
                LandName = group.Key.LandName,
                PlantingAreaStatistics = group.Sum(s => s.PlantingArea)
            });
            #endregion

            #region 赋值返回数据
            foreach (var groupItem in resultData)
            {
                PlantingAreaStatisticsResult item = new PlantingAreaStatisticsResult();
                item.LandName = groupItem.LandName;
                item.PlantingAreaStatistics = groupItem.PlantingAreaStatistics.ToString("0.00");
                result.Result.Add(item);
            }
            #endregion

            result.Status = 0;
            result.Msg = string.Empty;
            return result;
        }


        /// <summary>
        /// 不分页查询数据
        /// </summary>
        /// <param name="QueryConditions">查询条件</param>
        /// <param name="SortField">排序字段(默认创建时间)</param>
        /// <param name="SortMethod">排序方法[DESC|ASC(默认DESC)]</param>
        /// <returns>返回查询结果</returns>
        [HttpGet]
        //[Authorize]
        //[IdentityServerFilter(resultType = "ListResult",genericTypes = "mapOfIndustryWebApi.Models.Parameter.MassifGreenHouseVPParameter", genericAssemblyString= "mapOfIndustryWebApi")]
        public async Task<ActionResult<ListResult<MassifGreenHouseVPParameter>>> GetArray(
            string? where = "",
            string sortField = "CreatedTime",
            string sortMethod = "DESC"
            )
        {
            #region 声明变量

            //查询条件
            string SqlWhere = "";

            //错误消息
            string message = string.Empty;

            //数据库返回值
            List<MassifGreenHouseVP> dbResultData = null;

            //接口返回值
            List<MassifGreenHouseVPParameter> ResultData = null;

            //返回值
            ListResult<MassifGreenHouseVPParameter> result = null;
            #endregion

            //处理查询条件
            if (!string.IsNullOrEmpty(where))
                SqlWhere = $" {where} Order by {sortField} {sortMethod} ";
            else
                SqlWhere = $" 1=1 Order by {sortField} {sortMethod} ";

            #region 查询数据
            dbResultData = m_repository.QueryMassifGreenHouseVP(SqlWhere, out message);
            if (dbResultData == null || dbResultData.Count <= 0)
            {
                if (!string.IsNullOrEmpty(message))
                {
                    result = new ListResult<MassifGreenHouseVPParameter>()
                    {
                        Status = -1,
                        Msg = $" 查询数据出错，原因[{message}]",
                        Result = null
                    };
                    return result;
                }
            }
            #endregion

            #region 赋值返回值
            ResultData = new List<MassifGreenHouseVPParameter>();
            dbResultData.ForEach(item =>
            {
                ResultData.Add(m_mapper.Map<MassifGreenHouseVP, MassifGreenHouseVPParameter>(item));
            });
            result = new ListResult<MassifGreenHouseVPParameter>()
            {
                Status = 0,
                Msg = String.Empty,
                Result = ResultData
            };
            #endregion

            return result;
        }

        /// <summary>
        /// 分组返回数据
        /// </summary>
        /// <returns>分组后的数据</returns>
        [HttpGet("group")]
        public async Task<ActionResult<ListResult<GroupResult>>> GetAllGroup(int landId)
        {
            #region 声明变量

            //查询条件
            string SqlWhere = "";

            //错误消息
            string message = string.Empty;

            //数据库返回值
            List<vw_MassifGreenHouseVP> dbResultData = null;

            //分组后返回的数据
            ListResult<GroupResult> result = new ListResult<GroupResult>();
            #endregion

            #region 查询数据
            SqlWhere = $" LandId='{landId}' Or parent_id='{landId}'";
            dbResultData = m_repository.QueryViewMassifGreenHouseVP(SqlWhere, out message);
            if (dbResultData == null || dbResultData.Count <= 0)
            {
                if (!string.IsNullOrEmpty(message))
                {
                    result = new ListResult<GroupResult>()
                    {
                        Status = -1,
                        Msg = $" 查询数据出错，原因[{message}]",
                        Result = null
                    };
                    return result;
                }
            }
            #endregion

            #region 分组数据
            result.Result = new List<GroupResult>();
            var resultData = dbResultData.GroupBy(g => new
            {
                g.LandId,
                g.LandName
            }).Select(group => new
            {
                LandId = group.Key.LandId,
                LandName = group.Key.LandName,
                JobPopulation = group.Sum(s => s.JobPopulation),
                TotalPopulation = group.Sum(s => s.TotalPopulation),
                TotalOutput = group.Sum(s => s.TotalOutput),
                TotaValue = group.Sum(s => s.TotaValue),
                CropsList = group.ToList()
                                .GroupBy(cg => new { cg.CropsName })
                                .Select(CropsGroup => new
                                {
                                    CropsName = CropsGroup.Key.CropsName,
                                    PlantingArea = CropsGroup.Sum(s => s.PlantingArea),
                                    SoilType = CropsGroup.First().SoilType,
                                    WateringType = CropsGroup.First().WateringType,
                                    LandProperty = CropsGroup.First().LandProperty,
                                    LeaseYear = CropsGroup.Sum(s => s.LeaseYear),
                                    CropOutput = CropsGroup.Sum(s => s.CropOutput),
                                    LastYearOutput = CropsGroup.Sum(s => Utils.StrToDecimal(s.LastYearOutput)),
                                    CurrentYearOutput = CropsGroup.Sum(s => s.CurrentYearOutput),
                                    CropsSalesPrice = group.Sum(s => s.CropsSalesPrice)
                                })
            });
            #endregion

            #region 赋值返回数据
            foreach (var groupInfo in resultData)
            {
                GroupResult itemData = new GroupResult();
                itemData.LandId = groupInfo.LandId;
                itemData.LandName = groupInfo.LandName;
                itemData.JobPopulation = groupInfo.JobPopulation;
                itemData.TotalPopulation = groupInfo.TotalPopulation;
                itemData.TotalOutput = groupInfo.TotalOutput;
                itemData.TotaValue = groupInfo.TotaValue;
                itemData.CropsList = new List<CropsInfo>();
                foreach (var cropsInfo in groupInfo.CropsList)
                {
                    CropsInfo item = new CropsInfo();
                    item.CropsName = cropsInfo.CropsName;
                    item.PlantingArea = cropsInfo.PlantingArea;
                    item.SoilType = cropsInfo.SoilType;
                    item.WateringType = cropsInfo.WateringType;
                    item.LandProperty = cropsInfo.LandProperty;
                    item.LeaseYear = cropsInfo.LeaseYear;
                    item.CropOutput = cropsInfo.CropOutput;
                    item.LastYearOutput = cropsInfo.LastYearOutput.ToString();
                    item.CurrentYearOutput = cropsInfo.CurrentYearOutput;
                    item.CropsSalesPrice = cropsInfo.CropsSalesPrice;
                    itemData.CropsList.Add(item);
                }
                result.Result.Add(itemData);
            }
            #endregion

            result.Status = 0;
            result.Msg = string.Empty;
            return result;
        }


        #region 统计get

        /// <summary>
        /// 获取到每年的种植面积和产量
        /// </summary>
        /// <param name="regionId"></param>
        /// <param name="cropsName"></param>
        /// <returns></returns>
        [HttpGet("count-data-old")]
        public async Task<ActionResult<ArrayResult<CountData>>> GetCountDataOld(Int32 regionId, String cropsName)
        {
            ArrayResult<CountData> result = new ArrayResult<CountData>();
            var dataList = new List<CountData>();
            var regionList = new List<SowData>();
            var where = "1=1 and parent_id =" + regionId;
            //var where = "1=1";
            //获取到所有乡镇
            DataSet ds = await m_repository.ExecuteDataSetAsync($"SELECT * FROM osc_region WITH(NOLOCK) WHERE {where} ORDER BY Id ASC");
            if (ds != null && ds.Tables.Count > 0)
            {
                DataRowCollection rows = ds.Tables[0].Rows;
                foreach (DataRow row in rows)
                {
                    regionList.Add(new SowData
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        RegionName = row["Name"].ToString()
                    });
                }
                var maWhere = "1=1 and CropsName = N'" + cropsName + "'";
                //获取农作物的所有产业数据
                var res = await GetArray(maWhere, "CreatedTime", "DESC");
                if (res.Value.Status == 0)
                {
                    //循环分出有几个年份的数据
                    for (int i = 0; i < res.Value.Result.Count; i++)
                    {
                        var resValue = res.Value.Result[i];
                        var time = Utils.StrToDateTime(resValue.EnterTime).Year;
                        if (i == 0 || !(dataList.Any(m => m.Year == time)))
                        {
                            dataList.Add(new CountData { Year = time, SowDatas = regionList });
                        }
                    }

                    foreach (var item in dataList)
                    {
                        decimal areas = 0;
                        decimal yield = 0;
                        foreach (var sow in item.SowDatas)
                        {
                            var maResult = res.Value.Result.Where(m => m.EnterTime.Contains(item.Year.ToString()) && m.LandId == sow.Id.ToString()).ToList();
                            foreach (var maitem in maResult)
                            {
                                areas += Convert.ToDecimal(maitem.PlantingArea);
                                yield += Convert.ToDecimal(maitem.TotalOutput);
                            }
                            sow.SowArea = areas;
                            sow.Yield = yield;
                        }

                    }
                    result.Result = dataList.ToArray();
                    result.Status = 0;

                }
                else
                {
                    result.Status = 3;
                    result.Msg = "读取数据库表失败";
                }


            }
            else
            {
                result.Status = 3;
                result.Msg = "读取数据库表失败";
            }

            return result;
        }

        /// <summary>
        /// 获取按农作物分组种植面积
        /// </summary>
        /// <param name="regionId">地块儿编号</param>
        /// <returns>错误消息</returns>
        [HttpGet("statistics/plantingArea/GroupCrops/old")]
        public async Task<ListResult<CropsPlantingAreaStatisticsResult>> GetPlantingAreaStatisticsGroupCropsOld(int regionId)
        {
            #region 声明变量

            //查询条件
            string SqlWhere = string.Empty;

            //错误消息
            string message = string.Empty;

            //非空验证消息
            string checkEmptyMessage = string.Empty;

            //数据库返回值
            List<vw_MassifGreenHouseVP> databaseResult = null;

            //返回数据
            List<CropsPlantingAreaStatisticsResult> resultData = new List<CropsPlantingAreaStatisticsResult>();

            //返回值
            ListResult<CropsPlantingAreaStatisticsResult> result = null;
            #endregion

            #region 非空验证
            if (regionId <= 0)
            {
                result = new ListResult<CropsPlantingAreaStatisticsResult>()
                {
                    Status = -1,
                    Msg = "地块儿编号不能小于0",
                    Result = null
                };
                return result;
            }
            #endregion

            #region 查询数据库
            SqlWhere = $" LandId='{regionId}' ";
            databaseResult = m_repository.QueryViewMassifGreenHouseVP(SqlWhere, out message);
            if (databaseResult == null || databaseResult.Count <= 0)
            {
                if (string.IsNullOrEmpty(message))
                    message = $"读取不到地块儿编号为[{regionId}]的今年种植面积统计数据";
                else
                    message = $"读取地块儿编号为[{regionId}]的今年种植面积统计数据出错，原因[{message}]";
                result = new ListResult<CropsPlantingAreaStatisticsResult>()
                {
                    Status = -1,
                    Msg = message,
                    Result = null
                };
                return result;
            }
            #endregion

            #region 分组统计数据
            var resultGroup = databaseResult
                .GroupBy(group => group.CropsName)
                .Select(group => new
                {
                    CropsName = group.Key,
                    PlantArea = group.Sum(sum => sum.PlantingArea)
                });
            #endregion

            #region 赋值返回值
            foreach (var group in resultGroup)
            {
                resultData.Add(new CropsPlantingAreaStatisticsResult()
                {
                    CropsName = group.CropsName,
                    PlantingAreaCount = Math.Round(group.PlantArea, 2)
                });
            }
            #endregion

            result = new ListResult<CropsPlantingAreaStatisticsResult>()
            {
                Status = 0,
                Msg = string.Empty,
                Result = resultData
            };
            return result;
        }

        /// <summary>
        /// 获取按农作物分组种植面积
        /// </summary>
        /// <param name="regionId">地块儿编号</param>
        /// <returns>错误消息</returns>
        [HttpGet("statistics/plantingArea/GroupCrops")]
        public async Task<ListResult<CropsPlandArea>> GetPlantingAreaStatisticsGroupCrops(int regionId)
        {
            #region 声明变量

            //返回值
            ListResult<CropsPlandArea> result = new ListResult<CropsPlandArea>();

            //错误消息
            string message = string.Empty;

            //返回统计结果
            List<CropsPlandArea> crops = new List<CropsPlandArea>();
            #endregion

            #region 读取数据
            crops = m_repository.StatisticsCropsName(regionId, out message);
            if (crops == null || crops.Count == 0)
            {
                if (string.IsNullOrEmpty(message))
                    message = "查询不到任何数据";
                else
                    message = $"查询数据出错，原因[{message}]";
                result = new ListResult<CropsPlandArea>()
                {
                    Status = -1,
                    Msg = message,
                    Result = null
                };
                return result;
            }
            #endregion

            result = new ListResult<CropsPlandArea>()
            {
                Status = 0,
                Msg = message,
                Result = crops
            };
            return result;
        }


        /// <summary>
        /// 地块搜索不分页
        /// </summary>
        /// <param name="SerchKey"></param>
        /// <returns></returns>
        [HttpGet("search/osc-region")]
        public async Task<ListResult<osc_region>> SearchOSCRegion(string SearchKey)
        {
            #region 声明变量

            string SqlWhere = string.Empty;

            //错误消息
            string message = string.Empty;

            //数据库返回数据
            List<osc_region> resultDBList = new List<osc_region>();

            //返回值
            ListResult<osc_region> result = new ListResult<osc_region>();

            int PageCount = 0;
            #endregion

            if (string.IsNullOrEmpty(SearchKey))
            {
                result = new ListResult<osc_region>()
                {
                    Status = -1,
                    Msg = string.Empty,
                    Result = null
                };
                return result;
            }

            SqlWhere = $" name like '%{SearchKey}%' or id like '%{SearchKey}%' ";
            resultDBList = m_repository.QueryOscRegionPage(
                SqlWhere,
                "id",
                "desc",
                10,
                1,
                out PageCount,
                out message
            );
            if (resultDBList == null || resultDBList.Count <= 0)
            {
                if (!string.IsNullOrEmpty(message))
                {
                    result = new ListResult<osc_region>()
                    {
                        Status = -1,
                        Msg = $"读取数据出错，原因{message}",
                        Result = null
                    };
                    return result;
                }
            }

            result = new ListResult<osc_region>()
            {
                Status = 0,
                Msg = string.Empty,
                Result = resultDBList
            };
            return result;
        }

        /// <summary>
        /// 获取到每年的种植面积和产量
        /// </summary>
        /// <param name="regionId"></param>
        /// <param name="cropsName"></param>
        /// <returns></returns>
        [HttpGet("count-data")]
        public async Task<ArrayResult<CountData>> GetCountData(int regionId, String cropsName)
        {
            #region 声明变量

            //查询条件
            string SqlWhere = string.Empty;

            //错误消息
            string message = string.Empty;

            //非空验证消息
            string checkEmptyMessage = string.Empty;

            //数据库数据
            List<vw_MassifGreenHouseVP> databaseData = null;

            //返回值
            ArrayResult<CountData> result = new ArrayResult<CountData>();

            //返回数据List
            List<CountData> resultData = new List<CountData>();
            #endregion

            #region 非空验证
            if (regionId <= 0)
                checkEmptyMessage += "地块儿编号、";
            if (string.IsNullOrEmpty(cropsName))
                checkEmptyMessage += "农作物名称、";
            if (!string.IsNullOrEmpty(checkEmptyMessage))
            {
                checkEmptyMessage = checkEmptyMessage.Substring(0, checkEmptyMessage.Length - 1);
                result = new ArrayResult<CountData>()
                {
                    Status = -1,
                    Msg = $"非空验证出错，原因[{checkEmptyMessage}]",
                    Result = null
                };
                return result;
            }
            #endregion

            #region 读取数据库数据
            SqlWhere = $" (parent_id='{regionId}' Or LandId='{regionId}') and CropsName='{cropsName}' and level<>'district' ";
            databaseData = m_repository.QueryViewMassifGreenHouseVP(SqlWhere, out message);
            if (databaseData == null || databaseData.Count <= 0)
            {
                if (string.IsNullOrEmpty(message))
                {
                    result = new ArrayResult<CountData>()
                    {
                        Msg = "没有任何数据",
                        Status = -1,
                        Result = null
                    };
                    return result;
                }
                else
                {
                    result = new ArrayResult<CountData>()
                    {
                        Msg = $"查询数据出错,原因[{message}]",
                        Status = -1,
                        Result = null
                    };
                    return result;
                }
            }
            #endregion

            #region LINQ分组统计数据
            databaseData = databaseData.OrderBy(o => o.EnterTime.Year).ToList();
            var linqData = databaseData.GroupBy(g => new
            {
                //1、先以年份分组数据
                g.EnterTime.Year
            }).Select(group => new
            {
                Year = group.Key.Year,
                sowTotalArea = group.Sum(sum => sum.PlantingArea),
                yieldTotal = group.Sum(sum => sum.TotalOutput),
                YearStatisticsData = group.ToList()
            });
            #endregion

            #region 赋值返回数据
            foreach (var group in linqData)
            {
                CountData countData = new CountData();
                decimal sowTotalArea = Math.Round(group.sowTotalArea, 2);
                decimal yieldTotal = Math.Round(group.yieldTotal, 2);
                countData.Year = group.Year;
                countData.SowDatas = new List<SowData>();
                #region 再按地块分组
                var groupLand = group.YearStatisticsData
                    .GroupBy(g => g.LandId)
                    .Select(gland => new
                    {
                        id = gland.First().LandId,
                        regionName = gland.First().LandName,
                        sowArea = gland.Sum(sum => sum.PlantingArea),
                        yield = gland.Sum(sum => sum.TotalOutput)
                    });
                #endregion
                foreach (var sowDataItem in groupLand)
                {
                    SowData sowData = new SowData();
                    sowData.Id = sowDataItem.id;
                    sowData.RegionName = sowDataItem.regionName;
                    sowData.SowArea = Math.Round(sowDataItem.sowArea, 2);
                    sowData.Yield = Math.Round(sowDataItem.yield, 2);
                    if(sowTotalArea!=0)
                       sowData.SowAreaPercentage = Math.Round((sowData.SowArea / sowTotalArea) * 100, 2);
                    else
                       sowData.SowAreaPercentage = 0;
                    if (yieldTotal != 0)
                        sowData.YieldPercentage = Math.Round((sowData.Yield / yieldTotal) * 100, 2);
                    else
                        sowData.YieldPercentage = 0;
                    sowData.SowAreaPercentage = Math.Round(sowData.SowAreaPercentage, 2);
                    sowData.YieldPercentage = Math.Round(sowData.YieldPercentage, 2);
                    countData.SowDatas.Add(sowData);
                }
                resultData.Add(countData);
            }
            #endregion

            #region 循环排序
            resultData = resultData.OrderBy(o => o.Year).ToList();
            decimal totalSowAreaNumber = 0;
            decimal totalYieldNumber = 0;
            for (int i = 0; i < resultData.Count; i++)
            {
                resultData[i].SowDatas.ForEach(item =>
                {
                    totalSowAreaNumber += item.SowArea;
                    totalYieldNumber += item.Yield;
                });

                resultData[i].SowDatas = resultData[i].SowDatas
                    .OrderByDescending(o => o.SowArea)
                    .ThenByDescending(o => o.Yield).ToList();
            }
            for (int i = 0; i < resultData.Count; i++)
            {
                if (resultData[i].SowDatas.Count == 1)
                {
                    decimal SowArea = resultData[i].SowDatas[0].SowArea;
                    decimal Yield = resultData[i].SowDatas[0].Yield;
                    if (totalSowAreaNumber == 0)
                        resultData[i].SowDatas[0].SowAreaPercentage = Math.Round(SowArea / totalSowAreaNumber * 100, 2);
                    else
                        resultData[i].SowDatas[0].SowAreaPercentage = 0;
                    if (totalYieldNumber == 0)
                        resultData[i].SowDatas[0].YieldPercentage = Math.Round(Yield / totalYieldNumber * 100, 2);
                    else
                        resultData[i].SowDatas[0].YieldPercentage = 0;
                }
            }
            #endregion

            result = new ArrayResult<CountData>()
            {
                Status = 0,
                Msg = string.Empty,
                Result = resultData.ToArray()
            };
            return result;
        }

        /// <summary>
        /// 获取总的种植面积
        /// </summary>
        /// <param name="regionId"></param>
        /// <param name="cropsName"></param>
        /// <returns></returns>
        [HttpGet("plant-area/old")]
        public async Task<ActionResult<EntityResult<PlantArea>>> GetPlantAreaOld(Int32 regionId, String cropsName)
        {
            EntityResult<PlantArea> result = new EntityResult<PlantArea>();
            var plantArea = new PlantArea();
            var regionList = new List<RegionArea>();
            var where = $"1=1 and (parent_id ='{regionId}' Or id='{regionId}')";
            decimal total = 0;
            //var where = "1=1";
            //获取到所有乡镇
            DataSet ds = await m_repository.ExecuteDataSetAsync($"SELECT * FROM osc_region WITH(NOLOCK) WHERE {where} ORDER BY Id ASC");
            if (ds != null && ds.Tables.Count > 0)
            {
                DataRowCollection rows = ds.Tables[0].Rows;
                foreach (DataRow row in rows)
                {
                    regionList.Add(new RegionArea
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        RegionName = row["Name"].ToString()
                    });
                }
                var maWhere = "1=1 and CropsName = N'" + cropsName + "'";
                //获取农作物的所有产业数据
                var res = await GetArray(maWhere, "CreatedTime", "DESC");
                if (res.Value.Status == 0)
                {
                    foreach (var item in regionList)
                    {
                        var maResult = res.Value.Result.Where(m => m.LandId == item.Id.ToString()).ToList();
                        foreach (var maitem in maResult)
                        {
                            item.SowArea += Convert.ToDecimal(maitem.PlantingArea);
                            plantArea.CountArea += Convert.ToDecimal(maitem.PlantingArea);
                        }
                        item.SowArea = Math.Round(item.SowArea, 2);
                    }
                    plantArea.CountArea = Math.Round(plantArea.CountArea, 2);
                    plantArea.RegionAreas = regionList;
                    #region 计算地块儿百分比
                    for (var i = 0; i < regionList.Count; i++)
                    {
                        var region = regionList[i];
                        if (plantArea.CountArea != 0)
                            region.SowAreaPercentage = Math.Round((region.SowArea / plantArea.CountArea) * 100, 2);
                        else
                            region.SowAreaPercentage = 0;
                    }
                    #endregion
                    result.Result = plantArea;
                    result.Status = 0;
                }
                else
                {
                    result.Status = 3;
                    result.Msg = "读取数据库表失败";
                }
            }
            else
            {
                result.Status = 3;
                result.Msg = "读取数据库表失败";
            }

            return result;
        }

        /// <summary>
        /// 获取总的种植面积
        /// </summary>
        /// <param name="regionId">地块儿编号</param>
        /// <param name="cropsName">农作物名称</param>
        /// <returns>返回值</returns>
        [HttpGet("plant-area")]
        public async Task<ActionResult<EntityResult<PlantArea>>> GetPlantArea(int regionId, string cropsName)
        {
            EntityResult<PlantArea> result = new EntityResult<PlantArea>();
            string message = string.Empty;
            PlantArea plantArea = new PlantArea();
            plantArea = m_repository.StatisticsPlantAreaByCropsLand(regionId, cropsName, out message);
            if (!string.IsNullOrEmpty(message))
            {
                result = new EntityResult<PlantArea>()
                {
                    Status = -1,
                    Msg = message,
                    Result = null
                };
                return result;
            }
            result = new EntityResult<PlantArea>()
            {
                Status = 0,
                Msg = string.Empty,
                Result = plantArea
            };
            return result;
        }

        /// <summary>
        /// 获得平均价格数据
        /// </summary>
        /// <param name="LandId">地块编号</param>
        /// <returns>返回数据</returns>
        [HttpGet("average-price")]
        public async Task<ListResult<AveragePrice>> GetAveragePriceAsync(int LandId = 0)
        {
            #region 声明变量

            //位数
            int digit = 0;

            //查询条件
            string SqlWhere = string.Empty;

            //错误消息
            string message = string.Empty;

            //数据库数据
            List<vw_MassifGreenHouseVP> databaseData = null;

            //平均价格返回数据
            List<AveragePrice> resultData = new List<AveragePrice>();

            //返回数据
            ListResult<AveragePrice> result = null;
            #endregion

            #region 读取数据库数据
            if (LandId > 0)
                SqlWhere = $" (parent_id='{LandId}' Or LandId='{LandId}') ";
            else
                SqlWhere = "1=1";
            databaseData = m_repository.QueryViewMassifGreenHouseVP(SqlWhere, out message);
            if (databaseData == null || databaseData.Count <= 0)
            {
                if (string.IsNullOrEmpty(message))
                {
                    result = new ListResult<AveragePrice>()
                    {
                        Msg = "没有任何数据",
                        Status = -1,
                        Result = null
                    };
                }
                else
                {
                    result = new ListResult<AveragePrice>()
                    {
                        Msg = $"查询数据出错,原因[{message}]",
                        Status = -1,
                        Result = null
                    };
                }
            }
            #endregion

            digit = Math.Round(databaseData.Max(q => q.CropsSalesPrice)).ToString().Length;

            #region 分组数据
            databaseData = databaseData.OrderBy(o => o.CropsName).ThenBy(o => o.EnterTime.Year).ToList();
            var linqData = databaseData.GroupBy(g => new
            {
                g.CropsName
            }).Select(group => new
            {
                CropsName = group.Key.CropsName,
                StatisticsData = group.GroupBy(g => new
                {
                    g.EnterTime.Year
                }).Select(statisticsGroup => new
                {
                    Year = statisticsGroup.Key.Year,
                    AveragePrice = Math.Round(statisticsGroup.Average(a => a.CropsSalesPrice), 2)
                })
            });
            #endregion

            #region 循环赋值返回值
            foreach (var groupItem in linqData)
            {
                AveragePrice averagePriceData = new AveragePrice();
                averagePriceData.CropsName = groupItem.CropsName;
                averagePriceData.YearsData = new List<int>();
                averagePriceData.PriceData = new List<decimal>();
                foreach (var priceDataItem in groupItem.StatisticsData)
                {
                    averagePriceData.YearsData.Add(priceDataItem.Year);
                    averagePriceData.PriceData.Add(CarryCalculation(priceDataItem.AveragePrice, digit));
                }
                resultData.Add(averagePriceData);
            }
            #endregion

            result = new ListResult<AveragePrice>()
            {
                Msg = string.Empty,
                Status = 0,
                Result = resultData
            };
            return result;
        }
        #endregion

        #endregion

        #region Delete
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="ID">数据编号</param>
        /// <returns>是否成功消息</returns>
        [EnableCors("Cors")]
        [HttpDelete("{ID}")]
        [LogFilter]
        //[Authorize]
        //[IdentityServerFilter(resultType= "Result")]
        [SqlInjectionFilter(VerifyParameterNames = "ID")]
        public async Task<Result> DeleteData(int ID)
        {
            #region 声明变量

            //数据库返回消息
            Message dbResultMsg;

            //查询条件
            string SqlWhere = string.Empty;

            //返回值
            Result result = null;
            #endregion

            if (ID <= 0)
            {
                result = new Result()
                {
                    Status = -1,
                    Msg = "ID数据编号不能小于0"
                };
                return result;
            }

            SqlWhere = $" ID='{ID}' ";
            dbResultMsg = m_repository.DeleteMassifGreenHouseVP(SqlWhere);

            result = new Result()
            {
                Status = dbResultMsg.Successful ? 0 : -1,
                Msg = dbResultMsg.Successful ? String.Empty : dbResultMsg.Content
            };
            return result;
        }
        #endregion

        #region Private
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="SaveMethod">保存方式(Add|Edit)</param>
        /// <param name="ID">编号</param>
        /// <param name="parameter">添加参数</param>
        /// <returns>0成功，非0失败</returns>
        private async Task<Result> SaveData(string SaveMethod, MassifGreenHouseVPParameter parameter)
        {
            #region 声明和初始化

            //返回值
            Result result = null;

            //错误消息
            string message = string.Empty;

            //非空验证
            string checkMessage = string.Empty;

            //要插入数据库的数据
            MassifGreenHouseVP savetData = new MassifGreenHouseVP();

            //验证数据是否存在
            List<MassifGreenHouseVP> isHave = null;

            //数据库返回值
            Message dbResult = null;
            #endregion

            #region 非空验证
            if (parameter == null)
            {
                result = new Result()
                {
                    Status = -1,
                    Msg = "参数不能为空,请检查参数是否有错"
                };
                return result;
            }
            if (string.IsNullOrEmpty(parameter.LandId))
                checkMessage += "地块编号、";
            if (string.IsNullOrEmpty(parameter.CropsName))
                checkMessage += "农作物名称、";
            if (string.IsNullOrEmpty(parameter.EnterTime))
                checkMessage += "录入时间、";
            if (!string.IsNullOrEmpty(checkMessage))
            {
                checkMessage = checkMessage.Substring(0, checkMessage.Length - 1);
                result = new Result()
                {
                    Status = -1,
                    Msg = $"非空验证出错，原因[{checkMessage}]不能为空"
                };
                return result;
            }
            #endregion

            #region 有效验证
            if (!ValidatorHelper.IsInt(parameter.LandId))
                checkMessage += "地块编号非整数、";
            if (!Utils.IsDate(parameter.EnterTime))
                checkMessage += "录入时间不是日期格式[建议格式yyyy-MM-dd]、";
            if (!string.IsNullOrEmpty(parameter.PlantingArea) && !ValidatorHelper.IsDecimal(parameter.PlantingArea))
                checkMessage += "农作物种植面积非数字、";
            if (!string.IsNullOrEmpty(parameter.JobPopulation) && !ValidatorHelper.IsInt(parameter.JobPopulation))
                checkMessage += "务工人口数量非数字、";
            if (!string.IsNullOrEmpty(parameter.TotalPopulation) && !ValidatorHelper.IsInt(parameter.TotalPopulation))
                checkMessage += "总人口数量非数字、";
            if (!string.IsNullOrEmpty(parameter.TotalOutput) && !ValidatorHelper.IsDecimal(parameter.TotalOutput))
                checkMessage += "总产量非数字、";
            if (!string.IsNullOrEmpty(parameter.TotaValue) && !ValidatorHelper.IsDecimal(parameter.TotaValue))
                checkMessage += "总产值非数字、";
            if (!string.IsNullOrEmpty(parameter.LeaseYear) && !ValidatorHelper.IsInt(parameter.LeaseYear))
                checkMessage += "土地租赁年限非数字、";
            if (!string.IsNullOrEmpty(parameter.CropOutput) && !ValidatorHelper.IsFloat(parameter.CropOutput))
                checkMessage += "同一作物种植年限非数字、";
            if (!string.IsNullOrEmpty(parameter.CurrentYearOutput) && !ValidatorHelper.IsDecimal(parameter.CurrentYearOutput))
                checkMessage += "本年度作物产量非数字、";
            if (!string.IsNullOrEmpty(parameter.CropsSalesPrice) && !ValidatorHelper.IsDecimal(parameter.CropsSalesPrice))
                checkMessage += "农作物售价非数字、";
            if (!string.IsNullOrEmpty(checkMessage))
            {
                checkMessage = checkMessage.Substring(0, checkMessage.Length - 1);
                result = new Result()
                {
                    Status = -1,
                    Msg = $"有效验证出错，原因[{checkMessage}]"
                };
                return result;
            }
            #endregion

            #region 保存到数据库
            //赋值参数到实体类
            savetData = m_mapper.Map<MassifGreenHouseVP>(parameter);
            if (SaveMethod == "Add")
            {
                savetData.CreatedTime = DateTime.Now;
                savetData.Modifier = savetData.Creater;
                savetData.ModifiedTime = DateTime.Now;
                dbResult = await m_repository.addMassifGreenHouseVP(savetData);
            }
            else if (SaveMethod == "Edit")
            {
                #region 验证数据是否存在
                isHave = m_repository.QueryMassifGreenHouseVP($" ID='{parameter.ID}' ", out message);
                if (isHave == null || isHave.Count <= 0)
                {
                    if (!string.IsNullOrEmpty(message))
                    {
                        result = new Result()
                        {
                            Status = -1,
                            Msg = $"保存失败，原因[{message}]"
                        };
                        return result;
                    }
                    else
                    {
                        result = new Result()
                        {
                            Status = -1,
                            Msg = $"保存失败，原因[找不到指定的数据]"
                        };
                        return result;
                    }
                }
                #endregion
                savetData.Creater = isHave[0].Creater;
                savetData.CreatedTime = isHave[0].CreatedTime;
                savetData.Modifier = savetData.Creater;
                savetData.ModifiedTime = DateTime.Now;
                dbResult = m_repository.UpdateMassifGreenHouseVP(new List<MassifGreenHouseVP>() { savetData }, $" ID='{parameter.ID}' ");
            }
            if (dbResult != null && !dbResult.Successful)
            {
                result = new Result()
                {
                    Status = -1,
                    Msg = $"保存失败，原因[{dbResult.Content}]"
                };
                return result;
            }
            #endregion

            result = new Result()
            {
                Status = 0,
                Msg = String.Empty
            };
            return result;
        }

        /// <summary>
        /// 获得全部省市数据
        /// </summary>
        /// <param name="message">错误消息</param>
        /// <returns>所有省市数据</returns>
        private OscRegionRoot GetAllOscRegionRoot(out string message)
        {
            #region 声明变量

            //错误消息
            message = string.Empty;

            //返回值
            OscRegionRoot result = null;

            //获得目前运行的根目录
            string WebRootPath = CoreHttpContext.WebPath;

            //JSON文件路径
            string JsonPath = string.Empty;

            //json字符串
            string jsonString = string.Empty;
            #endregion

            #region 验证文件
            JsonPath = $@"{WebRootPath}\osc_region.json";
            if (!System.IO.File.Exists(JsonPath))
                return null;
            #endregion

            #region 获得json字符串
            jsonString = Utils.getTxtFileBody(JsonPath, Encoding.UTF8);
            if (string.IsNullOrEmpty(jsonString))
                return null;
            #endregion

            try
            {
                result = JsonConvert.DeserializeObject<OscRegionRoot>(jsonString);
            }
            catch (Exception exp)
            {
                message = exp.Message;
            }

            return result;
        }

        /// <summary>
        /// 进位计算
        /// </summary>
        /// <param name="SourceNumber">原始数据</param>
        /// <param name="digit">位数</param>
        /// <returns></returns>
        private decimal CarryCalculation(decimal SourceNumber, int digit)
        {
            decimal result = 0;
            string digitNumber = "";
            for (int i = 0; i < digit; i++)
            {
                if (i == 0)
                    digitNumber += "1";
                else
                    digitNumber += "0";
            }
            result = SourceNumber / Convert.ToDecimal(digitNumber);
            result = Math.Round(result, 2);
            return result;
        }

        /// <summary>
        /// 读取Redis地图缓存版本数据
        /// </summary>
        /// <param name="landId">地块儿编号</param>
        /// <param name="message">错误消息</param>
        /// <returns></returns>
        private List<MapLocationVerison> ReadRedisMapLocationVerison(int landId,out string message) 
        {
            #region 声明变量

            //错误消息
            message = string.Empty;

            //缓存key
            string RedisKey = $"RDKEY_landId_Verison_{landId}";

            //返回值
            List<MapLocationVerison> result = null;

            //json字符串
            string jsonString = string.Empty;

            //Redis缓存类
            RedisHelper redisHelper = null;
            #endregion

            #region 读取缓存数据
            try 
            {
                redisHelper = m_repository.CreateRedisHelper();
                jsonString = redisHelper.GetStringValue(RedisKey);
                if(!string.IsNullOrEmpty(jsonString))
                    result = JSONHelper.JSONToList<MapLocationVerison>(jsonString,out message);
            }
            catch(Exception exp) 
            {
                message=$"读取地块儿缓存数据出错,{exp.Message}";
            }
            #endregion

            return result;
        }

        /// <summary>
        /// 同步地图版本
        /// </summary>
        /// <param name="landId">地块儿编号</param>
        /// <param name="databaseData">数据库数据</param>
        /// <param name="isHaveNewVerison">是否有最新版本</param>
        /// <param name="message">错误消息</param>
        /// <returns>同步是否成功</returns>
        private bool SyncRedisMapLocationVerison(int landId, List<MapLocationVerison> databaseData,out bool isHaveNewVerison,out string message) 
        {
            #region 声明变量

            //是否有最新版本
            isHaveNewVerison = false;

            //缓存Key
            string RedisKey = $"RDKEY_landId_Verison_{landId}";

            //缓存帮助类
            RedisHelper redisHelper = null;

            //错误消息
            message = string.Empty;

            //返回值
            bool result = false;

            string jsonString = string.Empty;

            //List集合
            List<MapLocationVerison> redisData = null;
            #endregion

            redisData = ReadRedisMapLocationVerison(landId,out message);
            if(redisData == null || redisData.Count <= 0) 
            {
                if (!string.IsNullOrEmpty(message))
                {
                    message = $"读取地图版本缓存出错,原因[{message}]";
                    return false;
                }
                else
                {
                    try
                    {
                        redisHelper = m_repository.CreateRedisHelper();
                        if (redisHelper.KeyExists(RedisKey))
                        {
                            if (redisHelper.KeyDelete(RedisKey))
                            {
                                jsonString = JSONHelper.ListToJSON<MapLocationVerison>(databaseData, "", out message);
                                redisHelper.SetStringValue(RedisKey, jsonString);
                                isHaveNewVerison = true;
                                return true;
                            }
                        }
                        else 
                        {
                            jsonString = JSONHelper.ListToJSON<MapLocationVerison>(databaseData, "", out message);
                            redisHelper.SetStringValue(RedisKey, jsonString);
                            isHaveNewVerison = true;
                            return true;
                        }
                    }
                    catch (Exception exp)
                    {
                        message = $"保存地块儿版本数据到缓存出错,原因[{exp.Message}]";
                    }
                }
            }


            #region 版本比较
            if (redisData[0].MapLocalVerison < databaseData[0].MapLocalVerison)
            {
                try
                {
                    redisHelper = m_repository.CreateRedisHelper();
                    if (redisHelper.KeyDelete(RedisKey))
                    {
                        jsonString = JSONHelper.ListToJSON<MapLocationVerison>(databaseData, "", out message);
                        redisHelper.SetStringValue(RedisKey, jsonString);
                        isHaveNewVerison = true;
                    }
                }
                catch (Exception exp)
                {
                    message = $"保存地块儿版本数据到缓存出错,原因[{exp.Message}]";
                }
            }
            else
                result = true;
            #endregion

            return result;
        }
        #endregion

        #endregion
    }
}
