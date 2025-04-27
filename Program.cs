using IMEAutomationDBOperations.Data;
using IMEAutomationDBOperations.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string is not configured properly.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IRepository>(provider => new SqlRepository(connectionString));
builder.Services.AddScoped<DatabaseService>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=KonyaTecnicalUnivercityIMEAutomation}/{id?}");

app.MapControllerRoute(
    name: "studentLogin",
    pattern: "StudentLogin",
    defaults: new { controller = "Home", action = "StudentLogin" });

app.MapControllerRoute(
    name: "studentPage",
    pattern: "Account/StudentPage",
    defaults: new { controller = "Home", action = "StudentPage" });

app.MapControllerRoute(
    name: "aboutme",
    pattern: "aboutme",
    defaults: new { controller = "Home", action = "aboutme" });

app.MapControllerRoute(
    name: "isletmedeMeslekiEgitimSozlesmesi",
    pattern: "IsletmedeMeslekiEgitimSozlesmesi",
    defaults: new { controller = "Home", action = "IsletmedeMeslekiEgitimSozlesmesi" });

app.MapControllerRoute(
    name: "generateIMEBasvuruFormu",
    pattern: "Home/GenerateIMEBasvuruFormu",
    defaults: new { controller = "Home", action = "GenerateIMEBasvuruFormu" });

app.Run();
