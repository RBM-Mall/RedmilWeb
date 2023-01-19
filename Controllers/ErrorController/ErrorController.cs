using Microsoft.AspNetCore.Mvc;

namespace Project_Redmil_MVC.Controllers.ErrorController
{
    public class ErrorController : Controller
    {
        public IActionResult ErrorHandle()
        {
            return View();
        }
        public IActionResult ErrorForLogin()
        {
            return View();
        }
        public IActionResult ErrorForExceptionLog()
        {
            return View();  
        }
        public IActionResult ErrorForNullHandle()
        {
            return View();
        }
    }
}
