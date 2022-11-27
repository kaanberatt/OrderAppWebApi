using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OrderAppWebApi.BackgroundService;
using OrderAppWebApi.Context;
using Serilog;
using Serilog.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

#region MyServices

string connectionString = builder.Configuration.GetConnectionString("MySql");

builder.Services.AddDbContext<OrderContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), null);
});

builder.Services.AddMemoryCache(); // memory-cache
builder.Services.AddAutoMapper(typeof(MapperProfile).Assembly);

Logger log = new LoggerConfiguration() //SeriLog
    .WriteTo.File("logs/log.txt") // logs klasörü altına log.txt isimli dosya oluşturacak ve loglar kayıt edilecek.
    .WriteTo.MySQL(connectionString, "Logs") //Veritabanında Logs tablosu altında logları kaydedecek.
    .MinimumLevel.Information()
    .CreateLogger();

builder.Host.UseSerilog(log);

builder.Services.AddHostedService<SendMailService>();

builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
#endregion

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
