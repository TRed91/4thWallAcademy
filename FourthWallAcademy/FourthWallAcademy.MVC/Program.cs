using FourthWallAcademy.App;
using FourthWallAcademy.App.Services;
using FourthWallAcademy.MVC;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

var builder = WebApplication.CreateBuilder(args);

var appconfig = new AppConfiguration(builder.Configuration);

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

builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();