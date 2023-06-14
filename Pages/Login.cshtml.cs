namespace PlanningPoker.Pages;

using System.Security.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[AllowAnonymous]
public class LoginModel : PageModel
{
    
    public string? ReturnUrl { get; set; }
    
    public async Task<IActionResult> 
        OnGetAsync(string userName, string isSpectator, string redirectUrl)
    {
        try {
            await HttpContext
                .SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme);
        } catch (Exception ex) {
            throw new AuthenticationException($"Failed to sign out. Name: {userName}, Message: {ex.Message}");
        }
        
        var userRole = isSpectator == "True" ? "spectator" : "participant";
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, userName),
            new(ClaimTypes.Role, userRole),
        };
        var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            RedirectUri = redirectUrl
        };

        try {
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        } catch (Exception ex) {
            throw new AuthenticationException($"Failed to sign in: {userName}; {ex.Message}"); 
        }
        
        return LocalRedirect($"~/{redirectUrl}");
    }
    
}
