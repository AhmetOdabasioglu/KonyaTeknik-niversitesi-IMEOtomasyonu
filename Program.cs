using IMEAutomationDBOperations.Data;
using IMEAutomationDBOperations.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Veritabanı bağlantısını `appsettings.json`'dan almak yerine burada doğrudan yazıyoruz
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// connectionString'in null olmadığından emin olalım
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string is not configured properly.");
}

// DBContext kaydını yapıyoruz
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Repository'yi Scoped olarak kaydediyoruz
builder.Services.AddScoped<IRepository>(provider => new SqlRepository(connectionString));

// DatabaseService ve diğer servisleri kaydediyoruz
builder.Services.AddScoped<DatabaseService>();

// MVC veya Razor Pages desteği ekliyoruz
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Geliştirme ortamı kontrolü
if (app.Environment.IsDevelopment())
{
    // Geliştirme sırasında hataların ayrıntılı şekilde görüntülenmesini sağlarız
    app.UseDeveloperExceptionPage();
}
else
{
    // Üretim ortamında daha güvenli bir hata yönetimi
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// HTTPS yönlendirmesi ve statik dosyalar
app.UseHttpsRedirection();
app.UseStaticFiles();

// Routing ve authorization middleware'lerini aktif ediyoruz
app.UseRouting();
app.UseAuthorization();

// Varsayılan route'u ayarlıyoruz
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Uygulama çalıştırılıyor
app.Run();
