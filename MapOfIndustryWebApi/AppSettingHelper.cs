namespace MapOfIndustryWebApi
{
    /// <summary>
    /// 配置文件读取帮助类
    /// </summary>
    public class AppSettingHelper
    {
        /// <summary>
        /// 配置文件读取系统类
        /// </summary>
        private static IConfiguration _config;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configuration">配置信息</param>
        public AppSettingHelper(IConfiguration configuration)
        {
            _config = configuration;
        }

        /// <summary>
        /// 读取指定节点的字符串
        /// </summary>
        /// <param name="sessions">配置文件节点路径</param>
        /// <returns>返回数据</returns>
        public static string ReadAppSettings(params string[] sessions)
        {
            try
            {
                if (sessions.Any())
                {
                    return _config[string.Join(":", sessions)];
                }
            }
            catch
            {
                return "";
            }
            return "";
        }

        /// <summary>
        /// 读取实体信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session"></param>
        /// <returns></returns>
        public static List<T> ReadAppSettings<T>(params string[] session)
        {
            List<T> list = new List<T>();
            _config.Bind(string.Join(":", session), list);
            return list;
        }
    }
}
