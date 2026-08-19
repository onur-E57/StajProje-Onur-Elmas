var builder = WebApplication.CreateBuilder(args);

// 1. Projeye HTML (View) desteğini ekliyoruz
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();

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

app.Run();