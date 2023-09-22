using Identity.App.Data;
using Identity.App.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Principal;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddAuthentication();

builder.Services.AddControllers();
builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(Path.GetTempPath()));

builder.Services.AddDbContext<ApplicationDataContext>(options =>
    options.UseSqlServer("Data Source=.\\SQLEXPRESS;Initial Catalog=IdentityAppDb;Integrated Security=True")
    );

builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<ApplicationDataContext>()
                .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});


builder.Services.AddMvc();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseAuthentication();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Register}"
    );

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}"
    );

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Logout}"
    );

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=ForgotPassword}"
    );

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=}/{action=Home}"
    );

app.UseAuthorization();

app.MapRazorPages();

app.Run();
