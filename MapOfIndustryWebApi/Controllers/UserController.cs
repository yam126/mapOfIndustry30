using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Snowflake.Net;
using Common;
using MapOfIndustryWebApi.Models;

namespace MapOfIndustryWebApi.Controllers
{
    /// <summary>
    /// 用户控制器
    /// </summary>
    [Route("api/user/")]
    [ApiController]
    public class UserController : Controller
    {
        /// <summary>
        /// 登陆类
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="client_id">客户端ID</param>
        /// <param name="client_secret">客户端密钥</param>
        /// <param name="grant_type">授予类型</param>
        /// <returns></returns>
        [HttpPost]
        [HttpGet]
        [Route("login")]
        public JsonResult Login(
            string username,
            string password,
            string client_id,
            string client_secret,
            string grant_type)
        {
            IdWorker snowId = new IdWorker(1, 1);
            string access_token =$"{username}-{password}-{client_id}-{client_secret}-{grant_type}-{snowId.NextId()}";
            access_token = DESHelper.DESEncrypt(access_token);
            return Json(new { 
                access_token= access_token,
                userInfo=new LoginResult() {
                    Account=username,
                    CompanyId="11",
                    Level="3"
                } 
            });
        }
    }
}
