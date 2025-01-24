using FourthWallAcademy.API;
using FourthWallAcademy.App;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var appConfig = new AppConfiguration(builder.Configuration);

builder.Services.AddControllers();

// override launchsettings.jso to listen to ports 3000 and 5000
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(3000);
    options.ListenAnyIP(5000, listenOptions =>
    {
        listenOptions.UseHttps();
    });
});

// add cors policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyGetOrigins", policy =>
    {
        policy.AllowAnyOrigin();
        policy.WithMethods("GET");
    });
});

// add Nswag
builder.Services.AddOpenApiDocument();

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

// Inject services
var sf = new ServiceFactory(appConfig);
builder.Services.AddScoped(_ => sf.CreateStudentService());
builder.Services.AddScoped(_ => sf.CreateSectionService());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.UseCors("AllowAnyGetOrigins");
app.UseHttpsRedirection();

/*
 * app.UseAuthentication();
 * app.UseAuthorization();
 */

app.Run();