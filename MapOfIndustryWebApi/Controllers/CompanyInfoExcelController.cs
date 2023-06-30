using AutoMapper;
using MapOfIndustryDataAccess.Data;
using MapOfIndustryDataAccess.Models;
using MapOfIndustryWebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace MapOfIndustryWebApi.Controllers
{
    /// <summary>
    /// 公司信息导出Excel
    /// </summary>
    [Route("/CompanyInfoExcel/[action]")]
    public class CompanyInfoExcelController : Controller
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
        /// 构造函数
        /// </summary>
        /// <param name="repository">数据库操作类</param>
        /// <param name="_mapper">AutoMapper参数映射类</param>
        /// <exception cref="ArgumentNullException"></exception>
        public CompanyInfoExcelController(IMOIRepository repository, IMapper _mapper)
        {
            m_repository = repository;
            m_mapper = _mapper == null ? throw new ArgumentNullException(nameof(_mapper)) : _mapper;
        }
        #endregion

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="selectedIDStr">选择要导出的编号</param>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        [HttpPost]
        [HttpGet]
        public IActionResult ExportExcel(string selectedIDStr, string where)
        {
            #region 声明变量

            //ID字符串
            string IDStr = string.Empty;

            //查询条件
            string SqlWhere = string.Empty;

            //查询条件集合
            List<string> whereList = new List<string>();

            //错误消息
            string message = string.Empty;

            //输出文件类型
            string ContentType = "application/vnd.ms-excel";

            //Stream流
            MemoryStream stream = null;

            //excel文件
            Npoi.Mapper.Mapper excelMap = new Npoi.Mapper.Mapper();

            //公司编号信息
            List<CompanyInfo> companyInfos = null;

            //导出数据
            List<CompanyInfoParameter> exportData = new List<CompanyInfoParameter>();
            #endregion

            #region 处理ID字符串查询条件
            if (!string.IsNullOrEmpty(selectedIDStr) && selectedIDStr != "null")
            {
                IDStr = "'" + selectedIDStr.Replace("-", "','") + "'";
                whereList.Add(" RecordId in (" + IDStr + ")");
            }
            if (!string.IsNullOrEmpty(where) && where != "null")
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
                whereList.Add($" ({where}) ");
            }
            if (whereList.Count > 0)
                SqlWhere = string.Join(" and ", whereList);
            #endregion

            #region 查询数据
            companyInfos = m_repository.QueryCompanyInfo(SqlWhere, out message);
            if (companyInfos == null || companyInfos.Count <= 0)
            {
                Response.ContentType = "text/html";
                if (!string.IsNullOrEmpty(message))
                    return Content($"公司数据读取出错，原因[{message}]");
                else
                    return Content(string.Empty);
            }
            #endregion

            #region 转换数据
            companyInfos.ForEach(companyInfo => {
                CompanyInfoParameter itemData = m_mapper.Map<CompanyInfo, CompanyInfoParameter>(companyInfo);
                exportData.Add(itemData);
            });
            #endregion

            #region 输出Excel
            Response.ContentType = ContentType;
            stream = new MemoryStream();
            excelMap.Put<CompanyInfoParameter>(exportData, "sheet1", false);
            excelMap.Save(stream);
            #endregion

            return File(stream.ToArray(), ContentType);
        }
    }
}
