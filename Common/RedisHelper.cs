using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// Redis帮助类
    /// </summary>
    public class RedisHelper
    {
        /// <summary>
        /// redis连接类
        /// </summary>
        private ConnectionMultiplexer redis { get; set; }
        
        /// <summary>
        /// 数据库类
        /// </summary>
        private IDatabase db { get; set; }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connection">连接字符串</param>
        public RedisHelper(string connection)
        {
            redis = ConnectionMultiplexer.Connect(connection);
            db = redis.GetDatabase();
        }

        /// <summary>
        /// 增加/修改
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns>是否成功</returns>
        public bool SetStringValue(string key, string value)
        {
            return db.StringSet(key, value);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>是否成功</returns>
        public string GetStringValue(string key)
        {
            return db.StringGet(key);
        }

        /// <summary>
        /// 获取哈希值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="hashField">哈希字段</param>
        /// <returns>返回值</returns>
        public RedisValue HashGet(string key, RedisValue hashField) 
        {
            return db.HashGet(key, hashField);
        }

        /// <summary>
        /// 查看哈希值是否存在
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="hashField">哈希字段</param>
        /// <returns>是否存在</returns>
        public bool HashExists(string key, RedisValue hashField) 
        {
            return db.HashExists(key, hashField);
        }

        /// <summary>
        /// 查看key值是否存在
        /// </summary>
        /// <param name="key">key值</param>
        /// <returns>是否存在</returns>
        public bool KeyExists(RedisKey key) 
        {
            return db.KeyExists(key);
        }

        /// <summary>
        /// 删除key数据
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>是否成功</returns>
        public bool KeyDelete(RedisKey key) 
        {
            return db.KeyDelete(key);
        }

        /// <summary>
        /// set删除
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns>是否成功</returns>
        public bool SetRemove(RedisKey key, RedisValue value) 
        {
            return db.SetRemove(key, value);
        }

        /// <summary>
        /// 添加Set
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="values">值</param>
        /// <returns></returns>
        public long SetAdd(RedisKey key, RedisValue[] values) 
        {
            return db.SetAdd(key, values);
        }


        /// <summary>
        /// Set读取数据
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public RedisValue[] SetMembers(RedisKey key) 
        {
            return db.SetMembers(key);
        }

        /// <summary>
        /// 添加Set
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool SetAdd(RedisKey key, RedisValue value)
        {
            return db.SetAdd(key, value);
        }



        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>是否成功</returns>
        public bool DeleteKey(string key)
        {
            return db.KeyDelete(key);
        }
    }
}
