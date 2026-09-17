using StajProje.WebUI.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Projeye HTML (View) desteğini ekliyoruz
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
// SignalR servisini projeye dahil ediyoruz
builder.Services.AddSignalR();

var app = builder.Build();

// 2. Hata ayıklama ve yönlendirme ayarları
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// 3. wwwroot klasöründeki temanın, CSS ve JS'lerin çalışması için kapıyı açıyoruz
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

// 4. Proje açılır açılmaz direkt senin DefaultController -> Index sayfana gitmesini sağlayan rota
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Default}/{action=Index}/{id?}");
// JavaScript'in aradığı /chathub adresini bizim ChatHub sınıfına bağlıyoruz
app.MapHub<ChatHub>("/chathub");

app.Run();