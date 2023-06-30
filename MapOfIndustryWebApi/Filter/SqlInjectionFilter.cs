using mapOfIndustryWebApi.Models.Parameter;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MapOfIndustryWebApi.Filter
{
    /// <summary>
    /// 对指定参数进行SQL注入验证
    /// </summary>
    public class SqlInjectionFilter : ActionFilterAttribute
    {
        #region Fields

        /// <summary>
        /// 要过滤的参数名称集合[参数1,参数2]
        /// </summary>
        private string m_verifyParameterNames;
        #endregion

        #region Property
        /// <summary>
        /// 要验证的参数名称集合[参数1,参数2]
        /// </summary>
        public string VerifyParameterNames
        {
            get { return m_verifyParameterNames; }
            set { m_verifyParameterNames = value; }
        }
        #endregion

        #region Public
        /// <summary>
        /// Action 方法执行之前触发
        /// </summary>
        /// <param name="context">Action执行上下文</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            #region 声明变量

            //错误消息
            string message = string.Empty;

            //Action名称
            string actionName = ((ControllerActionDescriptor)context.ActionDescriptor).ActionName;

            //控制器名称
            string controllerName = ((ControllerActionDescriptor)context.ActionDescriptor).ControllerName;

            //Action参数集合
            List<ActionParameter> actionParameters = new List<ActionParameter>();

            //验证参数
            List<string> verifyParameterNames = null;
            #endregion

            #region 记录传参
            if (!string.IsNullOrEmpty(m_verifyParameterNames))
            {
                verifyParameterNames = m_verifyParameterNames.Split(",").ToList();
                if (context.ActionArguments != null && context.ActionArguments.Keys.Count > 0)
                {
                    foreach (var key in context.ActionArguments.Keys)
                    {
                        object argsValue = context.ActionArguments[key];
                        if (verifyParameterNames.Contains(key))
                            context.ActionArguments[key] = Convert.ToInt32(FilteSQLStr(argsValue));
                    }
                }
            }
            #endregion
            base.OnActionExecuting(context);
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
        #endregion
    }
}
