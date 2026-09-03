using Microsoft.AspNetCore.Mvc;

namespace StajProje.WebUI.ViewComponents
{
    public class _MessageDefaultComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
