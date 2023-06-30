using Microsoft.AspNetCore.Mvc.Filters;

namespace MapOfIndustryWebApi.Filter
{
    /// <summary>
    /// 跨域设置
    /// </summary>
    public class CustomCorsActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            context.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            base.OnActionExecuting(context);
        }

        /// <summary>
        /// Action 方法执行之后 Result 调用之前触发
        /// </summary>
        /// <param name="context">Action执行上下文</param>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            context.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
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
    }
}
