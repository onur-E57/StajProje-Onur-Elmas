using Microsoft.AspNetCore.Mvc;

namespace StajProje.WebUI.ViewComponents
{
    public class _HeadDefaultComponentPartial:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
