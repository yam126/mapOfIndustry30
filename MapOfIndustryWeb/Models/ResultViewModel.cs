using System.Threading.Tasks;

namespace MapOfIndustryWeb.Models
{
    public class ResultViewModel
    {

    }
    public class LCYResult<T>
    {
        public T Result { get; set; }
        public string Id { get; set; }
    }
    public class tokenValue<T>
    {
        public String msg { get; set; }
        public String code { get; set; }
        public T data { get; set; }
    }
    public class KitData_value
    {
        public String expireTime { get; set; }
        public String kitToken { get; set; }
    }
    public class ConsoleResult
    {
        public String Msg { get; set; }
        public String Code { get; set; }
    }
    public class data_value
    {
        public String expireTime { get; set; }
        public String accessToken { get; set; }
    }
}
