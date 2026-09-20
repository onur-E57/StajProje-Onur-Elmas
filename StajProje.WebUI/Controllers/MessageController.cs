using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StajProje.WebUI.Dtos.MessageDtos;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Text.Json;

namespace StajProje.WebUI.Controllers
{
    public class MessageController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        public MessageController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> MessageList()
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync("https://localhost:7143/api/Messages");
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultMessageDto>>(jsonData);
                return View(values);
            }
            return View(new List<ResultMessageDto>());
        }

        [HttpGet]
        public IActionResult CreateMessage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(CreateMessageDto createMessageDto)
        {
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(createMessageDto);
            StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var responseMessage = await client.PostAsync("https://localhost:7143/api/Messages", stringContent);

            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Yeni Mesaj başarıyla eklendi!";
                return RedirectToAction("MessageList");
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.DeleteAsync($"https://localhost:7143/api/Messages?id={id}");

            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Mesaj silme işlemi başarıyla gerçekleştirildi!";
                return RedirectToAction("MessageList");
            }
            return View(new List<ResultMessageDto>());
        }

        [HttpGet]
        public async Task<IActionResult> UpdateMessage(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync($"https://localhost:7143/api/Messages/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<GetByIdMessageDto>(jsonData);
                return View(value);
            }
            return RedirectToAction("MessageList");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateMessage(UpdateMessageDto updateMessageDto)
        {
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(updateMessageDto);
            StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var responseMessage = await client.PutAsync("https://localhost:7143/api/Messages/", stringContent);

            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Mesaj başarıyla güncellendi!";
                return RedirectToAction("MessageList");
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AnswerMessageWithGemini(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync($"https://localhost:7143/api/Messages/{id}");

            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<GetByIdMessageDto>(jsonData);

                var apiKey = _configuration["GeminiConfig:ApiKey"];
                using var geminiClient = new HttpClient();

                var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3.5-flash-lite:generateContent?key={apiKey}";

                string aiPrompt = $"Sen Yummy Restoran'ın profesyonel müşteri ilişkileri yöneticisisin. Müşterimiz {value.NameSurname} bize '{value.Subject}' konusunda şu mesajı gönderdi: '{value.MessageDetails}'. Bu müşteriye kibar, kurumsal, çözüm odaklı ve restoranımızın kalitesini yansıtan bir e-posta yanıtı hazırla. Başka hiçbir açıklama yapma, sadece e-posta metnini ver.";

                var requestData = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[] { new { text = aiPrompt } }
                        }
                    }
                };

                var response = await geminiClient.PostAsJsonAsync(url, requestData);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<GeminiResponse>();
                    var aiAnswer = result?.candidates?[0]?.content?.parts?[0]?.text;

                    ViewBag.AiAnswer = aiAnswer;
                }
                else
                {
                    ViewBag.AiAnswer = "Yapay zeka şu an yanıt veremiyor, lütfen cevabı manuel olarak doldurun.";
                }
                return View(value);
            }
            return RedirectToAction("MessageList");
        }
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

        [HttpPost]
        public async Task<IActionResult> AnswerMessageWithGemini(int MessageId, string aiAnswerText, string Email, string Subject)
        {
            try
            {
                string smtpServer = _configuration["MailConfig:SmtpServer"];
                int port = Convert.ToInt32(_configuration["MailConfig:Port"]);
                string senderEmail = _configuration["MailConfig:SenderEmail"];
                string senderPassword = _configuration["MailConfig:SenderPassword"];

                SmtpClient smtpClient = new SmtpClient(smtpServer, port);
                smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);
                smtpClient.EnableSsl = true;

                // 2. Mail İçeriğini Hazırlama
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(senderEmail, "Yummy Restoran");
                mailMessage.To.Add(Email);
                mailMessage.Subject = "RE: " + Subject;
                mailMessage.Body = aiAnswerText;
                mailMessage.IsBodyHtml = false;

                // 3. Maili Gönder
                await smtpClient.SendMailAsync(mailMessage);

                // --- 4. API'YE İSTEK ATIP "ISREAD" DURUMUNU TRUE YAPMA ---
                var client = _httpClientFactory.CreateClient();

                // Önce mesajın mevcut halini veritabanından çekiyoruz
                var getResponse = await client.GetAsync($"https://localhost:7143/api/Messages/{MessageId}");

                if (getResponse.IsSuccessStatusCode)
                {
                    var jsonData = await getResponse.Content.ReadAsStringAsync();

                    // Gelen veriyi güncelleme DTO'suna çeviriyoruz
                    var updateMessageDto = JsonConvert.DeserializeObject<UpdateMessageDto>(jsonData);

                    // Mesajı okundu olarak işaretliyoruz
                    updateMessageDto.IsRead = true;

                    // Güncel veriyi tekrar JSON'a çevirip API'ye PUT isteği atıyoruz
                    var updateJson = JsonConvert.SerializeObject(updateMessageDto);
                    var stringContent = new StringContent(updateJson, Encoding.UTF8, "application/json");

                    await client.PutAsync("https://localhost:7143/api/Messages/", stringContent);
                }
                TempData["SuccessMessage"] = "Yapay zeka yanıtı müşteriye başarıyla iletildi ve mesaj okundu işaretlendi!";
            }
            catch (Exception ex)
            {
                TempData["SuccessMessage"] = "Mail gönderilemedi! Hata: " + ex.Message;
            }

            return RedirectToAction("MessageList");
        }
        
        [HttpGet]
        public PartialViewResult SendMessage()
        {
            return PartialView();
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> SendMessage(CreateMessageDto createMessageDto)
        {
            createMessageDto.SendDate = DateTime.Now;
            createMessageDto.IsRead = false;
            // 1. Dış Dünyaya (Hugging Face) gidecek olan kurye
            var aiClient = _httpClientFactory.CreateClient();

            var token = _configuration["HuggingFaceConfig:ApiKey"];
            aiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                // Direkt Türkçe Müşteri Memnuniyeti (Duygu Analizi) Modeli
                var sentimentRequestBody = new { inputs = createMessageDto.MessageDetails };
                var sentimentJson = System.Text.Json.JsonSerializer.Serialize(sentimentRequestBody);
                var sentimentContent = new StringContent(sentimentJson, Encoding.UTF8, "application/json");

                var response = await aiClient.PostAsync("https://api-inference.huggingface.co/models/savasy/bert-base-turkish-sentiment-cased", sentimentContent);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode) // API başarılı yanıt (200 OK) döndüyse
                {
                    if (responseString.TrimStart().StartsWith("["))
                    {
                        var doc = JsonDocument.Parse(responseString);
                        var rootArray = doc.RootElement;

                        // Modelin iç içe (nested) dizi dönme ihtimaline karşı güvenlik kontrolü
                        if (rootArray.GetArrayLength() > 0 && rootArray[0].ValueKind == JsonValueKind.Array)
                        {
                            rootArray = rootArray[0];
                        }

                        // Güvenli okuma: Patlamayı önlemek için TryGetProperty kullanıyoruz
                        foreach (var item in rootArray.EnumerateArray())
                        {
                            if (item.TryGetProperty("label", out var labelProp) && item.TryGetProperty("score", out var scoreProp))
                            {
                                string label = labelProp.GetString();
                                double score = scoreProp.GetDouble();

                                if ((label == "negative" || label == "LABEL_0") && score > 0.5)
                                {
                                    createMessageDto.Status = "Şikayet / İnceleme Bekliyor";
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    // Eğer model uykudaysa (503) veya Token yanlışsa (401), hatayı veritabanına yaz!
                    createMessageDto.Status = $"HF Reddedildi: {response.StatusCode}";
                }

                if (string.IsNullOrEmpty(createMessageDto.Status))
                {
                    createMessageDto.Status = "Standart (Olumlu)";
                }
            }
            catch (Exception ex)
            {
                // C# tarafında başka bir Exception patlarsa, tam hatayı DB'ye yazdıralım ki katili bulalım!
                string errorMsg = ex.Message;
                if (errorMsg.Length > 50) errorMsg = errorMsg.Substring(0, 50); // DB kolonuna sığması için kısaltıyoruz
                createMessageDto.Status = $"Kod Hatası: {errorMsg}";
            }

            // 2. Kendi API'ne (Yummy Veritabanına) gidecek olan kurye
            var apiClient = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(createMessageDto);
            StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var responseMessage = await apiClient.PostAsync("https://localhost:7143/api/Messages", stringContent);

            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Mesajınız başarıyla gönderildi, en kısa sürede dönüş yapacağız!";
                return RedirectToAction("Index", "Default");
            }
            return View();
        }
    }
}