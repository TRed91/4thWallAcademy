using FourthWallAcademy.API;
using FourthWallAcademy.API.db;
using FourthWallAcademy.API.db.Entities;
using FourthWallAcademy.App;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var appConfig = new AppConfiguration(builder.Configuration);

// add Nswag
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Configure Identity
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration["ConnectionString"]));

builder.Services.AddAuthorization();

builder.Services.AddIdentityApiEndpoints<ApplicationUser>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 8;
        
        options.User.RequireUniqueEmail = false;
        options.SignIn.RequireConfirmedEmail = false;
        options.SignIn.RequireConfirmedPhoneNumber = false;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// configure logger (serilog)
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var loggerConfig = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console();

if (builder.Configuration.GetValue<bool>("Logging:DbLogging:Enabled"))
{
    loggerConfig.WriteTo.MSSqlServer(
        connectionString: builder.Configuration["ConnectionString"],
        tableName: "API_LogEvents",
        autoCreateSqlTable: true,
        restrictedToMinimumLevel: appConfig.GetDbLogLevel());
}
Log.Logger = loggerConfig.CreateLogger();

builder.Services.AddLogging(loggingBuilder => 
    loggingBuilder.AddSerilog(dispose: true));

builder.Services.AddControllers();

// Inject services
var sf = new ServiceFactory(appConfig);
builder.Services.AddScoped(_ => sf.CreateStudentService());
builder.Services.AddScoped(_ => sf.CreateSectionService());

// add cors policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AnyOrigins", policy =>
    {
        policy.AllowAnyOrigin();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapIdentityApi<ApplicationUser>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AnyOrigins");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();