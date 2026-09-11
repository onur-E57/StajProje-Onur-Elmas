using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace StajProje.WebUI.Controllers
{
    public class AIController : Controller
    {
        private readonly IConfiguration _configuration;
        public AIController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult CreateRecipeWithGemini()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRecipeWithGemini([FromBody] PromptDto dto)
        {
            var apiKey = _configuration["GeminiConfig:ApiKey"];
            using var client = new HttpClient();

            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3.5-flash-lite:generateContent?key={apiKey}";

            var requestData = new
            {
                contents = new[]
                {
            new
            {
                parts = new[]
                {
                    new { text = "Sen Yummy Restoran'ın profesyonel yapay zeka şefisin. Kullanıcının elindeki malzemelere göre iştah kabartan, samimi ve pratik bir yemek tarifi öner." },
                    new { text = dto.Prompt }
                }
            }
        }
            };

            var response = await client.PostAsJsonAsync(url, requestData);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GeminiResponse>();
                var content = result?.candidates?[0]?.content?.parts?[0]?.text;
                return Json(new { success = true, recipe = content });
            }

            return Json(new { success = false, message = "Şef şu anda meşgul, lütfen biraz sonra tekrar deneyin." });
        }

        [HttpPost]
        public async Task<IActionResult> CreateRecipeForAdmin(string prompt)
        {
            var apiKey = _configuration["GeminiConfig:ApiKey"];
            using var client = new HttpClient();

            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3.5-flash-lite:generateContent?key={apiKey}";

            var requestData = new
            {
                contents = new[]
                {
            new
            {
                parts = new[]
                {
                    new { text = "Sen Yummy Restoran'ın profesyonel yapay zeka şefisin. Kullanıcının elindeki malzemelere göre iştah kabartan, samimi ve pratik bir yemek tarifi öner." },
                    new { text = prompt } // DTO yerine direkt string prompt alıyoruz
                }
            }
        }
            };

            var response = await client.PostAsJsonAsync(url, requestData);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GeminiResponse>();
                // JSON dönmek yerine View (HTML) sayfasına veri taşıyoruz
                ViewBag.recipe = result?.candidates?[0]?.content?.parts?[0]?.text;
            }
            else
            {
                ViewBag.recipe = "Şef şu anda meşgul, lütfen biraz sonra tekrar deneyin.";
            }

            // Aynı form sayfasına geri dönüp ViewBag ile sonucu ekrana basıyoruz
            return View("CreateRecipeWithGemini");
        }

        public class PromptDto
        {
            public string Prompt { get; set; }
        }

        // --- GEMINI RESPONSE MODEL SINIFLARI ---
        public class GeminiResponse
        {
            public List<Candidate> candidates { get; set; }
        }

        public class Candidate
        {
            public Content content { get; set; }
        }

        public class Content
        {
            public List<Part> parts { get; set; }
        }

        public class Part
        {
            public string text { get; set; }
        }
    }
}