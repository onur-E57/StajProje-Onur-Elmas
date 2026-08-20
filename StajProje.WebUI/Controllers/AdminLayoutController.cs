using Microsoft.AspNetCore.Mvc;

namespace StajProje.WebUI.Controllers
{
    public class AdminLayoutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
