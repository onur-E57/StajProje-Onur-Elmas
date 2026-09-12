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

            // 1. Müşteri (UI) Tarafı İçin Yapay Zeka Kısıtlamaları (Guardrails)
            string systemPrompt = @"Sen Yummy Restoran'ın profesyonel yapay zeka şefisin. SADECE mutfak, yemek tarifleri, malzemeler, pişirme teknikleri ve restoranımız hakkında konuşabilirsin. 
            KESİN KURALLAR:
            1. Kullanıcı sana yazılım (C#, AutoMapper, React vb.), teknoloji, siyaset, tarih, matematik veya mutfak dışı herhangi bir konu sorarsa KESİNLİKLE cevap verme.
            2. Mutfak dışı konularda rol yapmaya veya soruyu yemeğe bağlamaya çalışma.
            3. Böyle bir durumda sadece şunu söyle: 'Ben sadece mutfaktan sorumlu bir aşçıyım kanka, kodlardan veya o dediklerinden hiç anlamam! Bana dolabındaki malzemeleri söyle, sana harika bir yemek yapayım.'";

            var requestData = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = systemPrompt },
                            new { text = dto.Prompt } // DTO'dan gelen veri, altı kızarmaz.
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

            // 2. Admin Paneli İçin Yapay Zeka Kısıtlamaları (Guardrails)
            string systemPrompt = @"Sen Yummy Restoran'ın profesyonel yapay zeka şefisin. SADECE mutfak, yemek tarifleri, malzemeler, pişirme teknikleri ve restoranımız hakkında konuşabilirsin. 
            KESİN KURALLAR:
            1. Kullanıcı sana yazılım (C#, AutoMapper, React vb.), teknoloji, siyaset, tarih, matematik veya mutfak dışı herhangi bir konu sorarsa KESİNLİKLE cevap verme.
            2. Mutfak dışı konularda rol yapmaya veya soruyu yemeğe bağlamaya çalışma.
            3. Böyle bir durumda sadece şunu söyle: 'Ben sadece mutfaktan sorumlu bir aşçıyım kanka, kodlardan veya o dediklerinden hiç anlamam! Bana dolabındaki malzemeleri söyle, sana harika bir yemek yapayım.'";

            var requestData = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = systemPrompt },
                            new { text = prompt }
                        }
                    }
                }
            };

            var response = await client.PostAsJsonAsync(url, requestData);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GeminiResponse>();
                ViewBag.recipe = result?.candidates?[0]?.content?.parts?[0]?.text;
            }
            else
            {
                ViewBag.recipe = "Şef şu anda meşgul, lütfen biraz sonra tekrar deneyin.";
            }

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