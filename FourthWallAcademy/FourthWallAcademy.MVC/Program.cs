using FourthWallAcademy.App;
using FourthWallAcademy.App.Services;
using FourthWallAcademy.MVC;
using FourthWallAcademy.MVC.db;
using FourthWallAcademy.MVC.db.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

var builder = WebApplication.CreateBuilder(args);

var appconfig = new AppConfiguration(builder.Configuration);

// Register DB Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration["ConnectionString"]));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 8;

        options.User.RequireUniqueEmail = false;
        options.SignIn.RequireConfirmedAccount = false;
        
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register Serilog with optional db logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var loggerConfiguration = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console();

if (builder.Configuration.GetValue<bool>("Logging:DbLogging:Enabled"))
{
    loggerConfiguration.WriteTo.MSSqlServer(
        connectionString: builder.Configuration["ConnectionString"],
        tableName: "MVC_LogEvents",
        autoCreateSqlTable: true,
        restrictedToMinimumLevel: appconfig.GetDbLogLevel()
    );
}
Log.Logger = loggerConfiguration.CreateLogger();

builder.Services.AddLogging(loggingBuilder => 
    loggingBuilder.AddSerilog(dispose: true));

// Register FourthWallAcademy Services
var sf = new ServiceFactory(appconfig);
builder.Services.AddScoped(_ => sf.CreateCourseService());
builder.Services.AddScoped(_ => sf.CreateStudentService());
builder.Services.AddScoped(_ => sf.CreateSectionService());
builder.Services.AddScoped(_ => sf.CreateInstructorService());
builder.Services.AddScoped(_ => sf.CreatePowerService());
builder.Services.AddScoped(_ => sf.CreateWeaknessService());

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();