using Microsoft.AspNetCore.Mvc;

namespace Project_Redmil_MVC.Controllers.ServiceNavigationController
{
    public class BankingServicesController : Controller
    {
        public IActionResult BankingServices()
        {
            return View();
        }
    }
}
