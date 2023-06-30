using AutoMapper;
using Common;
using ePioneer.Data.Kernel;
using MapOfIndustryDataAccess.Data;
using MapOfIndustryDataAccess.Models;
using MapOfIndustryWebApi.Models;
using MapOfIndustryWebApi.Models.Result;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Snowflake.Net;

namespace MapOfIndustryWebApi.Controllers
{
    /// <summary>
    /// 地块信息控制器
    /// </summary>
    [Route("api/osc-region")]
    [ApiController]
    public class OscRegionController : Controller
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

        /// <summary>
        /// 获取网站路径
        /// </summary>
        private readonly IWebHostEnvironment m_webHostEnvironment;

        /// <summary>
        /// 地块儿级别对应字典
        /// </summary>
        private Dictionary<string, string> m_level = new Dictionary<string, string>()
        {
            { "country","国家" },
            { "countryside","乡村" },
            { "province","省" },
            { "city","市" },
            { "district","区县" },
            { "town","镇" },
            { "street","街道" },
            { "village","村庄" },
            { "businessDistrict","热点商圈" },
            { "pointInterest","兴趣点" },
            { "houseNumber","门牌号" },
            { "unitNumber","单元号" },
            { "road","道路" },
            { "roadIntersection","道路交叉路口" },
            { "platform","公交站台、地铁站" },
            { "unknown","未知" }
        };
        #endregion

        #region Constructors

        /// <summary>
        /// 地块儿信息控制器
        /// </summary>
        /// <param name="repository">数据库操作类</param>
        public OscRegionController(IMOIRepository repository, IMapper _mapper, IWebHostEnvironment webHostEnvironment)
        {
            m_repository = repository;
            m_mapper = _mapper == null ? throw new ArgumentNullException(nameof(_mapper)) : _mapper;
            m_webHostEnvironment = webHostEnvironment;
        }
        #endregion

        #region Get

        /// <summary>
        /// 根据根地块儿编号查看全部
        /// </summary>
        /// <param name="rootLandId">根地块儿编号</param>
        /// <returns>所有范围数据</returns>
        [HttpGet("all/regionGis/by/root/landId/{rootLandId}")]
        public async Task<ListResult<RegionGisParameter>> GetAllRegionGisByRootLand(string rootLandId) 
        {
            string message = string.Empty;
            List<vw_osc_region> osc_Regions = new List<vw_osc_region>();
            List<RegionGisParameter> regionGis=new List<RegionGisParameter>();
            var result = new ListResult<RegionGisParameter>();
            if (string.IsNullOrEmpty(rootLandId)) 
            {
                result = new ListResult<RegionGisParameter>() 
                {
                    Status=-1,
                    Msg="根地块儿编号不能为空"
                };
                return result;
            }
            osc_Regions = m_repository.QueryViewOscRegion($" Parent_id='{rootLandId}' ", out message);
            if(osc_Regions==null|| osc_Regions.Count <= 0) 
            {
                if (!string.IsNullOrEmpty(message))
                    message = $"获取地块儿数据出错,原因[{message}]";
                else
                    message = $"获取地块儿数据出错,原因[没有获取到根地块儿编号为[{rootLandId}]的数据]";
                result = new ListResult<RegionGisParameter>()
                {
                    Status = -1,
                    Msg = message
                };
                return result;
            }
            osc_Regions.ForEach(item => {
                var itemResult=new RegionGisParameter() {
                    landId=item.LandId.ToString(),
                    GPSLocations=item.GPSLocations,
                    Color=item.Color
                };
                regionGis.Add(itemResult);
            });
            result = new ListResult<RegionGisParameter>() 
            {
                Status=0,
                Msg=string.Empty,
                Result=regionGis
            };
            return result;
        }

        /// <summary>
        /// 省份自动完成
        /// </summary>
        /// <returns></returns>
        [HttpGet("auto-complate")]
        public async Task<ListResult<OscRegionItem>> OscRegionAutoComplate(
            int? landId = -1,
            string keyword = "null",
            int resultCount = 10,
            string sortField = "id",
            string sortMethod = "DESC"
            )
        {
            #region 声明变量

            //查询条件字符串
            string SqlWhere = string.Empty;

            //错误消息
            string message = string.Empty;

            //数据库返回值
            List<osc_region> osc_Regions = new List<osc_region>();

            //数据返回值
            List<OscRegionItem> resultList = new List<OscRegionItem>();

            //返回的总数据量
            int RecordCount = 0;

            //返回值
            ListResult<OscRegionItem> result = null;
            #endregion

            if (resultCount < 0)
                resultCount = 10;
            if (string.IsNullOrEmpty(sortField))
                sortField = "id";
            if (string.IsNullOrEmpty(sortMethod))
                sortField = "DESC";

            #region 查询数据库
            if (!string.IsNullOrEmpty(keyword) && keyword != "null")
                SqlWhere = $" name like '%{keyword}%'";
            if (landId != null && landId != -1)
                SqlWhere = $" id='{landId}'";
            osc_Regions = m_repository.QueryOscRegionPage(
                SqlWhere,
                sortField,
                sortMethod,
                resultCount,
                1,
                out RecordCount,
                out message
                );
            if (osc_Regions == null || osc_Regions.Count <= 0)
            {
                if (!string.IsNullOrEmpty(message))
                {
                    result = new ListResult<OscRegionItem>()
                    {
                        Status = -1,
                        Msg = $"查询地块儿数据出错，原因[{message}]",
                        Result = null
                    };
                    return result;
                }
                else
                {
                    result = new ListResult<OscRegionItem>()
                    {
                        Status = 0,
                        Msg = string.Empty,
                        Result = null
                    };
                    return result;
                }
            }
            #endregion

            #region 赋值返回数据
            resultList = new List<OscRegionItem>();
            osc_Regions.ForEach(item =>
            {
                OscRegionItem itemParam = m_mapper.Map<osc_region, OscRegionItem>(item);
                resultList.Add(itemParam);
            });
            result = new ListResult<OscRegionItem>()
            {
                Status = 0,
                Msg = string.Empty,
                Result = resultList
            };
            #endregion

            return result;
        }

        /// <summary>
        /// 地块儿分页
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="level">级别</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortMethod">排序方法</param>
        /// <returns></returns>
        [HttpGet("page")]
        public async Task<PageResult<OscRegionItem>> OscRegionPage(
            string? where = "",
            string? level = "",
            int? pageIndex = 1,
            int? pageSize = 10,
            string sortField = "id",
            string sortMethod = "DESC"
            )
        {
            #region 声明变量

            //总页数
            int pageCount = 0;

            //总记录数
            int totalRecordCount = 0;

            //错误消息
            string message = string.Empty;

            //错误消息
            string checkMessage = string.Empty;

            //页面返回值
            List<osc_region> pageData = null;

            //接口返回值
            List<OscRegionItem> pageResultData = null;

            //返回值
            PageResult<OscRegionItem> result = new PageResult<OscRegionItem>();
            #endregion

            #region 非空验证
            if (pageIndex == null)
                checkMessage += "当前页、";
            if (pageSize == null)
                checkMessage += "每页记录数、";
            if (!string.IsNullOrEmpty(checkMessage))
            {
                checkMessage = checkMessage.Substring(0, checkMessage.Length - 1);
                result = new PageResult<OscRegionItem>()
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
                result = new PageResult<OscRegionItem>()
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
            if (!string.IsNullOrEmpty(where))
            {
                List<string> sqlWhere = new List<string>();
                sqlWhere.Add($" name like '%{where}%' ");
                sqlWhere.Add($" citycode like '%{where}%' ");
                sqlWhere.Add($" adcode like '%{where}%' ");
                where = String.Join(" Or ", sqlWhere.ToArray());
            }
            if (!string.IsNullOrEmpty(level) && !string.IsNullOrEmpty(where))
            {
                level = m_level.Where(dic => dic.Value == level).First().Key;
                where = $" ({where}) ";
                where = $" {where} and level='{level}' ";
            }
            else if (!string.IsNullOrEmpty(level))
            {
                level = m_level.Where(dic => dic.Value == level).First().Key;
                where = $" level='{level}' ";
            }
            pageData = m_repository.QueryOscRegionPage(
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
                    result = new PageResult<OscRegionItem>()
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
            pageResultData = new List<OscRegionItem>();
            pageData.ForEach(item =>
            {
                var resultItem = m_mapper.Map<OscRegionItem>(item);
                resultItem.level = m_level.Where(dic => dic.Key == item.level).First().Value;
                pageResultData.Add(resultItem);
            });
            result = new PageResult<OscRegionItem>()
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
        /// 获得所有地块儿级别
        /// </summary>
        /// <returns></returns>
        [HttpGet("all/region/level")]
        public async Task<ListResult<OscRegionLevelOption>> GetAllRegionLevel()
        {
            var result = new ListResult<OscRegionLevelOption>();
            List<OscRegionLevelOption> resultList = new List<OscRegionLevelOption>();
            m_level.Keys.ToList().ForEach(key =>
            {
                var resultItem = new OscRegionLevelOption();
                resultItem.Key = key;
                resultItem.Value = m_level[key];
                resultList.Add(resultItem);
            });
            result.Msg = String.Empty;
            result.Status = 0;
            result.Result = resultList;
            return result;
        }

        /// <summary>
        /// 读取根地块儿
        /// </summary>
        /// <returns>返回读取的根地块儿数据</returns>
        [HttpGet("root/land")]
        public async Task<EntityResult<OscRegionItem>> GetRootLand() 
        {
            List<osc_region> dbData = new List<osc_region>();
            OscRegionItem rootLand = new OscRegionItem();
            var result = new EntityResult<OscRegionItem>();
            string message = string.Empty;
            dbData = m_repository.QueryOscRegion($" IsRoot='1' ", out message);
            if (dbData == null || dbData.Count <= 0) 
            {
                if (!string.IsNullOrEmpty(message))
                {
                    result.Status = -1;
                    result.Msg = $" 获取根地块儿数据出错,原因[{message}] ";
                    return result;
                }
            }
            else
                rootLand = m_mapper.Map<OscRegionItem>(dbData[0]);
            result.Status = 0;
            result.Msg = String.Empty;
            result.Result = rootLand;
            return result;
        }

        /// <summary>
        /// 根据地块儿编号获取地块儿数据
        /// </summary>
        /// <param name="landId">地块儿编号</param>
        /// <returns>返回值</returns>
        [HttpGet("region/gis")]
        public async Task<ListResult<region_gis>> GetRegionGISByLandId(string landId)
        {
            string message = string.Empty;
            var result = new ListResult<region_gis>();
            List<region_gis> resultList = new List<region_gis>();
            if (string.IsNullOrEmpty(landId))
            {
                result.Status = -1;
                result.Msg = $"读取地块儿范围数据出错,原因[地块儿编号[{landId}]不能为空]";
                return result;
            }
            resultList = m_repository.QueryRegionGis($" landId='{landId}' ", out message);
            if (resultList == null || resultList.Count <= 0)
            {
                if (!string.IsNullOrEmpty(message))
                {
                    result.Status = -1;
                    result.Msg = $"读取地块儿编号为[{landId}]的范围数据出错,原因[{message}]";
                    return result;
                }
            }
            result.Status = 0;
            result.Msg = string.Empty;
            result.Result = resultList;
            return result;
        }
        #endregion

        #region Post

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="parameter">添加参数</param>
        /// <param name="UserName">用户名</param> 
        /// <returns>返回结果</returns> 
        [EnableCors("Cors")]
        [HttpPost]
        public async Task<ActionResult<EntityResult<int>>> AddData([FromBody] OscRegionItem parameter,string UserName)
        {
            return await SaveData(parameter, "Add", UserName);
        }

        /// <summary>
        /// 导入Excel文件
        /// </summary>
        /// <param name="formFile">上传的Excel文件</param>
        /// <returns></returns>
        [HttpPost("importExcel")]
        public async Task<ActionResult<Result>> ImportExcel([FromForm] IFormCollection formData)
        {
            #region 声明变量

            //添加状态
            Message insertDbStatus = null;

            //文件
            IFormFile formFile = null;

            //excel文件
            Npoi.Mapper.Mapper excelMap = null;

            //上传Excel文件
            string uploadFilePath = "uploadExcelFiles";

            //上传文件保存的物理路径
            string realUploadFilePath = string.Empty;

            //文件扩展名
            string fileExtension = "";

            //保存的文件名
            string SaveFileName = "";

            //网站根目录
            string wwwrootPath = "";

            //错误消息
            string message = string.Empty;

            //用户名
            string userName = string.Empty;

            //文件扩展名限制
            string[] limitFileExtension = { "xls", "xlsx" };

            //文件大小MB
            int limitMaxFileSize = 300;

            //返回值
            EntityResult<int> itemResult = null;

            //返回值
            Result result = null;

            //雪花ID
            IdWorker snowId = new IdWorker(1, 1);

            //添加数据
            List<osc_region> inserData = new List<osc_region>();
            #endregion

            formFile = (formData == null || formData.Files == null || formData.Files.Count == 0) ? null : formData.Files[0];
            userName = formData["userName"];

            #region 参数验证
            if (formFile == null)
            {
                result = new Result()
                {
                    Status = -1,
                    Msg = "没有上传文件不能导入"
                };
                return result;
            }
            #endregion

            #region 文件验证
            fileExtension = Path.GetExtension(formFile.FileName);
            if (!limitFileExtension.Any(item => item == fileExtension.Replace(".", "").ToLower()))
            {
                result = new Result()
                {
                    Status = -1,
                    Msg = "上传的文件不是Excel文件"
                };
                return result;
            }
            if (formFile.Length > 1024 * 1024 * limitMaxFileSize)
            {
                result = new Result()
                {
                    Status = -1,
                    Msg = $"上传的文件不能超过{limitMaxFileSize}MB"
                };
                return result;
            }
            #endregion

            #region 保存文件
            try
            {
                wwwrootPath = m_webHostEnvironment.ContentRootPath;
                realUploadFilePath = @$"{wwwrootPath}\{uploadFilePath}\";
                if (!Directory.Exists(realUploadFilePath))
                    Directory.CreateDirectory(realUploadFilePath);
                SaveFileName = $"{new IdWorker(1, 1).NextId()}{fileExtension}";
                using (var fileStream = new FileStream($@"{realUploadFilePath}\{SaveFileName}", FileMode.Create))
                {
                    await formFile.CopyToAsync(fileStream);
                }
            }
            catch (Exception ex)
            {
                result = new Result()
                {
                    Status = -1,
                    Msg = $"文件保存出错，原因[{ex.Message}]"
                };
                return result;
            }
            #endregion

            #region 导入文件
            excelMap = new Npoi.Mapper.Mapper($@"{realUploadFilePath}\{SaveFileName}");
            var excelData = excelMap.Take<OscRegionItem>("sheet1");
            foreach (var item in excelData)
            {
                var itemDb = m_mapper.Map<osc_region>(item.Value);
                itemResult = await SaveData(item.Value,"Import", userName);
                if (itemResult.Status != 0)
                    message += $"第{item.RowNumber}行导入出错,原因[{itemResult.Msg}]\r\n";
            }
            if (!string.IsNullOrEmpty(message))
            {
                result = new Result()
                {
                    Status = -1,
                    Msg = message
                };
                return result;
            }
            #endregion

            result = new Result()
            {
                Status = 0,
                Msg = string.Empty
            };
            return result;
        }

        /// <summary>
        /// 保存Gis范围数据
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [EnableCors("Cors")]
        [HttpPost("Save/RegionGis")]
        public async Task<Result> SaveRegionGisData(List<RegionGisParameter> parameter)
        {
            #region 声明变量
            var result = new Result();
            string message = string.Empty;
            Message dbResult = new Message();
            List<string> IDList = new List<string>();
            List<osc_region> isHaveRegion = new List<osc_region>();
            List<region_gis> insertData = new List<region_gis>();
            #endregion

            #region 参数验证
            if (parameter == null || parameter.Count <= 0)
            {
                result = new Result()
                {
                    Status = -1,
                    Msg = "参数不能为空"
                };
                return result;
            }
            for (var i = 0; i < parameter.Count; i++)
            {
                string itemMessage = string.Empty;
                if (!Utils.IsIntNum(parameter[i].landId) || Utils.StrToInt(parameter[i].landId, -1) <= 0)
                    message += $"第{i + 1}条数据地块儿编号非数字\r\t\n";
                isHaveRegion = m_repository.QueryOscRegion($" id='{parameter[i].landId}' ", out itemMessage);
                if (isHaveRegion == null || isHaveRegion.Count <= 0)
                {
                    if (!string.IsNullOrEmpty(itemMessage))
                        message += $"第{i + 1}条数据地块儿数据读取出错,原因[{itemMessage}]\r\t\n"; ;
                }
            }
            if (!string.IsNullOrEmpty(message))
            {
                result = new Result()
                {
                    Status = -1,
                    Msg = message
                };
                return result;
            }
            #endregion

            #region 保存范围数据
            parameter.ForEach(item =>
            {
                IDList.Add(item.landId);
            });
            dbResult = await m_repository.BetchDeleteRegionGisByLandId(string.Join("','", IDList));
            if (!dbResult.Successful)
            {
                result.Status = -1;
                result.Msg = $"删除地块儿范围数据出错,原因[{dbResult.Content}]";
                return result;
            }
            parameter.ForEach(item =>
            {
                insertData.Add(new region_gis()
                {
                    landId = Utils.StrToInt(item.landId, 0),
                    GPSLocations = item.GPSLocations,
                    Color = item.Color.Replace("#","")
                });
            });
            dbResult = m_repository.InsertRegionGis(insertData);
            #endregion

            #region 更新地图版本信息
            if (!UpdateMapLocationVerison(out message)) 
            {
                result.Status = -1;
                result.Msg = $"更新地图版本出错,原因[{message}]";
                return result;
            }
            #endregion

            result.Status = dbResult.Successful ? 0 : -1;
            result.Msg = dbResult.Content;
            return result;
        }
        #endregion

        #region Put

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [EnableCors("Cors")]
        [HttpPut("{id}")]
        public async Task<ActionResult<EntityResult<int>>> ModifyData(string id,string UserName,[FromBody] OscRegionItem parameter)
        {
            parameter.id = id;
            return await SaveData(parameter,"Edit",UserName);
        }


        /// <summary>
        /// 设置根地块儿
        /// </summary>
        /// <param name="landId">地块儿编号</param>
        /// <returns>错误消息</returns>
        [HttpPut("set-rootland/{landId}")]
        public Result SetRootLand(string landId)
        {
            Message dbResult = null;
            string message = string.Empty;
            Result result = new Result() { Status = 0, Msg = string.Empty };
            if (!string.IsNullOrEmpty(landId))
            {
                dbResult = m_repository.SetRootLand(landId);
                if (dbResult.Successful)
                {
                    if (!UpdateMapLocationVerison(out message))
                    {
                        result.Status = -1;
                        result.Msg = $"更新地图版本失败,原因[{message}]";
                        return result;
                    }
                }
            }           
            return result;
        }
        #endregion

        #region Delete

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="IDStr">数据编号字符串ID的形式[123-456-678]</param>
        /// <returns>是否成功消息</returns>
        [EnableCors("Cors")]
        [HttpDelete("{IDStr}")]
        public async Task<Result> DeleteData(string IDStr)
        {
            #region 声明变量

            //错误消息
            string message = string.Empty;

            //数据库返回消息
            Message dbMessage = null;

            //返回值
            Result result = null;
            #endregion

            if (string.IsNullOrEmpty(IDStr))
            {
                result = new Result()
                {
                    Msg = "参数为空不能删除",
                    Status = -1
                };
            }

            try
            {
                IDStr = "'" + IDStr.Replace("-", "','") + "'";
                dbMessage = await m_repository.DeleteOscRegion(IDStr);
            }
            catch (Exception ex)
            {
                result = new Result()
                {
                    Msg = $"删除地块儿信息报错，原因[{ex.Message}]",
                    Status = -1
                };
                return result;
            }
            if(!UpdateMapLocationVerison(out message))
            {
                result = new Result()
                {
                    Msg = $"更新地图版本失败,原因[{message}]",
                    Status = -1
                };
                return result;
            }
            result = new Result()
            {
                Msg = string.Empty,
                Status = 0
            };
            return result;
        }
        #endregion

        #region Private

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <param name="SaveMethod">保存方式</param>
        /// <param name="UserName">用户名</param>
        /// <returns>返回结果</returns>
        private async Task<EntityResult<int>> SaveData(OscRegionItem parameter, string SaveMethod,string UserName)
        {
            #region 声明和初始化

            int identityId = 0;

            //查询条件
            string SqlWhere = string.Empty;

            //错误消息
            string message = string.Empty;

            //验证消息
            string checkMessage = string.Empty;

            //地块儿信息
            List<osc_region> osc_Regions = null;

            //上级地块儿信息
            List<osc_region> osc_ParentRegions = null;

            //数据库返回值
            Message dbResult = null;

            //返回值
            EntityResult<int> result = null;

            //保存数据
            osc_region saveData = new osc_region();

            //层级键
            List<string> levelKeys = new List<string>();

            //层级值
            List<string> levelValues = new List<string>();
            #endregion


            #region 参数非空验证
            if (parameter == null)
            {
                result = new EntityResult<int>()
                {
                    Status = -1,
                    Msg = "参数不能为空"
                };
            }
            if (string.IsNullOrEmpty(SaveMethod))
                checkMessage += "保存方式、";
            if (SaveMethod == "Edit" && string.IsNullOrEmpty(parameter.id))
                checkMessage += "编号、";
            if (string.IsNullOrEmpty(parameter.name))
                checkMessage += "地块儿名字、";
            if (string.IsNullOrEmpty(parameter.parent_id))
                checkMessage += "上级地块儿ID、";
            if (string.IsNullOrEmpty(parameter.citycode))
                checkMessage += "区号、";
            if (string.IsNullOrEmpty(parameter.adcode))
                checkMessage += "城市编码、";
            if (string.IsNullOrEmpty(parameter.center))
                checkMessage += "经纬度、";
            if (string.IsNullOrEmpty(parameter.level))
                checkMessage += "地区层级、";
            if (!string.IsNullOrEmpty(checkMessage))
            {
                checkMessage = checkMessage.Substring(0, checkMessage.Length - 1);
                result = new EntityResult<int>()
                {
                    Status = -1,
                    Msg = $"非空验证出错，原因[{checkMessage}]不能为空"
                };
                return result;
            }
            #endregion

            #region 有效验证
            if (SaveMethod == "Edit"&&!Utils.IsIntNum(parameter.id))
                checkMessage += "编号不是整数、";
            if (!Utils.IsIntNum(parameter.parent_id))
                checkMessage += "上级地块儿ID不是整数、";
            levelKeys = m_level.Keys.ToList();
            levelValues = m_level.Values.ToList();
            if (levelKeys.Any(q => q == parameter.level) == false && levelValues.Any(q => q == parameter.level) == false)
                checkMessage += $"地区层级错误,地区层级只能是[{string.Join(",", m_level.Keys)}]或者[{string.Join(",", m_level.Values)}]的一种";
            if (!string.IsNullOrEmpty(checkMessage))
            {
                checkMessage = checkMessage.Substring(0, checkMessage.Length - 1);
                result = new EntityResult<int>()
                {
                    Status = -1,
                    Msg = $"有效验证出错，原因[{checkMessage}]不能为空"
                };
                return result;
            }
            #endregion

            #region 数据验证
            if (SaveMethod == "Edit"||SaveMethod=="Import")
            {
                osc_Regions = m_repository.QueryOscRegion($" id='{parameter.id}' ", out message);
                if (osc_Regions == null || osc_Regions.Count <= 0)
                {
                    if (!string.IsNullOrEmpty(message))
                    {
                        result = new EntityResult<int>()
                        {
                            Status = -1,
                            Msg = $"读取地块儿信息出错，原因[{message}]"
                        };
                        return result;
                    }
                    if (SaveMethod == "Import")
                        SaveMethod = "Edit";
                }
            }
            #endregion

            #region 验证上级地儿编号
            if (Utils.StrToInt(parameter.parent_id, 0) != 0)
            {
                osc_ParentRegions = m_repository.QueryOscRegion($" parent_id='{parameter.parent_id}' ", out message);
                if (osc_ParentRegions == null || osc_ParentRegions.Count <= 0)
                {
                    if (!string.IsNullOrEmpty(message))
                        message = $"读取上级地块出错,原因[{message}]";
                    else
                        message = $"读取上级地块出错,原因[读取不到编号为[{parameter.parent_id}]的上级地块儿数据]";
                    result = new EntityResult<int>()
                    {
                        Status = -1,
                        Msg = message
                    };
                    return result;
                }
            }
            #endregion

            #region 保存数据
            if (SaveMethod == "Add")
            {
                saveData = m_mapper.Map<osc_region>(parameter);
                saveData.created = UserName;
                saveData.CreatedTime = DateTime.Now;
                saveData.Modifier = UserName;
                saveData.ModifiedTime = DateTime.Now;
                saveData.level = m_level.Where(dic => dic.Value.Equals(parameter.level)).First().Key;
                dbResult = m_repository.Insertosc_region(new List<osc_region> { saveData },out identityId);
            }
            else
            {
                saveData = m_mapper.Map<osc_region>(parameter);
                identityId = osc_Regions[0].id.GetValueOrDefault();
                saveData.created = osc_Regions[0].created;
                saveData.CreatedTime = osc_Regions[0].CreatedTime;
                saveData.Modifier = UserName;
                saveData.ModifiedTime = DateTime.Now;
                saveData.level = m_level.Where(dic => dic.Value.Equals(parameter.level)).First().Key;
                dbResult = m_repository.Updateosc_region(new List<osc_region> { saveData }, $" id='{parameter.id}' ");
            }
            #endregion

            result = new EntityResult<int>()
            {
                Status = dbResult.Successful ? 0 : -1,
                Msg = dbResult.Content,
                Result= identityId
            };
            return result;
        }

        /// <summary>
        /// 更新地图版本
        /// </summary>
        /// <param name="message">错误消息</param>
        /// <returns>是否成功</returns>
        private bool UpdateMapLocationVerison(out string message)
        {
            Message dbResult = null;
            int landId = -1;
            message = string.Empty;
            bool result = false;
            List<osc_region> regionList = new List<osc_region>();
            List<MapLocationVerison> mapLocationVerisons = new List<MapLocationVerison>();
            regionList = m_repository.QueryOscRegion(" IsRoot=1 ", out message);
            if (regionList == null || regionList.Count <= 0)
            {
                if (!string.IsNullOrEmpty(message))
                    message = $"读取根地块儿数据出错,原因[{message}]";
                else
                    message = "读取根地块儿数据出错,原因[没有设置根地块儿请先设置根地块儿]";
                return false;
            }
            landId = regionList[0].id.GetValueOrDefault();
            mapLocationVerisons = m_repository.QueryMapLocationVerison($" landId='{landId}' ", out message);
            if (mapLocationVerisons == null || mapLocationVerisons.Count <= 0)
            {
                if (!string.IsNullOrEmpty(message))
                {
                    message = $"读取地图版本数据出错,原因[{message}]";
                    return false;
                }
                else
                {
                    mapLocationVerisons.Add(new MapLocationVerison()
                    {
                        landId = landId,
                        MapLocalVerison = 1,
                        Created = regionList[0].created,
                        CreatedTime = DateTime.Now
                    });
                    dbResult = m_repository.InsertMapLocationVerison(mapLocationVerisons);
                }
            }
            else
            {
                mapLocationVerisons[0].MapLocalVerison += 1;
                mapLocationVerisons[0].Created = regionList[0].created;
                mapLocationVerisons[0].CreatedTime = DateTime.Now;
                dbResult = m_repository.UpdateMapLocationVerison(mapLocationVerisons, $" Id={mapLocationVerisons[0].Id} ");
            }
            if (dbResult.Successful)
                result = true;
            else
            {
                result = false;
                message = $"更新地图版本出错,原因[{message}]";
            }
            return result;
        }
        #endregion

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="selectedIDStr">选择要导出的编号</param>
        /// <param name="where">查询条件</param>
        /// <param name="level">层级</param>
        /// <returns></returns>
        [HttpPost("/osc-region/ExportExcel")]
        [HttpGet("/osc-region/ExportExcel")]
        public IActionResult ExportExcel(
            string selectedIDStr = "",
            string where = "",
            string level = "")
        {
            #region 声明变量

            //ID字符串
            string IDStr = string.Empty;

            //查询条件
            string SqlWhere = string.Empty;

            //错误消息
            string message = string.Empty;

            //输出文件类型
            string ContentType = "application/vnd.ms-excel";

            //Stream流
            MemoryStream stream = null;

            //excel文件
            Npoi.Mapper.Mapper excelMap = new Npoi.Mapper.Mapper();

            //数据库数据
            List<osc_region> dbData = null;

            //导出数据
            List<OscRegionItem> exportData = new List<OscRegionItem>();

            //查询条件集合
            List<string> sqlWhere = new List<string>();
            #endregion

            #region 处理ID字符串查询条件
            if (!string.IsNullOrEmpty(selectedIDStr) && selectedIDStr != "null")
            {
                IDStr = "'" + selectedIDStr.Replace("-", "','") + "'";
                sqlWhere.Add(" RecordId in (" + IDStr + ")");
            }
            if (!string.IsNullOrEmpty(where) && where != "null")
            {
                sqlWhere.Add($" name like '%{where}%' ");
                sqlWhere.Add($" citycode like '%{where}%' ");
                sqlWhere.Add($" adcode like '%{where}%' ");
            }
            if (sqlWhere != null && sqlWhere.Count > 0)
                SqlWhere = String.Join(" Or ", sqlWhere.ToArray());
            if (!string.IsNullOrEmpty(level) && level != "null" && !string.IsNullOrEmpty(SqlWhere))
            {
                SqlWhere = $" ({SqlWhere}) ";
                level = m_level.Where(dic => dic.Value.Contains(level)).First().Key;
                SqlWhere = $" {SqlWhere} and level='{level}' ";
            }
            else if (!string.IsNullOrEmpty(level) && level != "null")
            {
                level = m_level.Where(dic => dic.Value.Contains(level)).First().Key;
                SqlWhere = $" level='{level}' ";
            }
            #endregion

            #region 查询数据
            if (string.IsNullOrEmpty(SqlWhere))
                SqlWhere = " 1=1 ";
            dbData = m_repository.QueryOscRegion(SqlWhere, out message);
            if (dbData == null || dbData.Count <= 0)
            {
                Response.ContentType = "text/html";
                if (!string.IsNullOrEmpty(message))
                    return Content($"公司数据读取出错，原因[{message}]");
                else
                    return Content(string.Empty);
            }
            #endregion

            #region 转换数据
            dbData.ForEach(dbItem =>
            {
                OscRegionItem itemData = m_mapper.Map<OscRegionItem>(dbItem);
                if(!string.IsNullOrEmpty(level)&&m_level.Any(dic => dic.Key.Contains(level)))
                   itemData.level = m_level.Where(dic => dic.Key.Contains(level)).First().Value;
                exportData.Add(itemData);
            });
            #endregion

            #region 输出Excel
            Response.ContentType = ContentType;
            stream = new MemoryStream();
            excelMap.Put<OscRegionItem>(exportData, "sheet1", false);
            excelMap.Save(stream);
            #endregion

            return File(stream.ToArray(), ContentType);
        }
    }
}
