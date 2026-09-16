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
        public async Task<IActionResult> SendMessage(CreateMessageDto createMessageDto)
        {
            // 1. Dış Dünyaya (Hugging Face) gidecek olan kurye
            var aiClient = _httpClientFactory.CreateClient();
            var token = _configuration["HuggingFaceConfig:ApiKey"];
            aiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var translateRequestBody = new
                {
                    inputs = createMessageDto.MessageDetails
                };
                var translateJson = System.Text.Json.JsonSerializer.Serialize(translateRequestBody);
                var translateContent = new StringContent(translateJson, Encoding.UTF8, "application/json");
                var translateResponse = await aiClient.PostAsync("https://api-inference.huggingface.co/models/Helsinki-NLP/opus-mt-tr-en", translateContent);
                var translateResponseString = await translateResponse.Content.ReadAsStringAsync();

                string englishText = createMessageDto.MessageDetails;

                if (translateResponseString.TrimStart().StartsWith("["))
                {
                    var translateDoc = JsonDocument.Parse(translateResponseString);
                    englishText = translateDoc.RootElement[0].GetProperty("translation_text").GetString();
                }

                // 🕵️‍♂️ DEDEKTİF KONTROL NOKTASI 1: Çeviri işleminden hemen sonra, toksik sorgusundan önce!
                Console.WriteLine("=========== DEDEKTİF RAPORU 1 ===========");
                Console.WriteLine("Orijinal Mesaj: " + createMessageDto.MessageDetails);
                Console.WriteLine("Çeviri Sonucu: " + englishText);
                Console.WriteLine("=========================================");

                var toxicityRequestBody = new
                {
                    inputs = englishText
                };
                var toxicityJson = System.Text.Json.JsonSerializer.Serialize(toxicityRequestBody);
                var toxicityContent = new StringContent(toxicityJson, Encoding.UTF8, "application/json");
                var toxicityResponse = await aiClient.PostAsync("https://api-inference.huggingface.co/models/unitary/toxic-bert", toxicityContent);

                // API'nin ham cevabını string olarak alıyoruz
                var toxicityResponseString = await toxicityResponse.Content.ReadAsStringAsync();

                // 🕵️‍♂️ DEDEKTİF KONTROL NOKTASI 2: Ham cevabı aldığımız gibi, FOREACH'E GİRMEDEN ÖNCE ekrana basıyoruz!
                Console.WriteLine("=========== DEDEKTİF RAPORU 2 ===========");
                Console.WriteLine("Toksik API Ham Cevabı: " + toxicityResponseString);
                Console.WriteLine("=========================================");

                if (toxicityResponseString.TrimStart().StartsWith("["))
                {
                    var toxicityDoc = JsonDocument.Parse(toxicityResponseString);
                    foreach (var item in toxicityDoc.RootElement[0].EnumerateArray())
                    {
                        string label = item.GetProperty("label").GetString();
                        double score = item.GetProperty("score").GetDouble();
                        if (score > 0.5)
                        {
                            createMessageDto.Status = "Toksik";
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Yapay zeka (Hugging Face) servisine ulaşılamadı: " + ex.Message);
            }

            // --- GARANTİ BÖLGE: TRY-CATCH'TEN KURTULUP BURAYA GELDİK ---
            // Eğer HuggingFace patladıysa veya mesaj temizse (Toksik değilse), Status null kalmasın:
            if (string.IsNullOrEmpty(createMessageDto.Status))
            {
                createMessageDto.Status = "Standart";
            }
            // ------------------------------------------------------------

            createMessageDto.IsRead = false;
            createMessageDto.SendDate = DateTime.Now;

            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(createMessageDto);
            StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

            // Veriyi API'deki MessageController'ına (mutfağa) gönderiyoruz
            var responseMessage = await client.PostAsync("https://localhost:7143/api/Messages", stringContent);

            if (responseMessage.IsSuccessStatusCode)
            {
                // --- MAİL GÖNDERME İŞLEMİ ---
                try
                {
                    string smtpServer = _configuration["MailConfig:SmtpServer"];
                    int port = Convert.ToInt32(_configuration["MailConfig:Port"]);
                    string senderEmail = _configuration["MailConfig:SenderEmail"];
                    string senderPassword = _configuration["MailConfig:SenderPassword"];

                    using (SmtpClient smtpClient = new SmtpClient(smtpServer, port))
                    {
                        smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);
                        smtpClient.EnableSsl = true;

                        MailMessage mailMessage = new MailMessage();
                        mailMessage.From = new MailAddress(senderEmail, "Yummy Web İletişim Formu");
                        mailMessage.To.Add(senderEmail); // Restoranın kendi mailine bildirim
                        mailMessage.Subject = "YENİ MÜŞTERİ MESAJI: " + createMessageDto.Subject;
                        mailMessage.Body = $"Web sitenizden yeni bir iletişim formu dolduruldu.\n\n" +
                                           $"Müşteri Adı: {createMessageDto.NameSurname}\n" +
                                           $"E-Posta: {createMessageDto.Email}\n" +
                                           $"Konu: {createMessageDto.Subject}\n\n" +
                                           $"Mesaj Detayı:\n{createMessageDto.MessageDetails}";
                        mailMessage.IsBodyHtml = false;

                        await smtpClient.SendMailAsync(mailMessage);
                    }
                }
                catch (Exception)
                {
                    // Google SMTP anlık hata verirse sistem çökmesin diye hatayı eziyoruz
                }
                // ---------------------------------------------

                TempData["MessageSuccess"] = "Mesajınız başarıyla gönderildi! Sizi aramızda görmek için sabırsızlanıyoruz.";
            }
            else
            {
                // API'nin gönderdiği gerçek hata mesajını okuyoruz
                var errorDetail = await responseMessage.Content.ReadAsStringAsync();

                // Hatayı ekrana basıyoruz ki katilin kim olduğunu görelim
                TempData["MessageSuccess"] = $"API Hata Kodu: {responseMessage.StatusCode} | Detay: {errorDetail}";
            }

            return RedirectToAction("Index", "Default");
        }
    }
}