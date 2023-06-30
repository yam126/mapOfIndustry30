using Microsoft.AspNetCore.Mvc;

namespace MapOfIndustryWeb.Controllers
{
    public class LandController : Controller
    {
        public IActionResult LandManage()
        {
            ViewData["LandId"] = "";
            ViewData["CurrentPage"] = "LandManage";
            return View("LandManage");
        }
    }
}
