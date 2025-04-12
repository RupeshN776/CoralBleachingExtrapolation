using CoralBleachingExtrapolation.Data;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// 25-02-2025  1.0     Ben       Point GBR Implimenting UML (GBRCoral.cs) 
/// 06-02-2025  1.0.1   Keelin   Adding context for ApplicationDbContextGBR
/// 04-11-2025  1.0.2   Ben       Add config for google API
/// </summary>
/// 

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        x => x.UseNetTopologySuite() // Enable spatial support
    ));
builder.Services.AddDbContext<ApplicationDbContextGBR>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        x => x.UseNetTopologySuite() // Enable spatial support
    ));
builder.Services.Configure<GoogleApiSettings>(
    builder.Configuration.GetSection("GoogleApi"));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
