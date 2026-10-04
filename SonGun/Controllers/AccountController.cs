using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using SonGun.Models;
using SonGun.Models.ViewModels;

namespace SonGun.Controllers;

public class AccountController : Controller
{
    private readonly AuthSettings _authSettings;
    private readonly ILogger<AccountController> _logger;

    public AccountController(IOptions<AuthSettings> authSettings, ILogger<AccountController> logger)
    {
        _authSettings = authSettings.Value;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToLocal(returnUrl);
        }

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("login-limiter")]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        string? role = null;

        // Check if admin credentials match safely
        if (SafeEqualsUsername(model.Username, _authSettings.AdminUsername) &&
            SafeEqualsPassword(model.Password, _authSettings.AdminPassword))
        {
            role = "Admin";
        }
        // Check if editor credentials match safely
        else if (SafeEqualsUsername(model.Username, _authSettings.EditorUsername) &&
                 SafeEqualsPassword(model.Password, _authSettings.EditorPassword))
        {
            role = "Editor";
        }

        if (role != null)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, model.Username.Trim()),
                new(ClaimTypes.Role, role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            _logger.LogInformation("Kullanıcı {Username} ({Role}) başarıyla giriş yaptı.", model.Username, role);
            return RedirectToLocal(model.ReturnUrl);
        }

        _logger.LogWarning("Başarısız giriş denemesi: {Username}", model.Username);
        ModelState.AddModelError(string.Empty, "Geçersiz kullanıcı adı veya şifre.");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    private static bool SafeEqualsUsername(string? a, string? b)
    {
        if (string.IsNullOrWhiteSpace(a) || string.IsNullOrWhiteSpace(b))
        {
            return false;
        }

        return string.Equals(a.Trim(), b.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    private static bool SafeEqualsPassword(string? input, string? actual)
    {
        if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(actual))
        {
            return false;
        }

        var aBytes = Encoding.UTF8.GetBytes(input.Trim());
        var bBytes = Encoding.UTF8.GetBytes(actual.Trim());

        return CryptographicOperations.FixedTimeEquals(aBytes, bBytes);
    }
}
