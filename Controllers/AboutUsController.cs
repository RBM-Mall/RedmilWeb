using Microsoft.AspNetCore.Mvc;
using Project_Redmil_MVC.CommonHelper;

namespace Project_Redmil_MVC.Controllers
{
    public class AboutUsController : Controller
    {
        private readonly string Baseurl;
        private readonly IConfiguration _config;
        public AboutUsController(IConfiguration config)
        {
            _config = config;
            Baseurl = HelperMethod.GetBaseURl(_config);
        }
        public IActionResult AboutUs()
        {
            return View();
        }
        
    }
}
