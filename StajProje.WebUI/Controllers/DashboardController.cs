using Microsoft.AspNetCore.Mvc;

namespace StajProje.WebUI.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
