using Microsoft.AspNetCore.Mvc;

namespace MapOfIndustryWeb.Controllers
{
    public class CompanyController : Controller
    {
        public IActionResult CompanyMaster()
        {
            ViewData["LandId"] = "";
            ViewData["CurrentPage"] = "CompanyMaster";
            return View("CompanyMaster");
        }
    }
}
