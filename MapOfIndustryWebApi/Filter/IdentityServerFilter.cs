using MapOfIndustryWebApi.Models.Result;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;
using System.Security.Claims;

namespace MapOfIndustryWebApi.Filter
{
    /// <summary>
    /// IdentityServer验证过滤器
    /// </summary>
    public class IdentityServerFilter : ActionFilterAttribute
    {
        #region Fields

        //IdentityServer身份验证和状态说明
        private ClaimsPrincipal m_user;

        /// <summary>
        /// 返回值类型
        /// </summary>
        private string m_resultType;

        /// <summary>
        /// 泛型类型
        /// </summary>
        private string m_genericTypes;

        /// <summary>
        /// 泛型程序集
        /// </summary>
        private string m_genericAssemblyString;
        #endregion

        #region Property

        /// <summary>
        /// 返回值类型
        /// </summary>
        public string resultType
        {
            get { return m_resultType; }
            set { m_resultType = value; }
        }

        /// <summary>
        /// 泛型类型
        /// </summary>
        public string genericTypes
        {
            get { return m_genericTypes; }
            set { m_genericTypes = value; }
        }

        /// <summary>
        /// 泛型程序集
        /// </summary>
        public string genericAssemblyString
        {
            get { return m_genericAssemblyString; }
            set { m_genericAssemblyString = value; }
        }
        #endregion

        #region Public
        /// <summary>
        /// Action 方法执行之前触发
        /// </summary>
        /// <param name="context">Action执行上下文</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            m_user = context.HttpContext.User;
            Claim levelClaim = m_user.Claims.Where(d => d.Type == "Level").FirstOrDefault();
            Int32 userLevel = levelClaim == null ? -1 : Convert.ToInt32(levelClaim.Value);
            object result = null;
            if (userLevel > 2 || userLevel == -1)
            {
                if (string.IsNullOrEmpty(m_genericTypes))
                    result = CreateResult(-1, "用户权限不足");
                else
                    result = RunGenericMethod(-1, "用户权限不足");
                //拦截并返回
                context.Result = new JsonResult(result);
                return;
            }
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
        /// 创建返回值Object
        /// </summary>
        /// <param name="Status">返回值状态</param>
        /// <param name="Message">返回值消息</param>
        /// <param name="ReturnData">返回数据</param>
        /// <returns>创建后的返回值object</returns>
        private object CreateResult(int Status, string Message)
        {
            object result = null;
            result = new Result()
            {
                Status = Status,
                Msg = Message
            };
            return result;
        }

        /// <summary>
        /// 创建返回值Object
        /// </summary>
        /// <param name="Status">返回值状态</param>
        /// <param name="Message">返回值消息</param>
        /// <param name="ReturnData">返回数据</param>
        /// <returns>创建后的返回值object</returns>
        private object CreateResultObject<T>(int Status, string Message)
        {
            object result = null;
            switch (m_resultType)
            {
                case "Result":
                    result = new Result()
                    {
                        Status = Status,
                        Msg = Message
                    };
                    break;
                case "EntityResult":
                    result = new EntityResult<T>()
                    {
                        Status = Status,
                        Msg = Message
                    };
                    break;
                case "ArrayResult":
                    result = new ArrayResult<T>()
                    {
                        Status = Status,
                        Msg = Message
                    };
                    break;
                case "ListResult":
                    result = new ListResult<T>()
                    {
                        Status = Status,
                        Msg = Message
                    };
                    break;
                case "PageResult":
                    result = new PageResult<T>()
                    {
                        Status = Status,
                        Msg = Message
                    };
                    break;
            }
            return result;
        }

        /// <summary>
        /// 执行泛型方法
        /// </summary>
        /// <param name="Status">返回值状态</param>
        /// <param name="Message">返回值消息</param>
        /// <returns>创建后的返回值object</returns>
        private object RunGenericMethod(int Status, string Message)
        {
            #region 声明变量

            //获取泛型方法
            MethodInfo methodInfo = null;

            //程序集文件路径
            string binPath = string.Empty;

            //程序集名称
            string assemblyName = string.Empty;

            //泛型程序集
            Assembly genericAssembly;

            //泛型程序集类型
            Type genericAssemblyType;

            //泛型类对象
            object genericObject;

            //泛型方法
            MethodInfo generic;
            #endregion

            //获取泛型方法
            methodInfo = GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(m => m.Name == "CreateResultObject" && m.IsGenericMethod).First();

            //获取程序集路径
            binPath = Path.GetDirectoryName(GetType().Assembly.Location);

            //获取程序集名称
            assemblyName = m_genericAssemblyString.Split(".")[0];

            //加载程序集
            genericAssembly = Assembly.LoadFrom($"{binPath}\\{assemblyName}.dll");

            //获取泛型类型
            genericAssemblyType = genericAssembly.GetType(m_genericTypes);

            //创建泛型对象
            genericObject = Activator.CreateInstance(genericAssemblyType);

            //获取泛型最终方法
            generic = methodInfo.MakeGenericMethod(genericTypes.GetType());

            //调用返回结果
            return generic.Invoke(this, new object[] { Status, Message });
        }
        #endregion
    }
}
