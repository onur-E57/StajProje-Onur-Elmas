using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;

namespace StajProje.WebUI.ViewComponents
{
    public class _StatsDefaultComponentPartial : ViewComponent
    {
        public IHttpClientFactory _httpClientFactory;

        public _StatsDefaultComponentPartial(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://localhost:7143/api/Stats/ChefCount");
            var jsonData = await response.Content.ReadAsStringAsync();
            ViewBag.v1 = jsonData;

            var client2 = _httpClientFactory.CreateClient();
            var response2 = await client2.GetAsync("https://localhost:7143/api/Stats/ProductCount");
            var jsonData2 = await response2.Content.ReadAsStringAsync();
            ViewBag.v2 = jsonData2;


            var client3 = _httpClientFactory.CreateClient();
            var response3 = await client3.GetAsync("https://localhost:7143/api/Stats/ReservationCount");
            var jsonData3 = await response3.Content.ReadAsStringAsync();
            ViewBag.v3 = jsonData3; 


            var client4 = _httpClientFactory.CreateClient();
            var response4 = await client4.GetAsync("https://localhost:7143/api/Stats/MessageCount");
            var jsonData4 = await response4.Content.ReadAsStringAsync();
            ViewBag.v4 = jsonData4;
            return View();
        }
    }
}
