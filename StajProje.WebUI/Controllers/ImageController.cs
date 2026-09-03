using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StajProje.WebUI.Dtos.ImageDtos;
using System.Text;

namespace StajProje.WebUI.Controllers
{
    public class ImageController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ImageController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> ImageList()
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync("https://localhost:7143/api/Images");
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultImageDto>>(jsonData);
                return View(values);
            }
            return View(new List<ResultImageDto>());
        }
        [HttpGet]
        public IActionResult CreateImage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateImage(CreateImageDto createImageDto)
        {
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(createImageDto);
            StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var responseMessage = await client.PostAsync("https://localhost:7143/api/Images", stringContent);

            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Yeni Görsel başarıyla eklendi!";
                return RedirectToAction("ImageList");
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.DeleteAsync($"https://localhost:7143/api/Images?id={id}");

            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Görsel silme işlemi başarıyla gerçekleştirildi!";
                return RedirectToAction("ImageList");
            }
            return View(new List<ResultImageDto>());
        }

        [HttpGet]
        public async Task<IActionResult> UpdateImage(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync($"https://localhost:7143/api/Images/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<GetByIdImageDto>(jsonData);
                return View(value);
            }
            return RedirectToAction("ImageList");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateImage(UpdateImageDto updateImageDto)
        {
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(updateImageDto);
            StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var responseMessage = await client.PutAsync("https://localhost:7143/api/Images/", stringContent);

            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Görsel başarıyla güncellendi!";
                return RedirectToAction("ImageList");
            }
            return View();
        }
    }
}