using Common;
using ePioneer.Data.Kernel;
using MapOfIndustryDataAccess.Data;
using MapOfIndustryDataAccess.Entities;
using MapOfIndustryDataAccess.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapOfIndustryDataAccess
{
    /// <summary>
    /// 数据库操作类
    /// </summary>
    public class MOIRepository : BaseDataProvider, IMOIRepository
    {
        #region Fields

        /// <summary>
        /// 连接字符串
        /// </summary>
        private static string m_connectionString;

        /// <summary>
        /// 数据库帮助类
        /// </summary>
        private static DbHelper m_dbHelper = null;

        /// <summary>
        /// Redis帮助类
        /// </summary>
        private static RedisHelper m_redisHelper = null;
        #endregion

        public MOIRepository() : base(MOIRepository.m_connectionString)
        {
            CreateDBHelper();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        public MOIRepository(String connectionString) : base(connectionString)
        {
            if (Database != null)
                m_dbHelper = Database;
        }


        /// <summary>
        /// 创建Redis缓存对象
        /// </summary>
        /// <returns>Redis缓存</returns>
        public RedisHelper CreateRedisHelper()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            string m_redis_connectionString = configuration["RedisConnection:Connection"];
            if (m_redisHelper == null)
                m_redisHelper = new RedisHelper(m_redis_connectionString);
            return m_redisHelper;
        }

        #region Common

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="where">查询条件</param>
        /// <param name="oderBy">排序字符串</param>
        /// <param name="fields">字段列表</param>
        /// <returns>返回值</returns>
        public Task<PagerSet> GetPagerSetAsync(String tableName, Int32 pageIndex, Int32 pageSize, String where, String oderBy, String[] fields)
        {
            return GetPagerSet2Async(new PagerParameters(tableName, "ORDER BY " + oderBy, "WHERE " + where, pageIndex, pageSize, fields)
            {
                CacherSize = 2
            });
        }

        /// <summary>
        /// 分页方法重载
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="where">查询条件</param>
        /// <param name="oderBy">排序字符串</param>
        /// <returns>返回值</returns>
        public Task<PagerSet> GetPagerSetAsync(String tableName, Int32 pageIndex, Int32 pageSize, String where, String orderBy)
        {
            return GetPagerSet2Async(new PagerParameters(tableName, "ORDER BY " + orderBy, "WHERE " + where, pageIndex, pageSize));
        }

        /// <summary>
        /// 异步查询方法(有返回结果)
        /// </summary>
        /// <param name="commandText">SQL字符串</param>
        /// <returns>查询结果</returns>
        public Task<DataSet> ExecuteDataSetAsync(String commandText)
        {
            return m_dbHelper.ExecuteDataSetAsync(CommandType.Text, commandText);
        }

        /// <summary>
        /// 查询方法(有返回结果)
        /// </summary>
        /// <param name="commandText">SQL字符串</param>
        /// <returns>查询结果</returns>
        public DataSet ExecuteDataSet(String commandText)
        {
            return m_dbHelper.ExecuteDataSet(CommandType.Text, commandText);
        }

        /// <summary>
        /// 带参数查询方法(有返回结果)
        /// </summary>
        /// <param name="commandText">SQL字符串</param>
        /// <param name="commandType">SQL语句类型</param>
        /// <param name="sqlparms">参数集合</param>
        /// <param name="message">错误消息</param>
        /// <returns>查询结果</returns>
        public DataSet ExecuteDataSet(string commandText, CommandType commandType, DbParameter[] sqlparms, out string message)
        {
            DataSet result = null;
            message = string.Empty;
            try
            {
                result = m_dbHelper.ExecuteDataSet(commandType, commandText, sqlparms);
            }
            catch (Exception exp)
            {
                message = exp.Message;
            }
            return result;
        }

        /// <summary>
        /// 异步查询方法(增删改语句)
        /// </summary>
        /// <param name="commandText">SQL字符串</param>
        /// <returns>查询结果</returns>
        public Task<Int32> ExecuteNonQueryAsync(String commandText)
        {
            return m_dbHelper.ExecuteNonQueryAsync(commandText);
        }

        /// <summary>
        /// 查询方法(增删改查语句)
        /// </summary>
        /// <param name="commandText">SQL字符串</param>
        /// <returns>查询结果</returns>
        public Int32 ExecuteNonQuery(String commandText)
        {
            return m_dbHelper.ExecuteNonQuery(commandText);
        }

        public Int32 ExecuteNonQuery(CommandType commandType, String commandText, params DbParameter[] commandParameters)
        {
            return m_dbHelper.ExecuteNonQuery(commandType, commandText, commandParameters);
        }

        public List<T> QueryPage<T>(
                string SqlWhere,
                string SortField,
                string SortMethod,
                string FieldStr,
                string TableName,
                int PageSize,
                int CurPage,
                Func<DataRow, T, T> ReadDataRow,
                out int TotalNumber,
                out int PageCount,
                out string message)
        {
            message = string.Empty;
            PageCount = 0;
            TotalNumber = 0;
            DataSet ds = null;
            DataTable dt = null;
            List<T> result = new List<T>();
            DbParameter[] sqlparm = new DbParameter[] {
                m_dbHelper.MakeInParam("CurPage",CurPage),
                m_dbHelper.MakeInParam("PageSize",PageSize),
                m_dbHelper.MakeOutParam("TotalNumber",typeof(System.Int32)),
                m_dbHelper.MakeOutParam("PageCount",typeof(System.Int32)),
                m_dbHelper.MakeInParam("FieldStr",FieldStr),
                m_dbHelper.MakeInParam("TableName",TableName),
                m_dbHelper.MakeInParam("SortMethod",SortMethod),
                m_dbHelper.MakeInParam("SortField",SortField),
                m_dbHelper.MakeInParam("SqlWhere",SqlWhere)
            };
            try
            {
                ds = m_dbHelper.ExecuteDataSet(CommandType.StoredProcedure, "QueryPage", sqlparm);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            #region 非空检查
            if (ds == null)
                return result;
            if (ds.Tables == null || ds.Tables.Count == 0)
                return result;
            dt = ds.Tables[0];
            if (dt == null)
                return result;
            if (dt.Rows.Count == 0)
                return result;
            #endregion
            TotalNumber = Convert.ToInt32(sqlparm[2].Value);
            foreach (DataRow dr in dt.Rows)
            {
                T t = default(T);
                result.Add(ReadDataRow(dr, t));
            }
            return result;
        }
        #endregion

        #region Private

        private void CreateDBHelper()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            m_connectionString = configuration["ConnectionStrings:MOIConnStr"];
            m_dbHelper = new DbHelper(m_connectionString);
        }

        /// <summary>
        /// 读取数据行到model
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="dr">数据行</param>
        private void readMassifGreenHouseVPDataRow(ref MassifGreenHouseVP model, DataRow dr)
        {
            //自增编号
            model.ID = Convert.IsDBNull(dr["ID"]) ? 0 : Convert.ToInt64(dr["ID"]);
            //地块编号_1条地块对应多条数据
            model.LandId = Convert.IsDBNull(dr["LandId"]) ? 0 : Convert.ToInt32(dr["LandId"]);
            //地块名称
            model.LandName = Convert.IsDBNull(dr["LandName"]) ? string.Empty : Convert.ToString(dr["LandName"]).Trim();
            //农作物名称
            model.CropsName = Convert.IsDBNull(dr["CropsName"]) ? string.Empty : Convert.ToString(dr["CropsName"]).Trim();
            //农作物的种植面积_单位亩
            model.PlantingArea = Convert.IsDBNull(dr["PlantingArea"]) ? 0 : Convert.ToDecimal(dr["PlantingArea"]);
            //务工人口数量
            model.JobPopulation = Convert.IsDBNull(dr["JobPopulation"]) ? 0 : Convert.ToInt32(dr["JobPopulation"]);
            //总人口数量
            model.TotalPopulation = Convert.IsDBNull(dr["TotalPopulation"]) ? 0 : Convert.ToInt32(dr["TotalPopulation"]);
            //总产量
            model.TotalOutput = Convert.IsDBNull(dr["TotalOutput"]) ? 0 : Convert.ToDecimal(dr["TotalOutput"]);
            //总产值_农作物的总产值
            model.TotaValue = Convert.IsDBNull(dr["TotaValue"]) ? 0 : Convert.ToDecimal(dr["TotaValue"]);
            //土壤类型
            model.SoilType = Convert.IsDBNull(dr["SoilType"]) ? string.Empty : Convert.ToString(dr["SoilType"]).Trim();
            //灌溉类型
            model.WateringType = Convert.IsDBNull(dr["WateringType"]) ? string.Empty : Convert.ToString(dr["WateringType"]).Trim();
            //土地属性_租赁或者购买
            model.LandProperty = Convert.IsDBNull(dr["LandProperty"]) ? string.Empty : Convert.ToString(dr["LandProperty"]).Trim();
            //土地租赁年限
            model.LeaseYear = Convert.IsDBNull(dr["LeaseYear"]) ? 0 : Convert.ToInt32(dr["LeaseYear"]);
            //同一作物种植年限
            model.CropOutput = Convert.IsDBNull(dr["CropOutput"]) ? 0 : Convert.ToDouble(dr["CropOutput"]);
            //上一年度作物产量
            model.LastYearOutput = Convert.IsDBNull(dr["LastYearOutput"]) ? string.Empty : Convert.ToString(dr["LastYearOutput"]).Trim();
            //本年度作物产量
            model.CurrentYearOutput = Convert.IsDBNull(dr["CurrentYearOutput"]) ? 0 : Convert.ToDecimal(dr["CurrentYearOutput"]);
            //农作物售价_单位公斤
            model.CropsSalesPrice = Convert.IsDBNull(dr["CropsSalesPrice"]) ? 0 : Convert.ToDecimal(dr["CropsSalesPrice"]);
            //录入时间_录入数据的时间_用户可编辑
            model.EnterTime = Convert.IsDBNull(dr["EnterTime"]) ? (DateTime)SqlDateTime.MinValue : Convert.ToDateTime(dr["EnterTime"]);
            //添加人
            model.Creater = Convert.IsDBNull(dr["Creater"]) ? string.Empty : Convert.ToString(dr["Creater"]).Trim();
            //添加时间
            model.CreatedTime = Convert.IsDBNull(dr["CreatedTime"]) ? (DateTime)SqlDateTime.MinValue : Convert.ToDateTime(dr["CreatedTime"]);
            //修改人
            model.Modifier = Convert.IsDBNull(dr["Modifier"]) ? string.Empty : Convert.ToString(dr["Modifier"]).Trim();
            //修改时间
            model.ModifiedTime = Convert.IsDBNull(dr["ModifiedTime"]) ? (DateTime)SqlDateTime.MinValue : Convert.ToDateTime(dr["ModifiedTime"]);
        }

        ///<summary>
        ///检查是否超过长度
        ///</summary>
        ///<param name="lists">数据集</param>
        ///<param name="message">错误消息</param>
        private void CheckMaxLength(ref List<MassifGreenHouseVP> lists, out string message)
        {
            #region 声明变量

            //错误消息
            message = string.Empty;

            //超过的长度
            int OutLength = 0;
            #endregion

            #region 循环验证长度
            for (int i = 0; i < lists.Count; i++)
            {
                if (!string.IsNullOrEmpty(lists[i].LandName))
                {
                    if (lists[i].LandName.Length > 100)
                    {
                        OutLength = lists[i].CropsName.Length - 100;
                        message += "字段名[LandName]描述[地块名称]超长、字段最长[100]实际" + lists[i].CropsName.Length + "超过长度" + OutLength + ",";
                    }
                }
                if (!string.IsNullOrEmpty(lists[i].CropsName))
                {
                    if (lists[i].CropsName.Length > 16)
                    {
                        OutLength = lists[i].CropsName.Length - 16;
                        message += "字段名[CropsName]描述[农作物名称]超长、字段最长[16]实际" + lists[i].CropsName.Length + "超过长度" + OutLength + ",";
                    }
                }
                if (!string.IsNullOrEmpty(lists[i].SoilType))
                {
                    if (lists[i].SoilType.Length > 16)
                    {
                        OutLength = lists[i].SoilType.Length - 16;
                        message += "字段名[SoilType]描述[土壤类型]超长、字段最长[16]实际" + lists[i].SoilType.Length + "超过长度" + OutLength + ",";
                    }
                }
                if (!string.IsNullOrEmpty(lists[i].WateringType))
                {
                    if (lists[i].WateringType.Length > 16)
                    {
                        OutLength = lists[i].WateringType.Length - 16;
                        message += "字段名[WateringType]描述[灌溉类型]超长、字段最长[16]实际" + lists[i].WateringType.Length + "超过长度" + OutLength + ",";
                    }
                }
                if (!string.IsNullOrEmpty(lists[i].LandProperty))
                {
                    if (lists[i].LandProperty.Length > 50)
                    {
                        OutLength = lists[i].LandProperty.Length - 50;
                        message += "字段名[LandProperty]描述[土地属性_租赁或者购买]超长、字段最长[50]实际" + lists[i].LandProperty.Length + "超过长度" + OutLength + ",";
                    }
                }
                if (!string.IsNullOrEmpty(lists[i].LastYearOutput))
                {
                    if (lists[i].LastYearOutput.Length > 16)
                    {
                        OutLength = lists[i].LastYearOutput.Length - 16;
                        message += "字段名[LastYearOutput]描述[上一年度作物产量]超长、字段最长[16]实际" + lists[i].LastYearOutput.Length + "超过长度" + OutLength + ",";
                    }
                }
                if (!string.IsNullOrEmpty(lists[i].Creater))
                {
                    if (lists[i].Creater.Length > 16)
                    {
                        OutLength = lists[i].Creater.Length - 16;
                        message += "字段名[Creater]描述[添加人]超长、字段最长[16]实际" + lists[i].Creater.Length + "超过长度" + OutLength + ",";
                    }
                }
                if (!string.IsNullOrEmpty(lists[i].Modifier))
                {
                    if (lists[i].Modifier.Length > 16)
                    {
                        OutLength = lists[i].Modifier.Length - 16;
                        message += "字段名[Modifier]描述[修改人]超长、字段最长[16]实际" + lists[i].Modifier.Length + "超过长度" + OutLength + ",";
                    }
                }
            }
            #endregion

            if (!string.IsNullOrEmpty(message)) message = message.Substring(0, message.Length - 1);
        }

        ///<summary>
        ///检查是否空值
        ///</summary>
        private void CheckEmpty(ref List<MassifGreenHouseVP> lists)
        {
            for (int i = 0; i < lists.Count; i++)
            {
                //自增编号
                lists[i].ID = lists[i].ID == null ? Convert.ToInt64(0) : Convert.ToInt64(lists[i].ID);
                //地块编号_1条地块对应多条数据
                lists[i].LandId = lists[i].LandId == null ? Convert.ToInt32(0) : Convert.ToInt32(lists[i].LandId);
                //地块名称
                lists[i].LandName = string.IsNullOrEmpty(lists[i].LandName) ? string.Empty : Convert.ToString(lists[i].LandName).Trim();
                //农作物名称
                lists[i].CropsName = string.IsNullOrEmpty(lists[i].CropsName) ? string.Empty : Convert.ToString(lists[i].CropsName).Trim();
                //农作物的种植面积_单位亩
                lists[i].PlantingArea = lists[i].PlantingArea == null ? Convert.ToDecimal(0) : Convert.ToDecimal(lists[i].PlantingArea);
                //务工人口数量
                lists[i].JobPopulation = lists[i].JobPopulation == null ? Convert.ToInt32(0) : Convert.ToInt32(lists[i].JobPopulation);
                //总人口数量
                lists[i].TotalPopulation = lists[i].TotalPopulation == null ? Convert.ToInt32(0) : Convert.ToInt32(lists[i].TotalPopulation);
                //总产量
                lists[i].TotalOutput = lists[i].TotalOutput == null ? Convert.ToDecimal(0) : Convert.ToDecimal(lists[i].TotalOutput);
                //总产值_农作物的总产值
                lists[i].TotaValue = lists[i].TotaValue == null ? Convert.ToDecimal(0) : Convert.ToDecimal(lists[i].TotaValue);
                //土壤类型
                lists[i].SoilType = string.IsNullOrEmpty(lists[i].SoilType) ? string.Empty : Convert.ToString(lists[i].SoilType).Trim();
                //灌溉类型
                lists[i].WateringType = string.IsNullOrEmpty(lists[i].WateringType) ? string.Empty : Convert.ToString(lists[i].WateringType).Trim();
                //土地属性_租赁或者购买
                lists[i].LandProperty = string.IsNullOrEmpty(lists[i].LandProperty) ? string.Empty : Convert.ToString(lists[i].LandProperty).Trim();
                //土地租赁年限
                lists[i].LeaseYear = lists[i].LeaseYear == null ? Convert.ToInt32(0) : Convert.ToInt32(lists[i].LeaseYear);
                //同一作物种植年限
                lists[i].CropOutput = lists[i].CropOutput == null ? Convert.ToDouble(0) : Convert.ToDouble(lists[i].CropOutput);
                //上一年度作物产量
                lists[i].LastYearOutput = string.IsNullOrEmpty(lists[i].LastYearOutput) ? string.Empty : Convert.ToString(lists[i].LastYearOutput).Trim();
                //本年度作物产量
                lists[i].CurrentYearOutput = lists[i].CurrentYearOutput == null ? Convert.ToDecimal(0) : Convert.ToDecimal(lists[i].CurrentYearOutput);
                //农作物售价_单位公斤
                lists[i].CropsSalesPrice = lists[i].CropsSalesPrice == null ? Convert.ToDecimal(0) : Convert.ToDecimal(lists[i].CropsSalesPrice);
                //录入时间_录入数据的时间_用户可编辑
                lists[i].EnterTime = lists[i].EnterTime == null ? (DateTime)SqlDateTime.MinValue : Convert.ToDateTime(lists[i].EnterTime.GetValueOrDefault());
                //添加人
                lists[i].Creater = string.IsNullOrEmpty(lists[i].Creater) ? string.Empty : Convert.ToString(lists[i].Creater).Trim();
                //添加时间
                lists[i].CreatedTime = lists[i].CreatedTime == null ? (DateTime)SqlDateTime.MinValue : Convert.ToDateTime(lists[i].CreatedTime.GetValueOrDefault());
                //修改人
                lists[i].Modifier = string.IsNullOrEmpty(lists[i].Modifier) ? string.Empty : Convert.ToString(lists[i].Modifier).Trim();
                //修改时间
                lists[i].ModifiedTime = lists[i].ModifiedTime == null ? (DateTime)SqlDateTime.MinValue : Convert.ToDateTime(lists[i].ModifiedTime.GetValueOrDefault());
            }
        }

        /// <summary>
        /// 读取数据行到model
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="dr">数据行</param>
        private void ReadDataRow(ref vw_MassifGreenHouseVP model, DataRow dr)
        {
            //
            model.ID = Convert.IsDBNull(dr["ID"]) ? 0 : Convert.ToInt64(dr["ID"]);
            //
            model.LandId = Convert.IsDBNull(dr["LandId"]) ? 0 : Convert.ToInt32(dr["LandId"]);
            //
            model.LandName = Convert.IsDBNull(dr["LandName"]) ? string.Empty : Convert.ToString(dr["LandName"]).Trim();
            //
            model.CropsName = Convert.IsDBNull(dr["CropsName"]) ? string.Empty : Convert.ToString(dr["CropsName"]).Trim();
            //
            model.PlantingArea = Convert.IsDBNull(dr["PlantingArea"]) ? 0 : Convert.ToDecimal(dr["PlantingArea"]);
            //
            model.JobPopulation = Convert.IsDBNull(dr["JobPopulation"]) ? 0 : Convert.ToInt32(dr["JobPopulation"]);
            //
            model.TotalPopulation = Convert.IsDBNull(dr["TotalPopulation"]) ? 0 : Convert.ToInt32(dr["TotalPopulation"]);
            //
            model.TotalOutput = Convert.IsDBNull(dr["TotalOutput"]) ? 0 : Convert.ToDecimal(dr["TotalOutput"]);
            //
            model.TotaValue = Convert.IsDBNull(dr["TotaValue"]) ? 0 : Convert.ToDecimal(dr["TotaValue"]);
            //
            model.SoilType = Convert.IsDBNull(dr["SoilType"]) ? string.Empty : Convert.ToString(dr["SoilType"]).Trim();
            //
            model.WateringType = Convert.IsDBNull(dr["WateringType"]) ? string.Empty : Convert.ToString(dr["WateringType"]).Trim();
            //
            model.LandProperty = Convert.IsDBNull(dr["LandProperty"]) ? string.Empty : Convert.ToString(dr["LandProperty"]).Trim();
            //
            model.CropOutput = Convert.IsDBNull(dr["CropOutput"]) ? 0 : Convert.ToDouble(dr["CropOutput"]);
            //
            model.LeaseYear = Convert.IsDBNull(dr["LeaseYear"]) ? 0 : Convert.ToInt32(dr["LeaseYear"]);
            //
            model.LastYearOutput = Convert.IsDBNull(dr["LastYearOutput"]) ? string.Empty : Convert.ToString(dr["LastYearOutput"]).Trim();
            //
            model.CurrentYearOutput = Convert.IsDBNull(dr["CurrentYearOutput"]) ? 0 : Convert.ToDecimal(dr["CurrentYearOutput"]);
            //
            model.CropsSalesPrice = Convert.IsDBNull(dr["CropsSalesPrice"]) ? 0 : Convert.ToDecimal(dr["CropsSalesPrice"]);
            //
            model.EnterTime = Convert.IsDBNull(dr["EnterTime"]) ? (DateTime)SqlDateTime.MinValue : Convert.ToDateTime(dr["EnterTime"]);
            //
            model.Creater = Convert.IsDBNull(dr["Creater"]) ? string.Empty : Convert.ToString(dr["Creater"]).Trim();
            //
            model.CreatedTime = Convert.IsDBNull(dr["CreatedTime"]) ? (DateTime)SqlDateTime.MinValue : Convert.ToDateTime(dr["CreatedTime"]);
            //
            model.Modifier = Convert.IsDBNull(dr["Modifier"]) ? string.Empty : Convert.ToString(dr["Modifier"]).Trim();
            //
            model.ModifiedTime = Convert.IsDBNull(dr["ModifiedTime"]) ? (DateTime)SqlDateTime.MinValue : Convert.ToDateTime(dr["ModifiedTime"]);
            //
            model.parent_id = Convert.IsDBNull(dr["parent_id"]) ? 0 : Convert.ToInt32(dr["parent_id"]);
            //
            model.citycode = Convert.IsDBNull(dr["citycode"]) ? string.Empty : Convert.ToString(dr["citycode"]).Trim();
            //
            model.adcode = Convert.IsDBNull(dr["adcode"]) ? string.Empty : Convert.ToString(dr["adcode"]).Trim();
            //
            model.level = Convert.IsDBNull(dr["level"]) ? string.Empty : Convert.ToString(dr["level"]).Trim();
            //
            model.center = Convert.IsDBNull(dr["center"]) ? string.Empty : Convert.ToString(dr["center"]).Trim();
        }

        /// <summary>
        /// 读取数据行到model
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="dr">数据行</param>
        private void ReadDataRow(ref vw_osc_region model, DataRow dr)
        {
            //
            model.Id = Convert.IsDBNull(dr["id"]) ? 0 : Convert.ToInt32(dr["id"]);
            //
            model.Name = Convert.IsDBNull(dr["name"]) ? string.Empty : Convert.ToString(dr["name"]).Trim();
            //
            model.Parent_id = Convert.IsDBNull(dr["parent_id"]) ? 0 : Convert.ToInt32(dr["parent_id"]);
            //
            model.Citycode = Convert.IsDBNull(dr["citycode"]) ? string.Empty : Convert.ToString(dr["citycode"]).Trim();
            //
            model.Adcode = Convert.IsDBNull(dr["adcode"]) ? string.Empty : Convert.ToString(dr["adcode"]).Trim();
            //
            model.Center = Convert.IsDBNull(dr["center"]) ? string.Empty : Convert.ToString(dr["center"]).Trim();
            //
            model.Level = Convert.IsDBNull(dr["level"]) ? string.Empty : Convert.ToString(dr["level"]).Trim();
            //
            model.GPSLocations = Convert.IsDBNull(dr["GPSLocations"]) ? string.Empty : Convert.ToString(dr["GPSLocations"]).Trim();
            //
            model.LandId = Convert.IsDBNull(dr["LandId"]) ? 0 : Convert.ToInt32(dr["LandId"]);
            //
            model.LandName = Convert.IsDBNull(dr["LandName"]) ? string.Empty : Convert.ToString(dr["LandName"]).Trim();
            //
            model.CropsName = Convert.IsDBNull(dr["CropsName"]) ? string.Empty : Convert.ToString(dr["CropsName"]).Trim();
            //
            model.PlantingArea = Convert.IsDBNull(dr["PlantingArea"]) ? 0 : Convert.ToDecimal(dr["PlantingArea"]);
            //
            model.JobPopulation = Convert.IsDBNull(dr["JobPopulation"]) ? 0 : Convert.ToInt32(dr["JobPopulation"]);
            //
            model.TotalPopulation = Convert.IsDBNull(dr["TotalPopulation"]) ? 0 : Convert.ToInt32(dr["TotalPopulation"]);
            //
            model.TotalOutput = Convert.IsDBNull(dr["TotalOutput"]) ? 0 : Convert.ToDecimal(dr["TotalOutput"]);
            //
            model.TotaValue = Convert.IsDBNull(dr["TotaValue"]) ? 0 : Convert.ToDecimal(dr["TotaValue"]);
            //
            model.SoilType = Convert.IsDBNull(dr["SoilType"]) ? string.Empty : Convert.ToString(dr["SoilType"]).Trim();
            //
            model.WateringType = Convert.IsDBNull(dr["WateringType"]) ? string.Empty : Convert.ToString(dr["WateringType"]).Trim();
            //
            model.LandProperty = Convert.IsDBNull(dr["LandProperty"]) ? string.Empty : Convert.ToString(dr["LandProperty"]).Trim();
            //
            model.LeaseYear = Convert.IsDBNull(dr["LeaseYear"]) ? 0 : Convert.ToInt32(dr["LeaseYear"]);
            //
            model.CropOutput = Convert.IsDBNull(dr["CropOutput"]) ? 0 : Convert.ToDouble(dr["CropOutput"]);
            //
            model.LastYearOutput = Convert.IsDBNull(dr["LastYearOutput"]) ? string.Empty : Convert.ToString(dr["LastYearOutput"]).Trim();
            //
            model.CurrentYearOutput = Convert.IsDBNull(dr["CurrentYearOutput"]) ? 0 : Convert.ToDecimal(dr["CurrentYearOutput"]);
            //
            model.CropsSalesPrice = Convert.IsDBNull(dr["CropsSalesPrice"]) ? 0 : Convert.ToDecimal(dr["CropsSalesPrice"]);
            //
            model.EnterTime = Convert.IsDBNull(dr["EnterTime"]) ? (DateTime)SqlDateTime.MinValue : Convert.ToDateTime(dr["EnterTime"]);
            //
            model.Creater = Convert.IsDBNull(dr["Creater"]) ? string.Empty : Convert.ToString(dr["Creater"]).Trim();
            //
            model.CreatedTime = Convert.IsDBNull(dr["CreatedTime"]) ? (DateTime)SqlDateTime.MinValue : Convert.ToDateTime(dr["CreatedTime"]);
            //
            model.Modifier = Convert.IsDBNull(dr["Modifier"]) ? string.Empty : Convert.ToString(dr["Modifier"]).Trim();
            //
            model.ModifiedTime = Convert.IsDBNull(dr["ModifiedTime"]) ? (DateTime)SqlDateTime.MinValue : Convert.ToDateTime(dr["ModifiedTime"]);
            //
            model.Color = Convert.IsDBNull(dr["Color"]) ? string.Empty : Convert.ToString(dr["Color"]).Trim();
        }

        /// <summary>
        /// 读取数据行到model
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="dr">数据行</param>
        private void ReadDataRow(ref osc_region model, DataRow dr)
        {
            //
            model.id = Convert.IsDBNull(dr["id"]) ? 0 : Convert.ToInt32(dr["id"]);
            //
            model.name = Convert.IsDBNull(dr["name"]) ? string.Empty : Convert.ToString(dr["name"]).Trim();
            //
            model.parent_id = Convert.IsDBNull(dr["parent_id"]) ? 0 : Convert.ToInt32(dr["parent_id"]);
            //
            model.citycode = Convert.IsDBNull(dr["citycode"]) ? string.Empty : Convert.ToString(dr["citycode"]).Trim();
            //
            model.adcode = Convert.IsDBNull(dr["adcode"]) ? string.Empty : Convert.ToString(dr["adcode"]).Trim();
            //
            model.center = Convert.IsDBNull(dr["center"]) ? string.Empty : Convert.ToString(dr["center"]).Trim();
            //
            model.level = Convert.IsDBNull(dr["level"]) ? string.Empty : Convert.ToString(dr["level"]).Trim();
        }

        /// <summary>
        /// 读取数据行到model
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="dr">数据行</param>
        private void ReadDataRow(ref vw_osc_region_group model, DataRow dr)
        {
            //
            model.Id = Convert.IsDBNull(dr["id"]) ? 0 : Convert.ToInt32(dr["id"]);
            //
            model.Name = Convert.IsDBNull(dr["name"]) ? string.Empty : Convert.ToString(dr["name"]).Trim();
            //
            model.Parent_id = Convert.IsDBNull(dr["parent_id"]) ? 0 : Convert.ToInt32(dr["parent_id"]);
            //
            model.Citycode = Convert.IsDBNull(dr["citycode"]) ? string.Empty : Convert.ToString(dr["citycode"]).Trim();
            //
            model.Adcode = Convert.IsDBNull(dr["adcode"]) ? string.Empty : Convert.ToString(dr["adcode"]).Trim();
            //
            model.Center = Convert.IsDBNull(dr["center"]) ? string.Empty : Convert.ToString(dr["center"]).Trim();
            //
            model.Level = Convert.IsDBNull(dr["level"]) ? string.Empty : Convert.ToString(dr["level"]).Trim();
            //
            model.GPSLocations = Convert.IsDBNull(dr["GPSLocations"]) ? string.Empty : Convert.ToString(dr["GPSLocations"]).Trim();
            //
            model.CropsName = Convert.IsDBNull(dr["CropsName"]) ? string.Empty : Convert.ToString(dr["CropsName"]).Trim();
            //
            model.SoilType = Convert.IsDBNull(dr["SoilType"]) ? string.Empty : Convert.ToString(dr["SoilType"]).Trim();
            //
            model.WateringType = Convert.IsDBNull(dr["WateringType"]) ? string.Empty : Convert.ToString(dr["WateringType"]).Trim();
            //
            model.LandProperty = Convert.IsDBNull(dr["LandProperty"]) ? string.Empty : Convert.ToString(dr["LandProperty"]).Trim();
            //
            model.PlantingArea = Convert.IsDBNull(dr["PlantingArea"]) ? 0 : Convert.ToDecimal(dr["PlantingArea"]);
            //
            model.LeaseYear = Convert.IsDBNull(dr["LeaseYear"]) ? 0 : Convert.ToDecimal(dr["LeaseYear"]);
            //
            model.CropOutput = Convert.IsDBNull(dr["CropOutput"]) ? 0 : Convert.ToDecimal(dr["CropOutput"]);
            //
            model.LastYearOutput = Convert.IsDBNull(dr["LastYearOutput"]) ? 0 : Convert.ToDecimal(dr["LastYearOutput"]);
            //
            model.CurrentYearOutput = Convert.IsDBNull(dr["CurrentYearOutput"]) ? 0 : Convert.ToDecimal(dr["CurrentYearOutput"]);
            //
            model.CropsSalesPrice = Convert.IsDBNull(dr["CropsSalesPrice"]) ? 0 : Convert.ToDecimal(dr["CropsSalesPrice"]);
            //
            model.JobPopulation = Convert.IsDBNull(dr["JobPopulation"]) ? 0 : Convert.ToDecimal(dr["JobPopulation"]);
            //
            model.TotalPopulation = Convert.IsDBNull(dr["TotalPopulation"]) ? 0 : Convert.ToDecimal(dr["TotalPopulation"]);
            //
            model.TotalOutput = Convert.IsDBNull(dr["TotalOutput"]) ? 0 : Convert.ToDecimal(dr["TotalOutput"]);
            //
            model.TotaValue = Convert.IsDBNull(dr["TotaValue"]) ? 0 : Convert.ToDecimal(dr["TotaValue"]);
        }

        /// <summary>
        /// 获得存储过程参数
        /// </summary>
        /// <param name="addMOI_massifGreenHouseVP">数据</param>
        /// <returns>参数List</returns>
        private List<DbParameter> GetDbParameters(MassifGreenHouseVP addMassifGreenHouseVP, string SqlWhere = "")
        {
            List<DbParameter> list = null;
            if (addMassifGreenHouseVP != null)
            {
                list = new List<DbParameter>() {
                 Database.MakeInParam("ID",addMassifGreenHouseVP.ID),
                 Database.MakeInParam("LandId",addMassifGreenHouseVP.LandId),
                 Database.MakeInParam("LandName",addMassifGreenHouseVP.LandName),
                 Database.MakeInParam("CropsName",addMassifGreenHouseVP.CropsName),
                 Database.MakeInParam("PlantingArea",addMassifGreenHouseVP.PlantingArea),
                 Database.MakeInParam("JobPopulation",addMassifGreenHouseVP.JobPopulation),
                 Database.MakeInParam("TotalPopulation",addMassifGreenHouseVP.TotalPopulation),
                 Database.MakeInParam("TotalOutput",addMassifGreenHouseVP.TotalOutput),
                 Database.MakeInParam("TotaValue",addMassifGreenHouseVP.TotaValue),
                 Database.MakeInParam("SoilType",addMassifGreenHouseVP.SoilType),
                 Database.MakeInParam("WateringType",addMassifGreenHouseVP.WateringType),
                 Database.MakeInParam("LandProperty",addMassifGreenHouseVP.LandProperty),
                 Database.MakeInParam("LeaseYear",addMassifGreenHouseVP.LeaseYear),
                 Database.MakeInParam("CropOutput",addMassifGreenHouseVP.CropOutput),
                 Database.MakeInParam("LastYearOutput",addMassifGreenHouseVP.LastYearOutput),
                 Database.MakeInParam("CurrentYearOutput",addMassifGreenHouseVP.CurrentYearOutput),
                 Database.MakeInParam("CropsSalesPrice",addMassifGreenHouseVP.CropsSalesPrice),
                 Database.MakeInParam("EnterTime",addMassifGreenHouseVP.EnterTime),
                 Database.MakeInParam("Creater",addMassifGreenHouseVP.Creater),
                 Database.MakeInParam("CreatedTime",addMassifGreenHouseVP.CreatedTime),
                 Database.MakeInParam("Modifier",addMassifGreenHouseVP.Modifier),
                 Database.MakeInParam("ModifiedTime",addMassifGreenHouseVP.ModifiedTime)
              };
                if (!string.IsNullOrEmpty(SqlWhere))
                    list.Add(m_dbHelper.MakeInParam("SqlWhere", SqlWhere));
            }
            return list;
        }

        ///<summary>
        ///生成更新Sql语句
        ///</summary>
        ///<param name="lists">数据List</param>
        ///<param name="SqlWhere">更新条件</param>
        ///<returns>更新Sql语句字符串数组</returns>
        private List<string> MarkUpdateSql(List<MassifGreenHouseVP> lists, string SqlWhere)
        {
            List<string> result = new List<string>();
            foreach (MassifGreenHouseVP model in lists)
            {
                #region 拼写Sql语句
                string Sql = "update MassifGreenHouseVP set ";
                Sql += "LandId='" + FilteSQLStr(model.LandId.GetValueOrDefault()) + "',";
                Sql += "LandName='" + FilteSQLStr(model.LandName) + "',";
                Sql += "CropsName='" + FilteSQLStr(model.CropsName) + "',";
                Sql += "PlantingArea='" + FilteSQLStr(model.PlantingArea.GetValueOrDefault()) + "',";
                Sql += "JobPopulation='" + FilteSQLStr(model.JobPopulation.GetValueOrDefault()) + "',";
                Sql += "TotalPopulation='" + FilteSQLStr(model.TotalPopulation.GetValueOrDefault()) + "',";
                Sql += "TotalOutput='" + FilteSQLStr(model.TotalOutput.GetValueOrDefault()) + "',";
                Sql += "TotaValue='" + FilteSQLStr(model.TotaValue.GetValueOrDefault()) + "',";
                Sql += "SoilType='" + FilteSQLStr(model.SoilType) + "',";
                Sql += "WateringType='" + FilteSQLStr(model.WateringType) + "',";
                Sql += "LandProperty='" + FilteSQLStr(model.LandProperty) + "',";
                Sql += "LeaseYear='" + FilteSQLStr(model.LeaseYear.GetValueOrDefault()) + "',";
                Sql += "CropOutput='" + FilteSQLStr(model.CropOutput.GetValueOrDefault()) + "',";
                Sql += "LastYearOutput='" + FilteSQLStr(model.LastYearOutput) + "',";
                Sql += "CurrentYearOutput='" + FilteSQLStr(model.CurrentYearOutput.GetValueOrDefault()) + "',";
                Sql += "CropsSalesPrice='" + FilteSQLStr(model.CropsSalesPrice.GetValueOrDefault()) + "',";
                Sql += "EnterTime=CAST('" + model.EnterTime.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss") + "' AS DATETIME),";
                Sql += "Creater='" + FilteSQLStr(model.Creater) + "',";
                Sql += "CreatedTime=CAST('" + model.CreatedTime.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss") + "' AS DATETIME),";
                Sql += "Modifier='" + FilteSQLStr(model.Modifier) + "',";
                Sql += "ModifiedTime=CAST('" + model.ModifiedTime.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss") + "' AS DATETIME)";
                if (!string.IsNullOrEmpty(SqlWhere))
                    Sql += " Where " + SqlWhere;
                #endregion
                result.Add(Sql);
            }
            return result;
        }

        /// <summary>
        /// 过滤不安全的字符串
        /// </summary>
        /// <param name="Str">要过滤的值</param>
        /// <returns>返回结果</returns>
        private string FilteSQLStr(object str)
        {
            if (str == null)
                return string.Empty;
            if (IsNumeric(str))
                return Convert.ToString(str);
            string Str = Convert.ToString(str);
            if (!string.IsNullOrEmpty(Str))
            {
                Str = Str.Replace("'", "");
                Str = Str.Replace("\"", "");
                Str = Str.Replace("&", "&amp");
                Str = Str.Replace("<", "&lt");
                Str = Str.Replace(">", "&gt");

                Str = Str.Replace("delete", "");
                Str = Str.Replace("update", "");
                Str = Str.Replace("insert", "");
            }
            return Str;
        }

        /// <summary>
        /// 判断object是否数字
        /// </summary>
        /// <param name="AObject">要判断的Object</param>
        /// <returns>是否数字</returns>       
        private bool IsNumeric(object AObject)
        {
            return AObject is sbyte || AObject is byte ||
                AObject is short || AObject is ushort ||
                AObject is int || AObject is uint ||
                AObject is long || AObject is ulong ||
                AObject is double || AObject is char ||
                AObject is decimal || AObject is float ||
                AObject is double;
        }

        /// <summary>
        /// 获得数据行
        /// </summary>
        /// <param name="dr">数据行</param>
        /// <param name="columnName">列名</param>
        /// <returns>返回值</returns>
        private object GetDataRow(DataRow dr, string columnName)
        {
            object result = null;
            if (dr.Table.Columns.Contains(columnName))
                result = Convert.IsDBNull(dr[columnName]) ? null : dr[columnName];
            else
                result = null;
            return result;
        }

        #region MapLocationVerison基础方法
        /// <summary>
        /// 返回MapLocationVerison字段列表
        /// </summary>
        /// <returns>字段列表</returns>
        private string FieldMapLocationVerison()
        {
            return @"
                    [Id],
                    [MapLocalVerison],
                    [Created],
                    [CreatedTime],
                    [landId]
                     "
                     .Trim()
                     .Replace("\t", "")
                     .Replace("\r", "")
                     .Replace("\n", "");
        }

        /// <summary>
        /// 读取数据行到model(MapLocationVerison)
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="dr">数据行</param>
        private MapLocationVerison ReadDataRow(DataRow dr, MapLocationVerison model)
        {
            model = new MapLocationVerison();
            //自增编号
            model.Id = GetDataRow(dr, "Id") == null ? 0 : Convert.ToInt32(GetDataRow(dr, "Id"));
            //地图边界编号
            model.MapLocalVerison = GetDataRow(dr, "MapLocalVerison") == null ? 0 : Convert.ToInt32(GetDataRow(dr, "MapLocalVerison"));
            //创建人
            model.Created = GetDataRow(dr, "Created") == null ? string.Empty : Convert.ToString(GetDataRow(dr, "Created")).Trim();
            //创建时间
            model.CreatedTime = GetDataRow(dr, "CreatedTime") == null ? (DateTime)SqlDateTime.MinValue : Convert.ToDateTime(GetDataRow(dr, "CreatedTime"));
            //地块儿编号
            model.landId = GetDataRow(dr, "landId") == null ? 0 : Convert.ToInt32(GetDataRow(dr, "landId"));

            return model;
        }

        ///<summary>
        ///检查是否空值(MapLocationVerison)
        ///</summary>
        private void CheckEmpty(ref List<MapLocationVerison> lists)
        {
            for (int i = 0; i < lists.Count; i++)
            {
                //自增编号
                lists[i].Id = lists[i].Id == null ? Convert.ToInt32(0) : Convert.ToInt32(lists[i].Id);
                //地图边界编号
                lists[i].MapLocalVerison = lists[i].MapLocalVerison == null ? Convert.ToInt32(0) : Convert.ToInt32(lists[i].MapLocalVerison);
                //创建人
                lists[i].Created = string.IsNullOrEmpty(lists[i].Created) ? string.Empty : Convert.ToString(lists[i].Created).Trim();
                //创建时间
                lists[i].CreatedTime = lists[i].CreatedTime == null ? (DateTime)SqlDateTime.MinValue : Convert.ToDateTime(lists[i].CreatedTime.GetValueOrDefault());
                //地块儿编号
                lists[i].landId = lists[i].landId == null ? Convert.ToInt32(0) : Convert.ToInt32(lists[i].landId);
            }
        }

        ///<summary>
        ///检查是否超过长度(MapLocationVerison)
        ///</summary>
        ///<param name="lists">数据集</param>
        ///<param name="message">错误消息</param>
        private void CheckMaxLength(ref List<MapLocationVerison> lists, out string message)
        {
            #region 声明变量

            //错误消息
            message = string.Empty;

            //超过的长度
            int OutLength = 0;
            #endregion

            #region 循环验证长度
            for (int i = 0; i < lists.Count; i++)
            {
                if (!string.IsNullOrEmpty(lists[i].Created))
                {
                    if (lists[i].Created.Length > 50)
                    {
                        OutLength = lists[i].Created.Length - 50;
                        message += "字段名[Created]描述[创建人]超长、字段最长[50]实际" + lists[i].Created.Length + "超过长度" + OutLength + ",";
                    }
                }
            }
            #endregion

            if (!string.IsNullOrEmpty(message)) message = message.Substring(0, message.Length - 1);
        }

        ///<summary>
        ///生成插入Sql语句(MapLocationVerison)
        ///</summary>
        ///<param name="lists">数据List</param>
        ///<returns>插入Sql语句字符串数组</returns>
        private List<string> MarkInsertSql(List<MapLocationVerison> lists)
        {
            List<string> result = new List<string>();
            foreach (MapLocationVerison model in lists)
            {
                #region 拼写Sql语句
                string Sql = "insert into MapLocationVerison(";
                Sql += "MapLocalVerison,";
                Sql += "Created,";
                Sql += "CreatedTime,";
                Sql += "landId";
                Sql += ") values(";
                Sql += "'" + FilteSQLStr(model.MapLocalVerison) + "',";
                Sql += "'" + FilteSQLStr(model.Created) + "',";
                Sql += "CAST('" + model.CreatedTime.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss") + "' AS DATETIME),";
                Sql += "'" + FilteSQLStr(model.landId) + "'";
                Sql += ")";
                #endregion
                result.Add(Sql);
            }
            return result;
        }

        ///<summary>
        ///生成更新Sql语句(MapLocationVerison)
        ///</summary>
        ///<param name="lists">数据List</param>
        ///<param name="SqlWhere">更新条件</param>
        ///<returns>更新Sql语句字符串数组</returns>
        private List<string> MarkUpdateSql(List<MapLocationVerison> lists, string SqlWhere)
        {
            List<string> result = new List<string>();
            foreach (MapLocationVerison model in lists)
            {
                #region 拼写Sql语句
                string Sql = "update MapLocationVerison set ";
                Sql += "MapLocalVerison='" + FilteSQLStr(model.MapLocalVerison) + "',";
                Sql += "Created='" + FilteSQLStr(model.Created) + "',";
                Sql += "CreatedTime=CAST('" + model.CreatedTime.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss") + "' AS DATETIME),";
                Sql += "landId='" + FilteSQLStr(model.landId) + "'";
                if (!string.IsNullOrEmpty(SqlWhere))
                    Sql += " Where " + SqlWhere;
                #endregion
                result.Add(Sql);
            }
            return result;
        }
        #endregion

        #endregion

        #region MassifGreenHouseVP 增删改查

        /// <summary>
        /// 添加地块儿和温室大棚数据
        /// </summary>
        /// <param name="addMassifGreenHouseVP">添加参数</param>
        /// <returns>返回值</returns>
        public Task<Message> addMassifGreenHouseVP(MassifGreenHouseVP addMassifGreenHouseVP)
        {
            //返回值
            Task<Message> result = null;

            //错误消息
            string message = string.Empty;

            //存储过程参数
            List<DbParameter> parameters = null;
            if (addMassifGreenHouseVP != null)
            {
                try
                {
                    parameters = GetDbParameters(addMassifGreenHouseVP);
                    result = MessageHelper.GetMessageForObjectAsync<Id32>(m_dbHelper, "AddMassifGreenHouseVP", parameters);
                }
                catch (Exception ex)
                {
                    result = new Task<Message>(() => new Message(false, ex.Message));
                }
            }
            else
            {
                result = new Task<Message>(() => new Message(false, "参数为空不能添加"));
            }
            return result;
        }

        #region 查询方法
        /// <summary>
        /// 查询单表方法
        /// </summary>
        /// <param name="SqlWhere">查询条件</param>
        /// <param name="message">错误消息(成功消息为空)</param>
        /// <returns>查询结果</returns>
        public List<MassifGreenHouseVP> QueryMassifGreenHouseVP(string SqlWhere, out string message)
        {
            message = string.Empty;
            List<MassifGreenHouseVP> result = new List<MassifGreenHouseVP>();
            DataTable dt = null;
            DataSet ds = null;
            DbParameter[] sqlparms = new DbParameter[] {
                 m_dbHelper.MakeInParam("SqlWhere",SqlWhere)
            };
            try
            {
                ds = m_dbHelper.ExecuteDataSet(CommandType.StoredProcedure, "GetMassifGreenHouseVP", sqlparms);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            #region 非空检查
            if (ds == null)
                return result;
            if (ds.Tables == null || ds.Tables.Count == 0)
                return result;
            dt = ds.Tables[0];
            if (dt == null)
                return result;
            if (dt.Rows.Count == 0)
                return result;
            #endregion
            foreach (DataRow dr in dt.Rows)
            {
                MassifGreenHouseVP model = new MassifGreenHouseVP();
                readMassifGreenHouseVPDataRow(ref model, dr);
                result.Add(model);
            }
            return result;
        }

        /// <summary>
        /// 分页查询方法
        /// </summary>
        /// <param name="SqlWhere">查询条件(可为空,为空则返回所有)</param>
        /// <param name="SortField">排序字段名(必填)</param>
        /// <param name="SortMethod">排序方法[ASC|DESC](必填)</param>
        /// <param name="PageSize">每页记录数(必填)</param>
        /// <param name="CurPage">当前页(必填)</param>
        /// <param name="TotalNumber">总记录数(返回参数)</param>
        /// <param name="message">错误消息(成功消息为空)</param>
        /// <returns>查询结果</returns>
        public List<MassifGreenHouseVP> QueryMassifGreenHouseVP(string SqlWhere, string SortField, string SortMethod, int PageSize, int CurPage, out int TotalNumber, out string message)
        {
            message = string.Empty;
            TotalNumber = 0;
            List<MassifGreenHouseVP> result = new List<MassifGreenHouseVP>();
            DataSet ds = null;
            DataTable dt = null;
            DbParameter[] sqlparm = new DbParameter[] {
                m_dbHelper.MakeInParam("StartRow",((CurPage - 1) * PageSize + 1)),
                m_dbHelper.MakeInParam("EndRow",(CurPage * PageSize)),
                m_dbHelper.MakeOutParam("TotalNumber",typeof(System.Int32)),
                m_dbHelper.MakeInParam("SortMethod",SortMethod),
                m_dbHelper.MakeInParam("SortField",SortField),
                m_dbHelper.MakeInParam("SqlWhere",SqlWhere)
            };
            try
            {
                ds = m_dbHelper.ExecuteDataSet(CommandType.StoredProcedure, "GetMassifGreenHouseVPPage", sqlparm);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            #region 非空检查
            if (ds == null)
                return result;
            if (ds.Tables == null || ds.Tables.Count == 0)
                return result;
            dt = ds.Tables[0];
            if (dt == null)
                return result;
            if (dt.Rows.Count == 0)
                return result;
            #endregion
            TotalNumber = Convert.ToInt32(sqlparm[2].Value);
            foreach (DataRow dr in dt.Rows)
            {
                MassifGreenHouseVP model = new MassifGreenHouseVP();
                readMassifGreenHouseVPDataRow(ref model, dr);
                result.Add(model);
            }
            return result;
        }
        #endregion


        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="lists">批量数据</param>
        /// <param name="SqlWhere">更新条件</param>
        /// <param name="message">错误消息</param>
        /// <returns>修改条数</returns>
        public Message UpdateMassifGreenHouseVP(List<MassifGreenHouseVP> lists, string SqlWhere)
        {
            int resultDBState = -1;
            Message result;
            string message = string.Empty;
            List<string> sqls = null;
            CheckEmpty(ref lists);
            CheckMaxLength(ref lists, out message);
            if (!string.IsNullOrEmpty(message))
            {
                result = new Message(false, message);
                return result;
            }
            try
            {
                sqls = this.MarkUpdateSql(lists, SqlWhere);
                resultDBState = m_dbHelper.ExecuteNonQuery(CommandType.Text, string.Join<string>(";", sqls));
            }
            catch (Exception ex)
            {
                result = new Message(false, ex.Message);
                return result;
            }

            result = new Message(true, "");
            return result;
        }

        /// <summary>
        /// 统计信息
        /// </summary>
        /// <param name="LandId">地块编号</param>
        /// <returns>数据表</returns>
        public DataTable StatisticsInfo(int LandId)
        {
            string sql = $" select sum(TotalOutput) TotalOutput,sum(TotaValue) TotaValue from vw_MassifGreenHouseVP ";
            string message = string.Empty;
            DataTable result = null;
            DataSet ds = null;
            if (LandId > 0)
                sql += $" where LandId='{LandId}' ";
            ds = ExecuteDataSet(sql, CommandType.Text, null, out message);
            if (ds == null || ds.Tables.Count <= 0)
                result = null;
            else
                result = ds.Tables[0];
            return result;
        }

        /// <summary>
        /// 删除方法
        /// </summary>
        /// <param name="SqlWhere">删除条件(必填不能省略)</param>
        /// <returns>错误消息</returns>
        public Message DeleteMassifGreenHouseVP(string SqlWhere)
        {
            int resultDBState = -1;
            Message result;
            string sql = string.Empty;
            if (string.IsNullOrEmpty(SqlWhere))
                return new Message(false, "没有删除条件不能删除");
            try
            {
                sql = $"delete from MassifGreenHouseVP where {SqlWhere}";
                resultDBState = m_dbHelper.ExecuteNonQuery(CommandType.Text, sql);
            }
            catch (Exception ex)
            {
                result = new Message(false, ex.Message);
                return result;
            }
            result = new Message(true, "");
            return result;
        }
        #endregion

        #region MapLocationVerison增删改查
        /// <summary>
        /// 添加地图边界版本号 
        /// </summary>
        /// <param name="lists">批量数据</param>
        /// <param name="message">错误消息</param>
        /// <returns>添加条数</returns>
        public Message InsertMapLocationVerison(List<MapLocationVerison> lists)
        {
            int DbState = -1;
            Message result = null;
            string message = string.Empty;
            if (lists == null)
                return new Message(false, "参数为空不能添加");
            CheckEmpty(ref lists);
            CheckMaxLength(ref lists, out message);
            if (!string.IsNullOrEmpty(message))
                return new Message(false, message);
            List<string> sqls = this.MarkInsertSql(lists);
            try
            {
                DbState = ExecuteNonQuery(string.Join(';', sqls.ToArray()));
            }
            catch (Exception ex)
            {
                result = new Message(false, ex.Message);
            }
            if (result == null)
            {
                result = new Message(true, string.Empty);
            }
            return result;
        }

        /// <summary>
        /// 修改地图边界版本号 
        /// </summary>
        /// <param name="lists">批量数据</param>
        /// <param name="SqlWhere">更新条件</param>
        /// <param name="message">错误消息</param>
        /// <returns>修改条数</returns>
        public Message UpdateMapLocationVerison(List<MapLocationVerison> lists, string SqlWhere)
        {
            Message result = null;
            int DbState = -1;
            string message = string.Empty;
            CheckEmpty(ref lists);
            List<string> sqls = this.MarkUpdateSql(lists, SqlWhere);
            try
            {
                DbState = ExecuteNonQuery(CommandType.Text, string.Join(';', sqls.ToArray()));
            }
            catch (Exception ex)
            {
                message = ex.Message;
                result = new Message(false, ex.Message);
            }
            if (result == null)
            {
                result = new Message(true, string.Empty);
            }
            return result;
        }

        /// <summary>
        /// 查询MapLocationVerison数据
        /// </summary>
        /// <param name="SqlWhere">查询条件</param>
        /// <param name="message">错误消息</param>
        /// <returns>返回值</returns>
        public List<MapLocationVerison> QueryMapLocationVerison(string SqlWhere, out string message)
        {
            List<MapLocationVerison> result = new List<MapLocationVerison>();
            string sql = $"select {FieldMapLocationVerison()} from [MapLocationVerison]";
            DataSet ds = new DataSet();
            DataTable dt = null;
            message = string.Empty;
            if (!string.IsNullOrEmpty(SqlWhere))
                sql += $" where {SqlWhere} ";
            try
            {
                ds = m_dbHelper.ExecuteDataSet(sql);
                #region 非空检查
                if (ds == null)
                    return result;
                if (ds.Tables == null || ds.Tables.Count == 0)
                    return result;
                dt = ds.Tables[0];
                if (dt == null)
                    return result;
                if (dt.Rows.Count == 0)
                    return result;
                #endregion
                foreach (DataRow dtRow in dt.Rows)
                    result.Add(ReadDataRow(dtRow, new MapLocationVerison()));
            }
            catch (Exception exp)
            {
                message = exp.Message;
            }
            return result;
        }

        /// <summary>
        /// 分页查询MapLocationVerison数据
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
        public List<MapLocationVerison> QueryPageMapLocationVerison(
            string SqlWhere,
            string SortField,
            string SortMethod,
            int PageSize,
            int CurPage,
            out int TotalNumber,
            out int PageCount,
            out string message
            )
        {
            List<MapLocationVerison> result = null;
            string FieldStr = FieldMapLocationVerison();
            Func<DataRow, MapLocationVerison, MapLocationVerison> func = (DataRow dr, MapLocationVerison model) => {
                return ReadDataRow(dr, model);
            };
            result = QueryPage<MapLocationVerison>(
                SqlWhere,
                SortField,
                SortMethod,
                FieldStr,
                "MapLocationVerison",
                PageSize,
                CurPage,
                func,
                out TotalNumber,
                out PageCount,
                out message
                );
            return result;
        }
        #endregion

        #region vw_MassifGreenHouseVP查询方法
        /// <summary>
        /// 查询单表方法
        /// </summary>
        /// <param name="SqlWhere">查询条件</param>
        /// <param name="message">错误消息</param>
        /// <returns>查询结果</returns>
        public List<vw_MassifGreenHouseVP> QueryViewMassifGreenHouseVP(string SqlWhere, out string message)
        {
            message = string.Empty;
            List<vw_MassifGreenHouseVP> result = new List<vw_MassifGreenHouseVP>();
            DbParameter[] sqlparms = new DbParameter[] {
                 Database.MakeInParam("SqlWhere",SqlWhere)
            };
            DataSet ds = ExecuteDataSet("Query_vw_MassifGreenHouseVP", CommandType.StoredProcedure, sqlparms, out message);
            #region 非空检查
            if (ds == null)
                return result;
            if (ds.Tables == null || ds.Tables.Count == 0)
                return result;
            DataTable dt = ds.Tables[0];
            if (dt == null)
                return result;
            if (dt.Rows.Count == 0)
                return result;
            #endregion
            foreach (DataRow dr in dt.Rows)
            {
                vw_MassifGreenHouseVP model = new vw_MassifGreenHouseVP();
                this.ReadDataRow(ref model, dr);
                result.Add(model);
            }
            return result;
        }

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
        public List<vw_MassifGreenHouseVP> QueryViewMassifGreenHouseVP_Page(string SqlWhere, string SortField, string SortMethod, int PageSize, int CurPage, out int PageCount, out string message)
        {
            message = string.Empty;
            PageCount = 0;
            List<vw_MassifGreenHouseVP> result = new List<vw_MassifGreenHouseVP>();
            DbParameter[] sqlparm = new DbParameter[] {
                Database.MakeInParam("StartRow",((CurPage - 1) * PageSize + 1)),
                Database.MakeInParam("EndRow",(CurPage * PageSize)),
                Database.MakeOutParam("TotalNumber",typeof(System.Int32)),
                Database.MakeInParam("SortMethod",SortMethod),
                Database.MakeInParam("SortField",SortField),
                Database.MakeInParam("SqlWhere",SqlWhere)
            };
            DataSet ds = ExecuteDataSet("Query_vw_MassifGreenHouseVP_Page", CommandType.StoredProcedure, sqlparm, out message);
            #region 非空检查
            if (ds == null)
                return result;
            if (ds.Tables == null || ds.Tables.Count == 0)
                return result;
            DataTable dt = ds.Tables[0];
            if (dt == null)
                return result;
            if (dt.Rows.Count == 0)
                return result;
            #endregion
            PageCount = Convert.ToInt32(sqlparm[2].Value);
            foreach (DataRow dr in dt.Rows)
            {
                vw_MassifGreenHouseVP model = new vw_MassifGreenHouseVP();
                this.ReadDataRow(ref model, dr);
                result.Add(model);
            }
            return result;
        }

        /// <summary>
        /// 按农作物分类统计种植面积
        /// </summary>
        /// <param name="landId">地块儿编号</param>
        /// <returns>统计结果</returns>
        public List<CropsPlandArea> StatisticsCropsName(int landId, out string message)
        {
            #region 声明变量

            //错误消息
            message = string.Empty;

            //sql语句
            string sql = $@"select 
                               CropsName,
                               cast(Round(sum(PlantingArea),2) as decimal(18,2)) PlantingAreaCount  
                            from vw_MassifGreenHouseVP  
                            where LandId='{landId}' 
                            group by CropsName ";

            //SQL返回值
            DataSet dataSet = null;

            //SQL返回数据表
            DataTable datatable = null;

            //返回值
            List<CropsPlandArea> result = new List<CropsPlandArea>();
            #endregion

            #region 读取数据库数据
            dataSet = ExecuteDataSet(sql, CommandType.Text, null, out message);
            #region 非空检查
            if (dataSet == null)
                return result;
            if (dataSet.Tables == null || dataSet.Tables.Count == 0)
                return result;
            datatable = dataSet.Tables[0];
            if (datatable == null)
                return result;
            if (datatable.Rows.Count == 0)
                return result;
            #endregion
            #endregion

            #region 赋值返回值
            foreach (DataRow dr in datatable.Rows)
            {
                CropsPlandArea model = new CropsPlandArea();
                model.CropsName = Utils.GetDataRow(dr, "CropsName") == null ? string.Empty : Convert.ToString(Utils.GetDataRow(dr, "CropsName"));
                model.PlantingAreaCount = Utils.GetDataRow(dr, "PlantingAreaCount") == null ? 0 : Utils.StrToDecimal(Convert.ToString(Utils.GetDataRow(dr, "PlantingAreaCount")));
                result.Add(model);
            }
            #endregion

            return result;
        }

        /// <summary>
        /// 按地块儿和农作物统计种植面积
        /// </summary>
        /// <param name="landId">地块儿编号</param>
        /// <param name="cropsName">农作物名称</param>
        /// <param name="message">错误消息</param>
        /// <returns>返回数据</returns>
        public PlantArea StatisticsPlantAreaByCropsLand(int landId, string cropsName, out string message)
        {
            #region 声明变量
            //错误消息
            message = string.Empty;

            //sql语句
            string sql = $@"select 
                                  LandId,
                                  LandName,
                                  cast(Round(sum(PlantingArea),2) as decimal(18,2)) PlantingAreaCount  
                            from vw_MassifGreenHouseVP  
                            where (parent_id='{landId}' Or LandId='{landId}') and CropsName='{cropsName}'
                            group by LandId,LandName ";

            //SQL返回值
            DataSet dataSet = null;

            //SQL返回数据表
            DataTable datatable = null;

            //地块种植面积统计
            List<RegionArea> RegionAreas = new List<RegionArea>();

            //种植面积总数
            decimal plantAreaTotal = 0;

            //返回值
            PlantArea result = new PlantArea();
            #endregion

            #region 读取数据库数据
            dataSet = ExecuteDataSet(sql, CommandType.Text, null, out message);
            #region 非空检查
            if (dataSet == null)
                return result;
            if (dataSet.Tables == null || dataSet.Tables.Count == 0)
                return result;
            datatable = dataSet.Tables[0];
            if (datatable == null)
                return result;
            if (datatable.Rows.Count == 0)
                return result;
            #endregion
            #endregion

            //求和总数
            plantAreaTotal = Math.Round(datatable.AsEnumerable().Sum(sum => sum.Field<decimal>("PlantingAreaCount")), 2);

            #region 赋值返回值
            foreach (DataRow dr in datatable.Rows)
            {
                RegionArea model = new RegionArea();
                model.Id = Utils.GetDataRow(dr, "LandId") == null ? 0 : Utils.StrToInt(Convert.ToString(Utils.GetDataRow(dr, "LandId")), 0);
                model.RegionName = Utils.GetDataRow(dr, "LandName") == null ? string.Empty : Convert.ToString(Utils.GetDataRow(dr, "LandName"));
                model.SowArea = Utils.GetDataRow(dr, "PlantingAreaCount") == null ? 0 : Utils.StrToDecimal(Convert.ToString(Utils.GetDataRow(dr, "PlantingAreaCount")));
                model.SowAreaPercentage = Math.Round((model.SowArea / plantAreaTotal) * 100, 2);
                RegionAreas.Add(model);
            }
            #endregion

            result = new PlantArea()
            {
                CountArea = plantAreaTotal,
                RegionAreas = RegionAreas
            };

            return result;
        }
        #endregion

        #region vw_osc_region 查询方法

        /// <summary>
        /// 查询单表方法
        /// </summary>
        /// <param name="SqlWhere">查询条件</param>
        /// <param name="message">错误消息</param>
        /// <returns>查询结果</returns>
        public List<vw_osc_region_group> QueryViewOscRegionGroup(string SqlWhere, out string message)
        {
            message = string.Empty;
            List<vw_osc_region_group> result = new List<vw_osc_region_group>();
            DbParameter[] sqlparms = new DbParameter[] {
                 Database.MakeInParam("SqlWhere",SqlWhere)
            };
            DataSet ds = ExecuteDataSet("Query_vw_osc_region_group", CommandType.StoredProcedure, sqlparms, out message);
            #region 非空检查
            if (ds == null)
                return result;
            if (ds.Tables == null || ds.Tables.Count == 0)
                return result;
            DataTable dt = ds.Tables[0];
            if (dt == null)
                return result;
            if (dt.Rows.Count == 0)
                return result;
            #endregion
            foreach (DataRow dr in dt.Rows)
            {
                vw_osc_region_group model = new vw_osc_region_group();
                this.ReadDataRow(ref model, dr);
                result.Add(model);
            }
            return result;
        }

        /// <summary>
        /// 查询单表方法
        /// </summary>
        /// <param name="SqlWhere">查询条件</param>
        /// <param name="message">错误消息</param>
        /// <returns>查询结果</returns>
        public List<vw_osc_region> QueryViewOscRegion(string SqlWhere, out string message)
        {
            message = string.Empty;
            List<vw_osc_region> result = new List<vw_osc_region>();
            DbParameter[] sqlparms = new DbParameter[] {
                 Database.MakeInParam("SqlWhere",SqlWhere)
            };
            DataSet ds = ExecuteDataSet("Query_vw_osc_region", CommandType.StoredProcedure, sqlparms, out message);
            #region 非空检查
            if (ds == null)
                return result;
            if (ds.Tables == null || ds.Tables.Count == 0)
                return result;
            DataTable dt = ds.Tables[0];
            if (dt == null)
                return result;
            if (dt.Rows.Count == 0)
                return result;
            #endregion
            foreach (DataRow dr in dt.Rows)
            {
                vw_osc_region model = new vw_osc_region();
                this.ReadDataRow(ref model, dr);
                result.Add(model);
            }
            return result;
        }

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
        public List<vw_osc_region> QueryViewOscRegionPage(string SqlWhere, string SortField, string SortMethod, int PageSize, int CurPage, out int PageCount, out string message)
        {
            message = string.Empty;
            PageCount = 0;
            List<vw_osc_region> result = new List<vw_osc_region>();
            DbParameter[] sqlparm = new DbParameter[] {
                Database.MakeInParam("StartRow",((CurPage - 1) * PageSize + 1)),
                Database.MakeInParam("EndRow",(CurPage * PageSize)),
                Database.MakeOutParam("TotalNumber",typeof(System.Int32)),
                Database.MakeInParam("SortMethod",SortMethod),
                Database.MakeInParam("SortField",SortField),
                Database.MakeInParam("SqlWhere",SqlWhere)
            };
            DataSet ds = ExecuteDataSet("Query_vw_osc_region_Page", CommandType.StoredProcedure, sqlparm, out message);
            #region 非空检查
            if (ds == null)
                return result;
            if (ds.Tables == null || ds.Tables.Count == 0)
                return result;
            DataTable dt = ds.Tables[0];
            if (dt == null)
                return result;
            if (dt.Rows.Count == 0)
                return result;
            #endregion
            PageCount = Convert.ToInt32(sqlparm[2].Value);
            foreach (DataRow dr in dt.Rows)
            {
                vw_osc_region model = new vw_osc_region();
                this.ReadDataRow(ref model, dr);
                result.Add(model);
            }
            return result;
        }
        #endregion

        #region osc_region 查询方法

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
        public List<osc_region> QueryOscRegionPage(string SqlWhere, string SortField, string SortMethod, int PageSize, int CurPage, out int PageCount, out string message)
        {
            message = string.Empty;
            PageCount = 0;
            List<osc_region> result = new List<osc_region>();
            DbParameter[] sqlparm = new DbParameter[] {
                Database.MakeInParam("StartRow",((CurPage - 1) * PageSize + 1)),
                Database.MakeInParam("EndRow",(CurPage * PageSize)),
                Database.MakeOutParam("TotalNumber",typeof(System.Int32)),
                Database.MakeInParam("SortMethod",SortMethod),
                Database.MakeInParam("SortField",SortField),
                Database.MakeInParam("SqlWhere",SqlWhere)
            };
            DataSet ds = ExecuteDataSet("Query_osc_region_Page", CommandType.StoredProcedure, sqlparm, out message);
            #region 非空检查
            if (ds == null)
                return result;
            if (ds.Tables == null || ds.Tables.Count == 0)
                return result;
            DataTable dt = ds.Tables[0];
            if (dt == null)
                return result;
            if (dt.Rows.Count == 0)
                return result;
            #endregion
            PageCount = Convert.ToInt32(sqlparm[2].Value);
            foreach (DataRow dr in dt.Rows)
            {
                osc_region model = new osc_region();
                this.ReadDataRow(ref model, dr);
                result.Add(model);
            }
            return result;
        }

        /// <summary>
        /// 查询区域信息
        /// </summary>
        /// <param name="SqlWhere">查询条件</param>
        /// <param name="message">错误信息</param>
        /// <returns>返回值</returns>
        public List<osc_region> QueryOscRegion(string SqlWhere, out string message)
        {
            message = string.Empty;
            List<osc_region> result = new List<osc_region>();
            DbParameter[] sqlparms = new DbParameter[] {
                 Database.MakeInParam("SqlWhere",SqlWhere)
            };
            DataSet ds = ExecuteDataSet("Query_osc_region", CommandType.StoredProcedure, sqlparms, out message);
            #region 非空检查
            if (ds == null)
                return result;
            if (ds.Tables == null || ds.Tables.Count == 0)
                return result;
            DataTable dt = ds.Tables[0];
            if (dt == null)
                return result;
            if (dt.Rows.Count == 0)
                return result;
            #endregion
            foreach (DataRow dr in dt.Rows)
            {
                osc_region model = new osc_region();
                this.ReadDataRow(ref model, dr);
                result.Add(model);
            }
            return result;
        }
        #endregion

        #region osc_region 增加方法

        /// <summary>
        /// 添加 
        /// </summary>
        /// <param name="lists">批量数据</param>
        /// <param name="message">错误消息</param>
        /// <returns>添加条数</returns>
        public Message Insertosc_region(List<osc_region> lists,out int identity)
        {
            int DbState = -1;
            DataSet ds = null;
            identity = 0;
            Message result = null;
            string message = string.Empty;
            if (lists == null)
                return new Message(false, "参数为空不能添加");
            CheckEmpty(ref lists);
            CheckMaxLength(ref lists, out message);
            if (!string.IsNullOrEmpty(message))
                return new Message(false, message);
            List<string> sqls = this.MarkInsertSql(lists);
            try
            {
                ds = ExecuteDataSet(string.Join(';', sqls.ToArray()));
                identity = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            }
            catch (Exception ex)
            {
                result = new Message(false, ex.Message);
            }
            if (result == null)
            {
                result = new Message(true, string.Empty);
            }
            return result;
        }
        #endregion

        #region osc_region 修改方法

        /// <summary>
        /// 修改 
        /// </summary>
        /// <param name="lists">批量数据</param>
        /// <param name="SqlWhere">更新条件</param>
        /// <param name="message">错误消息</param>
        /// <returns>修改条数</returns>
        public Message Updateosc_region(List<osc_region> lists, string SqlWhere)
        {
            Message result = null;
            int DbState = -1;
            string message = string.Empty;
            CheckEmpty(ref lists);
            List<string> sqls = this.MarkUpdateSql(lists, SqlWhere);
            try
            {
                DbState = ExecuteNonQuery(CommandType.Text, string.Join(';', sqls.ToArray()));
            }
            catch (Exception ex)
            {
                message = ex.Message;
                result = new Message(false, ex.Message);
            }
            if (result == null)
            {
                result = new Message(true, string.Empty);
            }
            return result;
        }

        /// <summary>
        /// 设置根地块儿
        /// </summary>
        /// <param name="landId">地块儿编号</param>
        /// <returns>错误消息</returns>
        public Message SetRootLand(string landId) 
        {
            #region 声明和初始化

            //错误消息
            string message = string.Empty;

            //返回值
            Message result = null;

            //SQL语句
            string sql = string.Empty;

            //数据库状态
            int DbState = -1;
            #endregion

            sql = $" update osc_region set IsRoot='0';update osc_region set IsRoot='1' where id='{landId}'   ";

            #region 执行SQL语句
            try
            {
                DbState = ExecuteNonQuery(CommandType.Text, sql);
            }
            catch (Exception ex)
            {
                message = ex.Message;
                result = new Message(false, ex.Message);
            }
            #endregion

            if (result == null)
                result = new Message(true, string.Empty);

            return result;
        }
        #endregion

        #region osc_region 删除方法

        /// <summary>
        /// 删除地块儿
        /// </summary>
        /// <param name="IDStr">ID字符串</param>
        /// <returns>返回消息</returns>
        public async Task<Message> DeleteOscRegion(string IDStr) 
        {
            #region 声明和初始化

            //错误消息
            string message = string.Empty;

            //返回值
            Message result = null;

            //SQL语句
            string sql = string.Empty;

            //数据库状态
            int DbState = -1;
            #endregion

            sql = $" delete from osc_region where id in ({IDStr}); ";
            sql += $" delete from region_gis where landId in ({IDStr}) ";

            #region 执行SQL语句
            try
            {
                DbState = ExecuteNonQuery(CommandType.Text, sql);
            }
            catch (Exception ex)
            {
                message = ex.Message;
                result = new Message(false, ex.Message);
            }
            #endregion

            if (result == null)
                result = new Message(true, string.Empty);

            return result;

        }
        #endregion

        #region osc_region基础方法
        /// <summary>
        /// 返回osc_region字段列表
        /// </summary>
        /// <returns>字段列表</returns>
        private string Fieldosc_region()
        {
            return @"
                    [id],
                    [name],
                    [parent_id],
                    [citycode],
                    [adcode],
                    [center],
                    [level],
                    [created],
                    [CreatedTime],
                    [Modifier],
                    [ModifiedTime],
                    [IsRoot]
                     "
                     .Trim()
                     .Replace("\t", "")
                     .Replace("\r", "")
                     .Replace("\n", "");
        }

        /// <summary>
        /// 读取数据行到model(osc_region)
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="dr">数据行</param>
        private osc_region ReadDataRow(DataRow dr, osc_region model)
        {
            model = new osc_region();
            //编号数据
            model.id = GetDataRow(dr, "id") == null ? 0 : Convert.ToInt32(GetDataRow(dr, "id"));
            //地块儿名称
            model.name = GetDataRow(dr, "name") == null ? string.Empty : Convert.ToString("name").Trim();
            //上级地块儿编号
            model.parent_id = GetDataRow(dr, "parent_id") == null ? 0 : Convert.ToInt32(GetDataRow(dr, "parent_id"));
            //城市编号
            model.citycode = GetDataRow(dr, "citycode") == null ? string.Empty : Convert.ToString("citycode").Trim();
            //区域编码
            model.adcode = GetDataRow(dr, "adcode") == null ? string.Empty : Convert.ToString("adcode").Trim();
            //中心坐标
            model.center = GetDataRow(dr, "center") == null ? string.Empty : Convert.ToString("center").Trim();
            //级别
            model.level = GetDataRow(dr, "level") == null ? string.Empty : Convert.ToString("level").Trim();
            //创建人
            model.created = GetDataRow(dr, "created") == null ? string.Empty : Convert.ToString("created").Trim();
            //创建时间
            model.CreatedTime = GetDataRow(dr, "CreatedTime") == null ? (DateTime)SqlDateTime.MinValue : Convert.ToDateTime("CreatedTime");
            //修改人
            model.Modifier = GetDataRow(dr, "Modifier") == null ? string.Empty : Convert.ToString("Modifier").Trim();
            //修改时间
            model.ModifiedTime = GetDataRow(dr, "ModifiedTime") == null ? (DateTime)SqlDateTime.MinValue : Convert.ToDateTime("ModifiedTime");
            //是否根地块儿_0否_1是
            model.IsRoot = GetDataRow(dr, "IsRoot") == null ? 0 : Convert.ToInt32(GetDataRow(dr, "IsRoot"));

            return model;
        }

        ///<summary>
        ///检查是否空值(osc_region)
        ///</summary>
        private void CheckEmpty(ref List<osc_region> lists)
        {
            for (int i = 0; i < lists.Count; i++)
            {
                //编号数据
                lists[i].id = lists[i].id == null ? Convert.ToInt32(0) : Convert.ToInt32(lists[i].id);
                //地块儿名称
                lists[i].name = string.IsNullOrEmpty(lists[i].name) ? string.Empty : Convert.ToString(lists[i].name).Trim();
                //上级地块儿编号
                lists[i].parent_id = lists[i].parent_id == null ? Convert.ToInt32(0) : Convert.ToInt32(lists[i].parent_id);
                //城市编号
                lists[i].citycode = string.IsNullOrEmpty(lists[i].citycode) ? string.Empty : Convert.ToString(lists[i].citycode).Trim();
                //区域编码
                lists[i].adcode = string.IsNullOrEmpty(lists[i].adcode) ? string.Empty : Convert.ToString(lists[i].adcode).Trim();
                //中心坐标
                lists[i].center = string.IsNullOrEmpty(lists[i].center) ? string.Empty : Convert.ToString(lists[i].center).Trim();
                //级别
                lists[i].level = string.IsNullOrEmpty(lists[i].level) ? string.Empty : Convert.ToString(lists[i].level).Trim();
                //创建人
                lists[i].created = string.IsNullOrEmpty(lists[i].created) ? string.Empty : Convert.ToString(lists[i].created).Trim();
                //创建时间
                lists[i].CreatedTime = lists[i].CreatedTime == null ? (DateTime)SqlDateTime.MinValue : Convert.ToDateTime(lists[i].CreatedTime.GetValueOrDefault());
                //修改人
                lists[i].Modifier = string.IsNullOrEmpty(lists[i].Modifier) ? string.Empty : Convert.ToString(lists[i].Modifier).Trim();
                //修改时间
                lists[i].ModifiedTime = lists[i].ModifiedTime == null ? (DateTime)SqlDateTime.MinValue : Convert.ToDateTime(lists[i].ModifiedTime.GetValueOrDefault());
                //是否根地块儿_0否_1是
                lists[i].IsRoot = lists[i].IsRoot == null ? Convert.ToInt32(0) : Convert.ToInt32(lists[i].IsRoot);
            }
        }

        ///<summary>
        ///检查是否超过长度(osc_region)
        ///</summary>
        ///<param name="lists">数据集</param>
        ///<param name="message">错误消息</param>
        private void CheckMaxLength(ref List<osc_region> lists, out string message)
        {
            #region 声明变量

            //错误消息
            message = string.Empty;

            //超过的长度
            int OutLength = 0;
            #endregion

            #region 循环验证长度
            for (int i = 0; i < lists.Count; i++)
            {
                if (!string.IsNullOrEmpty(lists[i].name))
                {
                    if (lists[i].name.Length > 150)
                    {
                        OutLength = lists[i].name.Length - 150;
                        message += "字段名[name]描述[地块儿名称]超长、字段最长[150]实际" + lists[i].name.Length + "超过长度" + OutLength + ",";
                    }
                }
                if (!string.IsNullOrEmpty(lists[i].citycode))
                {
                    if (lists[i].citycode.Length > 50)
                    {
                        OutLength = lists[i].citycode.Length - 50;
                        message += "字段名[citycode]描述[城市编号]超长、字段最长[50]实际" + lists[i].citycode.Length + "超过长度" + OutLength + ",";
                    }
                }
                if (!string.IsNullOrEmpty(lists[i].adcode))
                {
                    if (lists[i].adcode.Length > 50)
                    {
                        OutLength = lists[i].adcode.Length - 50;
                        message += "字段名[adcode]描述[区域编码]超长、字段最长[50]实际" + lists[i].adcode.Length + "超过长度" + OutLength + ",";
                    }
                }
                if (!string.IsNullOrEmpty(lists[i].center))
                {
                    if (lists[i].center.Length > 50)
                    {
                        OutLength = lists[i].center.Length - 50;
                        message += "字段名[center]描述[中心坐标]超长、字段最长[50]实际" + lists[i].center.Length + "超过长度" + OutLength + ",";
                    }
                }
                if (!string.IsNullOrEmpty(lists[i].level))
                {
                    if (lists[i].level.Length > 50)
                    {
                        OutLength = lists[i].level.Length - 50;
                        message += "字段名[level]描述[级别]超长、字段最长[50]实际" + lists[i].level.Length + "超过长度" + OutLength + ",";
                    }
                }
                if (!string.IsNullOrEmpty(lists[i].created))
                {
                    if (lists[i].created.Length > 50)
                    {
                        OutLength = lists[i].created.Length - 50;
                        message += "字段名[created]描述[创建人]超长、字段最长[50]实际" + lists[i].created.Length + "超过长度" + OutLength + ",";
                    }
                }
                if (!string.IsNullOrEmpty(lists[i].Modifier))
                {
                    if (lists[i].Modifier.Length > 50)
                    {
                        OutLength = lists[i].Modifier.Length - 50;
                        message += "字段名[Modifier]描述[修改人]超长、字段最长[50]实际" + lists[i].Modifier.Length + "超过长度" + OutLength + ",";
                    }
                }
            }
            #endregion

            if (!string.IsNullOrEmpty(message)) message = message.Substring(0, message.Length - 1);
        }

        ///<summary>
        ///生成插入Sql语句(osc_region)
        ///</summary>
        ///<param name="lists">数据List</param>
        ///<returns>插入Sql语句字符串数组</returns>
        private List<string> MarkInsertSql(List<osc_region> lists)
        {
            List<string> result = new List<string>();
            foreach (osc_region model in lists)
            {
                #region 拼写Sql语句
                string Sql = "insert into osc_region(";
                Sql += "name,";
                Sql += "parent_id,";
                Sql += "citycode,";
                Sql += "adcode,";
                Sql += "center,";
                Sql += "level,";
                Sql += "created,";
                Sql += "CreatedTime,";
                Sql += "Modifier,";
                Sql += "ModifiedTime,";
                Sql += "IsRoot";
                Sql += ") values(";
                Sql += "'" + FilteSQLStr(model.name) + "',";
                Sql += "'" + FilteSQLStr(model.parent_id) + "',";
                Sql += "'" + FilteSQLStr(model.citycode) + "',";
                Sql += "'" + FilteSQLStr(model.adcode) + "',";
                Sql += "'" + FilteSQLStr(model.center) + "',";
                Sql += "'" + FilteSQLStr(model.level) + "',";
                Sql += "'" + FilteSQLStr(model.created) + "',";
                Sql += "CAST('" + model.CreatedTime.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss") + "' AS DATETIME),";
                Sql += "'" + FilteSQLStr(model.Modifier) + "',";
                Sql += "CAST('" + model.ModifiedTime.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss") + "' AS DATETIME),";
                Sql += "'" + FilteSQLStr(model.IsRoot) + "'";
                Sql += ");select @@identity;";
                #endregion
                result.Add(Sql);
            }
            return result;
        }

        ///<summary>
        ///生成更新Sql语句(osc_region)
        ///</summary>
        ///<param name="lists">数据List</param>
        ///<param name="SqlWhere">更新条件</param>
        ///<returns>更新Sql语句字符串数组</returns>
        private List<string> MarkUpdateSql(List<osc_region> lists, string SqlWhere)
        {
            List<string> result = new List<string>();
            foreach (osc_region model in lists)
            {
                #region 拼写Sql语句
                string Sql = "update osc_region set ";
                Sql += "name='" + FilteSQLStr(model.name) + "',";
                Sql += "parent_id='" + FilteSQLStr(model.parent_id) + "',";
                Sql += "citycode='" + FilteSQLStr(model.citycode) + "',";
                Sql += "adcode='" + FilteSQLStr(model.adcode) + "',";
                Sql += "center='" + FilteSQLStr(model.center) + "',";
                Sql += "level='" + FilteSQLStr(model.level) + "',";
                Sql += "created='" + FilteSQLStr(model.created) + "',";
                Sql += "CreatedTime=CAST('" + model.CreatedTime.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss") + "' AS DATETIME),";
                Sql += "Modifier='" + FilteSQLStr(model.Modifier) + "',";
                Sql += "ModifiedTime=CAST('" + model.ModifiedTime.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss") + "' AS DATETIME),";
                Sql += "IsRoot='" + FilteSQLStr(model.IsRoot) + "'";
                if (!string.IsNullOrEmpty(SqlWhere))
                    Sql += " Where " + SqlWhere;
                #endregion
                result.Add(Sql);
            }
            return result;
        }
        #endregion

        #region CompanyInfo 增删改

        #region 增加数据
        /// <summary>
        /// 单条增加
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="message">消息</param>
        /// <returns>添加条数</returns>
        public async Task<Message> InsertCompanyInfo(CompanyInfo model)
        {
            #region 声明和初始化

            //错误消息
            string message = string.Empty;

            //返回值
            Message result = null;

            //存储过程参数
            List<DbParameter> parameters = null;
            #endregion

            if (model != null)
            {
                try
                {
                    parameters = SetSqlParameter(model).ToList();
                    result = await MessageHelper.GetMessageAsync(m_dbHelper, "Create_CompanyInfo", parameters);
                }
                catch (Exception ex)
                {
                    result = new Message(false, ex.Message);
                }
            }
            else
            {
                result = new Message(false, "参数为空不能添加");
            }
            if (result == null)
                result = new Message(true, string.Empty);
            return result;
        }

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="lists">批量数据</param>
        /// <param name="message">错误消息</param>
        /// <returns>添加条数</returns>
        public Task<Message> InsertCompanyInfo(List<CompanyInfo> lists)
        {
            int DbState = -1;
            Task<Message> result = null;
            if (lists == null)
                return new Task<Message>(() => new Message(false, "参数为空不能添加"));
            CheckEmpty(ref lists);
            List<string> sqls = this.MarkInsertSql(lists);
            try
            {
                DbState = ExecuteNonQuery(string.Join(';', sqls.ToArray()));
            }
            catch (Exception ex)
            {
                result = new Task<Message>(() => new Message(false, ex.Message));
            }
            if (result == null)
            {
                result = new Task<Message>(() => new Message(true, string.Empty));
            }
            return result;
        }
        #endregion

        #region 修改方法
        /// <summary>
        /// 单条修改
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="SqlWhere">更新条件</param>
        /// <param name="message">消息</param>
        /// <returns>修改条数</returns>
        public Task<Message> UpdateCompanyInfo(CompanyInfo model, string SqlWhere, out string message)
        {
            message = string.Empty;
            List<DbParameter> sqlparms = this.SetSqlParameter(model).ToList<DbParameter>();
            sqlparms.Add(Database.MakeInParam("SqlWhere", SqlWhere));
            Task<Message> result = null;
            int DbState = -1;
            try
            {
                DbState = ExecuteNonQuery(CommandType.StoredProcedure, "Update_CompanyInfo", sqlparms.ToArray());
            }
            catch (Exception ex)
            {
                message = ex.Message;
                result = new Task<Message>(() => new Message(false, ex.Message));
            }
            if (result == null)
            {
                result = new Task<Message>(() => new Message(true, string.Empty));
            }
            return result;
        }

        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="lists">批量数据</param>
        /// <param name="SqlWhere">更新条件</param>
        /// <param name="message">错误消息</param>
        /// <returns>修改条数</returns>
        public async Task<Message> UpdateCompanyInfo(List<CompanyInfo> lists, string SqlWhere)
        {
            Message result = null;
            int DbState = -1;
            string message = string.Empty;
            CheckEmpty(ref lists);
            List<string> sqls = this.MarkUpdateSql(lists, SqlWhere);
            try
            {
                DbState = ExecuteNonQuery(CommandType.Text, string.Join(';', sqls.ToArray()));
            }
            catch (Exception ex)
            {
                message = ex.Message;
                result = new Message(false, ex.Message);
            }
            if (result == null)
            {
                result = new Message(true, string.Empty);
            }
            return result;
        }
        #endregion

        #region 删除方法

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="IDStr">编号字符串</param>
        /// <returns>返回消息</returns>
        public async Task<Message> DeleteCompanyInfo(string IDStr)
        {
            #region 声明和初始化

            //错误消息
            string message = string.Empty;

            //返回值
            Message result = null;

            //SQL语句
            string sql = string.Empty;

            //数据库状态
            int DbState = -1;
            #endregion

            //初始化SQL语句
            sql = $" delete from CompanyInfo where RecordId in ({IDStr}) ";

            #region 执行SQL语句
            try
            {
                DbState = ExecuteNonQuery(CommandType.Text, sql);
            }
            catch (Exception ex)
            {
                message = ex.Message;
                result = new Message(false, ex.Message);
            }
            #endregion

            if (result == null)
                result = new Message(true, string.Empty);

            return result;
        }
        #endregion

        #region 查询方法
        /// <summary>
        /// 查询单表方法
        /// </summary>
        /// <param name="SqlWhere">查询条件</param>
        /// <param name="message">错误消息</param>
        /// <returns>查询结果</returns>
        public List<CompanyInfo> QueryCompanyInfo(string SqlWhere, out string message)
        {
            message = string.Empty;
            List<CompanyInfo> result = new List<CompanyInfo>();
            DataTable dt = null;
            DataSet ds = null;
            DbParameter[] sqlparms = new DbParameter[] {
                 m_dbHelper.MakeInParam("SqlWhere",SqlWhere)
            };
            if (string.IsNullOrEmpty(SqlWhere))
            {
                sqlparms = new DbParameter[] {
                    m_dbHelper.MakeInParam("SqlWhere",null)
                };
            }
            try
            {
                ds = m_dbHelper.ExecuteDataSet(CommandType.StoredProcedure, "Query_CompanyInfo", sqlparms);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            #region 非空检查
            if (ds == null)
                return result;
            if (ds.Tables == null || ds.Tables.Count == 0)
                return result;
            dt = ds.Tables[0];
            if (dt == null)
                return result;
            if (dt.Rows.Count == 0)
                return result;
            #endregion
            foreach (DataRow dr in dt.Rows)
            {
                CompanyInfo model = new CompanyInfo();
                this.ReadDataRow(ref model, dr);
                result.Add(model);
            }
            return result;
        }

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
        public List<CompanyInfo> QueryCompanyInfo(string SqlWhere, string SortField, string SortMethod, int PageSize, int CurPage, out int PageCount, out string message)
        {
            message = string.Empty;
            PageCount = 0;
            DataTable dt = null;
            DataSet ds = null;
            List<CompanyInfo> result = new List<CompanyInfo>();
            if (string.IsNullOrEmpty(SqlWhere))
                SqlWhere = null;
            DbParameter[] sqlparm = new DbParameter[] {
                m_dbHelper.MakeInParam("StartRow",((CurPage - 1) * PageSize + 1)),
                m_dbHelper.MakeInParam("EndRow",(CurPage * PageSize)),
                m_dbHelper.MakeOutParam("TotalNumber",typeof(System.Int32)),
                m_dbHelper.MakeInParam("SortMethod",SortMethod),
                m_dbHelper.MakeInParam("SortField",SortField),
                m_dbHelper.MakeInParam("SqlWhere",SqlWhere)
            };
            try
            {
                ds = m_dbHelper.ExecuteDataSet(CommandType.StoredProcedure, "Query_CompanyInfo_Page", sqlparm);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            #region 非空检查
            if (ds == null)
                return result;
            if (ds.Tables == null || ds.Tables.Count == 0)
                return result;
            dt = ds.Tables[0];
            if (dt == null)
                return result;
            if (dt.Rows.Count == 0)
                return result;
            #endregion
            PageCount = Convert.ToInt32(sqlparm[2].Value);
            foreach (DataRow dr in dt.Rows)
            {
                CompanyInfo model = new CompanyInfo();
                this.ReadDataRow(ref model, dr);
                result.Add(model);
            }
            return result;
        }
        #endregion

        #endregion

        #region CompanyInfo 基础方法

        /// <summary>
        /// 读取数据行到model
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="dr">数据行</param>
        private void ReadDataRow(ref CompanyInfo model, DataRow dr)
        {
            //雪花ID
            model.RecordId = Convert.IsDBNull(dr["RecordId"]) ? 0 : Convert.ToInt64(dr["RecordId"]);
            //公司名称
            model.CompanyName = Convert.IsDBNull(dr["CompanyName"]) ? string.Empty : Convert.ToString(dr["CompanyName"]).Trim();
            //企业类型
            model.EnterpriseType = Convert.IsDBNull(dr["EnterpriseType"]) ? string.Empty : Convert.ToString(dr["EnterpriseType"]).Trim();
            //注册资本
            model.RegisteredCapital = Convert.IsDBNull(dr["RegisteredCapital"]) ? 0 : Convert.ToDecimal(dr["RegisteredCapital"]);
            //成立时间
            model.FoundedTime = Convert.IsDBNull(dr["FoundedTime"]) ? (DateTime)SqlDateTime.MinValue : Convert.ToDateTime(dr["FoundedTime"]);
            //所属乡镇
            model.TownShip = Convert.IsDBNull(dr["TownShip"]) ? string.Empty : Convert.ToString(dr["TownShip"]).Trim();
            //所属行业
            model.Industry = Convert.IsDBNull(dr["Industry"]) ? string.Empty : Convert.ToString(dr["Industry"]).Trim();
            //区域级别
            model.RegionalLevel = Convert.IsDBNull(dr["RegionalLevel"]) ? string.Empty : Convert.ToString(dr["RegionalLevel"]).Trim();
            //企业性质
            model.EnterpriseNature = Convert.IsDBNull(dr["EnterpriseNature"]) ? string.Empty : Convert.ToString(dr["EnterpriseNature"]).Trim();
            //员工人数
            model.EmployeesNum = Convert.IsDBNull(dr["EmployeesNum"]) ? 0 : Convert.ToInt32(dr["EmployeesNum"]);
            //联系人
            model.Contacts = Convert.IsDBNull(dr["Contacts"]) ? string.Empty : Convert.ToString(dr["Contacts"]).Trim();
            //联系电话
            model.ContactPhone = Convert.IsDBNull(dr["ContactPhone"]) ? string.Empty : Convert.ToString(dr["ContactPhone"]).Trim();
            //企业地址
            model.EnterpriseAddress = Convert.IsDBNull(dr["EnterpriseAddress"]) ? string.Empty : Convert.ToString(dr["EnterpriseAddress"]).Trim();
            //企业简介
            model.EnterpriseIntroduction = Convert.IsDBNull(dr["EnterpriseIntroduction"]) ? string.Empty : Convert.ToString(dr["EnterpriseIntroduction"]).Trim();
            //焦点图地址Json
            model.FocusImages = Convert.IsDBNull(dr["FocusImages"]) ? string.Empty : Convert.ToString(dr["FocusImages"]).Trim();
            //本年度销售额度
            model.CurrentYearSales = Convert.IsDBNull(dr["CurrentYearSales"]) ? 0 : Convert.ToDecimal(dr["CurrentYearSales"]);
            //公司类别[A类、B类、C类根据销售本年度销售额度判断A类销售额度大于800万、B类销售额度在500到800万之间、C类在200到500万之间]
            model.CompanyType = Convert.IsDBNull(dr["CompanyType"]) ? string.Empty : Convert.ToString(dr["CompanyType"]).Trim();
            //地图经度
            model.Lng = Convert.IsDBNull(dr["Lng"]) ? 0 : Convert.ToDouble(dr["Lng"]);
            //地图纬度
            model.Lat = Convert.IsDBNull(dr["Lat"]) ? 0 : Convert.ToDouble(dr["Lat"]);
            //视频地址
            model.videoUrl = Convert.IsDBNull(dr["videoUrl"]) ? string.Empty : Convert.ToString(dr["videoUrl"]).Trim();
            //备用字段01
            model.Backup01 = Convert.IsDBNull(dr["Backup01"]) ? string.Empty : Convert.ToString(dr["Backup01"]).Trim();
            //备用字段02
            model.Backup02 = Convert.IsDBNull(dr["Backup02"]) ? string.Empty : Convert.ToString(dr["Backup02"]).Trim();
            //备用字段03
            model.Backup03 = Convert.IsDBNull(dr["Backup03"]) ? string.Empty : Convert.ToString(dr["Backup03"]).Trim();
            //备用字段04
            model.Backup04 = Convert.IsDBNull(dr["Backup04"]) ? string.Empty : Convert.ToString(dr["Backup04"]).Trim();
            //备用字段05
            model.Backup05 = Convert.IsDBNull(dr["Backup05"]) ? string.Empty : Convert.ToString(dr["Backup05"]).Trim();
            //备用字段06
            model.Backup06 = Convert.IsDBNull(dr["Backup06"]) ? string.Empty : Convert.ToString(dr["Backup06"]).Trim();
            //备用字段07
            model.Backup07 = Convert.IsDBNull(dr["Backup07"]) ? string.Empty : Convert.ToString(dr["Backup07"]).Trim();
            //备用字段08
            model.Backup08 = Convert.IsDBNull(dr["Backup08"]) ? string.Empty : Convert.ToString(dr["Backup08"]).Trim();
            //备用字段09
            model.Backup09 = Convert.IsDBNull(dr["Backup09"]) ? string.Empty : Convert.ToString(dr["Backup09"]).Trim();
            //添加人
            model.Created = Convert.IsDBNull(dr["Created"]) ? string.Empty : Convert.ToString(dr["Created"]).Trim();
            //添加时间
            model.CreatedTime = Convert.IsDBNull(dr["CreatedTime"]) ? (DateTime)SqlDateTime.MinValue : Convert.ToDateTime(dr["CreatedTime"]);
            //修改人
            model.Modifier = Convert.IsDBNull(dr["Modifier"]) ? string.Empty : Convert.ToString(dr["Modifier"]).Trim();
            //修改时间
            model.ModifiedTime = Convert.IsDBNull(dr["ModifiedTime"]) ? (DateTime)SqlDateTime.MinValue : Convert.ToDateTime(dr["ModifiedTime"]);
        }

        ///<summary>
        ///检查是否空值
        ///</summary>
        private void CheckEmpty(ref List<CompanyInfo> lists)
        {
            for (int i = 0; i < lists.Count; i++)
            {
                //雪花ID
                lists[i].RecordId = lists[i].RecordId == null ? Convert.ToInt64(0) : Convert.ToInt64(lists[i].RecordId);
                //公司名称
                lists[i].CompanyName = string.IsNullOrEmpty(lists[i].CompanyName) ? string.Empty : Convert.ToString(lists[i].CompanyName).Trim();
                //企业类型
                lists[i].EnterpriseType = string.IsNullOrEmpty(lists[i].EnterpriseType) ? string.Empty : Convert.ToString(lists[i].EnterpriseType).Trim();
                //注册资本
                lists[i].RegisteredCapital = lists[i].RegisteredCapital == null ? Convert.ToDecimal(0) : Convert.ToDecimal(lists[i].RegisteredCapital);
                //成立时间
                lists[i].FoundedTime = lists[i].FoundedTime == null ? (DateTime)SqlDateTime.MinValue : Convert.ToDateTime(lists[i].FoundedTime.GetValueOrDefault());
                //所属乡镇
                lists[i].TownShip = string.IsNullOrEmpty(lists[i].TownShip) ? string.Empty : Convert.ToString(lists[i].TownShip).Trim();
                //所属行业
                lists[i].Industry = string.IsNullOrEmpty(lists[i].Industry) ? string.Empty : Convert.ToString(lists[i].Industry).Trim();
                //区域级别
                lists[i].RegionalLevel = string.IsNullOrEmpty(lists[i].RegionalLevel) ? string.Empty : Convert.ToString(lists[i].RegionalLevel).Trim();
                //企业性质
                lists[i].EnterpriseNature = string.IsNullOrEmpty(lists[i].EnterpriseNature) ? string.Empty : Convert.ToString(lists[i].EnterpriseNature).Trim();
                //员工人数
                lists[i].EmployeesNum = lists[i].EmployeesNum == null ? Convert.ToInt32(0) : Convert.ToInt32(lists[i].EmployeesNum);
                //联系人
                lists[i].Contacts = string.IsNullOrEmpty(lists[i].Contacts) ? string.Empty : Convert.ToString(lists[i].Contacts).Trim();
                //联系电话
                lists[i].ContactPhone = string.IsNullOrEmpty(lists[i].ContactPhone) ? string.Empty : Convert.ToString(lists[i].ContactPhone).Trim();
                //企业地址
                lists[i].EnterpriseAddress = string.IsNullOrEmpty(lists[i].EnterpriseAddress) ? string.Empty : Convert.ToString(lists[i].EnterpriseAddress).Trim();
                //企业简介
                lists[i].EnterpriseIntroduction = string.IsNullOrEmpty(lists[i].EnterpriseIntroduction) ? string.Empty : Convert.ToString(lists[i].EnterpriseIntroduction).Trim();
                //焦点图地址Json
                lists[i].FocusImages = string.IsNullOrEmpty(lists[i].FocusImages) ? string.Empty : Convert.ToString(lists[i].FocusImages).Trim();
                //本年度销售额度
                lists[i].CurrentYearSales = lists[i].CurrentYearSales == null ? Convert.ToDecimal(0) : Convert.ToDecimal(lists[i].CurrentYearSales);
                //公司类别[A类、B类、C类根据销售本年度销售额度判断A类销售额度大于800万、B类销售额度在500到800万之间、C类在200到500万之间]
                lists[i].CompanyType = string.IsNullOrEmpty(lists[i].CompanyType) ? string.Empty : Convert.ToString(lists[i].CompanyType).Trim();
                //地图经度
                lists[i].Lng = lists[i].Lng == null ? Convert.ToDouble(0) : Convert.ToDouble(lists[i].Lng);
                //地图纬度
                lists[i].Lat = lists[i].Lat == null ? Convert.ToDouble(0) : Convert.ToDouble(lists[i].Lat);
                //视频地址
                lists[i].videoUrl = string.IsNullOrEmpty(lists[i].videoUrl) ? string.Empty : Convert.ToString(lists[i].videoUrl).Trim();
                //备用字段01
                lists[i].Backup01 = string.IsNullOrEmpty(lists[i].Backup01) ? string.Empty : Convert.ToString(lists[i].Backup01).Trim();
                //备用字段02
                lists[i].Backup02 = string.IsNullOrEmpty(lists[i].Backup02) ? string.Empty : Convert.ToString(lists[i].Backup02).Trim();
                //备用字段03
                lists[i].Backup03 = string.IsNullOrEmpty(lists[i].Backup03) ? string.Empty : Convert.ToString(lists[i].Backup03).Trim();
                //备用字段04
                lists[i].Backup04 = string.IsNullOrEmpty(lists[i].Backup04) ? string.Empty : Convert.ToString(lists[i].Backup04).Trim();
                //备用字段05
                lists[i].Backup05 = string.IsNullOrEmpty(lists[i].Backup05) ? string.Empty : Convert.ToString(lists[i].Backup05).Trim();
                //备用字段06
                lists[i].Backup06 = string.IsNullOrEmpty(lists[i].Backup06) ? string.Empty : Convert.ToString(lists[i].Backup06).Trim();
                //备用字段07
                lists[i].Backup07 = string.IsNullOrEmpty(lists[i].Backup07) ? string.Empty : Convert.ToString(lists[i].Backup07).Trim();
                //备用字段08
                lists[i].Backup08 = string.IsNullOrEmpty(lists[i].Backup08) ? string.Empty : Convert.ToString(lists[i].Backup08).Trim();
                //备用字段09
                lists[i].Backup09 = string.IsNullOrEmpty(lists[i].Backup09) ? string.Empty : Convert.ToString(lists[i].Backup09).Trim();
                //添加人
                lists[i].Created = string.IsNullOrEmpty(lists[i].Created) ? string.Empty : Convert.ToString(lists[i].Created).Trim();
                //添加时间
                lists[i].CreatedTime = lists[i].CreatedTime == null ? (DateTime)SqlDateTime.MinValue : Convert.ToDateTime(lists[i].CreatedTime.GetValueOrDefault());
                //修改人
                lists[i].Modifier = string.IsNullOrEmpty(lists[i].Modifier) ? string.Empty : Convert.ToString(lists[i].Modifier).Trim();
                //修改时间
                lists[i].ModifiedTime = lists[i].ModifiedTime == null ? (DateTime)SqlDateTime.MinValue : Convert.ToDateTime(lists[i].ModifiedTime.GetValueOrDefault());
            }
        }

        ///<summary>
        ///赋值数据行
        ///</summary>
        ///<param name="model">GSK货品移动表C21A</param>
        private DbParameter[] SetSqlParameter(CompanyInfo model)
        {
            #region 赋值Sql参数
            DbParameter[] result = new DbParameter[]
            {
                    //雪花ID
                     Database.MakeInParam("RecordId",model.RecordId),
                    //公司名称
                     Database.MakeInParam("CompanyName",model.CompanyName),
                    //企业类型
                     Database.MakeInParam("EnterpriseType",model.EnterpriseType),
                    //注册资本
                     Database.MakeInParam("RegisteredCapital",model.RegisteredCapital),
                    //成立时间
                     Database.MakeInParam("FoundedTime",model.FoundedTime),
                    //所属乡镇
                     Database.MakeInParam("TownShip",model.TownShip),
                    //所属行业
                     Database.MakeInParam("Industry",model.Industry),
                    //区域级别
                     Database.MakeInParam("RegionalLevel",model.RegionalLevel),
                    //企业性质
                     Database.MakeInParam("EnterpriseNature",model.EnterpriseNature),
                    //员工人数
                     Database.MakeInParam("EmployeesNum",model.EmployeesNum),
                    //联系人
                     Database.MakeInParam("Contacts",model.Contacts),
                    //联系电话
                     Database.MakeInParam("ContactPhone",model.ContactPhone),
                    //企业地址
                     Database.MakeInParam("EnterpriseAddress",model.EnterpriseAddress),
                    //企业简介
                     Database.MakeInParam("EnterpriseIntroduction",model.EnterpriseIntroduction),
                    //焦点图地址Json
                     Database.MakeInParam("FocusImages",model.FocusImages),
                    //本年度销售额度
                     Database.MakeInParam("CurrentYearSales",model.CurrentYearSales),
                    //公司类别[A类、B类、C类根据销售本年度销售额度判断A类销售额度大于800万、B类销售额度在500到800万之间、C类在200到500万之间]
                     Database.MakeInParam("CompanyType",model.CompanyType),
                    //地图经度
                     Database.MakeInParam("Lng",model.Lng),
                    //地图纬度
                     Database.MakeInParam("Lat",model.Lat),
                    //视频地址
                     Database.MakeInParam("videoUrl",model.videoUrl),
                    //备用字段01
                     Database.MakeInParam("Backup01",model.Backup01),
                    //备用字段02
                     Database.MakeInParam("Backup02",model.Backup02),
                    //备用字段03
                     Database.MakeInParam("Backup03",model.Backup03),
                    //备用字段04
                     Database.MakeInParam("Backup04",model.Backup04),
                    //备用字段05
                     Database.MakeInParam("Backup05",model.Backup05),
                    //备用字段06
                     Database.MakeInParam("Backup06",model.Backup06),
                    //备用字段07
                     Database.MakeInParam("Backup07",model.Backup07),
                    //备用字段08
                     Database.MakeInParam("Backup08",model.Backup08),
                    //备用字段09
                     Database.MakeInParam("Backup09",model.Backup09),
                    //添加人
                     Database.MakeInParam("Created",model.Created),
                    //添加时间
                     Database.MakeInParam("CreatedTime",model.CreatedTime),
                    //修改人
                     Database.MakeInParam("Modifier",model.Modifier),
                    //修改时间
                     Database.MakeInParam("ModifiedTime",model.ModifiedTime),
            };
            #endregion

            return result;
        }

        ///<summary>
        ///生成插入Sql语句
        ///</summary>
        ///<param name="lists">数据List</param>
        ///<returns>插入Sql语句字符串数组</returns>
        private List<string> MarkInsertSql(List<CompanyInfo> lists)
        {
            List<string> result = new List<string>();
            foreach (CompanyInfo model in lists)
            {
                #region 拼写Sql语句
                string Sql = "insert into CompanyInfo(";
                Sql += "RecordId,";
                Sql += "CompanyName,";
                Sql += "EnterpriseType,";
                Sql += "RegisteredCapital,";
                Sql += "FoundedTime,";
                Sql += "TownShip,";
                Sql += "Industry,";
                Sql += "RegionalLevel,";
                Sql += "EnterpriseNature,";
                Sql += "EmployeesNum,";
                Sql += "Contacts,";
                Sql += "ContactPhone,";
                Sql += "EnterpriseAddress,";
                Sql += "EnterpriseIntroduction,";
                Sql += "FocusImages,";
                Sql += "CurrentYearSales,";
                Sql += "CompanyType,";
                Sql += "Lng,";
                Sql += "Lat,";
                Sql += "videoUrl,";
                Sql += "Backup01,";
                Sql += "Backup02,";
                Sql += "Backup03,";
                Sql += "Backup04,";
                Sql += "Backup05,";
                Sql += "Backup06,";
                Sql += "Backup07,";
                Sql += "Backup08,";
                Sql += "Backup09,";
                Sql += "Created,";
                Sql += "CreatedTime,";
                Sql += "Modifier,";
                Sql += "ModifiedTime";
                Sql += ") values(";
                Sql += "'" + FilteSQLStr(model.RecordId) + "',";
                Sql += "'" + FilteSQLStr(model.CompanyName) + "',";
                Sql += "'" + FilteSQLStr(model.EnterpriseType) + "',";
                Sql += "'" + FilteSQLStr(model.RegisteredCapital) + "',";
                Sql += "CAST('" + model.FoundedTime.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss") + "' AS DATETIME),";
                Sql += "'" + FilteSQLStr(model.TownShip) + "',";
                Sql += "'" + FilteSQLStr(model.Industry) + "',";
                Sql += "'" + FilteSQLStr(model.RegionalLevel) + "',";
                Sql += "'" + FilteSQLStr(model.EnterpriseNature) + "',";
                Sql += "'" + FilteSQLStr(model.EmployeesNum) + "',";
                Sql += "'" + FilteSQLStr(model.Contacts) + "',";
                Sql += "'" + FilteSQLStr(model.ContactPhone) + "',";
                Sql += "'" + FilteSQLStr(model.EnterpriseAddress) + "',";
                Sql += "'" + FilteSQLStr(model.EnterpriseIntroduction) + "',";
                Sql += "'" + model.FocusImages + "',";
                Sql += "'" + FilteSQLStr(model.CurrentYearSales) + "',";
                Sql += "'" + FilteSQLStr(model.CompanyType) + "',";
                Sql += "'" + FilteSQLStr(model.Lng) + "',";
                Sql += "'" + FilteSQLStr(model.Lat) + "',";
                Sql += "'" + model.videoUrl + "',";
                Sql += "'" + FilteSQLStr(model.Backup01) + "',";
                Sql += "'" + FilteSQLStr(model.Backup02) + "',";
                Sql += "'" + FilteSQLStr(model.Backup03) + "',";
                Sql += "'" + FilteSQLStr(model.Backup04) + "',";
                Sql += "'" + FilteSQLStr(model.Backup05) + "',";
                Sql += "'" + FilteSQLStr(model.Backup06) + "',";
                Sql += "'" + FilteSQLStr(model.Backup07) + "',";
                Sql += "'" + FilteSQLStr(model.Backup08) + "',";
                Sql += "'" + FilteSQLStr(model.Backup09) + "',";
                Sql += "'" + FilteSQLStr(model.Created) + "',";
                Sql += "CAST('" + model.CreatedTime.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss") + "' AS DATETIME),";
                Sql += "'" + FilteSQLStr(model.Modifier) + "',";
                Sql += "CAST('" + model.ModifiedTime.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss") + "' AS DATETIME)";
                Sql += ")";
                #endregion
                result.Add(Sql);
            }
            return result;
        }

        ///<summary>
        ///生成更新Sql语句
        ///</summary>
        ///<param name="lists">数据List</param>
        ///<param name="SqlWhere">更新条件</param>
        ///<returns>更新Sql语句字符串数组</returns>
        private List<string> MarkUpdateSql(List<CompanyInfo> lists, string SqlWhere)
        {
            List<string> result = new List<string>();
            foreach (CompanyInfo model in lists)
            {
                #region 拼写Sql语句
                string Sql = "update CompanyInfo set ";
                Sql += "RecordId='" + FilteSQLStr(model.RecordId) + "',";
                Sql += "CompanyName='" + FilteSQLStr(model.CompanyName) + "',";
                Sql += "EnterpriseType='" + FilteSQLStr(model.EnterpriseType) + "',";
                Sql += "RegisteredCapital='" + FilteSQLStr(model.RegisteredCapital) + "',";
                Sql += "FoundedTime=CAST('" + model.FoundedTime.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss") + "' AS DATETIME),";
                Sql += "TownShip='" + FilteSQLStr(model.TownShip) + "',";
                Sql += "Industry='" + FilteSQLStr(model.Industry) + "',";
                Sql += "RegionalLevel='" + FilteSQLStr(model.RegionalLevel) + "',";
                Sql += "EnterpriseNature='" + FilteSQLStr(model.EnterpriseNature) + "',";
                Sql += "EmployeesNum='" + FilteSQLStr(model.EmployeesNum) + "',";
                Sql += "Contacts='" + FilteSQLStr(model.Contacts) + "',";
                Sql += "ContactPhone='" + FilteSQLStr(model.ContactPhone) + "',";
                Sql += "EnterpriseAddress='" + FilteSQLStr(model.EnterpriseAddress) + "',";
                Sql += "EnterpriseIntroduction='" + FilteSQLStr(model.EnterpriseIntroduction) + "',";
                Sql += "FocusImages='" + model.FocusImages + "',";
                Sql += "CurrentYearSales='" + FilteSQLStr(model.CurrentYearSales) + "',";
                Sql += "CompanyType='" + FilteSQLStr(model.CompanyType) + "',";
                Sql += "Lng='" + FilteSQLStr(model.Lng) + "',";
                Sql += "Lat='" + FilteSQLStr(model.Lat) + "',";
                Sql += "videoUrl='" + model.videoUrl + "',";
                Sql += "Backup01='" + FilteSQLStr(model.Backup01) + "',";
                Sql += "Backup02='" + FilteSQLStr(model.Backup02) + "',";
                Sql += "Backup03='" + FilteSQLStr(model.Backup03) + "',";
                Sql += "Backup04='" + FilteSQLStr(model.Backup04) + "',";
                Sql += "Backup05='" + FilteSQLStr(model.Backup05) + "',";
                Sql += "Backup06='" + FilteSQLStr(model.Backup06) + "',";
                Sql += "Backup07='" + FilteSQLStr(model.Backup07) + "',";
                Sql += "Backup08='" + FilteSQLStr(model.Backup08) + "',";
                Sql += "Backup09='" + FilteSQLStr(model.Backup09) + "',";
                Sql += "Created='" + FilteSQLStr(model.Created) + "',";
                Sql += "CreatedTime=CAST('" + model.CreatedTime.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss") + "' AS DATETIME),";
                Sql += "Modifier='" + FilteSQLStr(model.Modifier) + "',";
                Sql += "ModifiedTime=CAST('" + model.ModifiedTime.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss") + "' AS DATETIME)";
                if (!string.IsNullOrEmpty(SqlWhere))
                    Sql += " Where " + SqlWhere;
                #endregion
                result.Add(Sql);
            }
            return result;
        }
        #endregion

        #region region_gis增删改查

        /// <summary>
        /// 添加地块儿坐标范围数据 
        /// </summary>
        /// <param name="lists">批量数据</param>
        /// <param name="message">错误消息</param>
        /// <returns>添加条数</returns>
        public Message InsertRegionGis(List<region_gis> lists)
        {
            int DbState = -1;
            Message result = null;
            string message = string.Empty;
            if (lists == null)
                return new Message(false, "参数为空不能添加");
            CheckEmpty(ref lists);
            CheckMaxLength(ref lists, out message);
            if (!string.IsNullOrEmpty(message))
                return  new Message(false, message);
            List<string> sqls = this.MarkInsertSql(lists);
            try
            {
                DbState = ExecuteNonQuery(string.Join(';', sqls.ToArray()));
            }
            catch (Exception ex)
            {
                result = new Message(false, ex.Message);
            }
            if (result == null)
            {
                result =  new Message(true, string.Empty);
            }
            return result;
        }

        /// <summary>
        /// 修改地块儿坐标范围数据 
        /// </summary>
        /// <param name="lists">批量数据</param>
        /// <param name="SqlWhere">更新条件</param>
        /// <param name="message">错误消息</param>
        /// <returns>修改条数</returns>
        public async Task<Message> UpdateRegionGis(List<region_gis> lists, string SqlWhere)
        {
            Message result = null;
            int DbState = -1;
            string message = string.Empty;
            CheckEmpty(ref lists);
            List<string> sqls = this.MarkUpdateSql(lists, SqlWhere);
            try
            {
                DbState = ExecuteNonQuery(CommandType.Text, string.Join(';', sqls.ToArray()));
            }
            catch (Exception ex)
            {
                message = ex.Message;
                result = new Message(false, ex.Message);
            }
            if (result == null)
            {
                result = new Message(true, string.Empty);
            }
            return result;
        }

        /// <summary>
        /// 删除地块儿范围数据
        /// </summary>
        /// <param name="landId">地块儿编号</param>
        /// <returns>返回值</returns>
        public async Task<Message> BetchDeleteRegionGisByLandId(string landId) 
        {
            var result = new Message();
            string sql = "";
            int DbState = -1;
            string message = string.Empty;
            if (string.IsNullOrEmpty(landId)) 
            {
                result.Successful = false;
                result.Content = "地块儿编号不能为空不能小于0";
                return result;
            }
            sql = $" delete from region_gis where landId in ('{landId}') ";
            try
            {
                DbState = ExecuteNonQuery(CommandType.Text, sql);
            }
            catch (Exception ex)
            {
                message = ex.Message;
                result = new Message(false, ex.Message);
            }
            if (result == null)
            {
                result = new Message(true, string.Empty);
            }
            return result;
        }

        /// <summary>
        /// 查询region_gis数据
        /// </summary>
        /// <param name="SqlWhere">查询条件</param>
        /// <param name="message">错误消息</param>
        /// <returns>返回值</returns>
        public List<region_gis> QueryRegionGis(string SqlWhere, out string message)
        {
            List<region_gis> result = new List<region_gis>();
            string sql = $"select {FieldRegionGis()} from region_gis";
            DataSet ds = new DataSet();
            DataTable dt = null;
            message = string.Empty;
            if (!string.IsNullOrEmpty(SqlWhere))
                sql += $" where {SqlWhere} ";
            try
            {
                ds = m_dbHelper.ExecuteDataSet(sql);
                #region 非空检查
                if (ds == null)
                    return result;
                if (ds.Tables == null || ds.Tables.Count == 0)
                    return result;
                dt = ds.Tables[0];
                if (dt == null)
                    return result;
                if (dt.Rows.Count == 0)
                    return result;
                #endregion
                foreach (DataRow dtRow in dt.Rows)
                    result.Add(ReadDataRow(dtRow, new region_gis()));
            }
            catch (Exception exp)
            {
                message = exp.Message;
            }
            return result;
        }

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
        public List<region_gis> QueryPageRegionGis(
            string SqlWhere,
            string SortField,
            string SortMethod,
            int PageSize,
            int CurPage,
            out int TotalNumber,
            out int PageCount,
            out string message
            )
        {
            List<region_gis> result = null;
            string FieldStr = FieldRegionGis();
            Func<DataRow, region_gis, region_gis> func = (DataRow dr, region_gis model) => {
                return ReadDataRow(dr, model);
            };
            result = QueryPage<region_gis>(
                SqlWhere,
                SortField,
                SortMethod,
                FieldStr,
                "region_gis",
                PageSize,
                CurPage,
                func,
                out TotalNumber,
                out PageCount,
                out message
                );
            return result;
        }
        #endregion

        #region region_gis基础方法
        /// <summary>
        /// 返回region_gis字段列表
        /// </summary>
        /// <returns>字段列表</returns>
        private string FieldRegionGis()
        {
            return @"
                    [Id],
                    [landId],
                    [GPSLocations],
                    [Color]"
                     .Trim()
                     .Replace("\t", "")
                     .Replace("\r", "")
                     .Replace("\n", "");
        }

        /// <summary>
        /// 读取数据行到model(region_gis)
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="dr">数据行</param>
        private region_gis ReadDataRow(DataRow dr, region_gis model)
        {
            model = new region_gis();

            //编号
            model.Id = GetDataRow(dr, "Id") == null ? 0 : Convert.ToInt32(GetDataRow(dr, "Id"));
            
            //地块儿编号
            model.landId = GetDataRow(dr, "landId") == null ? 0 : Convert.ToInt32(GetDataRow(dr, "landId"));
            
            //GPS坐标范围数据
            model.GPSLocations = GetDataRow(dr, "GPSLocations") == null ? string.Empty : Convert.ToString(GetDataRow(dr, "GPSLocations")).Trim();

            //Color颜色
            model.Color = GetDataRow(dr, "Color") == null ? string.Empty : Convert.ToString(GetDataRow(dr, "Color")).Trim();

            return model;
        }

        ///<summary>
        ///检查是否空值(region_gis)
        ///</summary>
        private void CheckEmpty(ref List<region_gis> lists)
        {
            for (int i = 0; i < lists.Count; i++)
            {
                //编号
                lists[i].Id = lists[i].Id == null ? Convert.ToInt32(0) : Convert.ToInt32(lists[i].Id);
                
                //地块儿编号
                lists[i].landId = lists[i].landId == null ? Convert.ToInt32(0) : Convert.ToInt32(lists[i].landId);
                
                //GPS坐标范围数据
                lists[i].GPSLocations = string.IsNullOrEmpty(lists[i].GPSLocations) ? string.Empty : Convert.ToString(lists[i].GPSLocations).Trim();

                //颜色值
                lists[i].Color = string.IsNullOrEmpty(lists[i].Color)?String.Empty:Convert.ToString(lists[i].Color).Trim();
            }
        }

        ///<summary>
        ///检查是否超过长度(region_gis)
        ///</summary>
        ///<param name="lists">数据集</param>
        ///<param name="message">错误消息</param>
        private void CheckMaxLength(ref List<region_gis> lists, out string message)
        {
            #region 声明变量

            //错误消息
            message = string.Empty;

            //超过的长度
            int OutLength = 0;
            #endregion

            #region 循环验证长度
            for (int i = 0; i < lists.Count; i++)
            {
                if (!string.IsNullOrEmpty(lists[i].GPSLocations))
                {
                    if (lists[i].GPSLocations.Length > 8000)
                    {
                        OutLength = lists[i].GPSLocations.Length- 8000;
                        message += "字段名[GPSLocations]描述[GPS坐标范围数据]超长、字段最长[8000]实际" + lists[i].GPSLocations.Length + "超过长度" + OutLength + ",";
                    }
                }
                if (!string.IsNullOrEmpty(lists[i].Color))
                {
                    if (lists[i].Color.Length > 50)
                    {
                        OutLength = lists[i].GPSLocations.Length - 50;
                        message += "字段名[Color]描述[地块颜色]超长、字段最长[50]实际" + lists[i].GPSLocations.Length + "超过长度" + OutLength + ",";
                    }
                }
            }
            #endregion

            if (!string.IsNullOrEmpty(message)) message = message.Substring(0, message.Length - 1);
        }

        ///<summary>
        ///生成插入Sql语句(region_gis)
        ///</summary>
        ///<param name="lists">数据List</param>
        ///<returns>插入Sql语句字符串数组</returns>
        private List<string> MarkInsertSql(List<region_gis> lists)
        {
            List<string> result = new List<string>();
            foreach (region_gis model in lists)
            {
                #region 拼写Sql语句
                string Sql = "insert into region_gis(";
                Sql += "landId,";
                Sql += "GPSLocations,";
                Sql += "Color";
                Sql += ") values(";
                Sql += "'" + FilteSQLStr(model.landId) + "',";
                Sql += "'" + FilteSQLStr(model.GPSLocations) + "',";
                Sql += "'" + FilteSQLStr(model.Color) + "'";
                Sql += ")";
                #endregion
                result.Add(Sql);
            }
            return result;
        }

        ///<summary>
        ///生成更新Sql语句(region_gis)
        ///</summary>
        ///<param name="lists">数据List</param>
        ///<param name="SqlWhere">更新条件</param>
        ///<returns>更新Sql语句字符串数组</returns>
        private List<string> MarkUpdateSql(List<region_gis> lists, string SqlWhere)
        {
            List<string> result = new List<string>();
            foreach (region_gis model in lists)
            {
                #region 拼写Sql语句
                string Sql = "update region_gis set ";
                Sql += "landId='" + FilteSQLStr(model.landId) + "',";
                Sql += "GPSLocations='" + FilteSQLStr(model.GPSLocations) + "',";
                Sql += "Color='" + FilteSQLStr(model.Color) + "'";
                if (!string.IsNullOrEmpty(SqlWhere))
                    Sql += " Where " + SqlWhere;
                #endregion
                result.Add(Sql);
            }
            return result;
        }
        #endregion
    }
}
