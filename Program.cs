using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.ResponseCompression;
using PlanningPoker.Hubs;
using PlanningPoker.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMvc(options => options.EnableEndpointRouting = false);

builder.Services.AddSingleton<IPlanningPokerService, PlanningPokerService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddAuthenticationCore();
builder.Services.AddAuthorizationCore();

builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => {
        options.Cookie.IsEssential = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.Name = "PlanningPokerAuthCookie";
    });

builder.Services.Configure<CookiePolicyOptions>(options => {
    options.ConsentCookie.IsEssential = true;
    options.CheckConsentNeeded = context => false;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<HttpContextAccessor>();
builder.Services.AddScoped<HttpClient>();
builder.Services.AddHttpClient();

var app = builder.Build();

app.UseStaticFiles();

app.UseMvcWithDefaultRoute();

app.UseRouting();

app.MapBlazorHub();
app.MapHub<PlanningHub>("/planninghub");
app.MapFallbackToPage("/_Host");

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapDefaultControllerRoute();

app.Run();
