using Microsoft.EntityFrameworkCore;
using PUNDERO.Helper;
using PUNDERO.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor(); //dodatnog zbog autorizacije
builder.Services.AddTransient<MyAuthService>(); //dodatnog zbog autorizacije
builder.Services.AddSwaggerGen(x => x.OperationFilter<AuthorizationSwagger>());//dodatno zbog autorizacije

// Register PunderoContext with dependency injection (replace with your actual connection string name)
builder.Services.AddDbContext<PunderoContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins("https://localhost:44306/", "https://localhost:3000/", "http://127.0.0.1:5500/")
            .AllowAnyHeader().
            AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
}

app.UseHttpsRedirection();

// app.UseAuthorization(); ISKLJUCENO DA RADI BEZ HTTPS

// Add CORS middleware after UseAuthorization
app.UseCors("AllowReact");
app.UseRouting();
app.UseCors("SiteCorsPolicy");

app.UseCors(builder =>
{
    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
});

app.MapControllers();

app.Run();
