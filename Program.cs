using Microsoft.EntityFrameworkCore;
using PUNDERO.Helper;
using PUNDERO.Models;
using PUNDERO.Services;
using System.Text.Json.Serialization;
using Dapper;
using System.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<MyAuthService>();
builder.Services.AddSwaggerGen(x => x.OperationFilter<AuthorizationSwagger>());
builder.Services.AddTransient<TokenGenerator>();

builder.Services.AddDbContext<PunderoContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Register the SalesDataRepository for Dapper
builder.Services.AddTransient<SalesDataRepository>(sp =>
    new SalesDataRepository(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register the SalesDataService and SalesForecasting
builder.Services.AddTransient<SalesDataService>();
builder.Services.AddSingleton<SalesForecasting>();

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins("http://localhost:8515", "http://localhost:3000", "http://127.0.0.1:5500")
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
}

// Apply CORS policy
app.UseCors();

// Ensure static files are served from the wwwroot directory
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
