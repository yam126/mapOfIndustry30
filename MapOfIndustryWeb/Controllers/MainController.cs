using Microsoft.AspNetCore.Mvc;

namespace MapOfIndustryWeb.Controllers
{
    public class MainController : Controller
    {
        public IActionResult Index(string regionId)
        {
            ViewData["regionId"] = regionId;
            return View("Index");
        }
    }
}
