using Microsoft.AspNetCore.ResponseCompression;
using PlanningPoker.Hubs;
using PlanningPoker.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<IPlanningPokerService, PlanningPokerService>();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapHub<PlanningHub>("/planninghub");
app.MapFallbackToPage("/_Host");

app.Run();
