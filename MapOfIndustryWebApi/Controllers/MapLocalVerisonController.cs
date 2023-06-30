using AutoMapper;
using MapOfIndustryDataAccess.Data;
using MapOfIndustryDataAccess.Models;
using MapOfIndustryWebApi.Models;
using MapOfIndustryWebApi.Models.Result;
using Microsoft.AspNetCore.Mvc;

namespace MapOfIndustryWebApi.Controllers
{
    /// <summary>
    /// 地图坐标数据编号
    /// </summary>
    [Route("api/map-local-verison")]
    [ApiController]
    public class MapLocalVerisonController : Controller
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
        public MapLocalVerisonController(IMOIRepository repository, IMapper _mapper)
        {
            m_repository = repository;
            m_mapper = _mapper == null ? throw new ArgumentNullException(nameof(_mapper)) : _mapper;
        }
        #endregion

        #region Method

        #region Get

        /// <summary>
        /// 获取最新版本
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<EntityResult<MapLocationVerisonParameter>>> GetNewMapVerison() 
        {
            #region 声明变量

            //方法返回错误消息
            string message = string.Empty;

            //数据库返回值
            List<MapLocationVerison> dbResult = new List<MapLocationVerison>();

            //地图边界版本号
            MapLocationVerisonParameter apiResult =new MapLocationVerisonParameter();

            //获取最新版本
            var result = new EntityResult<MapLocationVerisonParameter>();
            #endregion

            #region 读取数据库
            dbResult = m_repository.QueryMapLocationVerison(" 1=1 order by CreatedTime desc ", out message);
            if (dbResult == null || dbResult.Count <= 0) 
            {
                if (!string.IsNullOrEmpty(message)) 
                {
                    result = new EntityResult<MapLocationVerisonParameter>() 
                    {
                        Status=-1,
                        Msg=$"查询地图坐标数据版本出错,{message}",
                        Result=null
                    };
                    return result;
                }
                else 
                {
                    result = new EntityResult<MapLocationVerisonParameter>()
                    {
                        Status = 0,
                        Msg = string.Empty,
                        Result = null
                    };
                    return result;
                }
            }
            #endregion

            apiResult = m_mapper.Map<MapLocationVerisonParameter>(dbResult[0]);
            result = new EntityResult<MapLocationVerisonParameter>()
            {
                Status = 0,
                Msg = string.Empty,
                Result = apiResult
            };
            return result;
        }
        #endregion

        #endregion

    }
}
