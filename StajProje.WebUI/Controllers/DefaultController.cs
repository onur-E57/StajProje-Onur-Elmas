using Microsoft.AspNetCore.Mvc;

namespace StajProje.WebUI.Controllers
{
    public class DefaultController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
