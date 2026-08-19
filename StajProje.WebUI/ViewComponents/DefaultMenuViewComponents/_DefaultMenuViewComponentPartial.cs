using Microsoft.AspNetCore.Mvc;

namespace StajProje.WebUI.ViewComponents.DefaultMenuViewComponentPartial
{
    public class _DefaultMenuViewComponentPartial:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
