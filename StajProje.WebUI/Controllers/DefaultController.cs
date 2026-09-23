using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StajProje.WebUI.Dtos.MessageDtos;
using StajProje.WebUI.Dtos.ReservationDtos;
using System.Text;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace StajProje.WebUI.Controllers
{
    public class DefaultController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        // API'ye istek atabilmek için constructor (yapıcı metot) ile tanımlıyoruz
        public DefaultController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        /* İŞTE FORMUN YAKALANDIĞI YER BURASI
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
                // --- Restorana E-Posta Bildirimi Gönderme İşlemi Başlangıcı ---
                try
                {
                    string smtpServer = _configuration["MailConfig:SmtpServer"];
                    int port = Convert.ToInt32(_configuration["MailConfig:Port"]);
                    string senderEmail = _configuration["MailConfig:SenderEmail"];
                    string senderPassword = _configuration["MailConfig:SenderPassword"];

                    SmtpClient smtpClient = new SmtpClient(smtpServer, port);
                    smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);
                    smtpClient.EnableSsl = true;

                    MailMessage mailMessage = new MailMessage();

                    mailMessage.From = new MailAddress(senderEmail, "Yummy Web İletişim Formu");
                    mailMessage.To.Add(senderEmail);
                    mailMessage.Subject = "YENİ MÜŞTERİ MESAJI: " + createMessageDto.Subject;

                    // Mail içeriğine senin eklediğin SendDate (Tarih) verisini de ekledik
                    mailMessage.Body = $"Web sitenizden yeni bir iletişim formu dolduruldu.\n\n" +
                                       $"Tarih: {createMessageDto.SendDate}\n" +
                                       $"Müşteri Adı: {createMessageDto.NameSurname}\n" +
                                       $"E-Posta: {createMessageDto.Email}\n" +
                                       $"Konu: {createMessageDto.Subject}\n\n" +
                                       $"Mesaj Detayı:\n{createMessageDto.MessageDetails}";
                    mailMessage.IsBodyHtml = false;

                    await smtpClient.SendMailAsync(mailMessage);
                }
                catch (Exception)
                {
                    // Mail gönderiminde Google kaynaklı bir anlık kesinti olursa 
                    // veritabanı kaydı başarılı olduğu için sistemi çökertmeyip hatayı yutuyoruz.
                }
                // --- Restorana E-Posta Bildirimi Gönderme İşlemi Bitişi ---

                TempData["SuccessMessage"] = "Mesajınız başarıyla gönderildi! Sizi aramızda görmek için sabırsızlanıyoruz.";
            }
            else
            {
                TempData["SuccessMessage"] = "Mesaj gönderilirken bir hata oluştu, lütfen tekrar deneyin.";
            }

            return RedirectToAction("Index", "Default");
        }
        */
        
        /*
        [HttpPost]
        public async Task<IActionResult> BookTable(CreateReservationDto createReservationDto)
        {
            // Arka planda tarih ve okunma durumunu set ediyoruz
            createReservationDto.ReservationDate = DateTime.Now;
            createReservationDto.ReservationStatus = "Beklemede";

            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(createReservationDto);
            StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

            // Veriyi API'deki ReservationController'ına (mutfağa) gönderiyoruz
            var responseMessage = await client.PostAsync("https://localhost:7143/api/Reservations", stringContent);

            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["ReservationSuccess"] = "Masanız başarıyla ayrıldı! Sizi aramızda görmek için sabırsızlanıyoruz.";
            }
            else
            {
                TempData["ReservationSuccess"] = "Rezervasyon alınırken bir hata oluştu, lütfen tekrar deneyin.";
            }

            return RedirectToAction("Index", "Default");
        }
        */
    }
}