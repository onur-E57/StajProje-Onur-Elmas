using Microsoft.AspNetCore.Mvc;

namespace StajProje.WebUI.ViewComponents
{
    public class _ReservationDefaultComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
