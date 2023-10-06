using Identity.App.Contract.Repositories;
using Identity.App.Contract.Services;
using Identity.App.Data;
using Identity.App.Models;
using Identity.App.Repositories;
using Identity.App.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

var connectionString = configuration.GetConnectionString("DatabaseConnectionString");

services.AddTransient<IDbConnectionInterface, DatabaseConnectionFactory>()
        .AddTransient<IAccountRepository, AccountRepository>()
        .AddTransient<IEmailSender, EmailSender>();

services.AddRazorPages();
services.AddAuthentication();
services.AddControllers();

services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(Path.GetTempPath()));

services.AddDbContext<ApplicationDataContext>(options => options.UseSqlServer(connectionString));

services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
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
     pattern: "{controller=Account}/{action=PasswordRedefinition}"
    );

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=}/{action=Home}"
    );


app.UseAuthorization();
app.MapRazorPages();
app.Run();