using ECommerce.Infrastructure;
using ECommerce.Api.Middlewares;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// --- Serilog bootstrap ---
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// --- Services ---
builder.Services.AddControllers().AddJsonOptions(_ => { });

builder.Services
    .AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var cs = builder.Configuration.GetConnectionString("DefaultConnection")
         ?? "Server=(localdb)\\MSSQLLocalDB;Database=ECommerceDb;Trusted_Connection=True;TrustServerCertificate=True;";
builder.Services.AddDbContext<ECommerceDbContext>(opt => opt.UseSqlServer(cs));

var app = builder.Build();

// --- Pipeline ---
// Our error handler first so it catches everything
app.UseMiddleware<ErrorHandlingMiddleware>();

// Request logging early to see full duration
app.UseMiddleware<RequestLoggingMiddleware>();

// Lightweight in-process metrics
app.UseMiddleware<MetricsMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStatusCodePages();
app.UseAuthorization();
app.MapControllers(); //Registers all my controller endpoints into the routing table during startup. It says “these are the possible endpoints you can match to.”

app.Run();
