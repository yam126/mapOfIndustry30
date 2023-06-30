namespace MapOfIndustryWebApi.Models
{
    /// <summary>
    /// 地图边界版本号
    /// </summary>
    public class MapLocationVerisonParameter
    {
        /// <summary>
        ///自增编号
        /// </summary>
        public System.String Id { get; set; }

        /// <summary>
        ///地图边界版本号
        /// </summary>
        public System.String MapLocalVerison { get; set; }

        /// <summary>
        ///创建人
        /// </summary>
        public System.String Created { get; set; }

        /// <summary>
        ///创建时间
        /// </summary>
        public System.String CreatedTime { get; set; }
    }
}
