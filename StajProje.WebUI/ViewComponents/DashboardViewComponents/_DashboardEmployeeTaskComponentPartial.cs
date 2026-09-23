using Microsoft.AspNetCore.Mvc;

namespace StajProje.WebUI.ViewComponents.DashboardViewComponents
{
    public class _DashboardEmployeeTaskComponentPartial : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public _DashboardEmployeeTaskComponentPartial(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public IViewComponentResult Invoke()
        {
            return View();
        }

    }
}
