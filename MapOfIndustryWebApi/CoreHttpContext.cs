namespace MapOfIndustryWebApi
{
    /// <summary>
    /// 用于获得服务器绝对路径地址
    /// </summary>
    public static class CoreHttpContext
    {
        [Obsolete]
        private static Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostEnviroment;

        [Obsolete]
        public static string WebPath => _hostEnviroment.ContentRootPath;

        [Obsolete]
        public static string MapPath(string path)
        {
            return _hostEnviroment.ContentRootPath + path;
        }

        [Obsolete]
        internal static void Configure(Microsoft.AspNetCore.Hosting.IHostingEnvironment hostEnviroment)
        {
            _hostEnviroment = hostEnviroment;
        }
    }

    /// <summary>
    /// 服务器绝对路径获取注册类
    /// </summary>
    public static class StaticHostEnviromentExtensions
    {
        /// <summary>
        /// 服务器绝对路径获取注册方法
        /// </summary>
        /// <param name="app">app</param>
        /// <returns></returns>
        [Obsolete]
        public static IApplicationBuilder UseStaticHostEnviroment(this IApplicationBuilder app)
        {
            var webHostEnvironment = app.ApplicationServices.GetRequiredService<Microsoft.AspNetCore.Hosting.IHostingEnvironment>();
            CoreHttpContext.Configure(webHostEnvironment);
            return app;
        }
    }
}
