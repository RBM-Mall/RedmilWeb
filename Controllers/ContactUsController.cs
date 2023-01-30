using Microsoft.AspNetCore.Mvc;
using Project_Redmil_MVC.CommonHelper;

namespace Project_Redmil_MVC.Controllers
{
    public class ContactUsController : Controller
    {

        private readonly string Baseurl;
        private readonly IConfiguration _config;
        public ContactUsController(IConfiguration config)
        {
            _config = config;
            Baseurl = HelperMethod.GetBaseURl(_config);
        }
        public IActionResult ContactUs()
        {
            return View();
        }
    }
}
