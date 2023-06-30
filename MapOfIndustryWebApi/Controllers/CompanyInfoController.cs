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
    /// 公司信息
    /// </summary>
    [Route("api/company-info")]
    [ApiController]
    public class CompanyInfoController : Controller
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
        #endregion

        #region Constructors

        /// <summary>
        /// 重载构造函数
        /// </summary>
        /// <param name="repository">数据库操作类</param>
        public CompanyInfoController(IMOIRepository repository, IMapper _mapper, IWebHostEnvironment webHostEnvironment)
        {
            m_repository = repository;
            m_mapper = _mapper == null ? throw new ArgumentNullException(nameof(_mapper)) : _mapper;
            m_webHostEnvironment = webHostEnvironment;
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
        public async Task<ActionResult<PageResult<CompanyInfoParameter>>> GetPage(
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
            List<CompanyInfo> pageData = null;

            //接口返回值
            List<CompanyInfoParameter> pageResultData = null;

            //返回值
            var result = new PageResult<CompanyInfoParameter>();
            #endregion

            #region 非空验证
            if (pageIndex == null)
                checkMessage += "当前页、";
            if (pageSize == null)
                checkMessage += "每页记录数、";
            if (!string.IsNullOrEmpty(checkMessage))
            {
                checkMessage = checkMessage.Substring(0, checkMessage.Length - 1);
                result = new PageResult<CompanyInfoParameter>()
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
                result = new PageResult<CompanyInfoParameter>()
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
                sqlWhere.Add($" CompanyName like '%{where}%' ");
                sqlWhere.Add($" EnterpriseType like '%{where}%' ");
                sqlWhere.Add($" TownShip like '%{where}%' ");
                sqlWhere.Add($" Contacts like '%{where}%' ");
                sqlWhere.Add($" ContactPhone like '%{where}%' ");
                sqlWhere.Add($" EnterpriseAddress like '%{where}%' ");
                sqlWhere.Add($" EnterpriseIntroduction like '%{where}%' ");
                sqlWhere.Add($" CompanyType like '%{where}%' ");
                where = String.Join(" Or ", sqlWhere.ToArray());
            }
            pageData = m_repository.QueryCompanyInfo(
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
                    result = new PageResult<CompanyInfoParameter>()
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
            pageResultData = new List<CompanyInfoParameter>();
            pageData.ForEach(item =>
            {
                var resultItem = m_mapper.Map<CompanyInfo, CompanyInfoParameter>(item);
                resultItem.FoundedTime = item.FoundedTime.GetValueOrDefault().ToString("yyyy-MM-dd");
                pageResultData.Add(resultItem);
            });
            result = new PageResult<CompanyInfoParameter>()
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
        /// 读取单条公司信息
        /// </summary>
        /// <param name="RecordId">公司编号</param>
        /// <returns>返回值</returns>
        [HttpGet("ByRecordId/{RecordId}")]
        public async Task<ListResult<CompanyInfoParameter>> GetCompanyInfoByRecordId(string RecordId)
        {
            string where = $" RecordId='{RecordId}' ";
            return await GetCompanyInfo(where, "CreatedTime", "DESC");
        }

        /// <summary>
        /// 根据地块儿编号读取公司信息
        /// </summary>
        /// <param name="LandId">地块儿编号</param>
        /// <returns>返回值</returns>
        [HttpGet("ByLandId/{LandId}")]
        public async Task<ListResult<CompanyInfoParameter>> GetCompanyInfoByLandId(string LandId)
        {
            string where = string.Empty;
            string message = string.Empty;
            List<osc_region> regionList = new List<osc_region>();
            ListResult<CompanyInfoParameter> result = null;
            regionList = m_repository.QueryOscRegion($" id='{LandId}' ", out message);
            if (regionList == null || regionList.Count <= 0)
            {
                if (!string.IsNullOrEmpty(message))
                    message = $"读取地块儿编号为[{LandId}]的数据出错,原因[{message}]";
                else
                    message = $"读取地块儿编号为[{LandId}]的数据出错,原因[没有读取到地块儿信息]";
                result = new ListResult<CompanyInfoParameter>()
                {
                    Status = -1,
                    Msg = message,
                    Result = null,
                };
                return result;
            }
            if (regionList[0].level == "district")
                where = $" Backup01='{LandId}' ";
            else
                where = $" Backup02='{LandId}' ";
            return await GetCompanyInfo(where, "CreatedTime", "DESC");
        }


        /// <summary>
        /// 联合查询方法
        /// </summary>
        /// <param name="LandId">地块儿编号_必填</param>
        /// <param name="CompanyType">公司分类_可选</param>
        /// <param name="EnterpriseNature">公司性质_可选</param>
        /// <param name="RegionalLevel">区域级别_可选</param>
        /// <returns></returns>
        [HttpGet("UnionQuery/{LandId}")]
        public async Task<ListResult<CompanyInfoParameter>> UnionQuery(
            string LandId,
            string CompanyType = "",
            string EnterpriseNature = "",
            string RegionalLevel = "",
            string sortField = "CreatedTime",
            string sortMethod = "DESC"
            )
        {
            #region 声明变量

            //查询条件
            string where = string.Empty;

            //错误消息
            string message = string.Empty;

            //查询条件列表
            List<string> whereList = new List<string>();

            //地块信息
            List<osc_region> regionList = new List<osc_region>();

            //数据库返回值
            List<CompanyInfo> DBResult = new List<CompanyInfo>();

            //返回值List
            List<CompanyInfoParameter> resultList = new List<CompanyInfoParameter>();

            //返回值
            ListResult<CompanyInfoParameter> result = null;
            #endregion

            if (string.IsNullOrEmpty(LandId))
            {
                result = new ListResult<CompanyInfoParameter>()
                {
                    Status = -1,
                    Msg = "地块儿编号不能为空",
                    Result = null,
                };
                return result;
            }

            #region 验证地块儿编号
            where = $" id='{LandId}' ";
            regionList = m_repository.QueryOscRegion(where, out message);
            if (regionList == null || regionList.Count <= 0)
            {
                if (!string.IsNullOrEmpty(message))
                    message = $"读取地块儿编号为[{LandId}]的数据出错,原因[{message}]";
                else
                    message = $"读取地块儿编号为[{LandId}]的数据出错,原因[没有读取到地块儿信息]";
                result = new ListResult<CompanyInfoParameter>()
                {
                    Status = -1,
                    Msg = message,
                    Result = null,
                };
                return result;
            }
            #endregion

            #region 拼查询条件
            if (!string.IsNullOrEmpty(CompanyType) && CompanyType != "null")
                whereList.Add($" CompanyType like '%{CompanyType}%' ");
            if (!string.IsNullOrEmpty(EnterpriseNature) && EnterpriseNature != "null")
                whereList.Add($" EnterpriseNature like '%{EnterpriseNature}%' ");
            if (!string.IsNullOrEmpty(RegionalLevel) && RegionalLevel != "null")
                whereList.Add($" RegionalLevel like '%{RegionalLevel}%' ");
            if (regionList[0].level == "district")
                whereList.Add($" Backup01='{LandId}' ");
            else
                whereList.Add($" Backup02='{LandId}' ");
            where = string.Join(" and ", whereList);
            #endregion

            return await GetCompanyInfo(where, sortField, sortMethod);

            //return result;
        }

        /// <summary>
        /// 读取指定分类公司信息
        /// </summary>
        /// <param name="CompanyType">公司分类</param>
        /// <returns>返回值</returns>
        [HttpGet("ByCompanyType/{CompanyType}/{LandId}")]
        public async Task<ListResult<CompanyInfoParameter>> GetCompanyInfoByCompanyType(string CompanyType, string LandId)
        {
            string where = $" CompanyType like '%{CompanyType}%' ";
            string message = string.Empty;
            List<osc_region> regionList = new List<osc_region>();
            ListResult<CompanyInfoParameter> result = null;
            regionList = m_repository.QueryOscRegion($" id='{LandId}' ", out message);
            if (regionList == null || regionList.Count <= 0)
            {
                if (!string.IsNullOrEmpty(message))
                    message = $"读取地块儿编号为[{LandId}]的数据出错,原因[{message}]";
                else
                    message = $"读取地块儿编号为[{LandId}]的数据出错,原因[没有读取到地块儿信息]";
                result = new ListResult<CompanyInfoParameter>()
                {
                    Status = -1,
                    Msg = message,
                    Result = null,
                };
                return result;
            }
            if (regionList[0].level == "district")
                where += $" and Backup01='{LandId}' ";
            else
                where += $" and Backup02='{LandId}' ";
            return await GetCompanyInfo(where, "CreatedTime", "DESC");
        }

        /// <summary>
        /// 获得公司信息
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortMethod">排序方法</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ListResult<CompanyInfoParameter>> GetCompanyInfo(
            string? where = "",
            string sortField = "CreatedTime",
            string sortMethod = "DESC"
            )
        {
            #region 声明变量

            //错误消息
            string message = string.Empty;

            //数据库
            List<CompanyInfo> DbList = new List<CompanyInfo>();

            //返回集合
            List<CompanyInfoParameter> resultList = new List<CompanyInfoParameter>();

            //返回值
            ListResult<CompanyInfoParameter> result = new ListResult<CompanyInfoParameter>();
            #endregion

            #region 查询数据
            if (string.IsNullOrEmpty(where))
                where = " 1=1 ";
            DbList = m_repository.QueryCompanyInfo($" {where} order by {sortField} {sortMethod} ", out message);
            if (DbList == null || DbList.Count == 0)
            {
                if (!string.IsNullOrEmpty(message))
                {
                    result = new ListResult<CompanyInfoParameter>()
                    {
                        Status = -1,
                        Msg = $"查询公司信息出错，原因[{message}]",
                        Result = null
                    };
                    return result;
                }
                result = new ListResult<CompanyInfoParameter>()
                {
                    Status = 0,
                    Msg = string.Empty,
                    Result = null
                };
                return result;
            }
            #endregion

            #region 赋值返回值
            DbList.ForEach(item => {
                resultList.Add(m_mapper.Map<CompanyInfo, CompanyInfoParameter>(item));
            });
            result = new ListResult<CompanyInfoParameter>()
            {
                Status = 0,
                Msg = string.Empty,
                Result = resultList
            };
            #endregion

            return result;
        }
        #endregion

        #region Post

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="parameter">添加参数</param>
        /// <returns>返回结果</returns>        
        [HttpPost]
        public async Task<ActionResult<Result>> AddData([FromBody] CompanyInfoParameter parameter)
        {
            return await SaveData("Add", parameter, null);
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
            Result itemResult = null;

            //返回值
            Result result = null;

            IdWorker snowId = new IdWorker(1, 1);
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
            var excelData = excelMap.Take<CompanyInfoParameter>("sheet1");
            foreach (var item in excelData)
            {
                item.Value.Created = userName;
                item.Value.Modifier = userName;
                item.Value.FoundedTime = Utils.StrToDateTime(item.Value.FoundedTime).ToString("yyyy-MM-dd");
                itemResult = await SaveData("Add", item.Value, snowId);
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
        #endregion

        #region Put

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [HttpPut("{RecordId}")]
        public async Task<ActionResult<Result>> ModifyData(string RecordId, [FromBody] CompanyInfoParameter parameter)
        {
            parameter.RecordId = RecordId;
            return await SaveData("Edit", parameter, null);
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
                dbMessage = await m_repository.DeleteCompanyInfo(IDStr);
            }
            catch (Exception ex)
            {
                result = new Result()
                {
                    Msg = $"删除公司信息报错，原因[{ex.Message}]",
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
        /// <param name="SaveMethod">[Add|Edit]</param>
        /// <param name="parameter">参数</param>
        /// <returns>返回结果</returns>
        private async Task<Result> SaveData(string SaveMethod, CompanyInfoParameter parameter, IdWorker snowId)
        {
            #region 声明和初始化

            //数据库返回值
            Message dbResult = null;

            //返回值
            Result result = null;

            //查询条件
            string SqlWhere = string.Empty;

            //错误消息
            string message = string.Empty;

            //所属乡镇
            string townShip = string.Empty;

            //验证消息
            string checkMessage = string.Empty;

            //农作物信息
            List<CompanyInfo> seedInfos = null;

            //地块儿信息
            List<osc_region> osc_Regions = null;

            //地块列表
            List<osc_region> townShipList = new List<osc_region>();

            //保存数据
            CompanyInfo saveData = new CompanyInfo();
            #endregion

            if (snowId == null)
                snowId = new IdWorker(1, 1);

            #region 参数非空验证
            if (parameter == null)
            {
                result = new Result()
                {
                    Status = -1,
                    Msg = "参数不能为空"
                };
            }
            if (string.IsNullOrEmpty(SaveMethod))
                checkMessage += "保存方式、";
            if (SaveMethod == "Edit" && string.IsNullOrEmpty(parameter.RecordId))
                checkMessage += "公司编号、";
            if (string.IsNullOrEmpty(parameter.CompanyName))
                checkMessage += "公司名称、";
            if (string.IsNullOrEmpty(parameter.Lng))
                checkMessage += "地图经度、";
            if (string.IsNullOrEmpty(parameter.Lat))
                checkMessage += "地图纬度、";
            //if (string.IsNullOrEmpty(parameter.FocusImages))
            //    checkMessage += "请至少上传一张焦点图、";
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
            if (!Utils.IsDouble(parameter.Lng))
                checkMessage += "地图经度非数字、";
            if (!Utils.IsDouble(parameter.Lat))
                checkMessage += "地图纬度非数字、";
            if (!string.IsNullOrEmpty(checkMessage))
            {
                checkMessage = checkMessage.Substring(0, checkMessage.Length - 1);
                result = new Result()
                {
                    Status = -1,
                    Msg = $"有效验证出错，原因[{checkMessage}]不能为空"
                };
                return result;
            }
            #endregion

            #region 地块儿信息读取
            SqlWhere = $" id='{parameter.Backup02}' or name like '%{parameter.TownShip}%' ";
            osc_Regions = m_repository.QueryOscRegion(SqlWhere, out message);
            if (osc_Regions == null || osc_Regions.Count <= 0)
            {
                if (!string.IsNullOrEmpty(message))
                {
                    result = new Result()
                    {
                        Status = -1,
                        Msg = $"读取地块信息出错，原因[{checkMessage}]不能为空"
                    };
                    return result;
                }

            }
            else
            {
                parameter.Backup02 = Convert.ToString(osc_Regions[0].id);
                parameter.Backup01 = Convert.ToString(osc_Regions[0].parent_id);
            }
            while (osc_Regions != null && osc_Regions.Count > 0 && string.IsNullOrEmpty(message))
            {
                townShipList.Add(osc_Regions[0]);
                SqlWhere = $" id='{osc_Regions[0].parent_id}' ";
                osc_Regions = m_repository.QueryOscRegion(SqlWhere, out message);
            }
            if (townShipList != null && townShipList.Count > 0)
            {
                townShipList.Reverse();
                townShipList.ForEach(townShipItem =>
                {
                    townShip += townShipItem.name;
                });
            }
            parameter.TownShip = townShip;
            #endregion

            #region 企业等级判断
            decimal CurrentYearSales = Utils.StrToDecimal(parameter.CurrentYearSales);
            CurrentYearSales = CurrentYearSales / 10000;
            if (CurrentYearSales > 800)
                parameter.CompanyType = "A";
            else if (CurrentYearSales > 500 && CurrentYearSales <= 800)
                parameter.CompanyType = "B";
            else if (CurrentYearSales > 200 && CurrentYearSales <= 500)
                parameter.CompanyType = "C";
            #endregion

            #region 保存数据
            if (SaveMethod == "Add")
            {
                saveData = m_mapper.Map<CompanyInfo>(parameter);
                saveData.CompanyType = parameter.CompanyType;
                //saveData.RecordId = new IdWorker(1, 1).NextId();//生成雪花ID
                saveData.RecordId = snowId.NextId();
                saveData.CreatedTime = DateTime.Now;
                saveData.ModifiedTime = DateTime.Now;
                dbResult = await m_repository.InsertCompanyInfo(saveData);
            }
            else if (SaveMethod == "Edit")
            {
                saveData = m_mapper.Map<CompanyInfo>(parameter);
                saveData.CreatedTime = saveData.CreatedTime;
                saveData.CompanyType = parameter.CompanyType;
                saveData.ModifiedTime = DateTime.Now;
                SqlWhere = $" RecordId='{saveData.RecordId}' ";
                dbResult = await m_repository.UpdateCompanyInfo(new List<CompanyInfo>() { saveData }, SqlWhere);
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
                Msg = string.Empty
            };
            return result;
        }
        #endregion
    }
}
