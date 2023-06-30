using Common;
using ePioneer.Data.Kernel;
using MapOfIndustryDataAccess.Models;
using System.Data;
using System.Data.Common;

namespace MapOfIndustryDataAccess.Data
{
    public interface IMOIRepository
    {

        #region Common

        public Task<PagerSet> GetPagerSetAsync(String tableName, Int32 pageIndex, Int32 pageSize, String where, String oderBy, String[] fields);

        public Task<PagerSet> GetPagerSetAsync(String tableName, Int32 pageIndex, Int32 pageSize, String where, String orderBy);

        public Task<DataSet> ExecuteDataSetAsync(String commandText);

        public DataSet ExecuteDataSet(String commandText);

        public Task<Int32> ExecuteNonQueryAsync(String commandText);

        public Int32 ExecuteNonQuery(String commandText);

        public Int32 ExecuteNonQuery(CommandType commandType, String commandText, params DbParameter[] commandParameters);
        #endregion

        /// <summary>
        /// 创建Redis缓存对象
        /// </summary>
        /// <returns>Redis缓存</returns>
        public RedisHelper CreateRedisHelper();

        #region MassifGreenHouseVP增删改查

        public Task<Message> addMassifGreenHouseVP(MassifGreenHouseVP addMassifGreenHouseVP);

        public List<MassifGreenHouseVP> QueryMassifGreenHouseVP(string SqlWhere, out string message);

        public List<MassifGreenHouseVP> QueryMassifGreenHouseVP(string SqlWhere, string SortField, string SortMethod, int PageSize, int CurPage, out int TotalNumber, out string message);

        public Message UpdateMassifGreenHouseVP(List<MassifGreenHouseVP> lists, string SqlWhere);

        public Message DeleteMassifGreenHouseVP(string SqlWhere);
        #endregion

        #region vw_MassifGreenHouseVP查询方法
        /// <summary>
        /// 查询单表方法
        /// </summary>
        /// <param name="SqlWhere">查询条件</param>
        /// <param name="message">错误消息</param>
        /// <returns>查询结果</returns>
        public List<vw_MassifGreenHouseVP> QueryViewMassifGreenHouseVP(string SqlWhere, out string message);

        /// <summary>
        /// 分页查询方法
        /// </summary>
        /// <param name="SqlWhere">查询条件</param>
        /// <param name="SortField">排序字段名</param>
        /// <param name="SortMethod">排序方法[ASC|DESC]</param>
        /// <param name="PageSize">每页记录数</param>
        /// <param name="CurPage">当前页</param>
        /// <param name="PageCount">总页数</param>
        /// <param name="message">错误消息</param>
        /// <returns>查询结果</returns>
        public List<vw_MassifGreenHouseVP> QueryViewMassifGreenHouseVP_Page(string SqlWhere, string SortField, string SortMethod, int PageSize, int CurPage, out int PageCount, out string message);

        /// <summary>
        /// 查询单表方法
        /// </summary>
        /// <param name="SqlWhere">查询条件</param>
        /// <param name="message">错误消息</param>
        /// <returns>查询结果</returns>
        public List<vw_osc_region> QueryViewOscRegion(string SqlWhere, out string message);

        /// <summary>
        /// 分页查询方法
        /// </summary>
        /// <param name="SqlWhere">查询条件</param>
        /// <param name="SortField">排序字段名</param>
        /// <param name="SortMethod">排序方法[ASC|DESC]</param>
        /// <param name="PageSize">每页记录数</param>
        /// <param name="CurPage">当前页</param>
        /// <param name="PageCount">总页数</param>
        /// <param name="message">错误消息</param>
        /// <returns>查询结果</returns>
        public List<vw_osc_region> QueryViewOscRegionPage(string SqlWhere, string SortField, string SortMethod, int PageSize, int CurPage, out int PageCount, out string message);
        #endregion

        #region MapLocationVerison增删改查
        /// <summary>
        /// 添加地图边界版本号 
        /// </summary>
        /// <param name="lists">批量数据</param>
        /// <param name="message">错误消息</param>
        /// <returns>添加条数</returns>
        public Message InsertMapLocationVerison(List<MapLocationVerison> lists);

        /// <summary>
        /// 修改地图边界版本号
        /// </summary>
        /// <param name="lists">批量数据</param>
        /// <param name="SqlWhere">更新条件</param>
        /// <param name="message">错误消息</param>
        /// <returns>修改条数</returns>
        public Message UpdateMapLocationVerison(List<MapLocationVerison> lists, string SqlWhere);

        /// <summary>
        /// 查询地图边界版本号数据
        /// </summary>
        /// <param name="SqlWhere">查询条件</param>
        /// <param name="message">错误消息</param>
        /// <returns>返回值</returns>
        public List<MapLocationVerison> QueryMapLocationVerison(string SqlWhere, out string message);

        /// <summary>
        /// 分页查询地图边界版本号数据
        /// </summary>
        /// <param name="SqlWhere">查询条件</param>
        /// <param name="SortField">排序字段</param>
        /// <param name="SortMethod">排序方法</param>
        /// <param name="PageSize">每页分页数据</param>
        /// <param name="CurPage">当前页</param>
        /// <param name="TotalNumber">总数据量</param>
        /// <param name="PageCount">总页数</param>
        /// <param name="message">错误消息</param>
        /// <returns></returns>
        public List<MapLocationVerison> QueryPageMapLocationVerison(string SqlWhere, string SortField, string SortMethod, int PageSize, int CurPage, out int TotalNumber, out int PageCount, out string message);
        #endregion

        /// <summary>
        /// 按农作物分类统计种植面积
        /// </summary>
        /// <param name="landId">地块儿编号</param>
        /// <param name="message">错误消息</param>
        /// <returns>统计结果</returns>
        public List<CropsPlandArea> StatisticsCropsName(int landId, out string message);

        /// <summary>
        /// 按地块儿和农作物统计种植面积
        /// </summary>
        /// <param name="landId">地块儿编号</param>
        /// <param name="cropsName">农作物名称</param>
        /// <param name="message">错误消息</param>
        /// <returns>返回数据</returns>
        public PlantArea StatisticsPlantAreaByCropsLand(int landId, string cropsName, out string message);

        /// <summary>
        /// 查询区域信息
        /// </summary>
        /// <param name="SqlWhere">查询条件</param>
        /// <param name="message">错误信息</param>
        /// <returns>返回值</returns>
        public List<osc_region> QueryOscRegion(string SqlWhere, out string message);

        /// <summary>
        /// 分页查询方法
        /// </summary>
        /// <param name="SqlWhere">查询条件</param>
        /// <param name="SortField">排序字段名</param>
        /// <param name="SortMethod">排序方法[ASC|DESC]</param>
        /// <param name="PageSize">每页记录数</param>
        /// <param name="CurPage">当前页</param>
        /// <param name="PageCount">总页数</param>
        /// <param name="message">错误消息</param>
        /// <returns>查询结果</returns>
        public List<osc_region> QueryOscRegionPage(string SqlWhere, string SortField, string SortMethod, int PageSize, int CurPage, out int PageCount, out string message);

        /// <summary>
        /// 查询单表方法
        /// </summary>
        /// <param name="SqlWhere">查询条件</param>
        /// <param name="message">错误消息</param>
        /// <returns>查询结果</returns>
        public List<vw_osc_region_group> QueryViewOscRegionGroup(string SqlWhere, out string message);

        /// <summary>
        /// 统计信息
        /// </summary>
        /// <param name="LandId">地块编号</param>
        /// <returns>数据表</returns>
        public DataTable StatisticsInfo(int LandId);

        #region CompanyInfo 增删改

        #region 增加数据

        /// <summary>
        /// 单条增加
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="message">消息</param>
        /// <returns>添加条数</returns>
        public Task<Message> InsertCompanyInfo(CompanyInfo model);

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="lists">批量数据</param>
        /// <param name="message">错误消息</param>
        /// <returns>添加条数</returns>
        public Task<Message> InsertCompanyInfo(List<CompanyInfo> lists);
        #endregion

        #region 修改数据
        /// <summary>
        /// 单条修改
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="SqlWhere">更新条件</param>
        /// <param name="message">消息</param>
        /// <returns>修改条数</returns>
        public Task<Message> UpdateCompanyInfo(CompanyInfo model, string SqlWhere, out string message);

        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="lists">批量数据</param>
        /// <param name="SqlWhere">更新条件</param>
        /// <param name="message">错误消息</param>
        /// <returns>修改条数</returns>
        public Task<Message> UpdateCompanyInfo(List<CompanyInfo> lists, string SqlWhere);
        #endregion

        #region 删除方法
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="IDStr">编号字符串</param>
        /// <returns>返回消息</returns>
        public Task<Message> DeleteCompanyInfo(string IDStr);
        #endregion

        #region 查询方法
        /// <summary>
        /// 查询单表方法
        /// </summary>
        /// <param name="SqlWhere">查询条件</param>
        /// <param name="message">错误消息</param>
        /// <returns>查询结果</returns>
        public List<CompanyInfo> QueryCompanyInfo(string SqlWhere, out string message);

        /// <summary>
        /// 分页查询方法
        /// </summary>
        /// <param name="SqlWhere">查询条件</param>
        /// <param name="SortField">排序字段名</param>
        /// <param name="SortMethod">排序方法[ASC|DESC]</param>
        /// <param name="PageSize">每页记录数</param>
        /// <param name="CurPage">当前页</param>
        /// <param name="PageCount">总页数</param>
        /// <param name="message">错误消息</param>
        /// <returns>查询结果</returns>
        public List<CompanyInfo> QueryCompanyInfo(string SqlWhere, string SortField, string SortMethod, int PageSize, int CurPage, out int PageCount, out string message);
        #endregion

        #endregion

        #region osc_region 增删改

        /// <summary>
        /// 添加 
        /// </summary>
        /// <param name="lists">批量数据</param>
        /// <param name="message">错误消息</param>
        /// <returns>添加条数</returns>
        public Message Insertosc_region(List<osc_region> lists,out int identity);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="lists">批量数据</param>
        /// <param name="SqlWhere">更新条件</param>
        /// <param name="message">错误消息</param>
        /// <returns>修改条数</returns>
        public Message Updateosc_region(List<osc_region> lists, string SqlWhere);

        /// <summary>
        /// 设置根地块儿
        /// </summary>
        /// <param name="landId">地块儿编号</param>
        /// <returns>错误消息</returns>
        public Message SetRootLand(string landId);

        /// <summary>
        /// 删除地块儿
        /// </summary>
        /// <param name="IDStr">ID字符串</param>
        /// <returns>返回消息</returns>
        public Task<Message> DeleteOscRegion(string IDStr);
        #endregion

        #region regions_gis 增删改

        /// <summary>
        /// 添加地块儿坐标范围数据 
        /// </summary>
        /// <param name="lists">批量数据</param>
        /// <param name="message">错误消息</param>
        /// <returns>添加条数</returns>
        public Message InsertRegionGis(List<region_gis> lists);

        /// <summary>
        /// 删除地块儿范围数据
        /// </summary>
        /// <param name="landId">地块儿编号</param>
        /// <returns>返回值</returns>
        public Task<Message> BetchDeleteRegionGisByLandId(string landId);

        /// <summary>
        /// 修改地块儿坐标范围数据 
        /// </summary>
        /// <param name="lists">批量数据</param>
        /// <param name="SqlWhere">更新条件</param>
        /// <param name="message">错误消息</param>
        /// <returns>修改条数</returns>
        public Task<Message> UpdateRegionGis(List<region_gis> lists, string SqlWhere);

        /// <summary>
        /// 查询region_gis数据
        /// </summary>
        /// <param name="SqlWhere">查询条件</param>
        /// <param name="message">错误消息</param>
        /// <returns>返回值</returns>
        public List<region_gis> QueryRegionGis(string SqlWhere, out string message);

        /// <summary>
        /// 分页查询region_gis数据
        /// </summary>
        /// <param name="SqlWhere">查询条件</param>
        /// <param name="SortField">排序字段</param>
        /// <param name="SortMethod">排序方法</param>
        /// <param name="PageSize">每页分页数据</param>
        /// <param name="CurPage">当前页</param>
        /// <param name="TotalNumber">总数据量</param>
        /// <param name="PageCount">总页数</param>
        /// <param name="message">错误消息</param>
        /// <returns>返回数据</returns>
        public List<region_gis> QueryPageRegionGis(string SqlWhere,string SortField,string SortMethod,int PageSize,int CurPage,out int TotalNumber,out int PageCount,out string message);
        #endregion
    }
}
