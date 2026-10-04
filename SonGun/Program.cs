using System.Threading.RateLimiting;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using SonGun.Data;
using SonGun.Models;

// Try candidate paths for .env
var candidatePaths = new[]
{
    Path.Combine(Directory.GetCurrentDirectory(), ".env"),
    Path.Combine(Directory.GetCurrentDirectory(), "..", ".env"),
    Path.Combine(AppContext.BaseDirectory, ".env"),
    Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".env"),
};

foreach (var path in candidatePaths)
{
    if (File.Exists(path))
    {
        Env.Load(path);
        break;
    }
}

var builder = WebApplication.CreateBuilder(args);

// Configure AuthSettings with robust fallback
string GetEnvOrConfig(string key, string fallback)
{
    var val = Environment.GetEnvironmentVariable(key);
    if (!string.IsNullOrWhiteSpace(val)) return val.Trim();
    val = builder.Configuration[key];
    if (!string.IsNullOrWhiteSpace(val)) return val.Trim();
    return fallback;
}

builder.Services.Configure<AuthSettings>(options =>
{
    options.AdminUsername = GetEnvOrConfig("ADMIN_USERNAME", "admin");
    options.AdminPassword = GetEnvOrConfig("ADMIN_PASSWORD", "Saddam123!");
    options.EditorUsername = GetEnvOrConfig("EDITOR_USERNAME", "editor");
    options.EditorPassword = GetEnvOrConfig("EDITOR_PASSWORD", "Haberci123!");
});

// Add services
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=songun.db"));

// Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.Name = "SonGun.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

// Rate limiting against brute-force attacks
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddFixedWindowLimiter("login-limiter", opt =>
    {
        opt.PermitLimit = 30;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });
});

var app = builder.Build();

// Automatically apply EF Core database migrations on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
