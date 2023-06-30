using Microsoft.AspNetCore.Mvc;

namespace MapOfIndustryWeb.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
