using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StajProje.WebUI.Dtos.CategoryDtos;
using System.Text;

namespace StajProje.WebUI.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CategoryController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> CategoryList()
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync("https://localhost:7143/api/Categories");
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultCategoryDto>>(jsonData);
                return View(values);
            }
            return View(new List<ResultCategoryDto>());
        }
        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryDto createCategoryDto)
        {
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(createCategoryDto);
            StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var responseMessage = await client.PostAsync("https://localhost:7143/api/Categories", stringContent);

            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Yeni kategori başarıyla eklendi!";
                return RedirectToAction("CategoryList");
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.DeleteAsync($"https://localhost:7143/api/Categories?id={id}");

            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Kategori silme işlemi başarıyla gerçekleştirildi!";
                return RedirectToAction("CategoryList");
            }
            return View(new List<ResultCategoryDto>());
        }

        [HttpGet]
        public async Task<IActionResult> UpdateCategory(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync($"https://localhost:7143/api/Categories/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<GetCategoryByIdDto>(jsonData);
                return View(value);
            }
            return RedirectToAction("CategoryList");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCategory(UpdateCategoryDto updateCategoryDto)
        {
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(updateCategoryDto);
            StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var responseMessage = await client.PutAsync("https://localhost:7143/api/Categories/", stringContent);

            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Kategori başarıyla güncellendi!";
                return RedirectToAction("CategoryList");
            }
            return View();
        }
    }
}