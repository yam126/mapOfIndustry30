using Microsoft.AspNetCore.Mvc;

namespace MapOfIndustryWeb.Controllers
{
    public class CountDataStatisticsDetailController : Controller
    {
        public IActionResult CountDataDetail(string landId, string cropsType)
        {
            ViewData["LandId"] = landId;
            ViewData["cropsType"] = cropsType;
            return View("CountDataDetail");
        }
    }
}
