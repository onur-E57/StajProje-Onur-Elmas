using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using StajProje.WebUI.Dtos.CategoryDtos;
using StajProje.WebUI.Dtos.ProductDtos;
using System.Text;

namespace StajProje.WebUI.Controllers
{
    public class ProductController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> ProductList()
        {
            var client = _httpClientFactory.CreateClient();

            // API'deki kategorili listeyi getiren adrese gidiyoruz
            var responseMessage = await client.GetAsync("https://localhost:7143/api/Products/ProductListWithCategory");

            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();

                // Gelen veriyi İÇİNDE KATEGORİ ADI OLAN Dto'muza dönüştürüyoruz
                var values = JsonConvert.DeserializeObject<List<ResultProductWithCategoryDto>>(jsonData);

                return View(values);
            }

            // Hata durumunda dönecek boş listeyi de güncelledik
            return View(new List<ResultProductWithCategoryDto>());
        } 
        [HttpGet]
        public async Task<IActionResult> CreateProduct()
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync("https://localhost:7143/api/Categories");

            var jsonData = await responseMessage.Content.ReadAsStringAsync();
            var values = JsonConvert.DeserializeObject<List<ResultCategoryDto>>(jsonData);
            List<SelectListItem> categoryItems = (from x in values
                                                   select new SelectListItem
                                                   {
                                                       Text = x.CategoryName,
                                                       Value = x.CategoryId.ToString()
                                                   }).ToList();
            ViewBag.CategoryItems = categoryItems;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(CreateProductDto createProductDto)
        {
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(createProductDto);
            StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var responseMessage = await client.PostAsync("https://localhost:7143/api/Products", stringContent);

            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Yeni Ürün başarıyla eklendi!";
                return RedirectToAction("ProductList");
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.DeleteAsync($"https://localhost:7143/api/Products/{id}");

            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Ürün silme işlemi başarıyla gerçekleştirildi!";
                return RedirectToAction("ProductList");
            }
            return RedirectToAction("ProductList");
        }

        [HttpGet]
        public async Task<IActionResult> UpdateProduct(int id)
        {
            var client = _httpClientFactory.CreateClient();

            // 1. AŞAMA: Dropdown için Kategorileri Çekip ViewBag'e Dolduruyoruz
            var categoryResponseMessage = await client.GetAsync("https://localhost:7143/api/Categories");
            if (categoryResponseMessage.IsSuccessStatusCode)
            {
                var categoryJsonData = await categoryResponseMessage.Content.ReadAsStringAsync();
                var categoryValues = JsonConvert.DeserializeObject<List<ResultCategoryDto>>(categoryJsonData);

                List<SelectListItem> categoryItems = (from x in categoryValues
                                                      select new SelectListItem
                                                      {
                                                          Text = x.CategoryName,
                                                          Value = x.CategoryId.ToString()
                                                      }).ToList();
                ViewBag.CategoryItems = categoryItems;
            }

            // 2. AŞAMA: Güncellenecek Ürünün Bilgilerini Çekiyoruz (Senin yazdığın kısım)
            var responseMessage = await client.GetAsync($"https://localhost:7143/api/Products/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<GetProductByIdDto>(jsonData);

                // Ürün bilgilerini Model olarak, Kategorileri de ViewBag olarak View'a gönderiyoruz!
                return View(value);
            }

            return RedirectToAction("ProductList");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProduct(UpdateProductDto updateProductDto)
        {
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(updateProductDto);
            StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var responseMessage = await client.PutAsync("https://localhost:7143/api/Products/", stringContent);

            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Ürün başarıyla güncellendi!";
                return RedirectToAction("ProductList");
            }
            return View();
        }
    }
}
