global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.Rendering;
global using Newtonsoft.Json;
global using Project_Redmil_MVC.Helper;
global using Project_Redmil_MVC.Models;
global using RestSharp;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.Configuration.Json;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Extensions.DependencyInjection.Extensions;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
// Add services to the container.
builder.Services.AddControllersWithViews();
var app = builder.Build();
//builder.Services.TryAddSingleton<IHttpContextAccessor,HttpContextAccessor>
    //TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
IWebHostEnvironment env = app.Environment;
app.Environment.IsDevelopment();
app.UseAuthentication();
app.UseRouting();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
        endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.MapRazorPages();

app.Run();
