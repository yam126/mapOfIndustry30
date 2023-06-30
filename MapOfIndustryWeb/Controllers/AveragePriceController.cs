using Microsoft.AspNetCore.Mvc;

namespace MapOfIndustryWeb.Controllers
{
    public class AveragePriceController : Controller
    {
        public IActionResult AveragePrice(string landId)
        {
            ViewData["LandId"] = landId;
            return View("AveragePrice");
        }
    }
}
