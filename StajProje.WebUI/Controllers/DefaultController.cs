using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StajProje.WebUI.Dtos.MessageDtos;
using System.Text;

namespace StajProje.WebUI.Controllers
{
    public class DefaultController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        // API'ye istek atabilmek için constructor (yapıcı metot) ile tanımlıyoruz
        public DefaultController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        // İŞTE FORMUN YAKALANDIĞI YER BURASI
        [HttpPost]
        public async Task<IActionResult> SendMessage(CreateMessageDto createMessageDto)
        {
            // Arka planda tarih ve okunma durumunu set ediyoruz
            createMessageDto.SendDate = DateTime.Now;
            createMessageDto.IsRead = false;

            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(createMessageDto);
            StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

            // Veriyi API'deki MessageController'ına (mutfağa) gönderiyoruz
            var responseMessage = await client.PostAsync("https://localhost:7143/api/Messages", stringContent);

            if (responseMessage.IsSuccessStatusCode)
            {
                // Başarılı olursa sitenin ana sayfasına (Index) geri döner
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }
    }
}