using Microsoft.AspNetCore.Mvc;

namespace MapOfIndustryWeb.Controllers
{
    public class DataManageController : Controller
    {
        public IActionResult Index(string LandId)
        {
            ViewData["Message"] = "test";
            ViewData["LandId"] = LandId;
            ViewData["CurrentPage"] = "NewHome";
            return View("Index");
        }
    }
}
