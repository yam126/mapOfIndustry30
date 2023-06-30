using Npoi.Mapper.Attributes;

namespace MapOfIndustryWebApi.Models
{
    public class CompanyInfoParameter
    {
        /// <summary>
        /// 雪花ID
        /// </summary>
        public System.String RecordId { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        [Column("公司名称")]
        public System.String CompanyName { get; set; }

        /// <summary>
        /// 企业类型
        /// </summary>
        [Column("企业类型")]
        public System.String EnterpriseType { get; set; }

        /// <summary>
        /// 注册资本
        /// </summary>
        [Column("注册资本")]
        public System.String RegisteredCapital { get; set; }

        /// <summary>
        /// 成立时间
        /// </summary>
        [Column("成立时间")]
        public System.String FoundedTime { get; set; }

        /// <summary>
        /// 所属乡镇
        /// </summary>
        [Column("所属乡镇")]
        public System.String TownShip { get; set; }

        /// <summary>
        /// 所属行业
        /// </summary>
        [Column("所属行业")]
        public System.String Industry { get; set; }

        /// <summary>
        /// 区域级别
        /// </summary>
        [Column("区域级别")]
        public System.String RegionalLevel { get; set; }

        /// <summary>
        /// 企业性质
        /// </summary>
        [Column("企业性质")]
        public System.String EnterpriseNature { get; set; }

        /// <summary>
        /// 员工人数
        /// </summary>
        [Column("员工人数")]
        public System.String EmployeesNum { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        [Column("联系人")]
        public System.String Contacts { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [Column("联系电话")]
        public System.String ContactPhone { get; set; }

        /// <summary>
        /// 企业地址
        /// </summary>
        [Column("企业地址")]
        public System.String EnterpriseAddress { get; set; }

        /// <summary>
        ///企业简介
        /// </summary>
        [Column("企业简介")]
        public System.String EnterpriseIntroduction { get; set; }

        /// <summary>
        ///焦点图地址Json
        /// </summary>
        public System.String FocusImages { get; set; }

        /// <summary>
        ///本年度销售额度
        /// </summary>
        [Column("本年度销售额度")]
        public System.String CurrentYearSales { get; set; }

        /// <summary>
        ///公司类别[A类、B类、C类根据销售本年度销售额度判断A类销售额度大于800万、B类销售额度在500到800万之间、C类在200到500万之间]
        /// </summary>
        [Column("公司类别")]
        public System.String CompanyType { get; set; }

        /// <summary>
        ///地图经度
        /// </summary>
        [Column("地图经度")]
        public System.String Lng { get; set; }

        /// <summary>
        ///地图纬度
        /// </summary>
        [Column("地图纬度")]
        public System.String Lat { get; set; }

        /// <summary>
        ///视频地址
        /// </summary>
        [Column("视频地址")]
        public System.String videoUrl { get; set; }

        /// <summary>
        ///备用字段01
        /// </summary>
        [Column("备用字段01")]
        public System.String Backup01 { get; set; }

        /// <summary>
        ///备用字段02
        /// </summary>
        [Column("备用字段02")]
        public System.String Backup02 { get; set; }

        /// <summary>
        ///备用字段03
        /// </summary>
        public System.String Backup03 { get; set; }

        /// <summary>
        ///备用字段04
        /// </summary>
        public System.String Backup04 { get; set; }

        /// <summary>
        ///备用字段05
        /// </summary>
        public System.String Backup05 { get; set; }

        /// <summary>
        ///备用字段06
        /// </summary>
        public System.String Backup06 { get; set; }

        /// <summary>
        ///备用字段07
        /// </summary>
        public System.String Backup07 { get; set; }

        /// <summary>
        ///备用字段08
        /// </summary>
        public System.String Backup08 { get; set; }

        /// <summary>
        ///备用字段09
        /// </summary>
        public System.String Backup09 { get; set; }

        /// <summary>
        ///添加人
        /// </summary>
        public System.String Created { get; set; }

        /// <summary>
        ///添加时间
        /// </summary>
        public System.String CreatedTime { get; set; }

        /// <summary>
        ///修改人
        /// </summary>
        public System.String Modifier { get; set; }

        /// <summary>
        ///修改时间
        /// </summary>
        public System.String ModifiedTime { get; set; }
    }
}
