namespace MapOfIndustryWebApi.Models
{
    /// <summary>
    /// 登陆返回值类
    /// </summary>
    public class LoginResult
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 公司编号
        /// </summary>
        public string CompanyId { get; set; }

        /// <summary>
        /// 级别
        /// </summary>
        public string Level { get; set; }
    }
}
