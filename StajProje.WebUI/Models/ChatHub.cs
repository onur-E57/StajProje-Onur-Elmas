using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace StajProje.WebUI.Models
{
    public class ChatHub : Hub
    {
        private readonly IConfiguration _configuration;
        private readonly string _apiKey;
        private readonly IHttpClientFactory _httpClientFactory;

        public ChatHub(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _apiKey = _configuration["GeminiConfig:ApiKey"];
            _httpClientFactory = httpClientFactory;
        }

        private static readonly Dictionary<string, List<GeminiContent>> _history = new();

        // Temel rolümüz
        private const string BaseSystemInstruction = @"Sen Yummy Restoran'ın profesyonel yapay zeka asistanısın. Müşterilere kibar ve net cevaplar ver. 
                                                    KESİN KURALLAR: 
                                                                    1. Kullanıcı sana yazılım (C#, AutoMapper, React vb.), teknoloji, siyaset, tarih, matematik veya restoran dışı herhangi bir konu sorarsa KESİNLİKLE cevap verme. 
                                                                    2. Mutfak dışı konularda rol yapmaya veya soruyu yemeğe bağlamaya çalışma. 
                                                                    3. Böyle bir durumda sadece şunu söyle: 'Ben sadece canlı destek asistanıyım, kodlardan veya o dediklerinden hiç anlamam! Restoranımızla ilgili bilgileri sana verebilirim.'";

        public override Task OnConnectedAsync()
        {
            _history[Context.ConnectionId] = new List<GeminiContent>();
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            _history.Remove(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }

        //DB'den RAG verisini çekip Gemini'ye fısıldayacak metot
        private async Task<string> GetMenuDataFromDatabaseAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                // 1. Kategorileri Çekiyoruz
                var catResponse = await client.GetAsync("https://localhost:7143/api/Categories");
                var categories = new List<CategoryDto>();
                if (catResponse.IsSuccessStatusCode)
                {
                    var catJson = await catResponse.Content.ReadAsStringAsync();
                    categories = JsonSerializer.Deserialize<List<CategoryDto>>(catJson, options) ?? new List<CategoryDto>();
                }

                // 2. Ürünleri Çekiyoruz
                var prodResponse = await client.GetAsync("https://localhost:7143/api/Products");

                if (prodResponse.IsSuccessStatusCode)
                {
                    var prodJson = await prodResponse.Content.ReadAsStringAsync();
                    var products = JsonSerializer.Deserialize<List<ProductDto>>(prodJson, options);

                    if (products != null && products.Any())
                    {
                        var menuBuilder = new StringBuilder();
                        menuBuilder.AppendLine("İŞTE RESTORANIMIZIN GÜNCEL MENÜSÜ, KATEGORİLERİ VE İÇERİKLERİ:");

                        // 3. Ürünleri CategoryId'ye göre grupluyoruz
                        var groupedProducts = products.GroupBy(p => p.CategoryId);

                        foreach (var group in groupedProducts)
                        {
                            // Kategori adını Categories listesinden buluyoruz
                            var categoryName = categories.FirstOrDefault(c => c.CategoryId == group.Key)?.CategoryName ?? "Diğer Lezzetlerimiz";

                            menuBuilder.AppendLine($"\n[{categoryName.ToUpper()}] KATEGORİSİ:");

                            foreach (var item in group)
                            {
                                // Hem ürün adını, hem içeriğini, hem fiyatını veriyoruz
                                menuBuilder.AppendLine($"- {item.ProductName} (İçindekiler: {item.ProductDescription}) : {item.ProductPrice} TL");
                            }
                        }

                        menuBuilder.AppendLine("\nÖNEMLİ KURAL: Müşteriye ürün önerirken SADECE YUKARIDAKİ GERÇEK MENÜYÜ kullan. Müşteri içerik veya alerjen sorarsa parantez içindeki 'İçindekiler' bilgisini baz al.");
                        return menuBuilder.ToString();
                    }
                }
            }
            catch
            {
                // Hata olursa sessizce geç
            }

            return "Şu an güncel menü veritabanından çekilemedi.";
        }

        public async Task SendMessage(string userMessage)
        {
            await Clients.Caller.SendAsync("ReceiveUserEcho", userMessage);
            var history = _history[Context.ConnectionId];

            history.Add(new GeminiContent
            {
                Role = "user",
                Parts = new List<GeminiPart> { new GeminiPart { Text = userMessage } }
            });

            await StreamGemini(history, Context.ConnectionAborted);
        }

        public async Task StreamGemini(List<GeminiContent> history, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                await Clients.Caller.SendAsync("ReceiveToken", "⚠️ Sistem Hatası: API Anahtarı bulunamadı.", cancellationToken);
                await Clients.Caller.SendAsync("CompleteMessage", "", cancellationToken);
                return;
            }

            // 1. Veritabanından RAG verisini çek
            string menuContext = await GetMenuDataFromDatabaseAsync();

            // 2. Base Instruction ile RAG Verisini birleştirip Gemini'ye fısılda
            string dynamicSystemInstruction = $"{BaseSystemInstruction}\n\n{menuContext}";

            var client = _httpClientFactory.CreateClient();
            var url = $"https://generativelanguage.googleapis.com/v1/models/gemini-3.5-flash-lite:streamGenerateContent?alt=sse&key={_apiKey}";

            var payload = new
            {
                system_instruction = new
                {
                    parts = new { text = dynamicSystemInstruction } // Dinamik RAG kuralımızı buraya gömdük
                },
                contents = history,
                generationConfig = new { temperature = 0.2 }
            };

            using var req = new HttpRequestMessage(HttpMethod.Post, url);
            req.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            using var resp = await client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (!resp.IsSuccessStatusCode)
            {
                await Clients.Caller.SendAsync("ReceiveToken", $"⚠️ Google API Hatası ({resp.StatusCode})", cancellationToken);
                await Clients.Caller.SendAsync("CompleteMessage", "", cancellationToken);
                return;
            }

            using var stream = await resp.Content.ReadAsStreamAsync(cancellationToken);
            using var reader = new StreamReader(stream);
            var sb = new StringBuilder();

            while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
            {
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (!line.StartsWith("data: ")) continue;

                var data = line.Substring(6).Trim();
                try
                {
                    var chunk = JsonSerializer.Deserialize<GeminiStreamChunk>(data);
                    var delta = chunk?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

                    if (!string.IsNullOrEmpty(delta))
                    {
                        sb.Append(delta);
                        await Clients.Caller.SendAsync("ReceiveToken", delta, cancellationToken);
                    }
                }
                catch { }
            }

            var full = sb.ToString();
            history.Add(new GeminiContent
            {
                Role = "model",
                Parts = new List<GeminiPart> { new GeminiPart { Text = full } }
            });

            await Clients.Caller.SendAsync("CompleteMessage", full, cancellationToken);
        }

        // --- Stream Parse Modelleri ---
        public sealed class GeminiStreamChunk { [JsonPropertyName("candidates")] public List<Candidate>? Candidates { get; set; } }
        public sealed class Candidate { [JsonPropertyName("content")] public GeminiContent? Content { get; set; } }
        public sealed class GeminiContent { [JsonPropertyName("role")] public string? Role { get; set; } [JsonPropertyName("parts")] public List<GeminiPart>? Parts { get; set; } }
        public sealed class GeminiPart { [JsonPropertyName("text")] public string? Text { get; set; } }

        // --- RAG İÇİN GÜNCEL DTO'LAR ---
        public class ProductDto
        {
            public string ProductName { get; set; }
            public string ProductDescription { get; set; } // İçindekiler kısmı eklendi
            public decimal ProductPrice { get; set; } // Sende kolon adı ProductPrice
            public int CategoryId { get; set; }
        }

        public class CategoryDto
        {
            public int CategoryId { get; set; }
            public string CategoryName { get; set; } // Kategori API'ndeki isim kolonuna göre
        }
    }
}