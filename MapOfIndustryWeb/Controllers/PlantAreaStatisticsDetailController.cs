using Microsoft.AspNetCore.Mvc;

namespace MapOfIndustryWeb.Controllers
{
    public class PlantAreaStatisticsDetailController : Controller
    {
        public IActionResult PlantAreaDetail(string landId, string cropsType)
        {
            ViewData["LandId"] = landId;
            ViewData["cropsType"] = cropsType;
            return View("PlantAreaDetail");
        }
    }
}
