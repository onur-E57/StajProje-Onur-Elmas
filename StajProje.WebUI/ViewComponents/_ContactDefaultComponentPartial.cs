using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StajProje.WebUI.Dtos.ContactDtos;

namespace StajProje.WebUI.ViewComponents
{
    public class _ContactDefaultComponentPartial : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public _ContactDefaultComponentPartial(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://localhost:7143/api/Contacts/");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();

                var values = JsonConvert.DeserializeObject<List<ResultContactDto>>(jsonData);

                return View(values);
            }
            return View();
        }
    }
}
