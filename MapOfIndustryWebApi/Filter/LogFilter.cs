using Common;
using MapOfIndustryDataAccess;
using MapOfIndustryDataAccess.Data;
using MapOfIndustryDataAccess.Models;
using mapOfIndustryWebApi.Models.Parameter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Text;

namespace MapOfIndustryWebApi.Filter
{
    /// <summary>
    /// 日志记录过滤器(用于接口的日志记录)
    /// </summary>
    public class LogFilter : ActionFilterAttribute
    {
        #region Fields
        /// <summary>
        /// 日志记录文件夹路径
        /// </summary>
        private readonly string m_logFilePath = @"\LogFile\";

        /// <summary>
        /// 数据库操作
        /// </summary>
        private IMOIRepository m_repository;
        #endregion

        #region Public
        /// <summary>
        /// Action 方法执行之前触发
        /// </summary>
        /// <param name="context">Action执行上下文</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            #region 声明变量

            //错误消息
            string message = string.Empty;

            //Action名称
            string actionName = ((ControllerActionDescriptor)context.ActionDescriptor).ActionName;

            //控制器名称
            string controllerName = ((ControllerActionDescriptor)context.ActionDescriptor).ControllerName;

            //Action参数集合
            List<ActionParameter> actionParameters = new List<ActionParameter>();

            //action参数Json字符串
            string actionParametersJsonStr = string.Empty;

            //删除的数据
            List<MassifGreenHouseVP> deleteData;
            #endregion

            //初始化数据库访问类
            m_repository = new MOIRepository();

            #region 记录传参
            if (context.ActionArguments != null && context.ActionArguments.Keys.Count > 0)
            {
                foreach (var key in context.ActionArguments.Keys)
                {
                    object argsValue = context.ActionArguments[key];
                    actionParameters.Add(new ActionParameter()
                    {
                        Name = key,
                        Value = argsValue
                    });
                }
            }
            #region 如果是数据删除方法
            if (actionName == "DeleteData")
            {
                string ID = Convert.ToString(context.ActionArguments["ID"]);
                deleteData = m_repository.QueryMassifGreenHouseVP($" ID='{ID}'", out message);
                #region 记录删除的数据
                actionParameters.Add(new ActionParameter()
                {
                    Name = "要删除的数据",
                    Value = deleteData
                });
                #endregion
            }
            #endregion

            #endregion

            #region 写入日志
            if (actionParameters.Count > 0)
                actionParametersJsonStr = JsonConvert.SerializeObject(actionParameters);
            WriteLog(controllerName, actionName, $"调用时间{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.tt")}\t\r\n参数\t\r\n{actionParametersJsonStr}");
            #endregion
        }

        /// <summary>
        /// Action 方法执行之后 Result 调用之前触发
        /// </summary>
        /// <param name="context">Action执行上下文</param>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
        }

        /// <summary>
        /// Result方法调用之前执行
        /// </summary>
        /// <param name="context">Action执行上下文</param>
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            base.OnResultExecuting(context);

            #region 声明变量

            //错误消息
            string message = string.Empty;

            //Action名称
            string actionName = ((ControllerActionDescriptor)context.ActionDescriptor).ActionName;

            //控制器名称
            string controllerName = ((ControllerActionDescriptor)context.ActionDescriptor).ControllerName;

            //返回值Json字符串
            string resultJsonStr = string.Empty;

            //Action的返回值
            ObjectResult ApiResult = null;
            #endregion

            #region 记录传参
            if (context.Result != null)
            {
                ApiResult = (ObjectResult)context.Result;
                resultJsonStr = JsonConvert.SerializeObject(ApiResult.Value).Replace("\u0022", "\"");
            }
            #endregion

            #region 写入日志
            WriteLog(controllerName, actionName, $"调用时间{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.tt")}\t\r\n返回值\t\r\n{resultJsonStr}");
            #endregion
        }

        /// <summary>
        /// Result方法调用之后执行
        /// </summary>
        /// <param name="context">Action执行上下文</param>
        public override void OnResultExecuted(ResultExecutedContext context)
        {
            base.OnResultExecuted(context);
        }
        #endregion

        #region Private
        /// <summary>
        /// 获取日志文件夹绝对路径
        /// </summary>
        /// <returns>日志文件夹绝对路径</returns>
        private string GetAbsolutelyLogPath()
        {
            string result = string.Empty;
            string serverPath = CoreHttpContext.MapPath(m_logFilePath);
            if (!Directory.Exists(serverPath))
                Directory.CreateDirectory(serverPath);
            result = serverPath;
            return result;
        }

        /// <summary>
        /// 写入日志方法
        /// </summary>
        /// <param name="ControllerName">控制器名</param>
        /// <param name="ActionMethodName">Action名称</param>
        /// <param name="LogContent">日志内容</param>
        private void WriteLog(string ControllerName, string ActionMethodName, string LogContent)
        {
            #region 声明变量

            //日志文件名
            string LogFileName = $"{ControllerName}_{ActionMethodName}_{DateTime.Now.ToString("yyyyMMdd")}.txt";

            //获得日志文件的绝对路径
            string LogPath = GetAbsolutelyLogPath();
            #endregion

            Utils.WriteTextToFile(LogPath + LogFileName, true, LogContent, Encoding.GetEncoding("GB2312"));
        }
        #endregion
    }
}
