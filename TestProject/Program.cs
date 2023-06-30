// See https://aka.ms/new-console-template for more information

using Common;
using StackExchange.Redis;
using TestProject;

/// <summary>
/// Redis缓存测试操作类
/// </summary>
void TestRedis()
{
    RedisHelper redisHelper = new RedisHelper("127.0.0.1:6379");
    List<RedisTest> redisTestData = new List<RedisTest>();
    List<RedisValue> redisValues = new List<RedisValue>();
    string message = string.Empty;
    for (var i = 0; i < 10; i++)
    {
        redisTestData.Add(new RedisTest()
        {
            TestKey = $"123{i}",
            TestValue = $"123{i}"
        });
    }
    redisHelper.SetAdd("testKey", JSONHelper.ListToJSON<RedisTest>(redisTestData,"",out message));
}

void TestReadRedis() 
{
    string message = string.Empty;
    RedisHelper redisHelper = new RedisHelper("127.0.0.1:6379");
    Console.WriteLine(redisHelper.KeyExists("testKey"));
    var setCollect = redisHelper.SetMembers("testKey");
    List<RedisTest> redisTestData = new List<RedisTest>();
    string jsonString = setCollect[0];
    redisTestData = JSONHelper.JSONToList<RedisTest>(jsonString, out message);
    Console.WriteLine(redisTestData);
}

void Main(string[] args=null) 
{
    TestReadRedis();
}

Main();